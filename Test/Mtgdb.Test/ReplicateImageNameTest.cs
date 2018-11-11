using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mtgdb.Dal;
using Newtonsoft.Json;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class ReplicateImageNameTest
	{
		[OneTimeSetUp]
		public void Setup()
		{
			var kernel = new StandardKernel();
			kernel.Load<DalModule>();
			kernel.Load<CoreModule>();

			_repo = kernel.Get<CardRepository>();
			_repo.LoadFile();
			_corrections = _repo.Patch.ImageOrder;
			_repo.Load();
		}

		[Test]
		public void Replicate()
		{
			var mismatches = new Dictionary<string, string>();
			var customOrderBySetByName = new Dictionary<string, Dictionary<string, string[]>>(Str.Comparer);

			foreach (var set in _repo.SetsByCode.Values)
				foreach (var pair in set.CardsByName)
				{
					var correction = _corrections.TryGet(set.Code)?.TryGet(pair.Key);
					if (correction?.Improved == true)
						continue;

					var cards = reOrderCards(pair.Value, correction);

					var calculatedImageNames = Enumerable.Range(0, cards.Count)
						.Select(i => calculateImageName(cards[i], i, cards.Count, correction))
						.ToList();

					if (cards.Count == 1 && Str.Equals(cards[0].ImageNameOriginal, calculatedImageNames[0] + "1"))
						continue;

					if (cards.Select(_ => _.ImageNameOriginal).SequenceEqual(calculatedImageNames, Str.Comparer))
						continue;

					if (calculatedImageNames.ToHashSet(Str.Comparer).SetEquals(cards.Select(_ => _.ImageNameOriginal)))
					{
						if (cards.Select(_ => _.Number).Distinct().Count() != cards.Count &&
							cards.Select(_ => _.MultiverseId).Distinct().Count() != cards.Count)
						{
							throw new ApplicationException($"cannot distinguish variants of card {set.Code} {pair.Key}");
						}

						var customOrder = cards.Select(_ => _.ImageNameOriginal).ToArray();
						var setCustomOrders = getSetCustomOrders(set);
						setCustomOrders.Add(pair.Key, customOrder);
					}
					else
						for (int i = 0; i < cards.Count; i++)
						{
							string key = $"{set.Code}|{cards[i].Number}|{calculatedImageNames[i]}";
							if (!mismatches.ContainsKey(key))
							{
								string value;
								if (!Str.Equals(cards[i].ImageName, cards[i].ImageNameOriginal))
									value = cards[i].ImageNameOriginal + "|" + cards[i].ImageName;
								else
									value = cards[i].ImageNameOriginal;

								mismatches.Add(key, value);
							}
						}
				}

			File.WriteAllText("D:\\temp\\CustomImageOrder.json", JsonConvert.SerializeObject(customOrderBySetByName, Formatting.Indented)
				.Replace("\",\r\n      \"", "\", \"").Replace("[\r\n      \"", "[\"").Replace("\"\r\n    ]", "\"]"));

			File.WriteAllText("D:\\temp\\ImageMismatches.json", JsonConvert.SerializeObject(mismatches, Formatting.Indented));

			Assert.That(mismatches, Is.Empty);

			Dictionary<string, string[]> getSetCustomOrders(Set set)
			{
				if (!customOrderBySetByName.TryGetValue(set.Code, out var setCustomOrders))
				{
					setCustomOrders = new Dictionary<string, string[]>(Str.Comparer);
					customOrderBySetByName.Add(set.Code, setCustomOrders);
				}

				return setCustomOrders;
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

		private static string calculateImageName(Card card, int i, int count, ImageNamePatch correction)
		{
			if (correction?.ImageName != null)
				return correction.ImageName;

			string imageName = Str.Equals(card.Layout, "split")
				? string.Concat(card.Names)
				: card.NameEn;

			var normalizedImageName = imageName.RemoveDiacritics().Replace("/", " ");
			var clearedImageName = _removedSubstringPattern.Replace(normalizedImageName, string.Empty);


			var result = count > 1
				? clearedImageName + (i + 1)
				: clearedImageName;

			return result;
		}

		private static readonly Regex _removedSubstringPattern = new Regex(@"[:?""®]| token card");

		private CardRepository _repo;
		private Dictionary<string, Dictionary<string, ImageNamePatch>> _corrections;
	}
}