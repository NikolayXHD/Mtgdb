using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
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
	public class PhraseQueryParser : QueryParserPatched
	{
		public PhraseQueryParser(string f, Analyzer a)
			: base(LuceneVersion.LUCENE_48, f, a)
		{
			_emptyPhrase = new string(MtgAlphabet.EmptyPhraseChar, 1);
		}

		public override Query Parse(string query)
		{
			_conjByClause.Clear();
			var contents = base.Parse(query);
			if (contents is BooleanQuery bq)
			{
				(SpanQuery spanQuery, bool _) = convertBooleanQuery(bq, Slop, InOrder);
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
			if (!field.Equals(PhraseField))
				throw new ParseException($"Clause for field \"{field}\": nested in phrase for \"{Field}\":");
		}

		protected internal override void AddClause(IList<BooleanClause> clauses, int conj, int mods, Query q)
		{
			var count = clauses.Count;
			base.AddClause(clauses, conj, mods, q);
			if (clauses.Count == count + 1)
			{
				var clause = clauses[clauses.Count - 1];
				_conjByClause[clause] = conj;
			}
			else if (clauses.Count > count)
				throw new ParseException("> 1 clauses added per 1 query");
		}

		private (SpanQuery query, bool orEmpty) convertBooleanQuery(BooleanQuery boolQuery, int slop, bool ordered)
		{
			var or = new List<SpanQuery>();
			var not = new List<SpanQuery>();
			var and = new List<SpanQuery>();
			var hasEmpty = new HashSet<List<SpanQuery>>();
			var hasNonEmpty = new HashSet<List<SpanQuery>>();

			var phrase = new List<List<SpanQuery>>();

			// For all clauses e.g. one* two~
			var clauses = boolQuery.GetClauses();
			for (int i = 0; i < clauses.Length; i++)
			{
				var clause = clauses[i];
				phrase.ForEach(_listPool.StoreUnusedList);
				phrase.Clear();
				for (int n = i + 1; n < clauses.Length; n++)
				{
					var nextClause = clauses[n];
					if (isImplicit(nextClause))
					{
						if (phrase.Count == 0)
						{
							phrase.Add(_listPool.GetEmptyList());
							appendToPhrase(phrase, convert(clause));
						}

						appendToPhrase(phrase, convert(nextClause));
						i++;
					}
					else
						break;
				}

				var list = clause.Occur switch
				{
					Occur.MUST => and,
					Occur.SHOULD => or,
					Occur.MUST_NOT => not,
					_ => throw new NotSupportedException()
				};

				phrase.RemoveAll(_ => _.Count == 0);
				switch (phrase.Count)
				{
					case 0:
						(SpanQuery converted, bool orEmptyClause) = convert(clause);
						if (converted != null)
						{
							list.Add(converted);
							hasNonEmpty.Add(list);
						}

						if (orEmptyClause)
							hasEmpty.Add(list);
						break;

					case 1:
						list.Add(nearQuery(phrase[0], slop, ordered));
						break;

					default:
						list.Add(new SpanOrQuery(
							phrase
								.Select(_ => nearQuery(_, slop, ordered))
								.ToArray()));
						break;
				}
			}

			if (hasEmpty.Contains(not))
				throw new ParseException($"NOT clause with empty span \"{_emptyPhrase}\": " + boolQuery);
			if (hasEmpty.Contains(and))
				throw new ParseException($"AND clause with empty span \"{_emptyPhrase}\": " + boolQuery);

			bool orEmpty = hasEmpty.Contains(or) && hasNonEmpty.Contains(or);

			int groupsCount = Math.Sign(and.Count) + Math.Sign(or.Count) + Math.Sign(not.Count);
			if (groupsCount == 0)
				return (null, orEmpty: true);

			if (groupsCount > 1 || and.Count > 0)
			{
				if (or.Count > 0)
					and.Add(orQuery(or));

				if (not.Count > 0)
					return (notQuery(andQuery(and), orQuery(not)), orEmpty);

				return (andQuery(and), orEmpty);
			}

			if (or.Count > 0)
				return (orQuery(or), orEmpty);

			throw new ParseException("Missing positive (AND, OR) clauses: " + boolQuery);
		}

		private void appendToPhrase(List<List<SpanQuery>> phrase, (SpanQuery converted, bool orEmpty) clause)
		{
			(SpanQuery converted, bool orEmpty) = clause;
			if (converted == null)
				return;

			if (orEmpty)
			{
				if (phrase.Count > 1 << 16)
					throw new InvalidOperationException($"Complex phrase query has too many empty clauses \"{_emptyPhrase}\"");

				// split each phrase into 2 variants:
				// where converted clause is added or not
				int count = phrase.Count;
				for (int i = 0; i < count; i++)
				{
					// create phrase variant to which converted clause is not added
					var variant = phrase[i];
					var copy = _listPool.GetEmptyList();
					copy.AddRange(variant);
					phrase.Add(copy);
					variant.Add(converted);
				}

				return;
			}

			for (int p = 0; p < phrase.Count; p++)
				phrase[p].Add(converted);
		}

		private (SpanQuery quert, bool orEmpty) convert(BooleanClause clause)
		{
			switch (clause.Query)
			{
				case BooleanQuery query:
					return convertBooleanQuery(query, 0, true);
				case MultiTermQuery query:
					return (
						new SpanMultiTermQueryWrapper<MultiTermQuery>(query) { Boost = query.Boost },
						orEmpty: false);
				case TermQuery query:
					return
						query.Term.Text() == _emptyPhrase
							? (null, orEmpty: true)
							: (new SpanTermQuery(query.Term) { Boost = query.Boost }, orEmpty: false);
				case PhraseQuery query:
					return (
						new SpanNearQuery(
							query.GetTerms().Select(t => new SpanTermQuery(t)).Cast<SpanQuery>().ToArray(),
							0, true) { Boost = query.Boost },
						orEmpty: false);
				default:
					throw new NotSupportedException("Unknown query type:" + clause.Query.GetType().Name);
			}
		}

		private static SpanQuery nearQuery(List<SpanQuery> qs, int slop, bool ordered) =>
			qs.Count > 1
				? new SpanNearQuery(qs.ToArray(), slop, ordered)
				: qs[0];

		private static SpanQuery andQuery(List<SpanQuery> qs) =>
			qs.Count > 1
				? new SpanNearQuery(qs.ToArray(), int.MaxValue, false)
				: qs[0];

		private static SpanQuery orQuery(List<SpanQuery> qs) =>
			qs.Count > 1
				? new SpanOrQuery(qs.ToArray())
				: qs[0];

		private static SpanQuery notQuery(SpanQuery include, SpanQuery exclude) =>
			new SpanNotQuery(include, exclude);

		private bool isImplicit(BooleanClause clause) =>
			!_conjByClause.TryGetValue(clause, out int conj) || conj == CONJ_NONE;

		public int Slop { private get; set; }
		public bool InOrder { private get; set; }

		public string PhraseField
		{
			get => m_field;
			set => m_field = value;
		}

		private readonly string _emptyPhrase;

		private static readonly ListPool<SpanQuery> _listPool = new ListPool<SpanQuery>();

		private readonly Dictionary<BooleanClause, int> _conjByClause =
			new Dictionary<BooleanClause, int>();
	}
}
