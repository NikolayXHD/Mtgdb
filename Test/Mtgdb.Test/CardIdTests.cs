using System.Linq;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class CardIdTests : TestsBase
	{
		[Test]
		public void Id_is_unique()
		{
			LoadCards();

			var groups = Repo.Cards.GroupBy(c => c.Id)
				.Select(gr => (Id: gr.Key, Cards: gr.ToList()))
				.OrderByDescending(gr => gr.Cards.Count)
				.ToList();

			Assert.That(groups[0].Cards.Count == 1);
		}
	}
}