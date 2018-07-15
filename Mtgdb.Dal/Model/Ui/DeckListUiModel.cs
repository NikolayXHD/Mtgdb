using JetBrains.Annotations;

namespace Mtgdb.Dal
{
	public class UiModelSnapshotFactory
	{
		[UsedImplicitly]
		public UiModelSnapshotFactory(CardRepository repo, CollectionEditorModel collection)
		{
			_repo = repo;
			_collection = collection;
		}

		public UiModel Snapshot() =>
			new UiModel(_repo, new CollectionSnapshot(_collection));

		private readonly CardRepository _repo;
		private readonly CollectionEditorModel _collection;
	}
}