using System.Linq;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class CardDataTests : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadCards();
		}

		[Test]
		public void Id_is_unique()
		{
			var groups = Repo.Cards.GroupBy(c => c.Id)
				.Select(gr => (Id: gr.Key, Cards: gr.ToList()))
				.OrderByDescending(gr => gr.Cards.Count)
				.ToList();

			Assert.That(groups[0].Cards.Count == 1);
		}

		[Test]
		public void Price_dates_are_correct()
		{
			foreach (var card in Repo.Cards)
			{
				if (card.MtgjsonPrices?.Paper == null || card.MtgjsonPrices.Paper.Count == 0)
					return;

				foreach (var date in card.MtgjsonPrices.Paper.Keys)
				{
					Assert.That(date, Is.Not.Null);
					Assert.That(date, Is.Not.Empty);
				}
			}
		}
	}
}