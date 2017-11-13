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
		public const string AnyField = "*";

		public string Language { get; set; }

		public HashSet<Term> NumericTerms { get; private set; }
		
		public NumericAwareQueryParser(LuceneVersion matchVersion, string f, Analyzer a) : base(matchVersion, f, a)
		{
		}

		public override Query Parse(string query)
		{
			if (NumericTerms != null)
				throw new InvalidOperationException("New instance must be created to parse each query");

			NumericTerms = new HashSet<Term>();
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

			if (field.IsNumericField())
				NumericTerms.Add(new Term(field, queryText));

			if (field.IsFloatField())
				return createRangeQuery<float>(field, queryText, tryParseFloat, NumericRangeQuery.NewSingleRange);

			if (field.IsIntField())
				return createRangeQuery<int>(field, queryText, int.TryParse, NumericRangeQuery.NewInt32Range);

			return base.GetFieldQuery(field, queryText, quoted);
		}

		private static bool isValueNumeric(string queryText)
		{
			int intVal;
			float floatVal;

			bool valueIsNumeric =
				int.TryParse(queryText, NumberStyles.Integer, CultureInfo.InvariantCulture, out intVal) ||
				float.TryParse(queryText, NumberStyles.Float, CultureInfo.InvariantCulture, out floatVal);
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

			if (field.IsNumericField())
			{
				NumericTerms.Add(new Term(field, part1));
				NumericTerms.Add(new Term(field, part2));
			}

			if (field.IsFloatField())
				return createRangeQuery<float>(field, query, tryParseFloat, NumericRangeQuery.NewSingleRange);

			if (field.IsIntField())
				return createRangeQuery<int>(field, query, int.TryParse, NumericRangeQuery.NewInt32Range);

			return createRangeQuery(field, query);
		}

		private static bool tryParseFloat(string s, out float f)
		{
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
			var lowerValue = getValue(query.LowerTerm.Utf8ToString());
			var upperValue = getValue(query.UpperTerm.Utf8ToString());
			var result = new TermRangeQuery(field, new BytesRef(lowerValue), new BytesRef(upperValue), query.IncludesLower, query.IncludesUpper);

			return result;
		}

		private static Query createRangeQuery<TVal>(string field, TermRangeQuery query, Parser<TVal> parser, RangeQueryFactory<TVal> rangeQueryFactory) 
			where TVal: struct, IComparable<TVal>
		{
			var lowerValue = getValue(field, parser, query.LowerTerm.Utf8ToString());
			var upperValue = getValue(field, parser, query.UpperTerm.Utf8ToString());
			var result = rangeQueryFactory(field, lowerValue, upperValue, query.IncludesLower, query.IncludesUpper);
			return result;
		}

		private static Query createRangeQuery<TVal>(string field, string queryText, Parser<TVal> parser, RangeQueryFactory<TVal> rangeQueryFactory)
			where TVal : struct, IComparable<TVal>
		{
			var value = getValue(field, parser, queryText);
			var result = rangeQueryFactory(field, value, value, true, true);
			return result;
		}

		private static string getValue(string term)
		{
			if (!isNumberSpecified(term))
				return null;

			return term;
		}

		private static TVal? getValue<TVal>(string field, Parser<TVal> parser, string term) 
			where TVal : struct, IComparable<TVal>
		{
			if (!isNumberSpecified(term))
				return null;

			TVal value;
			if (!parser(term, out value))
				throw new ParseException($"Non-numeric value '{term}' for numeric field {field}");

			return value;
		}

		private static bool isNumberSpecified(string term)
		{
			return !string.IsNullOrEmpty(term) && !_nonSpecifiedNubmer.Contains(term);
		}

		private string localize(string field)
		{
			return DocumentFactory.Localize(field, Language);
		}



		private delegate bool Parser<TVal>(string value, out TVal result)
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