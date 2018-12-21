using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mtgdb.Dal;
using Mtgdb.Ui;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class LegacyCardMappingTests: TestsBase
	{
		[Test]
		public void MapLegacyCardIds()
		{
			var kernel = new StandardKernel();
			kernel.Load<CoreModule>();
			kernel.Load<DalModule>();

			var cardRepo = kernel.Get<CardRepository>();
			var cardRepoLegacy = kernel.Get<CardRepositoryLegacy>();

			cardRepo.LoadFile();
			cardRepoLegacy.LoadFile();

			Parallel.Invoke(
				() => cardRepo.Load(),
				() => cardRepoLegacy.Load());

			var converter = new DeckLegacyConverter(cardRepo, cardRepoLegacy);

			var messages = new List<string>();

			foreach (var set in cardRepoLegacy.SetsByCode.Values)
			{
				foreach (var pair in set.CardsByName)
				{
					var nameEn = pair.Key;
					var cards = pair.Value;

					foreach (var card in cards)
					{
						var matchingCard = converter.FindMatchingCard(card);
						if (matchingCard == null)
							messages.Add($"{set.Code} {nameEn} {card.Number} {card.MultiverseId?.ToString() ?? "null"}");
					}
				}
			}

			if (messages.Count > 0)
				Assert.Fail(string.Join(Environment.NewLine, messages));
		}

		[Test]
		public void MapSets()
		{
			var kernel = new StandardKernel();
			kernel.Load<CoreModule>();
			kernel.Load<DalModule>();

			var cardRepo = kernel.Get<CardRepository>();
			var cardRepoLegacy = kernel.Get<CardRepositoryLegacy>();

			cardRepo.LoadFile();
			cardRepoLegacy.LoadFile();

			Parallel.Invoke(
				() => cardRepo.Load(),
				() => cardRepoLegacy.Load());

			var messages = new List<string>();

			var newSetCodes = new HashSet<string>(cardRepo.SetsByCode.Keys, Str.Comparer);
			var oldSetCodes = new HashSet<string>(cardRepoLegacy.SetsByCode.Keys, Str.Comparer);

			newSetCodes.ExceptWith(cardRepoLegacy.SetsByCode.Keys);
			oldSetCodes.ExceptWith(cardRepo.SetsByCode.Keys);

			var newSets = cardRepo.SetsByCode.Values.Where(s => newSetCodes.Contains(s.Code)).ToArray();
			var oldSets = cardRepoLegacy.SetsByCode.Values.Where(s => oldSetCodes.Contains(s.Code)).ToArray();
		}
	}
}