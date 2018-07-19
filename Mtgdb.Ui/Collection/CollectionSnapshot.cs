using System.Collections.Generic;
using Mtgdb.Dal;

namespace Mtgdb.Ui
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