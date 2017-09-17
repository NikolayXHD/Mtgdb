using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Version = Lucene.Net.Util.Version;

namespace Mtgdb.Dal.Index
{
	public class NumericAwareQueryParser : QueryParser
	{
		public string Language { get; set; }

		public HashSet<Term> NumericTerms { get; private set; }
		
		public NumericAwareQueryParser(Version matchVersion, string f, Analyzer a) : base(matchVersion, f, a)
		{
		}

		public override Query Parse(string query)
		{
			if (NumericTerms != null)
				throw new InvalidOperationException("New instance must be created to parse each query");

			NumericTerms = new HashSet<Term>();
			return base.Parse(query);
		}



		protected override Query GetFieldQuery(string field, string queryText)
		{
			field = localize(field);

			if (field.IsNumericField())
				NumericTerms.Add(new Term(field, queryText));

			if (field.IsFloatField())
				return createRangeQuery<float>(field, queryText, tryParseFloat, NumericRangeQuery.NewFloatRange);

			if (field.IsIntField())
				return createRangeQuery<int>(field, queryText, int.TryParse, NumericRangeQuery.NewIntRange);

			return base.GetFieldQuery(field, queryText);
		}

		protected override Query GetRangeQuery(string field, string part1, string part2, bool inclusive)
		{
			field = localize(field);

			var query = (TermRangeQuery) base.GetRangeQuery(field, part1, part2, inclusive);

			if (field.IsNumericField())
			{
				NumericTerms.Add(new Term(field, part1));
				NumericTerms.Add(new Term(field, part2));
			}

			if (field.IsFloatField())
				return createRangeQuery<float>(field, query, tryParseFloat, NumericRangeQuery.NewFloatRange);

			if (field.IsIntField())
				return createRangeQuery<int>(field, query, int.TryParse, NumericRangeQuery.NewIntRange);

			return query;
		}

		private static bool tryParseFloat(string s, out float f)
		{
			if (s.StartsWith("$"))
				return float.TryParse(s.Substring(1), out f);

			return float.TryParse(s, out f);
		}

		protected override Query GetFuzzyQuery(string field, string termStr, float minSimilarity)
		{
			field = localize(field);
			minSimilarity = minSimilarity.WithinRange(0.001f, 0.999f);
			return base.GetFuzzyQuery(field, termStr, minSimilarity);
		}

		protected override Query GetFieldQuery(string field, string queryText, int slop)
		{
			field = localize(field);
			return base.GetFieldQuery(field, queryText, slop);
		}

		protected override Query GetPrefixQuery(string field, string termStr)
		{
			field = localize(field);
			return base.GetPrefixQuery(field, termStr);
		}

		protected override Query GetWildcardQuery(string field, string termStr)
		{
			field = localize(field);
			return base.GetWildcardQuery(field, termStr);
		}



		private static NumericRangeQuery<TVal> createRangeQuery<TVal>(string field, TermRangeQuery query, Parser<TVal> parser, RangeQueryFactory<TVal> rangeQueryFactory) 
			where TVal: struct, IComparable<TVal>
		{
			var lowerValue = getValue(field, parser, query.LowerTerm);
			var upperValue = getValue(field, parser, query.UpperTerm);
			var result = rangeQueryFactory(field, lowerValue, upperValue, query.IncludesLower, query.IncludesUpper);
			return result;
		}

		private static NumericRangeQuery<TVal> createRangeQuery<TVal>(string field, string queryText, Parser<TVal> parser, RangeQueryFactory<TVal> rangeQueryFactory)
			where TVal : struct, IComparable<TVal>
		{
			var value = getValue(field, parser, queryText);
			var result = rangeQueryFactory(field, value, value, true, true);
			return result;
		}

		private static TVal? getValue<TVal>(string field, Parser<TVal> parser, string term) 
			where TVal : struct, IComparable<TVal>
		{
			if (!isNumberSpecified(term))
				return null;

			TVal lower;
			if (!parser(term, out lower))
				throw new ParseException($"Non-numeric value '{term}' for numeric field {field}");

			return lower;
		}

		private static bool isNumberSpecified(string term)
		{
			return !string.IsNullOrEmpty(term) && !_nonSpecifiedNubmer.Contains(term);
		}

		private string localize(string field)
		{
			return DocumentFactory.Normalize(field, Language);
		}



		private delegate bool Parser<TVal>(string value, out TVal result)
			where TVal : struct;

		private delegate NumericRangeQuery<TVal> RangeQueryFactory<TVal>(string field, TVal? lower, TVal? upper, bool includeLower, bool includeUpper)
			where TVal : struct, IComparable<TVal>;

		private static readonly HashSet<string> _nonSpecifiedNubmer = new HashSet<string>
		{
			"*",
			"?"
		};
	}
}