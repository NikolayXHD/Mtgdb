using System;
using System.Collections.Generic;
using System.Linq;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	class XitaxDeckTransformation
	{
		public XitaxDeckTransformation(CardRepository repo)
		{
			_repo = repo;
		}

		public void Transform(Deck deck)
		{
			if (!_repo.IsLoadingComplete)
				throw new InvalidOperationException();

			var cardIds = new HashSet<string>();
			cardIds.UnionWith(deck.MainDeck.Order);
			cardIds.UnionWith(deck.Sideboard.Order);

			var releaseDate = cardIds.Max(
				cardId => _repo.CardsById[cardId].Printings.Min(
						setcode => _repo.SetsByCode[setcode].ReleaseDate));
			
			var replacements = new Dictionary<string, string>();

			foreach (string cardId in cardIds)
			{
				var card = _repo.CardsById[cardId];

				var candidates = card.Printings
					.Select(setcode => _repo.SetsByCode[setcode])
					.Where(set => Str.Compare(set.ReleaseDate, releaseDate) <= 0)
					.AtMax(set => set.ReleaseDate)
					.Find()
					.CardsByName[card.NameNormalized];

				var replacement =
					candidates.FirstOrDefault(_ => _.ImageName == card.ImageName) ??
					candidates.First();

				replacements.Add(cardId, replacement.Id);
			}

			deck.Replace(replacements);
		}

		private readonly CardRepository _repo;
	}
}