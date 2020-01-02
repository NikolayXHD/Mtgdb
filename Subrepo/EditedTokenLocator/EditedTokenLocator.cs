using System.Collections.Generic;
using System.Linq;

namespace Lucene.Net.Contrib
{
	public static class EditedTokenLocator
	{
		public static Token GetEditedToken(this TolerantTokenizer tokenizer, int caret)
		{
			tokenizer.Parse();

			var tokens = tokenizer.Tokens;

			var overlappingToken = tokens.FirstOrDefault(_ => _.OverlapsCaret(caret));

			if (overlappingToken != null)
				return overlappingToken;

			var leftToken = tokens.LastOrDefault(_ => _.IsLeftToCaret(caret));
			var rightToken = tokens.FirstOrDefault(_ => _.IsRightToCaret(caret));

			if (leftToken?.IsConnectedToCaret(caret) != true && rightToken?.IsConnectedToCaret(caret) == true)
				return rightToken;

			if (leftToken?.Type.IsAny(TokenType.Modifier) == true && leftToken.TouchesCaret(caret))
				return new Token(caret, string.Empty, TokenType.ModifierValue, leftToken.ParentField);

			if (leftToken?.TouchesCaret(caret) == true)
			{
				if (leftToken.Type.IsAny(TokenType.Field | TokenType.FieldValue | TokenType.Modifier | TokenType.Wildcard))
					return leftToken;

				if (leftToken.Type.IsAny(TokenType.Boolean) && leftToken.Value.Length > 1)
					return leftToken;

				return tokenOnEmptyInput(tokens, caret);
			}

			return tokenOnEmptyInput(tokens, caret);
		}

		public static Token GetTokenForArbitraryInsertion(this TolerantTokenizer tokenizer, int caret)
		{
			tokenizer.Parse();

			var tokens = tokenizer.Tokens;

			var overlappingToken = tokens.FirstOrDefault(_ => _.OverlapsCaret(caret));

			if (overlappingToken != null)
				return tokenOnEmptyInput(tokens, overlappingToken.Position + overlappingToken.Value.Length);

			var leftToken = tokens.LastOrDefault(_ => _.IsLeftToCaret(caret));

			if (leftToken == null)
				return tokenOnEmptyInput(tokens, caret: 0);

			return tokenOnEmptyInput(tokens, caret);
		}

		public static Token GetTokenForTermInsertion(this TolerantTokenizer tokenizer, int caret)
		{
			tokenizer.Parse();

			var tokens = tokenizer.Tokens;

			var token = 
				tokens.FirstOrDefault(_ => _.OverlapsCaret(caret)) ??
				tokens.LastOrDefault(_ => _.IsLeftToCaret(caret));

			if (token == null)
				return tokenOnEmptyInput(tokens, caret: 0);

			var current = token;

			while (true)
			{
				if (!current.IsPhrase && string.IsNullOrEmpty(current.NextTokenField))
					return tokenOnEmptyInput(tokens, current.Position + current.Value.Length);

				current = current.Next;

				if (current == null)
					return tokenOnEmptyInput(tokens, caret: 0);
			}
		}

		private static Token tokenOnEmptyInput(List<Token> tokens, int caret)
		{
			var leftToken = tokens.LastOrDefault(_ => _.Position + _.Value.Length <= caret);
			var rightToken = tokens.FirstOrDefault(_ => _.Position >= caret);

			var field = leftToken?.NextTokenField;

			var lastValueDelimiter = tokens.LastOrDefault(_ =>
				_.Position < caret && _.Type.IsAny(TokenType.Quote | TokenType.RegexDelimiter));

			bool isFieldValue =
				!string.IsNullOrEmpty(field) ||
				lastValueDelimiter?.Type.IsAny(TokenType.OpenQuote | TokenType.OpenRegex) == true;

			if (!isFieldValue)
				return new Token(caret, string.Empty, TokenType.Field, field)
				{
					Previous = leftToken,
					Next = rightToken
				};

			var result = new Token(caret, string.Empty, TokenType.FieldValue, field)
			{
				Previous = leftToken,
				Next = rightToken
			};

			if (leftToken == null)
				return result;

			bool afterOpenQuote = leftToken == lastValueDelimiter;
			bool beforeCloseQuote = rightToken?.Type.IsAny(TokenType.CloseQuote) != false;

			if (afterOpenQuote && beforeCloseQuote)
			{
				// empty phrase, maybe not closed
				result.PhraseStart = result;
				result.PhraseHasSlop = rightToken?.Next?.Type.IsAny(TokenType.SlopeModifier) == true;
				result.IsPhraseComplex = false;
			}
			else
			{
				var phraseToken = afterOpenQuote
					? rightToken
					: leftToken;

				result.PhraseStart = phraseToken.PhraseStart;
				result.PhraseHasSlop = phraseToken.PhraseHasSlop;
				result.IsPhraseComplex = phraseToken.IsPhraseComplex;
			}

			return result;
		}
	}
}