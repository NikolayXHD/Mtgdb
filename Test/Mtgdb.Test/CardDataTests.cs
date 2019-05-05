using System;
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
				if (card.Prices?.Paper == null || card.Prices.Paper.Count == 0)
					return;

				for (var i = 0; i < card.Prices.Paper.Count; i++)
				{
					var date = card.Prices.Paper[i].Key;

					Assert.That(date, Is.Not.Null);
					Assert.That(date, Is.Not.Empty);
					Assert.That(date, Does.Match(@"^\d{4}-\d{2}-\d{2}$"));

					if (i > 0)
					{
						var prevDate = card.Prices.Paper[i - 1].Key;
						Assert.That(date, Is.GreaterThan(prevDate).Using<string>(StringComparer.Ordinal));
					}
				}
			}
		}
	}
}