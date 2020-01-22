using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using Lucene.Net.Search.Spans;

namespace Mtgdb.Data
{
	/*
	 * Licensed to the Apache Software Foundation (ASF) under one or more
	 * contributor license agreements.  See the NOTICE file distributed with
	 * this work for additional information regarding copyright ownership.
	 * The ASF licenses this file to You under the Apache License, Version 2.0
	 * (the "License"); you may not use this file except in compliance with
	 * the License.  You may obtain a copy of the License at
	 *
	 *     http://www.apache.org/licenses/LICENSE-2.0
	 *
	 * Unless required by applicable law or agreed to in writing, software
	 * distributed under the License is distributed on an "AS IS" BASIS,
	 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	 * See the License for the specific language governing permissions and
	 * limitations under the License.
	 */

	/// <summary>
	/// <see cref="QueryParser"/> which permits complex phrase query syntax eg "(john jon
	/// jonathan~) peters*".
	/// <para>
	/// Performs potentially multiple passes over Query text to parse any nested
	/// logic in PhraseQueries. - First pass takes any PhraseQuery content between
	/// quotes and stores for subsequent pass. All other query content is parsed as
	/// normal - Second pass parses any stored PhraseQuery content, checking all
	/// embedded clauses are referring to the same field and therefore can be
	/// rewritten as Span queries. All PhraseQuery clauses are expressed as
	/// ComplexPhraseQuery objects
	/// </para>
	/// <para>
	/// This could arguably be done in one pass using a new <see cref="QueryParser"/> but here I am
	/// working within the constraints of the existing parser as a base class. This
	/// currently simply feeds all phrase content through an analyzer to select
	/// phrase terms - any "special" syntax such as * ~ * etc are not given special
	/// status
	/// </para>
	/// </summary>
	public class ComplexPhraseQueryParserPatched : QueryParserPatched
	{
		public ComplexPhraseQueryParserPatched(LuceneVersion matchVersion,
			string f,
			Analyzer a,
			int slop,
			bool inOrder)
			: base(matchVersion, f, a)
		{
			_slop = slop;
			_inOrder = inOrder;
		}

		public override Query Parse(string query)
		{
			var contents = base.Parse(query);
			if (contents is BooleanQuery bq)
			{
				SpanQuery spanQuery = convertBooleanQuery(bq, _slop, _inOrder);
				return spanQuery;
			}

			return contents;
		}

		// There is No "getTermQuery throws ParseException" method to override so
		// unfortunately need
		// to throw a runtime exception here if a term for another field is embedded
		// in phrase query
		protected override Query NewTermQuery(Term term)
		{
			try
			{
				validateField(term.Field);
			}
			catch (ParseException pe)
			{
				throw new Exception("Error parsing complex phrase", pe);
			}

			return base.NewTermQuery(term);
		}

		protected internal override Query GetWildcardQuery(string field, string termStr)
		{
			validateField(field);
			return base.GetWildcardQuery(field, termStr);
		}

		protected internal override Query GetRangeQuery(string field, string part1, string part2, bool startInclusive, bool endInclusive)
		{
			validateField(field);
			return base.GetRangeQuery(field, part1, part2, startInclusive, endInclusive);
		}

		protected internal override Query NewRangeQuery(string field, string part1, string part2, bool startInclusive, bool endInclusive)
		{
			// Must use old-style RangeQuery in order to produce a BooleanQuery
			// that can be turned into SpanOr clause
			TermRangeQuery rangeQuery = TermRangeQuery.NewStringRange(field, part1, part2, startInclusive, endInclusive);
			rangeQuery.MultiTermRewriteMethod = MultiTermQuery.SCORING_BOOLEAN_QUERY_REWRITE;
			return rangeQuery;
		}

		protected internal override Query GetFuzzyQuery(string field, string termStr, float minSimilarity)
		{
			validateField(field);
			return base.GetFuzzyQuery(field, termStr, minSimilarity);
		}

		/// <summary>
		/// Helper method used to report on any clauses that appear in query syntax
		/// </summary>
		private void validateField(string field)
		{
			if (!field.Equals(Field))
				throw new ParseException($"Clause for field \"{field}\": nested in phrase for \"{Field}\":");
		}

		protected internal override void AddClause(IList<BooleanClause> clauses, int conj, int mods, Query q)
		{
			base.AddClause(clauses, conj, mods, q);
			var clause = clauses[clauses.Count - 1];
			_conjByClause.Add(clause, conj);
		}

		private SpanQuery convertBooleanQuery(BooleanQuery boolQuery, int slop, bool ordered)
		{
			var or = new List<SpanQuery>();
			var not = new List<SpanQuery>();
			var and = new List<SpanQuery>();
			var phrase = new List<SpanQuery>();

			// For all clauses e.g. one* two~
			var clauses = boolQuery.GetClauses();
			for (int i = 0; i < clauses.Length; i++)
			{
				var clause = clauses[i];
				phrase.Clear();

				for (int n = i + 1; n < clauses.Length; n++)
				{
					var nextClause = clauses[n];
					if (isImplicit(nextClause))
					{
						if (phrase.Count == 0)
							phrase.Add(convert(clause));

						phrase.Add(convert(nextClause));
						i++;
					}
					else
						break;
				}

				// select the list to which we will add these options
				var list = clause.Occur switch
				{
					Occur.MUST => and,
					Occur.SHOULD => or,
					Occur.MUST_NOT => not,
					_ => throw new NotSupportedException()
				};

				if (phrase.Count > 0)
					list.Add(new SpanNearQuery(phrase.ToArray(), slop, ordered));
				else
					list.Add(convert(clause));
			}

			int groupsCount = Math.Sign(and.Count) + Math.Sign(or.Count) + Math.Sign(not.Count);
			if (groupsCount == 0)
				// Insert fake term e.g. phrase query was for "Fred Smithe*" and
				// there were no "Smithe*" terms - need to
				// prevent match on just "Fred".
				return matchNothingQuery();

			if (groupsCount > 1 || and.Count > 0)
			{
				if (or.Count > 0)
					and.Add(orQuery(or));

				if (not.Count > 0)
					return notQuery(andQuery(and), andQuery(not));

				return andQuery(and);
			}

			if (or.Count > 0)
				return orQuery(or);

			throw new ParseException("Pure negative clause within phrase: " + boolQuery);

			SpanQuery convert(BooleanClause clause) =>
				clause.Query is BooleanQuery booleanQuery
					? convertBooleanQuery(booleanQuery, 1, true)
					: wrap(clause.Query);

			bool isImplicit(BooleanClause clause) =>
				!_conjByClause.TryGetValue(clause, out int conj) || conj == CONJ_NONE;

			static SpanQuery wrap(Query qc) =>
				qc switch
				{
					MultiTermQuery query => (SpanQuery) new SpanMultiTermQueryWrapper<MultiTermQuery>(query) { Boost = query.Boost },
					TermQuery query => new SpanTermQuery(query.Term) { Boost = query.Boost },
					_ => throw new NotSupportedException("Unknown query type:" + qc.GetType().Name)
				};
		}

		private static SpanQuery andQuery(List<SpanQuery> qs) =>
			qs.Count > 1 ? new SpanNearQuery(qs.ToArray(), int.MaxValue, false) : qs[0];

		private static SpanQuery orQuery(List<SpanQuery> qs) =>
			qs.Count > 1 ? new SpanOrQuery(qs.ToArray()) : qs[0];

		private static SpanQuery notQuery(SpanQuery and, SpanQuery not) =>
			new SpanNotQuery(and, not);

		private SpanQuery matchNothingQuery() =>
			new SpanTermQuery(new Term(Field, "fake_term_to_match_nothing"));

		private readonly int _slop;
		private readonly bool _inOrder;

		private readonly Dictionary<BooleanClause, int> _conjByClause =
			new Dictionary<BooleanClause, int>();
	}
}
