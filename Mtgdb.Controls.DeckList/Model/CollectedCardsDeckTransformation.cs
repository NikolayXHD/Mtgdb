using System;
using System.Collections.Generic;
using System.Linq;
using Mtgdb.Dal;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public class CollectedCardsDeckTransformation : IDeckTransformation
	{
		public CollectedCardsDeckTransformation(CardRepository repo)
		{
			_repo = repo;
		}

		public Deck Transform(Deck original, ICardCollection collection)
		{
			if (!_repo.IsLoadingComplete)
				return original;

			var target = Deck.Create();

			var usedCountById = new Dictionary<string, int>();

			void use(Card card, int count, DeckZone targetZone)
			{
				var id = card.Id;
				usedCountById.TryGetValue(id, out int currentTotalCount);
				usedCountById[id] = currentTotalCount + count;

				if (!targetZone.Count.TryGetValue(id, out var currentZoneCount))
					targetZone.Order.Add(id);

				targetZone.Count[id] = currentZoneCount + count;
			}

			int getUsedCount(Card c)
			{
				usedCountById.TryGetValue(c.Id, out int count);
				return count;
			}

			void add(Card card, int count, DeckZone targetZone)
			{
				var candidates = card.Namesakes.Prepend(card)
					.Select(c => (Card: c, CollectedCount: collection.GetCount(c) - getUsedCount(c)))
					.OrderBy(_ => _.CollectedCount <= 0)
					.ThenBy(_ => _.Card != card)
					.ThenBy(_ => _.Card.PriceMid == null)
					.ThenBy(_ => _.Card.PriceMid);

				foreach (var candidate in candidates)
				{
					int countTaken = candidate.CollectedCount > 0
						? Math.Min(count, candidate.CollectedCount)
						: count;

					count -= countTaken;
					use(candidate.Card, countTaken, targetZone);

					if (count == 0)
						break;
				}
			}

			var zones = new[] { original.MainDeck, original.Sideboard };
			var resultZones = new[] { target.MainDeck, target.Sideboard };

			for (int i = 0; i < zones.Length; i++)
			{
				var zone = zones[i];
				var targetZone = resultZones[i];

				foreach (string id in zone.Order)
					add(_repo.CardsById[id], zone.Count[id], targetZone);
			}

			return target;
		}

		private readonly CardRepository _repo;
	}
}