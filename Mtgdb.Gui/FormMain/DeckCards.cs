using System.Collections.Generic;
using System.Linq;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class DeckCards
	{
		public Dictionary<string, int> CountById { get; private set; } = new Dictionary<string, int>();

		private List<string> Order { get; set; } = new List<string>();

		public IEnumerable<string> CardsIds => Order;

		public DeckCards()
		{
		}

		public void Add(Card card, int newCount)
		{
			CountById[card.Id] = newCount;

			if (!Order.Contains(card.Id))
				Order.Add(card.Id);
		}

		public void Clear()
		{
			CountById.Clear();
			Order.Clear();
		}

		public void Remove(Card card, int newCount)
		{
			if (newCount > 0)
				CountById[card.Id] = newCount;
			else
			{
				CountById.Remove(card.Id);
				Order.Remove(card.Id);
			}
		}

		public void SetDeck(Dictionary<string, int> countById, List<string> order)
		{
			CountById = countById?.ToDictionary() ?? new Dictionary<string, int>();
			Order = order?.ToList() ?? CountById.Keys.ToList();
		}

		public void SetOrder(List<string> cardIds)
		{
			Order = cardIds;
		}

		public int GetCount(Card card)
		{
			int count;
			CountById.TryGetValue(card.Id, out count);
			return count;
		}
	}
}