using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Contrib;
using Lucene.Net.Index;
using Lucene.Net.Queries.Mlt;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Lucene.Net.Util.Automaton;
using Token = Lucene.Net.QueryParsers.Classic.Token;

namespace Mtgdb.Dal.Index
{
	public class MtgQueryParser : QueryParser
	{
		public MtgQueryParser(LuceneVersion matchVersion, string f, Analyzer a, CardRepository repository)
			: base(matchVersion, f, a)
		{
			_repository = repository;
			FuzzyMinSim = 0.5f;
		}

		public sealed override float FuzzyMinSim
		{
			get => base.FuzzyMinSim;
			set => base.FuzzyMinSim = value;
		}

		public override Query Parse(string query)
		{
			var result = base.Parse(query);
			return result;
		}

		protected override Query GetRegexpQuery(string field, string termStr)
		{
			if (string.IsNullOrEmpty(field) || field == AnyField)
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (userField.IsNumericField())
						continue;

					var specificFieldQuery = GetRegexpQuery(userField, termStr);

					if (specificFieldQuery != null)
						result.Add(specificFieldQuery, Occur.SHOULD);
				}

				if (result.Clauses.Count == 0)
					return _matchNothingQuery;

				return result;
			}

			field = localize(field);

			if (field.IsNumericField())
				return null;

			return base.GetRegexpQuery(field, termStr);
		}

		protected override Query NewRegexpQuery(Term regexp)
		{
			return new RegexpQuery(regexp, RegExpSyntax.ALL);
		}

		protected override Query GetFieldQuery(string field, string queryText, bool quoted)
		{
			if (Str.Equals(Like, field))
				return getMoreLikeQuery(queryText, 0);

			bool valueIsFloat = isValueFloat(queryText);
			bool valueIsInt = isValueInt(queryText);

			if (string.IsNullOrEmpty(field) || field == AnyField)
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (!valueIsFloat && userField.IsFloatField())
						continue;

					if (!valueIsInt && userField.IsIntField())
						continue;

					var specificFieldQuery = GetFieldQuery(userField, queryText, quoted);

					if (specificFieldQuery == null)
						continue;

					result.Add(specificFieldQuery, Occur.SHOULD);
				}

				if (result.Clauses.Count == 0)
					return _matchNothingQuery;

				return result;
			}

			field = localize(field);

			if (field.IsFloatField())
				return createRangeQuery<float>(field, queryText, IndexUtils.TryParseFloat, NumericRangeQuery.NewSingleRange);

			if (field.IsIntField())
				return createRangeQuery<int>(field, queryText, IndexUtils.TryParseInt, NumericRangeQuery.NewInt32Range);

			var tokens = Analyzer.GetTokens(field, queryText).ToList();

			if (tokens.Count > 1)
			{
				var query = new PhraseQuery();

				foreach (var token in tokens)
					query.Add(new Term(field, token.Term));

				return query;
			}

			return base.GetFieldQuery(field, queryText, quoted);
		}

		protected override Query GetRangeQuery(string field, string part1, string part2, bool startInclusive, bool endInclusive)
		{
			bool valuesAreInt =
				(isValueInt(part1) || _nonSpecifiedNubmer.Contains(part1)) &&
				(isValueInt(part2) || _nonSpecifiedNubmer.Contains(part2));

			bool valuesAreFloat =
				(isValueFloat(part1) || _nonSpecifiedNubmer.Contains(part1)) &&
				(isValueFloat(part2) || _nonSpecifiedNubmer.Contains(part2));

			if (string.IsNullOrEmpty(field) || field == AnyField)
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (userField.IsIntField() && !valuesAreInt)
						continue;

					if (userField.IsFloatField() && !valuesAreFloat)
						continue;

					var specificFieldQuery = GetRangeQuery(userField, part1, part2, startInclusive, endInclusive);

					if (specificFieldQuery != null)
						result.Add(specificFieldQuery, Occur.SHOULD);
				}

				if (result.Clauses.Count == 0)
					return _matchNothingQuery;

				return result;
			}

			field = localize(field);

			var query = (TermRangeQuery) base.GetRangeQuery(field, part1, part2, startInclusive, endInclusive);

			if (field.IsFloatField())
				return createRangeQuery<float>(field, query, IndexUtils.TryParseFloat, NumericRangeQuery.NewSingleRange);

			if (field.IsIntField())
				return createRangeQuery<int>(field, query, IndexUtils.TryParseInt, NumericRangeQuery.NewInt32Range);

			return createRangeQuery(field, query);
		}

		protected override Query GetFuzzyQuery(string field, string termStr, float minSimilarity)
		{
			if (Str.Equals(Like, field))
				return getMoreLikeQuery(termStr, minSimilarity);

			if (string.IsNullOrEmpty(field) || field == AnyField)
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (userField.IsNumericField())
						continue;

					var specificFieldQuery = GetFuzzyQuery(userField, termStr, minSimilarity);

					if (specificFieldQuery == null)
						continue;

					result.Add(specificFieldQuery, Occur.SHOULD);
				}

				if (result.Clauses.Count == 0)
					return _matchNothingQuery;

				return result;
			}

			field = localize(field);
			minSimilarity = minSimilarity.WithinRange(0.001f, 0.999f);
			return base.GetFuzzyQuery(field, termStr, minSimilarity);
		}

		public override Query HandleQuotedTerm(string qfield, Token term, Token fuzzySlop)
		{
			if (Str.Equals(Like, qfield))
			{
				string unquotedTerm = term.Image.Substring(1, term.Image.Length - 2);
				string unescapedTerm = StringEscaper.Unescape(unquotedTerm);

				float slop = parseSlop(fuzzySlop);
				return getMoreLikeQuery(unescapedTerm, slop);
			}

			return base.HandleQuotedTerm(qfield, term, fuzzySlop);
		}

		private static float parseSlop(Token fuzzySlop)
		{
			if (fuzzySlop == null)
				return 0f;

			if (fuzzySlop.Image.Length <= 1)
				return 0f;

			float.TryParse(fuzzySlop.Image.Substring(1), NumberStyles.Float, Str.Culture, out var slop);

			return slop;
		}

		protected override Query GetFieldQuery(string field, string queryText, int slop)
		{
			if (string.IsNullOrEmpty(field) || field == AnyField)
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (userField.IsNumericField())
						continue;

					var specificFieldQuery = GetFieldQuery(userField, queryText, slop);

					if (specificFieldQuery == null)
						continue;

					result.Add(specificFieldQuery, Occur.SHOULD);
				}

				if (result.Clauses.Count == 0)
					return _matchNothingQuery;

				return result;
			}

			field = localize(field);

			var tokens = Analyzer.GetTokens(field, queryText).ToList();

			if (tokens.Count > 1)
			{
				var query = new PhraseQuery { Slop = slop };

				foreach (var token in tokens)
					query.Add(new Term(field, token.Term));

				return query;
			}

			return base.GetFieldQuery(field, queryText, slop);
		}

		protected override Query GetPrefixQuery(string field, string termStr)
		{
			if (string.IsNullOrEmpty(field) || field == AnyField)
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (userField.IsNumericField())
						continue;

					var specificFieldQuery = GetPrefixQuery(userField, termStr);

					if (specificFieldQuery == null)
						continue;

					result.Add(specificFieldQuery, Occur.SHOULD);
				}

				if (result.Clauses.Count == 0)
					return _matchNothingQuery;

				return result;
			}

			field = localize(field);
			return base.GetPrefixQuery(field, termStr);
		}

		protected override Query GetWildcardQuery(string field, string termStr)
		{
			if (string.IsNullOrEmpty(field) || field == AnyField)
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (userField.IsNumericField())
						continue;

					var specificFieldQuery = GetWildcardQuery(userField, termStr);

					if (specificFieldQuery == null)
						continue;

					result.Add(specificFieldQuery, Occur.SHOULD);
				}

				if (result.Clauses.Count == 0)
					return _matchNothingQuery;

				return result;
			}

			if (field.IsFloatField() && !isValueFloat(termStr))
				return _matchNothingQuery;

			if (field.IsIntField() && !isValueInt(termStr))
				return _matchNothingQuery;

			field = localize(field);
			return base.GetWildcardQuery(field, termStr);
		}



		private Query getMoreLikeQuery(string queryText, float slop)
		{
			if (string.IsNullOrEmpty(queryText))
				return _matchNothingQuery;

			if (!_repository.IsLoadingComplete)
				return _matchNothingQuery;

			string cardName = queryText.RemoveDiacritics();
			if (!_repository.CardsByName.TryGetValue(cardName, out var cards))
				return _matchNothingQuery;

			var card = cards[0];

			var result = new DisjunctionMaxQuery(0.1f);

			if (!string.IsNullOrEmpty(card.TextEn))
				result.Add(createMoreLikeThisQuery(slop, card.TextEn, nameof(card.TextEn)));

			if (!string.IsNullOrEmpty(card.GeneratedMana))
				result.Add(createMoreLikeThisQuery(slop, card.GeneratedMana, nameof(card.GeneratedMana)));

			if (result.Disjuncts.Count == 0)
				return _matchNothingQuery;

			return result;
		}

		private MoreLikeThisQuery createMoreLikeThisQuery(float slop, string value, string field)
		{
			if (slop <= 0f)
				slop = 0.6f;
			else if (slop > 1f)
				slop = 1f;

			field = field.ToLower(Str.Culture);
			return new MoreLikeThisQuery(
				value,
				Array.From(field),
				Analyzer,
				field)
			{
				PercentTermsToMatch = slop,
				MaxQueryTerms = 20,
				MinDocFreq = 3,
				StopWords = _moreLikeStopWords
			};
		}

		private static bool isValueFloat(string queryText)
		{
			return float.TryParse(queryText, NumberStyles.Float, Str.Culture, out _);
		}

		private static bool isValueInt(string queryText)
		{
			return int.TryParse(queryText, NumberStyles.Integer, Str.Culture, out _);
		}

		private static Query createRangeQuery(string field, TermRangeQuery query)
		{
			var lowerValue = getRangeValue(query.LowerTerm);
			var upperValue = getRangeValue(query.UpperTerm);
			var result = TermRangeQuery.NewStringRange(field, lowerValue, upperValue, query.IncludesLower, query.IncludesUpper);
			return result;
		}

		private static Query createRangeQuery<TVal>(string field, TermRangeQuery query, Parser<TVal> parser, RangeQueryFactory<TVal> rangeQueryFactory)
			where TVal : struct, IComparable<TVal>
		{
			var lowerValue = getRangeValue(field, parser, query.LowerTerm);
			var upperValue = getRangeValue(field, parser, query.UpperTerm);
			var result = rangeQueryFactory(field, lowerValue, upperValue, query.IncludesLower, query.IncludesUpper);
			return result;
		}

		private static Query createRangeQuery<TVal>(string field, string queryText, Parser<TVal> parser, RangeQueryFactory<TVal> rangeQueryFactory)
			where TVal : struct, IComparable<TVal>
		{
			var value = getRangeValue(field, parser, new BytesRef(queryText));
			var result = rangeQueryFactory(field, value, value, true, true);
			return result;
		}

		private static string getRangeValue(BytesRef term)
		{
			if (!isNumberSpecified(term))
				return null;

			return term.Utf8ToString();
		}

		private static TVal? getRangeValue<TVal>(string field, Parser<TVal> parser, BytesRef term)
			where TVal : struct, IComparable<TVal>
		{
			if (!isNumberSpecified(term))
				return null;

			if (!parser(term, out var value))
				throw new ParseException($"Non-numeric value '{term}' for numeric field {field}");

			return value;
		}

		private static bool isNumberSpecified(BytesRef term)
		{
			if (term == null)
				return false;

			var str = term.Utf8ToString();
			return !string.IsNullOrEmpty(str) && !_nonSpecifiedNubmer.Contains(str);
		}

		private string localize(string field)
		{
			return field.GetFieldLocalizedIn(Language);
		}



		public const string AnyField = "*";
		public const string AnyValue = "*";

		public const string Like = nameof(Like);

		public string Language { get; set; }

		private delegate bool Parser<TVal>(BytesRef value, out TVal result)
			where TVal : struct;

		private delegate Query RangeQueryFactory<TVal>(string field, TVal? lower, TVal? upper, bool includeLower, bool includeUpper)
			where TVal : struct, IComparable<TVal>;

		private static readonly HashSet<string> _nonSpecifiedNubmer = new HashSet<string>
		{
			"*",
			"?"
		};

		private static readonly Query _matchNothingQuery = new TermQuery(new Term("none", "*"));

		private static readonly ISet<string> _moreLikeStopWords = new HashSet<string>(
			Sequence.From("a", "an", "the").Concat(MtgAplhabet.SingletoneWordChars.Select(c => new string(c, 1))),
			Str.Comparer);

		private readonly CardRepository _repository;
	}
}