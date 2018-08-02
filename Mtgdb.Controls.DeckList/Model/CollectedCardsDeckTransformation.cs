using System;
using System.Collections.Generic;
using System.Linq;
using Mtgdb.Dal;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public class CollectedCardsDeckTransformation
	{
		public CollectedCardsDeckTransformation(CardRepository repo)
		{
			_repo = repo;
		}

		public Deck Transform(Deck original, CollectionSnapshot collection, Deck previousTransformed = null, HashSet<string> affectedNames = null)
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

			void transform(Card card, int count, DeckZone targetZone)
			{
				var candidates = card.Namesakes.Prepend(card)
					.Select(c => (Card: c, AvailableCount: collection.GetCount(c) - getUsedCount(c)))
					.OrderBy(_ => _.AvailableCount <= 0)
					.ThenBy(_ => _.Card.PriceMid == null)
					.ThenBy(_ => _.Card.PriceMid)
					.ToList();

				for (int i = 0; i < candidates.Count; i++)
				{
					var candidate = candidates[i];
					if (candidate.AvailableCount <= 0)
					{
						var bestNotCollectedCandidate = candidates
							.AtMin(_ => _.Card.PriceMid == null)
							.ThenAtMin(_ => _.Card.PriceMid ?? 0)
							.Find();

						use(bestNotCollectedCandidate.Card, count, targetZone);
						return;
					}

					int takeCount = Math.Min(count, candidate.AvailableCount);

					count -= takeCount;
					use(candidate.Card, takeCount, targetZone);

					if (count == 0)
						return;
				}
			}

			var zones = new[] { original.MainDeck, original.Sideboard };
			var previousZones = new[] { previousTransformed?.MainDeck, previousTransformed?.Sideboard };
			var resultZones = new[] { target.MainDeck, target.Sideboard };

			for (int i = 0; i < zones.Length; i++)
			{
				bool transformPrevious = previousTransformed != null && affectedNames != null;

				var zone = transformPrevious
					? previousZones[i]
					: zones[i];

				var targetZone = resultZones[i];

				foreach (string id in zone.Order)
				{
					var card = _repo.CardsById[id];
					int count = zone.Count[id];

					if (transformPrevious && !affectedNames.Contains(card.NameEn))
						use(card, count, targetZone);
					else
						transform(card, count, targetZone);
				}
			}

			return target;
		}

		private readonly CardRepository _repo;
	}
}