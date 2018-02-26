using System;
using System.Collections.Generic;
using System.Globalization;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Lucene.Net.Util.Automaton;

namespace Mtgdb.Dal.Index
{
	public class NumericAwareQueryParser : QueryParser
	{
		public NumericAwareQueryParser(LuceneVersion matchVersion, string f, Analyzer a)
			: base(matchVersion, f, a)
		{
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
			bool valueIsNumeric = isValueNumeric(queryText);

			if (string.IsNullOrEmpty(field) || field == AnyField)
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (!valueIsNumeric && userField.IsNumericField())
						continue;

					var specificFieldQuery = GetFieldQuery(userField, queryText, quoted);

					if (specificFieldQuery == null)
						continue;

					result.Add(specificFieldQuery, Occur.SHOULD);
				}

				return result;
			}

			field = localize(field);

			if (field.IsFloatField())
				return createRangeQuery<float>(field, queryText, tryParseFloat, NumericRangeQuery.NewSingleRange);

			if (field.IsIntField())
				return createRangeQuery<int>(field, queryText, tryParseInt, NumericRangeQuery.NewInt32Range);

			return base.GetFieldQuery(field, queryText, quoted);
		}

		private static bool isValueNumeric(string queryText)
		{
			bool valueIsNumeric =
				int.TryParse(queryText, NumberStyles.Integer, Str.Culture, out _) ||
				float.TryParse(queryText, NumberStyles.Float, Str.Culture, out _);
			return valueIsNumeric;
		}

		protected override Query GetRangeQuery(string field, string part1, string part2, bool startInclusive, bool endInclusive)
		{
			bool valuesAreNumeric =
				(isValueNumeric(part1) || _nonSpecifiedNubmer.Contains(part1)) &&
				(isValueNumeric(part2) || _nonSpecifiedNubmer.Contains(part2));

			if (string.IsNullOrEmpty(field) || field == AnyField)
			{
				var result = new BooleanQuery(disableCoord: true);

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (!valuesAreNumeric && userField.IsNumericField())
						continue;

					var specificFieldQuery = GetRangeQuery(userField, part1, part2, startInclusive, endInclusive);

					if (specificFieldQuery != null)
						result.Add(specificFieldQuery, Occur.SHOULD);
				}

				return result;
			}

			field = localize(field);

			var query = (TermRangeQuery) base.GetRangeQuery(field, part1, part2, startInclusive, endInclusive);

			if (field.IsFloatField())
				return createRangeQuery<float>(field, query, tryParseFloat, NumericRangeQuery.NewSingleRange);

			if (field.IsIntField())
				return createRangeQuery<int>(field, query, tryParseInt, NumericRangeQuery.NewInt32Range);

			return createRangeQuery(field, query);
		}

		private static bool tryParseInt(BytesRef bytes, out int f)
		{
			var s = bytes.Utf8ToString();
			if (s.StartsWith("$"))
				return int.TryParse(s.Substring(1), out f);

			return int.TryParse(s, out f);
		}

		private static bool tryParseFloat(BytesRef bytes, out float f)
		{
			var s = bytes.Utf8ToString();
			if (s.StartsWith("$"))
				return float.TryParse(s.Substring(1), out f);

			return float.TryParse(s, out f);
		}

		protected override Query GetFuzzyQuery(string field, string termStr, float minSimilarity)
		{
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

				return result;
			}

			field = localize(field);
			minSimilarity = minSimilarity.WithinRange(0.001f, 0.999f);
			return base.GetFuzzyQuery(field, termStr, minSimilarity);
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

				return result;
			}

			field = localize(field);
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

				return result;
			}

			field = localize(field);
			return base.GetWildcardQuery(field, termStr);
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
			return DocumentFactory.Localize(field, Language);
		}



		public const string AnyField = "*";

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
	}
}