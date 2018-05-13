using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Spans;

namespace Mtgdb.Dal.Index
{
	public class RewriteableSpanNearQuery : SpanNearQuery
	{
		public RewriteableSpanNearQuery(SpanQuery[] clauses, int slop, bool inOrder) : base(clauses, slop,
			inOrder)
		{
		}

		public override Query Rewrite(IndexReader reader)
		{
			for (int i = 0; i < m_clauses.Count; i++)
			{
				if (!(m_clauses[i] is SpanOrQuery))
					continue;

				var clauses = ((SpanOrQuery) m_clauses[i]).GetClauses();

				// in query  text:"(> or target) creature"
				// (> or target) is parsed as SpanOr with 1 clause
				if (clauses.Length == 1 && m_clauses.Count > 1)
				{
					var skippedOrClause = m_clauses.ToList();
					skippedOrClause.RemoveAt(i);

					var nonSkippedOrClause = m_clauses.ToList();
					nonSkippedOrClause[i] = clauses[0];

					return new BooleanQuery(disableCoord: true)
					{
						{new RewriteableSpanNearQuery(skippedOrClause.ToArray(), Slop, IsInOrder), Occur.SHOULD},
						{new RewriteableSpanNearQuery(nonSkippedOrClause.ToArray(), Slop, IsInOrder), Occur.SHOULD}
					};
				}

				var spanLengths = clauses.Select(getSpanLength).ToArray();

				if (spanLengths.All(F.IsNotNull) && spanLengths.Skip(1).All(l => l == spanLengths[0]))
					continue;

				var nonFixedLengthSpans = Enumerable.Range(0, clauses.Length)
					.Where(j => spanLengths[j] == null)
					.Select(j => clauses[j])
					.ToArray();

				var spansByLength = Enumerable.Range(0, clauses.Length)
					.Where(j => spanLengths[j] != null)
					.GroupBy(j => spanLengths[j])
					.ToDictionary(gr => gr.Key, gr => gr.Select(j => clauses[j]).ToArray());

				var boost = m_clauses[i].Boost;
				var result = new BooleanQuery(disableCoord: true);

				foreach (var pair in spansByLength)
				{
					var clausesCopy = m_clauses.ToArray();
					clausesCopy[i] = getClause(pair.Value, boost);
					result.Add(new RewriteableSpanNearQuery(clausesCopy, Slop, IsInOrder), Occur.SHOULD);
				}

				foreach (var span in nonFixedLengthSpans)
				{
					var clausesCopy = m_clauses.ToArray();
					clausesCopy[i] = span;
					result.Add(new RewriteableSpanNearQuery(clausesCopy, Slop, IsInOrder), Occur.SHOULD);
				}

				return result;
			}

			return base.Rewrite(reader);
		}

		private static SpanQuery getClause(IList<SpanQuery> list, float boost)
		{
			if (list == null || list.Count == 0)
				throw new ArgumentException();

			if (list.Count == 1)
			{
				var c = list[0];
				c.Boost *= boost;
				return c;
			}

			return new SpanOrQuery(list.ToArray())
			{
				Boost = boost
			};
		}

		private static int? getSpanLength(SpanQuery c)
		{
			if (c is SpanTermQuery || c is SpanMultiTermQueryWrapper<MultiTermQuery> || c is SpanNotQuery)
				return 1;

			if (c is SpanOrQuery soq)
			{
				var subClauses = soq.GetClauses();

				if (subClauses.Length == 0)
					return 0;

				var lengths = subClauses.Select(getSpanLength).ToArray();

				if (lengths.Any(F.IsNull))
					return null;

				if (lengths.Skip(1).Any(l => l != lengths[0]))
					return null;

				return lengths[0];
			}

			if (c is SpanNearQuery snq)
			{
				var subClauses = snq.GetClauses();

				if (subClauses.Length == 0)
					return 0;

				var lengths = subClauses.Select(getSpanLength).ToArray();

				if (lengths.Any(F.IsNull))
					return null;

				return lengths.Cast<int>().Sum();
			}

			throw new ArgumentException();
		}
	}
}