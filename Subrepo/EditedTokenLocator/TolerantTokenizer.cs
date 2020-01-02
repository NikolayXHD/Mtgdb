using System.Collections.Generic;

namespace Lucene.Net.Contrib
{
	public class TolerantTokenizer
	{
		public List<Token> Tokens { get; } = new List<Token>();
		public List<string> SyntaxErrors { get; } = new List<string>();

		public TolerantTokenizer(string queryStr, params char[] customTokens)
		{
			_context = new ContextualEnumerator<EscapedChar>(new StringEscaper(queryStr));
			_customTokens = new HashSet<char>(customTokens);
		}

		public void Parse()
		{
			_substring = string.Empty;

			while (_context.MoveNext())
			{
				_position = _context.Current.Position;
				_substring += _context.Current.Value;

				bool beforeTerminator = nextIsTerminator();


				var tokenTypeNullable =
					getCustomTokenType(_substring) ??
					TokenCatalog.GetTokenType(_substring);

				if (_isRegexOpen)
				{
					// empty regex body
					if (tokenTypeNullable == TokenType.RegexDelimiter)
					{
						// close quote
						var token = addToken(TokenType.RegexDelimiter);
						_openOperators.Pop();
						updateCurrentField();
						token.NextTokenField = _currentField;
						_isRegexOpen = false;
					}
					// regex body token lasts until regex delimiter or End Of String
					else if (!_context.HasNext ||
						TokenCatalog.GetTokenType(_context.Next.Value) == TokenType.RegexDelimiter)
					{
						var token = addToken(TokenType.RegexBody);
						token.NextTokenField = _currentField;
						_isRegexOpen = false;
					}
				}
				else if (tokenTypeNullable.HasValue)
				{
					var tokenType = tokenTypeNullable.Value;

					if (tokenType.IsAny(TokenType.Open))
					{
						var token = addToken(tokenType);
						_openOperators.Push(token);
						token.NextTokenField = _currentField;
					}
					else if (tokenType.IsAny(TokenType.Close))
					{
						if (tokenType.IsLegalCloserOf(_openOperators.TryPeek()?.Type))
						{
							// close parenthesis
							var token = addToken(tokenType);
							_openOperators.Pop();
							updateCurrentField();
							token.NextTokenField = _currentField;
						}
						else
						{
							if (_openOperators.Count == 0)
								SyntaxErrors.Add($"Unmatched {_substring} at {_start}");
							else
								SyntaxErrors.Add(
									$"Unexpected {_substring} at {_start} closing {_openOperators.Peek().Value} at {_openOperators.Peek().Position}");

							var token = addToken(tokenType);
							token.NextTokenField = _currentField;
						}
					}
					else if (tokenType.IsAny(TokenType.Quote | TokenType.RegexDelimiter))
					{
						TokenType generalType;
						TokenType openType;
						TokenType closeType;
						bool isRegex;

						if (tokenType.IsAny(TokenType.Quote))
						{
							generalType = TokenType.Quote;
							openType = TokenType.OpenQuote;
							closeType = TokenType.CloseQuote;
							isRegex = false;
						}
						else
						{
							generalType = TokenType.RegexDelimiter;
							openType = TokenType.OpenRegex;
							closeType = TokenType.CloseRegex;
							isRegex = true;
						}

						if (_openOperators.Count > 0 && _openOperators.Peek().Type.IsAny(generalType))
						{
							// close quote
							var token = addToken(closeType);
							_openOperators.Pop();
							updateCurrentField();
							token.NextTokenField = _currentField;
						}
						else
						{
							var token = addToken(openType);
							_openOperators.Push(token);

							if (isRegex)
								_isRegexOpen = true;

							token.NextTokenField = _currentField;
						}
					}
					else if (tokenType.IsAny(TokenType.Modifier | TokenType.Colon) ||
						tokenType.IsAny(TokenType.Boolean) &&
						// To avoid recognizing AND in ANDY
						(StringEscaper.SpecialChars.Contains(_substring[0]) || beforeTerminator))
					{
						var token = addToken(tokenType);
						token.NextTokenField = _currentField;
					}
					else if (tokenType.IsAny(TokenType.Wildcard))
					{
						if (tokenType.IsAny(TokenType.AnyString) && nextIsColon())
						{
							// add field
							var token = addToken(TokenType.Field);
							_currentField = Tokens[Tokens.Count - 1].ParentField;
							token.NextTokenField = _currentField;
						}
						else
						{
							var previous = Tokens.TryGetLast();

							bool isAdjacentToPrevious =
								previous != null &&
								previous.Position + previous.Value.Length == _start;

							// adjacent wildcard and value tokens are related to the same field
							if (isAdjacentToPrevious && previous.Type.IsAny(TokenType.Wildcard | TokenType.FieldValue))
								_currentField = previous.ParentField;

							var token = addToken(tokenType);

							updateCurrentField();
							token.NextTokenField = _currentField;
						}
					}
					else if (tokenType.IsAny(TokenType.Custom))
					{
						var token = addToken(tokenType);
						token.NextTokenField = _currentField;
					}
					else if (_openOperators.TryPeek()?.Type.IsAny(TokenType.OpenRange) == true &&
						tokenType.IsAny(TokenType.To))
					{
						// interval extremes separator
						var token = addToken(tokenType);
						token.NextTokenField = _currentField;
					}
				}
				else if (TokenCatalog.IsWhitespace(_substring))
				{
					// ignore whitespace token
					_start = _position;
					_substring = string.Empty;
				}
				else if (nextIsColon())
				{
					// add field
					var token = addToken(TokenType.Field);
					_currentField = Tokens[Tokens.Count - 1].ParentField;
					token.NextTokenField = _currentField;
				}
				else if (beforeTerminator && prevIsModifier())
				{
					var token = addToken(TokenType.ModifierValue);

					updateCurrentField();
					token.NextTokenField = _currentField;
				}
				else if (beforeTerminator)
				{
					var previous = Tokens.TryGetLast();

					bool isAdjacentToPrevious =
						previous != null &&
						previous.Position + previous.Value.Length == _start;

					// adjacent wildcard and value tokens are related to the same field
					if (isAdjacentToPrevious && previous.Type.IsAny(TokenType.Wildcard | TokenType.FieldValue))
						_currentField = previous.ParentField;

					var token = addToken(TokenType.FieldValue);

					updateCurrentField();
					token.NextTokenField = _currentField;
				}
				else if (isCj())
				{
					var token = addToken(TokenType.FieldValue);
					token.NextTokenField = _currentField;
				}
			}
		}

		private TokenType? getCustomTokenType(string substring) =>
			substring.Length == 1 && _customTokens.Contains(substring[0])
				? (TokenType?) TokenType.Custom
				: null;

		private bool prevIsModifier()
		{
			return Tokens.TryGetLast()?.Type.IsAny(TokenType.Modifier) == true;
		}

		private bool isCj()
		{
			if (_context.Current.Value.Length == 0)
				return false;

			return _context.Current.Value[_context.Current.Value.Length - 1].IsCj();
		}

		private void updateCurrentField()
		{
			_currentField = getCurrentField();
		}

		private string getCurrentField()
		{
			return _openOperators.TryPeek()?.ParentField;
		}

		private bool nextIsTerminator()
		{
			if (!_context.HasNext)
				return true;

			if (TokenCatalog.IsWhitespace(_context.Next.Value))
				return true;

			bool nextIsTerminator = isTerminator(_context.Next.Value);
			bool currentIsTerminator = isTerminator(_context.Current.Value);

			if (!nextIsTerminator && !currentIsTerminator)
				return false;

			if (_context.Next.Value != _context.Current.Value)
				// There are no operators constructed from different special chars
				return true;

			return !StringEscaper.TwoSymbolOperators.ContainsString(_context.Current.Value);
		}

		private bool isTerminator(string value)
		{
			return
				StringEscaper.SpecialChars.ContainsString(value) ||
				value.Length == 1 && _customTokens.Contains(value[0]);
		}

		private bool nextIsColon()
		{
			return _context.HasNext &&
				TokenCatalog.GetTokenType(_context.Next.Value)?.IsAny(TokenType.Colon) == true;
		}

		private Token addToken(TokenType tokenType)
		{
			string field;
			var previous = Tokens.TryGetLast();

			bool startsPhrase = previous?.Type.IsAny(TokenType.OpenQuote) == true;

			switch (tokenType)
			{
				case TokenType.Field:
					field = _substring;
					break;

				case TokenType.FieldValue:
					field = previous?.Type.IsAny(TokenType.Modifier) == true
						? null
						: _currentField;
					break;

				default:
					field = _currentField;
					break;
			}

			string value = _substring.TrimEnd();

			var result = new Token(_start, value, tokenType, field);
			if (previous != null)
			{
				result.Previous = previous;
				previous.Next = result;
			}

			bool isPhraseComplex = !tokenType.IsAny(TokenType.FieldValue);

			if (startsPhrase)
			{
				if (!result.Type.IsAny(TokenType.CloseQuote))
				{
					result.PhraseStart = result;
					result.IsPhraseComplex = isPhraseComplex;
				}
			}
			else if (tokenType.IsAny(TokenType.CloseQuote))
			{
				result.PhraseStart = null;
				result.IsPhraseComplex = false;
			}
			else if ((result.PhraseStart = previous?.PhraseStart) != null)
			{
				if (previous.IsPhraseComplex)
					isPhraseComplex = true;
				else if (isPhraseComplex)
					foreach (var phraseToken in getPreviousPhraseTokens(result))
						phraseToken.IsPhraseComplex = true;

				result.IsPhraseComplex = isPhraseComplex;
			}

			if (tokenType.IsAny(TokenType.SlopeModifier) && previous?.Type.IsAny(TokenType.CloseQuote) == true)
				foreach (var phraseToken in getPreviousPhraseTokens(previous.Previous))
					phraseToken.PhraseHasSlop = true;

			Tokens.Add(result);

			_substring = string.Empty;
			_start = _position;

			return result;
		}

		private static IEnumerable<Token> getPreviousPhraseTokens(Token token)
		{
			var current = token;

			while (current != null)
			{
				yield return current;

				if (current.IsPhraseStart)
					yield break;

				current = current.Previous;
			}
		}

		private readonly Stack<Token> _openOperators = new Stack<Token>();
		private bool _isRegexOpen;
		private string _currentField;
		private int _start;
		private int _position;
		private string _substring;
		private readonly ContextualEnumerator<EscapedChar> _context;
		private readonly HashSet<char> _customTokens;
	}
}