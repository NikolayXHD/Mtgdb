using System.Text;
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
		[Test, Explicit]
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

			var converter = new DeckConverter(cardRepo, null, cardRepoLegacy);
			var messages = new StringBuilder();
			foreach (var set in cardRepoLegacy.SetsByCode.Values)
			{
				foreach (var pair in set.CardsByName)
				{
					var nameEn = pair.Key;
					var cards = pair.Value;

					foreach (var card in cards)
					{
						var matchingCard = converter.MatchLegacyCard(card);
						if (matchingCard == null)
							messages.AppendLine($"{set.Code} {nameEn} {card.Number} {card.MultiverseId?.ToString() ?? "null"}");
					}
				}
			}

			Assert.That(messages.ToString(), Is.Empty);
		}

		[Test]
		public void MapV3CardIds()
		{
			var kernel = new StandardKernel();
			kernel.Load<CoreModule>();
			kernel.Load<DalModule>();

			var cardRepo = kernel.Get<CardRepository>();
			var cardRepo42 = kernel.Get<CardRepository42>();

			cardRepo.LoadFile();
			cardRepo42.LoadFile();

			Parallel.Invoke(
				() => cardRepo.Load(),
				() => cardRepo42.Load());

			var converter = new DeckConverter(cardRepo, cardRepo42, null);
			var messages = new StringBuilder();

			foreach (string id in cardRepo42.Ids)
			{
				var matchingId = converter.MatchIdV3(id);
				if (matchingId == null)
				{
					(string scryfallId, string name) = cardRepo42.GetById(id);
					messages.AppendLine($"mtgjson42 id {id} {scryfallId} {name}");
				}
			}

			Assert.That(messages.ToString(), Is.Empty);
		}
	}
}