using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Mtgdb.Index;

namespace Mtgdb.Dal.Index
{
	public class CardSearcherState : LuceneSearcherState<int, Card>
	{
		public CardSearcherState(CardDocumentAdapter adapter, CardRepository repo, Func<Set, bool> filterSet)
			:base(adapter)
		{
			_repo = repo;
			_filterSet = filterSet;
		}

		protected override IEnumerable<IEnumerable<Document>> GetDocumentGroupsToIndex()
		{
			return _repo.SetsByCode.Values
				.Where(_filterSet)
				.Select(set => set.Cards.Select(Adapter.ToDocument));
		}

		private readonly CardRepository _repo;
		private readonly Func<Set, bool> _filterSet;
	}
}