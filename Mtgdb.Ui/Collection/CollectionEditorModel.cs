using System.Collections.Generic;
using System.Linq;
using Mtgdb.Dal;

namespace Mtgdb.Ui
{
	public class CollectionEditorModel : ICardCollection
	{
		public event CollectionChangedEventHandler CollectionChanged;

		public void LoadCollection(Deck deck, bool append)
		{
			IsLoaded = true;

			var pairs = append
				? CountById
				: Enumerable.Empty<KeyValuePair<string, int>>();

			var modified = pairs
				.Concat(deck.MainDeck.Count)
				.Concat(deck.Sideboard.Count)
				.Concat(deck.Maybeboard.Count)
				.GroupBy(_ => _.Key)
				.ToDictionary(
					gr => gr.Key,
					gr => gr.Sum(_ => _.Value));



			bool changed = !modified.IsEqualTo(CountById);

			if (changed)
				CountById = modified;

			CollectionChanged?.Invoke(listChanged: changed, countChanged: changed, card: null);
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

		public int GetCount(Card c) =>
			CountById.TryGet(c.Id);

		public int CollectionSize
		{
			get
			{
				lock (CountById)
					return CountById.Sum(_ => _.Value);
			}
		}

		public CollectionSnapshot Snapshot() =>
			new CollectionSnapshot(this);

		public Dictionary<string, int> CountById { get; private set; } = new Dictionary<string, int>(Str.Comparer);
		public bool IsLoaded { get; private set; }
	}
}