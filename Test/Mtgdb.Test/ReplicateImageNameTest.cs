using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mtgdb.Dal;
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

					var cards = ImageNameCalculator.ReOrderCards(pair.Value, correction);

					var calculatedImageNames = Enumerable.Range(0, cards.Count)
						.Select(i => ImageNameCalculator.CalculateImageName(cards[i], i, cards.Count, correction))
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

			// File.WriteAllText("D:\\temp\\CustomImageOrder.json", JsonConvert.SerializeObject(customOrderBySetByName, Formatting.Indented)
			// 	.Replace("\",\r\n      \"", "\", \"").Replace("[\r\n      \"", "[\"").Replace("\"\r\n    ]", "\"]"));
			//
			// File.WriteAllText("D:\\temp\\ImageMismatches.json", JsonConvert.SerializeObject(mismatches, Formatting.Indented));

			Assert.That(mismatches, Is.Empty);
			foreach (var orderByName in customOrderBySetByName.Values)
				foreach (var order in orderByName.Values)
					Assert.That(order, Is.Empty);

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

		private CardRepository _repo;
		private Dictionary<string, Dictionary<string, ImageNamePatch>> _corrections;
	}
}