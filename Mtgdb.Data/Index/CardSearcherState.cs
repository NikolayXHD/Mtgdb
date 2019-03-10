using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;

namespace Mtgdb.Data.Index
{
	public class CardSearcherState : LuceneSearcherState<int, Card>
	{
		public CardSearcherState(CardDocumentAdapter adapter, CardRepository repo)
			:base(adapter)
		{
			_repo = repo;
		}

		protected override IEnumerable<IEnumerable<Document>> GetDocumentGroupsToIndex() =>
			_repo.SetsByCode.Values.Select(set => set.Cards.Select(Adapter.ToDocument));

		private readonly CardRepository _repo;
	}
}