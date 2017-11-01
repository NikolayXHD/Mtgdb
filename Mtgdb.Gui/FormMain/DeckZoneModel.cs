using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Gui
{
	public class DeckZoneModel
	{
		public Dictionary<string, int> CountById { get; private set; } = new Dictionary<string, int>();

		private List<string> Order { get; set; } = new List<string>();

		public IList<string> CardsIds => Order;

		public DeckZoneModel()
		{
		}

		public void Add(string cardId, int newCount)
		{
			CountById[cardId] = newCount;

			if (!Order.Contains(cardId))
				Order.Add(cardId);
		}

		public void Clear()
		{
			CountById.Clear();
			Order.Clear();
		}

		public void Remove(string cardId, int newCount)
		{
			if (newCount > 0)
				CountById[cardId] = newCount;
			else
			{
				CountById.Remove(cardId);
				Order.Remove(cardId);
			}
		}

		public void SetDeck(DeckZone deckZone)
		{
			CountById = deckZone.Count.ToDictionary();
			Order = deckZone.Order.ToList();
		}

		public void SetOrder(List<string> cardIds)
		{
			Order = cardIds;
		}

		public int GetCount(string cardId)
		{
			int count;
			CountById.TryGetValue(cardId, out count);
			return count;
		}
	}
}