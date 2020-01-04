using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Spans;

namespace Mtgdb.Data
{
	public class RewritableSpanNearQuery : SpanNearQuery
	{
		public RewritableSpanNearQuery(SpanQuery[] clauses, int slop, bool inOrder) : base(clauses, slop,
			inOrder)
		{
		}

		public override Query Rewrite(IndexReader reader)
		{
			for (int i = 0; i < m_clauses.Count; i++)
			{
				if (!(m_clauses[i] is SpanOrQuery spanOrQuery))
					continue;

				var clauses = spanOrQuery.GetClauses();
				var lengths = clauses.Select(getSpanLength).ToArray();

				if (lengths.All(F.IsNotNull) && lengths.All(l => l == lengths[0]))
					continue;

				var spansByLength = Enumerable.Range(0, clauses.Length)
					.Where(j => lengths[j] != null)
					.GroupBy(j => lengths[j])
					.Select(gr => new KeyValuePair<int?, SpanQuery[]>(gr.Key, gr.Select(j => clauses[j]).ToArray()))
					.ToList();

				float boost = m_clauses[i].Boost;
				var result = new BooleanQuery(disableCoord: true);

				if (spansByLength.Any(p => p.Key == 0))
				{
					var clausesCopy = m_clauses.ToList();
					clausesCopy.RemoveAt(i);
					result.Add(new RewritableSpanNearQuery(clausesCopy.ToArray(), Slop, IsInOrder), Occur.SHOULD);
				}

				foreach (var (length, queries) in spansByLength)
				{
					if (length == 0)
						continue;

					var clausesCopy = m_clauses.ToArray();
					clausesCopy[i] = getClause(queries, boost);
					result.Add(new RewritableSpanNearQuery(clausesCopy, Slop, IsInOrder), Occur.SHOULD);
				}

				foreach (var span in clauses.Where((span, j) => lengths[j] == null))
				{
					var clausesCopy = m_clauses.ToArray();
					clausesCopy[i] = span;
					result.Add(new RewritableSpanNearQuery(clausesCopy, Slop, IsInOrder), Occur.SHOULD);
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
			if (c is EmptyPhraseSpanQuery)
				return 0;

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
