using System.Collections.Generic;
using System.Linq;
using Mtgdb.Data;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
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

		[Test]
		public void All_known_legalities_exist_in_cards()
		{
			var unusedFormats = new HashSet<string>(Legality.Formats, Str.Comparer);
			unusedFormats.ExceptWith(Repo.Cards.SelectMany(c => c.LegalityByFormat.Keys));
			Assert.That(unusedFormats, Is.Empty);
		}
	}
}
