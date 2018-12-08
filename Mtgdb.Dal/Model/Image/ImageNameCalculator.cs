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
				var customImageOrder = patch.ImageOrder.TryGet(set.Code)?.TryGet(pair.Key);
				var cards = ReOrderCards(pair.Value, customImageOrder);

				for (int i = 0; i < cards.Count; i++)
					cards[i].ImageName = CalculateImageName(cards[i], i, cards.Count, customImageOrder);
			}
		}

		internal static List<Card> ReOrderCards(List<Card> cardNamesakes, ImageNamePatch correction)
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

		internal static string CalculateImageName(Card card, int i, int count, ImageNamePatch correction)
		{
			if (correction?.ImageName != null)
				return correction.ImageName;

			string imageName = Str.Equals(card.Layout, "split")
				? string.Concat(card.Names)
				: card.NameEn;

			var normalizedImageName = imageName.RemoveDiacritics().Replace("/", " ");
			var clearedImageName = _removedSubstringPattern.Replace(normalizedImageName, string.Empty);


			if (count == 1)
				return clearedImageName;

			return clearedImageName + (i + 1);
		}

		private static readonly Regex _removedSubstringPattern = new Regex(@"[:?""®]| token card", RegexOptions.IgnoreCase);
	}
}