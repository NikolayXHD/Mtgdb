using System;
using System.Collections.Generic;
using System.Globalization;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Lucene.Net.Util.Automaton;
using Token = Lucene.Net.QueryParsers.Classic.Token;

namespace Mtgdb.Data
{
	public abstract class MtgQueryParser : QueryParserPatched
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

		protected internal override Query HandleQuotedTerm(string field, Token termToken, Token slopToken)
		{
			string image = termToken.Image.Substring(1, termToken.Image.Length - 2);

			(bool IsFloat, bool IsInt) numericTypeGetter() =>
				(false, false);

			Query queryFactory(string fld, bool analyzed)
			{
				if (analyzed)
				{
					float slop = slopToken == null
						? 1f
						: float.Parse(slopToken.Image.Substring(1), CultureInfo.InvariantCulture);
					bool isInteger = (slop % 1).Equals(0f);
					var parser = new ComplexPhraseQueryParserPatched(LuceneVersion.LUCENE_48, fld, Analyzer, (int) slop, isInteger);
					return parser.Parse(image);
				}

				// not analyzed field cannot have phrases in value, so create TermQuery
				var unescaped = DiscardEscapeChar(image);
				return GetFieldQuery(fld, unescaped, quoted: true);
			}

			var result = resolveField(field, numericTypeGetter, queryFactory);
			return result;
		}

		protected internal override Query HandleBareTokenQuery(
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

			Query queryFactory(string fld, bool analyzed) =>
				base.HandleBareTokenQuery(fld, termToken, slopToken, prefix, wildcard, fuzzy, regexp);

			var result = resolveField(field, numericTypeGetter, queryFactory);
			return result;
		}

		protected internal override Query HandleBareFuzzy(string field, Token slopToken, string termImage)
		{
			(bool IsFloat, bool IsInt) numericTypeGetter() => (false, false);
			Query queryFactory(string fld, bool analyzed) => base.HandleBareFuzzy(fld, slopToken, termImage);

			var result = resolveField(field, numericTypeGetter, queryFactory);
			return result;
		}

		protected internal override Query GetRangeQuery(
			string field,
			string part1,
			string part2,
			bool startInclusive,
			bool endInclusive)
		{
			(bool IsFloat, bool IsInt) numericTypeGetter()
			{
				bool isFloat =
					(isValueFloat(part1) || _nonSpecifiedNumber.Contains(part1)) &&
					(isValueFloat(part2) || _nonSpecifiedNumber.Contains(part2));

				bool isInt =
					(isValueInt(part1) || _nonSpecifiedNumber.Contains(part1)) &&
					(isValueInt(part2) || _nonSpecifiedNumber.Contains(part2));

				return (isFloat, isInt);
			}

			Query queryFactory(string fld, bool analyzed) => base.GetRangeQuery(fld, part1, part2, startInclusive, endInclusive);

			var result = resolveField(field, numericTypeGetter, queryFactory);
			return result;
		}

		protected internal override Query NewRangeQuery(string field, string v1, string v2, bool v1Incl, bool v2Incl)
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


		protected internal override Query NewRegexpQuery(Term regexp) =>
			new RegexpQuery(regexp, RegExpSyntax.ALL);

		protected internal override Query GetFieldQuery(string field, string value, bool quoted)
		{
			if (_adapter.IsFloatField(field))
				return createRangeQuery<float>(field, value, IndexUtils.TryParseFloat,
					NumericRangeQuery.NewSingleRange);

			if (_adapter.IsIntField(field))
				return createRangeQuery<int>(field, value, IndexUtils.TryParseInt,
					NumericRangeQuery.NewInt32Range);

			return base.GetFieldQuery(field, value, quoted);
		}

		protected internal override Query GetFuzzyQuery(string field, string value, float minSimilarity) =>
			base.GetFuzzyQuery(field, value, minSimilarity.WithinRange(0.001f, 0.999f));

		protected internal override void AddClause(IList<BooleanClause> clauses, int conj, int mods, Query q)
		{
			if (q == null)
				base.AddClause(clauses, conj, mods, new EmptyPhraseQuery(Field));

			base.AddClause(clauses, conj, mods, q);
		}

		private Query resolveField(string field, Func<(bool IsFloat, bool IsInt)> numericTypeGetter, QueryFactory queryFactory)
		{
			if (_adapter.FieldByAlias.TryGetValue(field, out string actualField))
				field = actualField;

			(bool valueIsFloat, bool valueIsInt) = numericTypeGetter();

			if (_adapter.IsAnyField(field))
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in ExpandAnyField())
				{
					if (!valueIsFloat && _adapter.IsFloatField(userField))
						continue;

					if (!valueIsInt && _adapter.IsIntField(userField))
						continue;

					var specificFieldQuery = queryFactory(_adapter.GetFieldLocalizedIn(userField, Language), !_adapter.IsNotAnalyzed(userField));

					if (specificFieldQuery == null)
						continue;

					result.Add(specificFieldQuery, Occur.SHOULD);
				}

				if (result.Clauses.Count == 0)
					return MatchNothingQuery;

				return result;
			}

			if (!valueIsFloat && _adapter.IsFloatField(field))
				return MatchNothingQuery;

			if (!valueIsInt && _adapter.IsIntField(field))
				return MatchNothingQuery;

			return queryFactory(_adapter.GetFieldLocalizedIn(field, Language), !_adapter.IsNotAnalyzed(field));
		}

		protected virtual IEnumerable<string> ExpandAnyField() =>
			_adapter.GetUserFields();

		private (bool IsFloat, bool IsInt) getNumericTypes(string termImage)
		{
			string value = DiscardEscapeChar(termImage);
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
			return !string.IsNullOrEmpty(str) && !_nonSpecifiedNumber.Contains(str);
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

		private static readonly HashSet<string> _nonSpecifiedNumber = new HashSet<string>
		{
			"*",
			"?"
		};

		protected static readonly Query MatchNothingQuery = new TermQuery(new Term("none", "*"));

		private readonly IDocumentAdapterBase _adapter;
	}
}
