using System.Collections.Generic;

namespace Mtgdb.Dal
{
	public class CollectionSnapshot : ICardCollection
	{
		public CollectionSnapshot(CollectionEditorModel original) =>
			_countById = original.CountById.ToDictionary(Str.Comparer);

		public int GetCount(Card c)
		{
			_countById.TryGetValue(c.Id, out int count);
			return count;
		}

		private readonly Dictionary<string, int> _countById;
	}
}