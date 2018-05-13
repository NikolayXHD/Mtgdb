using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lucene.Net.Contrib;
using Lucene.Net.Index;
using Lucene.Net.Queries.Mlt;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.QueryParsers.ComplexPhrase;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Token = Lucene.Net.QueryParsers.Classic.Token;

namespace Mtgdb.Dal.Index
{
	public class MtgQueryParser : ComplexPhraseQueryParser
	{
		public MtgQueryParser(
			LuceneVersion matchVersion,
			string f,
			MtgAnalyzer analyzer,
			CardRepository repository,
			string language)
			: base(matchVersion, f, analyzer)
		{
			_repository = repository;
			Language = language;
			FuzzyMinSim = 0.5f;
			AllowLeadingWildcard = true;
			AutoGeneratePhraseQueries = true;
		}

		public override Query Parse(string query)
		{
			var result = base.Parse(query);
			return result;
		}

		protected override Query HandleQuotedTerm(string field, Token termToken, Token slopToken)
		{
			string termImage = termToken.Image.Substring(1, termToken.Image.Length - 2);

			Query moreLikeFactory() => getMoreLikeQuery(termImage, slopToken);
			(bool IsFloat, bool IsInt) numericTypeGetter() => getNumericTypes(termImage);
			Query queryFactory(string fld, bool analyzed)
			{
				if (analyzed)
					return base.HandleQuotedTerm(fld, termToken, slopToken);

				var notAnalyzedQuery = GetFieldQuery(fld, termToken.Image.Substring(1, termToken.Image.Length - 2), true);
				return notAnalyzedQuery;
			}

			var result = resolveField(field, moreLikeFactory, numericTypeGetter, queryFactory);
			return result;
		}

		protected override Query HandleBareTokenQuery(
			string field,
			Token termToken,
			Token slopToken,
			bool prefix,
			bool wildcard,
			bool fuzzy,
			bool regexp)
		{
			(bool IsFloat, bool IsInt) numericTypesGetter()
			{
				if (prefix || wildcard || regexp || fuzzy)
					return (false, false);

				return getNumericTypes(termToken.Image);
			}

			Query moreLikeFactory()
			{
				if (prefix || wildcard || regexp || fuzzy)
					return _matchNothingQuery;

				return getMoreLikeQuery(termToken.Image, slopToken);
			}

			Query queryFactory(string fld, bool anazyed) => base.HandleBareTokenQuery(fld, termToken, slopToken, prefix, wildcard, fuzzy, regexp);

			var result = resolveField(field, moreLikeFactory, numericTypesGetter, queryFactory);
			return result;
		}

		protected override Query HandleBareFuzzy(string field, Token slopToken, string termImage)
		{
			Query moreLikeFactory() => _matchNothingQuery;
			(bool IsFloat, bool IsInt) numericTypeGetter() => (false, false);
			Query queryFactory(string fld, bool anazyed) => base.HandleBareFuzzy(fld, slopToken, termImage);

			var result = resolveField(field, moreLikeFactory, numericTypeGetter, queryFactory);
			return result;
		}

		protected override Query GetRangeQuery(
			string field,
			string part1,
			string part2,
			bool startInclusive,
			bool endInclusive)
		{
			(bool IsFloat, bool IsInt) numericTypeGetter()
			{
				bool isFloat = 
					(isValueFloat(part1) || _nonSpecifiedNubmer.Contains(part1)) && 
					(isValueFloat(part2) || _nonSpecifiedNubmer.Contains(part2));

				bool isInt = 
					(isValueInt(part1) || _nonSpecifiedNubmer.Contains(part1)) && 
					(isValueInt(part2) || _nonSpecifiedNubmer.Contains(part2));

				return (isFloat, isInt);
			}

			Query moreLikeFactory() => _matchNothingQuery;
			Query queryFactory(string fld, bool anazyed) => base.GetRangeQuery(fld, part1, part2, startInclusive, endInclusive);

			var result = resolveField(field, moreLikeFactory, numericTypeGetter, queryFactory);
			return result;
		}

		protected override Query NewRangeQuery(string field, string v1, string v2, bool v1Incl, bool v2Incl)
		{
			var query = (TermRangeQuery) base.NewRangeQuery(field, v1, v2, v1Incl, v2Incl);

			if (field.IsFloatField())
				return createRangeQuery<float>(field, query, IndexUtils.TryParseFloat,
					NumericRangeQuery.NewSingleRange);

			if (field.IsIntField())
				return createRangeQuery<int>(field, query, IndexUtils.TryParseInt,
					NumericRangeQuery.NewInt32Range);

			return createRangeQuery(field, query);
		}


		protected override Query GetRegexpQuery(string field, string value)
		{
			if (field.IsNumericField())
				return _matchNothingQuery;

			return base.GetRegexpQuery(field, value);
		}

		protected override Query NewRegexpQuery(Term regexp) =>
			new MtgRegexpQuery(regexp);



		protected override Query GetComplexPhraseQuery(string field, Token fuzzySlop, string phrase)
		{
			if (field.IsNumericField())
				return _matchNothingQuery;

			return base.GetComplexPhraseQuery(field, fuzzySlop, phrase);
		}

		protected override ComplexPhraseQuery NewComplexPhraseQuery(string qfield, string phrase, int slop, bool inOrder)
		{
			return new RewriteableComplexPhraseQuery(qfield, phrase, slop, inOrder);
		}

		protected override Query GetFieldQuery(string field, string value, bool quoted)
		{
			if (field.IsFloatField())
				return createRangeQuery<float>(field, value, IndexUtils.TryParseFloat,
					NumericRangeQuery.NewSingleRange);

			if (field.IsIntField())
				return createRangeQuery<int>(field, value, IndexUtils.TryParseInt,
					NumericRangeQuery.NewInt32Range);

			return base.GetFieldQuery(field, value, quoted);
		}

		protected override Query GetFieldQuery(string field, string value, int slop)
		{
			var result = base.GetFieldQuery(field, value, slop);
			return result;
		}



		protected override Query GetFuzzyQuery(string field, string value, float minSimilarity)
		{
			minSimilarity = minSimilarity.WithinRange(0.001f, 0.999f);
			return base.GetFuzzyQuery(field, value, minSimilarity);
		}

		protected override Query GetPrefixQuery(string field, string value)
		{
			if (IsAnyField(field) && !IsPass2ResolvingPhrases)
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (userField.IsNumericField())
						continue;

					var specificFieldQuery = base.GetPrefixQuery(userField, value);

					if (specificFieldQuery == null)
						continue;

					result.Add(specificFieldQuery, Occur.SHOULD);
				}

				if (result.Clauses.Count == 0)
					return _matchNothingQuery;

				return result;
			}

			return base.GetPrefixQuery(field, value);
		}

		protected override Query GetWildcardQuery(string field, string escapedValue)
		{
			if (IsAnyField(field) && !IsPass2ResolvingPhrases)
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (userField.IsNumericField())
						continue;

					var specificFieldQuery = base.GetWildcardQuery(userField, escapedValue);

					if (specificFieldQuery == null)
						continue;

					result.Add(specificFieldQuery, Occur.SHOULD);
				}

				if (result.Clauses.Count == 0)
					return _matchNothingQuery;

				return result;
			}

			var value = StringEscaper.Unescape(escapedValue);

			if (field.IsFloatField() && !isValueFloat(value))
				return _matchNothingQuery;

			if (field.IsIntField() && !isValueInt(value))
				return _matchNothingQuery;

			return base.GetWildcardQuery(field, escapedValue);
		}



		private Query resolveField(
			string field,
			Func<Query> moreLikeFactory,
			Func<(bool IsFloat, bool IsInt)> numericTypeGetter,
			QueryFactory queryFactory)
		{
			if (Str.Equals(Like, field))
				return moreLikeFactory();

			bool isAnalyzed(string userField) =>
				!DocumentFactory.NotAnalyzedFields.Contains(userField);

			if (IsAnyField(field))
			{
				(bool valueIsFloat, bool valueIsInt) = numericTypeGetter();

				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (!valueIsFloat && userField.IsFloatField())
						continue;

					if (!valueIsInt && userField.IsIntField())
						continue;

					var specificFieldQuery = queryFactory(localize(userField), isAnalyzed(userField));

					if (specificFieldQuery == null)
						continue;

					result.Add(specificFieldQuery, Occur.SHOULD);
				}

				if (result.Clauses.Count == 0)
					return _matchNothingQuery;
				
				return result;
			}

			return queryFactory(localize(field), isAnalyzed(field));
		}

		private Query getMoreLikeQuery(string termImage, Token slopToken)
		{
			string unescaped = StringEscaper.Unescape(termImage);
			float slop = parseSlop(slopToken);

			if (string.IsNullOrEmpty(unescaped))
				return _matchNothingQuery;

			if (!_repository.IsLoadingComplete)
				return _matchNothingQuery;

			string cardName = unescaped.RemoveDiacritics();
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

		private static float parseSlop(Token fuzzySlop)
		{
			if (fuzzySlop == null)
				return 0f;

			if (fuzzySlop.Image.Length <= 1)
				return 0f;

			float.TryParse(fuzzySlop.Image.Substring(1), NumberStyles.Float, Str.Culture, out var slop);

			return slop;
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



		private static (bool IsFloat, bool IsInt) getNumericTypes(string termImage)
		{
			string value = StringEscaper.Unescape(termImage);
			bool isFloat = isValueFloat(value);
			bool isInt = isValueInt(value);

			return (isFloat, isInt);
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
			var result = TermRangeQuery.NewStringRange(field, lowerValue, upperValue, query.IncludesLower,
				query.IncludesUpper);
			return result;
		}



		private static Query createRangeQuery<TVal>(
			string field,
			TermRangeQuery query,
			Parser<TVal> parser,
			RangeQueryFactory<TVal> rangeQueryFactory)
			where TVal : struct, IComparable<TVal>
		{
			var lowerValue = getRangeValue(field, parser, query.LowerTerm);
			var upperValue = getRangeValue(field, parser, query.UpperTerm);
			var result = rangeQueryFactory(field, lowerValue, upperValue, query.IncludesLower,
				query.IncludesUpper);
			return result;
		}

		private static Query createRangeQuery<TVal>(
			string field,
			string queryText,
			Parser<TVal> parser,
			RangeQueryFactory<TVal> rangeQueryFactory)
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

		public static bool IsAnyField(string field)
			=> string.IsNullOrEmpty(field) || field == AnyField;



		private const string AnyField = "*";
		public const string AnyValue = "*";

		public const string Like = nameof(Like);

		public string Language { get; }

		private delegate bool Parser<TVal>(BytesRef value, out TVal result)
			where TVal : struct;

		private delegate Query RangeQueryFactory<TVal>(
			string field,
			TVal? lower,
			TVal? upper,
			bool includeLower,
			bool includeUpper)
			where TVal : struct, IComparable<TVal>;

		private delegate Query QueryFactory(string field, bool isAnalyzed);

		private static readonly HashSet<string> _nonSpecifiedNubmer = new HashSet<string>
		{
			"*",
			"?"
		};

		private static readonly Query _matchNothingQuery = new TermQuery(new Term("none", "*"));

		private static readonly ISet<string> _moreLikeStopWords = new HashSet<string>(
			Sequence.From("a", "an", "the")
				.Concat(MtgAplhabet.SingletoneWordChars.Select(c => new string(c, 1))),
			Str.Comparer);

		private readonly CardRepository _repository;

		public sealed override float FuzzyMinSim
		{
			get => base.FuzzyMinSim;
			set => base.FuzzyMinSim = value;
		}

		public sealed override bool AllowLeadingWildcard
		{
			get => base.AllowLeadingWildcard;
			set => base.AllowLeadingWildcard = value;
		}
	}
}