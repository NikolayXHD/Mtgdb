using System;
using Mtgdb.Data;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class PriceDataTests : TestsBase
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			LoadMtgjsonPrices();
		}

		[Test]
		public void Price_dates_are_correct()
		{
			foreach (var mtgjsonPrices in PriceRepo.Prices.Values)
			{
				if (mtgjsonPrices.Paper == null)
					continue;

				foreach ((string shopName, ShopPriceHistory shopPrices) in mtgjsonPrices.Paper)
				{
					if ((shopPrices.Retail?.Normal?.Count ?? 0) == 0)
						continue;

					for (var i = 0; i < shopPrices.Retail.Normal.Count; i++)
					{
						var list = shopPrices.Retail.Normal;

						var date = list[i].Key;
						Assert.That(date, Is.Not.Null);
						Assert.That(date, Is.Not.Empty);
						Assert.That(date, Does.Match(@"^\d{4}-\d{2}-\d{2}$"));

						if (i > 0)
						{
							var prevDate = list[i - 1].Key;
							Assert.That(date, Is.GreaterThan(prevDate).Using<string>(StringComparer.Ordinal));
						}
					}
				}
			}
		}
	}
}
