using System;
using System.Collections.Generic;
using System.Globalization;
using Lucene.Net.Contrib;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.QueryParsers.ComplexPhrase;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Token = Lucene.Net.QueryParsers.Classic.Token;

namespace Mtgdb.Index
{
	public abstract class MtgQueryParser : ComplexPhraseQueryParser
	{
		protected MtgQueryParser(MtgAnalyzer analyzer, IDocumentAdapterBase adapter, string lang)
			: base(LuceneVersion.LUCENE_48, "*", analyzer)
		{
			_adapter = adapter;
			Language = lang;
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
			string image = termToken.Image.Substring(1, termToken.Image.Length - 2);
			string value = StringEscaper.Unescape(image);

			(bool IsFloat, bool IsInt) numericTypeGetter() => getNumericTypes(value);

			Query queryFactory(string fld, bool analyzed)
			{
				if (analyzed)
					return base.HandleQuotedTerm(fld, termToken, slopToken);

				var notAnalyzedQuery = GetFieldQuery(fld, value, quoted: true);
				return notAnalyzedQuery;
			}

			var result = resolveField(field, numericTypeGetter, queryFactory);
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
			(bool IsFloat, bool IsInt) numericTypeGetter()
			{
				if (prefix || wildcard || regexp || fuzzy)
					return (false, false);

				return getNumericTypes(termToken.Image);
			}

			Query queryFactory(string fld, bool anazyed) =>
				base.HandleBareTokenQuery(fld, termToken, slopToken, prefix, wildcard, fuzzy, regexp);

			var result = resolveField(field, numericTypeGetter, queryFactory);
			return result;
		}

		protected override Query HandleBareFuzzy(string field, Token slopToken, string termImage)
		{
			(bool IsFloat, bool IsInt) numericTypeGetter() => (false, false);
			Query queryFactory(string fld, bool anazyed) => base.HandleBareFuzzy(fld, slopToken, termImage);

			var result = resolveField(field, numericTypeGetter, queryFactory);
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

			Query queryFactory(string fld, bool anazyed) => base.GetRangeQuery(fld, part1, part2, startInclusive, endInclusive);

			var result = resolveField(field, numericTypeGetter, queryFactory);
			return result;
		}

		protected override Query NewRangeQuery(string field, string v1, string v2, bool v1Incl, bool v2Incl)
		{
			var query = (TermRangeQuery) base.NewRangeQuery(field, v1, v2, v1Incl, v2Incl);

			if (_adapter.IsFloatField(field))
				return createRangeQuery<float>(field, query, IndexUtils.TryParseFloat,
					NumericRangeQuery.NewSingleRange);

			if (_adapter.IsIntField(field))
				return createRangeQuery<int>(field, query, IndexUtils.TryParseInt,
					NumericRangeQuery.NewInt32Range);

			return createRangeQuery(field, query);
		}


		protected override Query GetRegexpQuery(string field, string value)
		{
			if (_adapter.IsNumericField(field))
				return _matchNothingQuery;

			return base.GetRegexpQuery(field, value);
		}

		protected override Query NewRegexpQuery(Term regexp) =>
			new MtgRegexpQuery(regexp);



		protected override Query GetComplexPhraseQuery(string field, Token fuzzySlop, string phrase)
		{
			if (_adapter.IsNumericField(field))
				return _matchNothingQuery;

			return base.GetComplexPhraseQuery(field, fuzzySlop, phrase);
		}

		protected override ComplexPhraseQuery NewComplexPhraseQuery(string qfield, string phrase, int slop, bool inOrder)
		{
			return new RewriteableComplexPhraseQuery(qfield, phrase, slop, inOrder);
		}

		protected override Query GetFieldQuery(string field, string value, bool quoted)
		{
			if (_adapter.IsFloatField(field))
				return createRangeQuery<float>(field, value, IndexUtils.TryParseFloat,
					NumericRangeQuery.NewSingleRange);

			if (_adapter.IsIntField(field))
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
			if (_adapter.IsAnyField(field) && !IsPass2ResolvingPhrases)
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in ExpandAnyField())
				{
					if (_adapter.IsNumericField(userField))
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
			if (_adapter.IsAnyField(field) && !IsPass2ResolvingPhrases)
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in ExpandAnyField())
				{
					if (_adapter.IsNumericField(userField))
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

			if (_adapter.IsFloatField(field) && !isValueFloat(value))
				return _matchNothingQuery;

			if (_adapter.IsIntField(field) && !isValueInt(value))
				return _matchNothingQuery;

			return base.GetWildcardQuery(field, escapedValue);
		}

		protected override void AddClause(IList<BooleanClause> clauses, int conj, int mods, Query q)
		{
			if (q == null)
				base.AddClause(clauses, conj, mods, new EmptyPhraseQuery(Field));

			base.AddClause(clauses, conj, mods, q);
		}

		private Query resolveField(
			string field,
			Func<(bool IsFloat, bool IsInt)> numericTypeGetter,
			QueryFactory queryFactory)
		{
			bool isAnalyzed(string userField) =>
				!_adapter.IsNotAnalyzed(userField);

			if (_adapter.IsAnyField(field))
			{
				(bool valueIsFloat, bool valueIsInt) = numericTypeGetter();

				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in ExpandAnyField())
				{
					if (!valueIsFloat && _adapter.IsFloatField(userField))
						continue;

					if (!valueIsInt && _adapter.IsIntField(userField))
						continue;

					var specificFieldQuery = queryFactory(_adapter.GetFieldLocalizedIn(userField, Language), isAnalyzed(userField));

					if (specificFieldQuery == null)
						continue;

					result.Add(specificFieldQuery, Occur.SHOULD);
				}

				if (result.Clauses.Count == 0)
					return _matchNothingQuery;

				return result;
			}

			return queryFactory(_adapter.GetFieldLocalizedIn(field, Language), isAnalyzed(field));
		}

		protected virtual IEnumerable<string> ExpandAnyField() =>
			_adapter.GetUserFields();

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



		protected string Language { get; }

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


		public const string AnyValue = "*";

		private static readonly HashSet<string> _nonSpecifiedNubmer = new HashSet<string>
		{
			"*",
			"?"
		};

		protected static readonly Query _matchNothingQuery = new TermQuery(new Term("none", "*"));

		private readonly IDocumentAdapterBase _adapter;
	}
}