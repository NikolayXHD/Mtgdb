using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Dal
{
	public class CollectionModel : ICardCollection
	{
		public event DeckChangedEventHandler CollectionChanged;

		public void LoadCollection(Deck deck, bool append)
		{
			IsLoaded = true;

			var modified = (append
					? CountById
					: Enumerable.Empty<KeyValuePair<string, int>>())
				.Concat(deck.MainDeck.Count)
				.Concat(deck.SideDeck.Count)
				.GroupBy(_ => _.Key)
				.ToDictionary(
					gr => gr.Key,
					gr => gr.Sum(_ => _.Value));

			CountById = modified;

			CollectionChanged?.Invoke(
				listChanged: true,
				countChanged: true,
				touchedChanged: false,
				card: null,
				changedZone: null);
		}

		public void Add(Card card, int increment)
		{
			var countBefore = GetCount(card);
			var count = countBefore + increment;

			if (count < 0)
				count = 0;

			if (increment > 0)
				add(card, newCount: count);
			else if (increment < 0)
				remove(card, newCount: count);

			bool listChanged = countBefore == 0 || GetCount(card) == 0;
			bool countChanged = countBefore != GetCount(card);

			CollectionChanged?.Invoke(
				listChanged,
				countChanged,
				card,
				touchedChanged: false,
				changedZone: null);
		}

		private void remove(Card card, int newCount)
		{
			if (newCount > 0)
				CountById[card.Id] = newCount;
			else
				CountById.Remove(card.Id);
		}

		private void add(Card card, int newCount)
		{
			CountById[card.Id] = newCount;
		}

		public int GetCount(Card c)
		{
			int count;
			CountById.TryGetValue(c.Id, out count);
			return count;
		}



		public int CollectionSize
		{
			get
			{
				lock (CountById)
					return CountById.Sum(_ => _.Value);
			}
		}



		public Dictionary<string, int> CountById { get; private set; } = new Dictionary<string, int>();
		public bool IsLoaded { get; private set; }
	}
}