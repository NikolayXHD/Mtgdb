using System;
using System.Collections.Generic;
using System.Linq;
using Mtgdb.Ui;

namespace Mtgdb.Data.Model
{
	public class CollectedCardsDeckTransformation
	{
		public CollectedCardsDeckTransformation(CardRepository repo, PriceRepository priceRepo)
		{
			_repo = repo;
			_priceRepo = priceRepo;
		}

		public Deck Transform(
			Deck original,
			CollectionSnapshot collection,
			Deck previousTransformed = null,
			HashSet<string> affectedNames = null)
		{
			if (!_priceRepo.IsLoadingPriceComplete.Signaled)
				return original;

			var target = Deck.Create();

			target.Name = original.Name + " transformed";

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
				var candidates = card.Namesakes
					.Select(c => (Card: c, AvailableCount: collection.GetCount(c) - getUsedCount(c)))
					.OrderBy(_ => _.AvailableCount <= 0)
					.ThenBy(_ => _.Card.Price == null)
					.ThenBy(_ => _.Card.Price)
					.ToList();

				for (int i = 0; i < candidates.Count; i++)
				{
					var candidate = candidates[i];
					if (candidate.AvailableCount <= 0)
					{
						var bestNotCollectedCandidate = candidates
							.AtMin(_ => _.Card.Price == null)
							.ThenAtMin(_ => _.Card.Price ?? 0)
							.Find();

						use(bestNotCollectedCandidate.Card, count, targetZone);
						return;
					}

					int takeCount = i < candidates.Count - 1
						? Math.Min(count, candidate.AvailableCount)
						: count;

					count -= takeCount;
					use(candidate.Card, takeCount, targetZone);

					if (count == 0)
						return;
				}
			}

			var zones = new[] { original.MainDeck, original.Sideboard, original.Maybeboard };
			var previousZones = new[] { previousTransformed?.MainDeck, previousTransformed?.Sideboard, previousTransformed?.Maybeboard };
			var resultZones = new[] { target.MainDeck, target.Sideboard, target.Maybeboard };

			for (int i = 0; i < zones.Length; i++)
			{
				bool transformPrevious = previousTransformed != null && affectedNames != null;

				var zone = transformPrevious
					? previousZones[i]
					: zones[i];

				var targetZone = resultZones[i];

				foreach (string id in zone.Order)
				{
					var card = _repo.CardsById.TryGet(id);
					if (card == null)
						continue;

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
		private readonly PriceRepository _priceRepo;
	}
}
