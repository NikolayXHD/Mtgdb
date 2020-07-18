using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Lucene.Net.Contrib;
using Mtgdb.Data;

namespace Mtgdb.Ui
{
	public class SearchResultHighlighter
	{
		public SearchResultHighlighter(
			ISearchSubsystem searchSubsystem,
			IDocumentAdapterBase adapter,
			IKeywordHighlighter keywordHighlighter)
		{
			_searchSubsystem = searchSubsystem;
			_adapter = adapter;
			_keywordHighlighter = keywordHighlighter;
			_analyzer = new MtgAnalyzer(adapter);
		}

		public List<TextRange> GetHighlightRanges(IList<TextRange> matches, IList<TextRange> contextMatches)
		{
			var result = new List<TextRange>();

			foreach (var match in contextMatches)
				match.IsContext = true;

			var orderedMatches = matches.Union(contextMatches)
				.OrderBy(_ => _.Index)
				.ThenByDescending(_ => _.Length);

			TextRange previousArea = null;
			int previousMatchEnd = 0;
			foreach (var m in orderedMatches)
			{
				var thisEnd = m.Index + m.Length;

				if (previousArea != null && m.Index < previousMatchEnd)
				{
					if (thisEnd > previousMatchEnd)
					{
						int newPreviousLength = m.Index - previousArea.Index;
						if (newPreviousLength > 0)
							previousArea.Length = newPreviousLength;
						else
							result.RemoveAt(result.Count - 1);

						previousArea = m;
						previousMatchEnd = thisEnd;
						result.Add(previousArea);
					}
				}
				else
				{
					previousArea = m;
					previousMatchEnd = thisEnd;
					result.Add(previousArea);
				}
			}

			return result;
		}

		public void AddSearchStringMatches(
			List<TextRange> matches,
			List<TextRange> contextMatches,
			string displayField,
			string displayText)
		{
			var searchResult = _searchSubsystem.SearchResult;
			if (!string.IsNullOrEmpty(searchResult?.ParseErrorMessage))
				return;

			addTermMatches(matches, contextMatches, displayField, displayText, searchResult);
			addPhraseMatches(matches, displayField, displayText, searchResult);
		}

		private void addTermMatches(
			List<TextRange> matches,
			List<TextRange> contextMatches,
			string displayField,
			string displayText,
			ISearchResultBase searchResult)
		{
			var highlightTerms = searchResult?.HighlightTerms;
			if (highlightTerms == null)
				return;

			foreach (var pair in highlightTerms.Where(term => isRelevantField(displayField, term.Key)))
			{
				var terms = pair.Value;
				var patternsSet = new Dictionary<string, (HashSet<string> TokenFields, Regex Pattern)>();
				var contextPatternsSet = new Dictionary<string, (HashSet<string> TokenFields, Regex Pattern)>();

				addTextualPatterns(searchResult.Query, terms, patternsSet, contextPatternsSet);
				_keywordHighlighter.AddKeywordPatterns(searchResult.Query, terms, patternsSet);

				addMatches(displayText, patternsSet.Values, matches);
				addMatches(displayText, contextPatternsSet.Values, contextMatches);
			}
		}

		private void addTextualPatterns(
			string query,
			IReadOnlyList<Token> terms,
			Dictionary<string, (HashSet<string> TokenFields, Regex Pattern)> patternsSet,
			Dictionary<string, (HashSet<string> TokenFields, Regex Pattern)> contextPatternsSet)
		{
			var textualTokens = terms.Where(t =>
				t.Type.IsAny(TokenType.FieldValue | TokenType.AnyChar | TokenType.RegexBody) &&
				!_keywordHighlighter.IsKeywordField(t.ParentField) &&
				!string.IsNullOrEmpty(t.Value));

			foreach (var token in textualTokens)
			{
				if (!getPattern(query, token, out string pattern, out var contextPatterns))
					continue;

				addPattern(token.ParentField, pattern, patternsSet);

				foreach (string contextPattern in contextPatterns)
					addPattern(token.ParentField, contextPattern, contextPatternsSet);
			}
		}




		private void addPhraseMatches(
			List<TextRange> matches,
			string displayField,
			string displayText,
			ISearchResultBase searchResult)
		{
			var highlightPhrases = searchResult?.HighlightPhrases;

			if (highlightPhrases == null)
				return;

			var displayTextTokens = new Lazy<IReadOnlyList<(string Term, int Offset)>>(() =>
				_analyzer.GetTokens(displayField, displayText).ToList());

			var displayTextValues = new Lazy<IReadOnlyList<string>>(() =>
				displayTextTokens.Value.Select(_ => _.Term).ToList());

			foreach (var term in highlightPhrases.Where(term => isRelevantField(displayField, term.Key)))
			{
				foreach (var tokenValues in term.Value)
				{
					if (tokenValues.Count == 0)
						continue;

					if (tokenValues.Count == 1)
					{
						var matchesToAdd = displayTextTokens.Value
							.Where(token => Str.Comparer.Equals(token.Term, tokenValues[0]))
							.Select(token => new TextRange(token.Offset, token.Term.Length));

						matches.AddRange(matchesToAdd);
						continue;
					}

					var searcher = new KnutMorrisPrattSubstringSearch<string>(tokenValues, Str.Comparer);

					foreach (int index in searcher.FindAll(displayTextValues.Value))
					{
						var firstWordMatch = displayTextTokens.Value[index];
						var lastWordMatch = displayTextTokens.Value[index + tokenValues.Count - 1];

						var startIndex = firstWordMatch.Offset;
						var length = lastWordMatch.Offset + lastWordMatch.Term.Length - startIndex;

						matches.Add(new TextRange(startIndex, length));
					}
				}
			}
		}

		private static bool isRelevantField(string displayField, string queryField)
		{
			return string.IsNullOrEmpty(queryField) || Str.Equals(queryField, displayField);
		}

		private static void addPattern(string termField, string pattern, IDictionary<string, (HashSet<string> TokenFields, Regex Pattern)> patternsSet)
		{
			if (pattern == null)
				return;

			if (patternsSet.TryGetValue(pattern, out var tokenPattern))
			{
				tokenPattern.TokenFields.Add(termField);
				return;
			}

			tokenPattern = (new HashSet<string>(Str.Comparer) { termField }, RegexUtil.GetCached(pattern));
			patternsSet.Add(pattern, tokenPattern);
		}

		private void addMatches(string displayText, IEnumerable<(HashSet<string> TokenField, Regex Pattern)> tokenPatterns, List<TextRange> matches)
		{
			foreach (var (tokenFields, findRegex) in tokenPatterns)
				foreach (var tokenField in tokenFields)
					foreach (var token in _analyzer.GetTokens(tokenField, displayText))
					{
						var toAdd = findRegex.Matches(token.Term)
							.Cast<Match>()
							.Where(match => match.Success && match.Length != 0)
							.Select(match => new TextRange(match.Index + token.Offset, match.Length));

						matches.AddRange(toAdd);
					}
		}

		private bool getPattern(string query, Token token, out string result, out List<string> contextPatterns)
		{
			if (token.IsPhraseStart && !token.IsPhraseComplex && !token.PhraseHasSlop)
			{
				var patternBuilder = new StringBuilder();
				appendFieldValuePattern(patternBuilder, token.ParentField, StringEscaper.Unescape(token.GetPhraseText(query)));

				result = patternBuilder.ToString();
				contextPatterns = new List<string>();
				return true;
			}

			if (token.Type.IsAny(TokenType.RegexBody))
			{
				result = token.Value;
				contextPatterns = new List<string>();
				return isValidRegex(result);
			}

			if (_adapter.IsNumericField(token.ParentField))
			{
				result = @"\$?" + Regex.Escape(token.Value);
				contextPatterns = new List<string>();
				return true;
			}

			var prefixTokens = getPrefixTokens(token);
			var currentTokens = new List<Token> { token };
			var suffixTokens = getSuffixTokens(token);

			if (token.Type.IsAny(TokenType.FieldValue | TokenType.RegexBody))
				result = getPattern(prefixTokens, currentTokens, suffixTokens);
			else
				result = null;

			// let's create 1 context pattern for each group of uninterrupted wildcard tokens sequence
			var tokenGroup = new List<Token>();

			tokenGroup.AddRange(prefixTokens);
			tokenGroup.Add(token);
			tokenGroup.AddRange(suffixTokens);

			contextPatterns = new List<string>();
			suffixTokens.Clear();
			suffixTokens.AddRange(tokenGroup);

			int i = 0;
			while (true)
			{
				i += suffixTokens.TakeWhile(_ => !_.Type.IsAny(TokenType.Wildcard)).Count();

				if (i == tokenGroup.Count)
					break;

				prefixTokens.Clear();
				prefixTokens.AddRange(
					tokenGroup.Take(i));

				currentTokens.Clear();
				currentTokens.AddRange(
					tokenGroup.Skip(i).TakeWhile(_ => _.Type.IsAny(TokenType.Wildcard)));

				i = prefixTokens.Count + currentTokens.Count;

				suffixTokens.Clear();
				suffixTokens.AddRange(
					tokenGroup.Skip(prefixTokens.Count + currentTokens.Count));

				contextPatterns.Add(getPattern(prefixTokens, currentTokens, suffixTokens));
			}

			return true;
		}

		private static bool isValidRegex(string result)
		{
			try
			{
				var unused = new Regex(result);
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		private string getPattern(List<Token> prefixTokens, List<Token> radixTokens, List<Token> suffixTokens)
		{
			string prefixPattern = getPattern(prefixTokens);
			string suffixPattern = getPattern(suffixTokens);
			string radixPattern = getPattern(radixTokens);

			string result = $"(?<=^{prefixPattern}){radixPattern}(?={suffixPattern}$)";
			return result;
		}

		private static List<Token> getSuffixTokens(Token token)
		{
			var suffixTokens = new List<Token>();

			if (token.Type.IsAny(TokenType.RegexBody))
				return suffixTokens;

			while (true)
			{
				if (token.Next == null ||
						!token.Next.TouchesCaret(token.Position + token.Value.Length) ||
						token.Type.IsAny(TokenType.FieldValue) && token.Value[token.Value.Length - 1].IsCj() ||
						token.Next.Type.IsAny(TokenType.FieldValue) && token.Next.Value[0].IsCj())
					// A value adjacent to a wildcard is its continuation, in contrast with the case when there is a whitespace in-between,
					// then it is another term
					break;

				if (token.Next.Type.IsAny(TokenType.Wildcard | TokenType.FieldValue))
					suffixTokens.Add(token.Next);
				else
					break;

				token = token.Next;
			}

			return suffixTokens;
		}

		private static List<Token> getPrefixTokens(Token token)
		{
			var prefixTokens = new List<Token>();

			if (token.Type.IsAny(TokenType.RegexBody))
				return prefixTokens;

			while (true)
			{
				if (token.Previous == null ||
						!token.Previous.TouchesCaret(token.Position) ||
						token.Type.IsAny(TokenType.FieldValue) && token.Value[0].IsCj() ||
						token.Previous.Type.IsAny(TokenType.FieldValue) && token.Previous.Value[token.Previous.Value.Length - 1].IsCj())
					// A value adjacent to a wildcard is its continuation, in contrast with the case when there is a whitespace in-between,
					// then it is another term
					break;

				if (token.Previous.Type.IsAny(TokenType.Wildcard | TokenType.FieldValue))
					prefixTokens.Insert(0, token.Previous);
				else
					break;

				token = token.Previous;
			}

			return prefixTokens;
		}

		private string getPattern(IEnumerable<Token> tokens)
		{
			var pattern = new StringBuilder();
			foreach (var token in tokens)
			{
				if (token.Type.IsAny(TokenType.AnyChar))
					pattern.Append(MtgAlphabet.CharPattern);
				else if (token.Type.IsAny(TokenType.AnyString))
					pattern.Append(MtgAlphabet.CharPattern + "*");
				else if (token.Type.IsAny(TokenType.FieldValue))
					appendFieldValuePattern(pattern, token.ParentField, StringEscaper.Unescape(token.Value));
			}

			return pattern.ToString();
		}

		private void appendFieldValuePattern(StringBuilder patternBuilder, string tokenField, string escapedTokenValue)
		{
			var builder = new StringBuilder();

			foreach (var word in _analyzer.GetTokens(tokenField, escapedTokenValue))
				builder.Append(word.Term);

			var luceneUnescaped = builder.ToString();

			foreach (char c in luceneUnescaped)
			{
				var equivalents = MtgAlphabet.GetEquivalents(c).ToArray();

				if (equivalents.Length == 0)
					continue;

				if (equivalents.Length == 1)
					patternBuilder.Append(Regex.Escape(new string(equivalents[0], 1)));
				else
				{
					patternBuilder.Append("(");

					patternBuilder.Append(Regex.Escape(new string(equivalents[0], 1)));

					for (int i = 1; i < equivalents.Length; i++)
					{
						patternBuilder.Append("|");
						patternBuilder.Append(Regex.Escape(new string(equivalents[i], 1)));
					}

					patternBuilder.Append(")");
				}
			}
		}



		private readonly ISearchSubsystem _searchSubsystem;
		private readonly IDocumentAdapterBase _adapter;
		private readonly IKeywordHighlighter _keywordHighlighter;
		private readonly MtgAnalyzer _analyzer;
	}
}
