using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Mtgdb.Index;

namespace Mtgdb.Controls
{
	public class DeckSearcherState : LuceneSearcherState<long, DeckModel>
	{
		public DeckSearcherState(DeckDocumentAdapter adapter, IReadOnlyList<DeckModel> models)
			:base(adapter)
		{
			Models = models;
		}

		protected override IEnumerable<IEnumerable<Document>> GetDocumentGroupsToIndex() =>
			Sequence.From(Models.Select(Adapter.ToDocument));

		public IReadOnlyList<DeckModel> Models { get; }
	}
}