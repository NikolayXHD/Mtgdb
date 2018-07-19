using System.Collections.Generic;
using Mtgdb.Dal;
using Mtgdb.Gui;

namespace Mtgdb.Ui
{
	public class DeckSnapshot: ICardCollection
	{
		public DeckSnapshot(DeckEditorModel original) =>
			_countById = original.Deck.CountById.ToDictionary(Str.Comparer);

		public int GetCount(Card c)
		{
			_countById.TryGetValue(c.Id, out int count);
			return count;
		}

		private readonly Dictionary<string, int> _countById;
	}
}