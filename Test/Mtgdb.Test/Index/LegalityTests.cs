using System.Linq;
using Mtgdb.Data;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class LegalityTests : TestsBase
	{
		[OneTimeSetUp]
		public static void Setup()
		{
			LoadCards();
		}

		[Test]
		public void All_legalities_are_known()
		{
			var unknownLegalities = Repo.Cards
				.SelectMany(card => card.LegalityByFormat.Keys)
				.ToHashSet(Str.Comparer);

			unknownLegalities.ExceptWith(Legality.Formats);
			Assert.That(unknownLegalities, Is.Empty);
		}
	}
}