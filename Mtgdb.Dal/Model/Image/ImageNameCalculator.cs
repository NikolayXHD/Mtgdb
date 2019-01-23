using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	internal static class ImageNameCalculator
	{
		public static void CalculateImageNames(Set set, Patch patch)
		{
			foreach (var pair in set.CardsByName)
			{
				var cards = pair.Value;
				var first = cards.First();

				if (first.IsFlipped())
				{
					var unflippedCardName = first.Faces.Main.NameNormalized;

					var customImageOrder = patch.ImageOrder.TryGet(set.Code)?.TryGet(unflippedCardName);
					string imageName = customImageOrder?.ImageName ?? unflippedCardName;

					for (int i = 0; i < cards.Count; i++)
						cards[i].ImageName = calculateImageName(imageName, i, cards.Count);
				}
				else
				{
					var customImageOrder = patch.ImageOrder.TryGet(set.Code)?.TryGet(pair.Key);
					cards = reOrderCards(pair.Value, customImageOrder);

					for (int i = 0; i < cards.Count; i++)
					{
						var card = cards[i];

						string imageName = customImageOrder?.ImageName ?? (
							Str.Equals(card.Layout, CardLayouts.Split)
								? string.Concat(card.Faces.Select(c=>c.NameNormalized))
								: card.NameNormalized);

						card.ImageName = calculateImageName(imageName, i, cards.Count);
					}
				}
			}
		}

		private static List<Card> reOrderCards(List<Card> cardNamesakes, ImageNamePatch correction)
		{
			var reordered = cardNamesakes
				.OrderBy(n =>
				{
					if (n.Number == null)
						return int.MinValue;

					var digits = new string(n.Number.TakeWhile(char.IsDigit).ToArray());
					if (digits.Length == 0)
						return int.MaxValue;

					return int.Parse(digits);
				})
				.ThenBy(_ => _.Number)
				.ThenBy(_ => _.MultiverseId)
				.ToList();

			if (correction?.Order != null)
			{
				if (correction.Order.Length != reordered.Count)
					throw new ArgumentException();

				reordered = Enumerable.Range(0, reordered.Count).OrderBy(i => correction.Order[i])
					.Select(i => reordered[i]).ToList();
			}

			return reordered;
		}

		private static string calculateImageName(string name, int i, int count)
		{
			var normalizedImageName = name.RemoveDiacritics().Replace("/", " ");
			var clearedImageName = _removedSubstringPattern.Replace(normalizedImageName, string.Empty);

			if (count == 1)
				return clearedImageName;

			return clearedImageName + (i + 1);
		}

		private static readonly Regex _removedSubstringPattern = new Regex(@"[:?""®]| token card", RegexOptions.IgnoreCase);
	}
}