using System.Collections.Generic;
using Mtgdb.Data;

namespace Mtgdb.Ui
{
	public class DeckZoneSnapshot: ICardCollection
	{
		public DeckZoneSnapshot(DeckEditorModel original) =>
			_countById = original.Deck.CountById.ToDictionary(Str.Comparer);

		public int GetCount(Card c) =>
			_countById.TryGet(c.Id);

		private readonly Dictionary<string, int> _countById;
	}
}