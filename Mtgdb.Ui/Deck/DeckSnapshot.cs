using System.Collections.Generic;
using Mtgdb.Dal;

namespace Mtgdb.Ui
{
	public class DeckSnapshot: ICardCollection
	{
		public DeckSnapshot(DeckEditorModel original) =>
			_countById = original.Deck.CountById.ToDictionary(Str.Comparer);

		public int GetCount(Card c) =>
			_countById.TryGet(c.Id);

		private readonly Dictionary<string, int> _countById;
	}
}