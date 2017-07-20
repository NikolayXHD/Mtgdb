using System;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Dal
{
	public class CollectionModel : ICardCollection
	{
		public event Action<bool, bool, Card> CollectionChanged;

		public Dictionary<string, int> CountById = new Dictionary<string, int>();
		public bool IsInitialized { get; private set; }

		public int CollectionSize
		{
			get
			{
				lock (CountById)
					return CountById.Sum(_ => _.Value);
			}
		}

		public void SetCollection(Dictionary<string, int> countById)
		{
			IsInitialized = true;
			CountById = countById?.ToDictionary() ?? new Dictionary<string, int>();
		}

		public void Add(Card card, int increment)
		{
			var countBefore = card.CollectionCount;
			var count = countBefore + increment;

			if (count < 0)
				count = 0;

			if (increment > 0)
				add(card, newCount: count);
			else if (increment < 0)
				remove(card, newCount: count);

			bool listChanged = countBefore == 0 || card.CollectionCount == 0;
			bool countChanged = countBefore != card.CollectionCount;

			CollectionChanged?.Invoke(listChanged, countChanged, card);
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
	}
}