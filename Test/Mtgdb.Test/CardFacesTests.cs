using System.Linq;
using Mtgdb.Data;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class CardLayoutTests : TestsBase
	{
		[OneTimeSetUp]
		public void Setup() =>
			LoadCards();

		[Test]
		public void All_cards_have_known_layout()
		{
			foreach (var card in Repo.Cards)
			{
				Assert.That(card.IsKnownLayout(), card.ToString);
				Assert.That(card.IsMultiFace() ^ card.IsSingleFace(), card.ToString);
			}
		}

		[Test]
		public void Single_faced_cards_dont_have_multiple_values_in_names_list()
		{
			foreach (var card in Repo.Cards.Where(CardLayouts.IsSingleFace))
			{
				Assert.That(card.Names, Is.Null.Or.Count.LessThanOrEqualTo(1), card.ToString);

				if (card.Names?.Count == 1)
					Assert.That(card.NameNormalized, Is.EqualTo(card.Names[0]), card.ToString);
			}
		}

		[Test]
		public void Multiface_cards_have_expected_faces_count()
		{
			foreach (var card in Repo.Cards.Where(CardLayouts.IsMultiFace))
			{
				Assert.That(card.Names, Is.Not.Null, card.ToString);

				if (card.IsMeld())
					Assert.That(card.Names.Count == 3, card.ToString);
				else if (card.IsSplit()) // some unset cards have > 2 faces
					Assert.That(card.Names.Count > 1, card.ToString);
				else
					Assert.That(card.Names.Count == 2, card.ToString);
			}
		}

		[Test]
		public void Multiface_cards_report_same_faces_order()
		{
			foreach (var card in Repo.Cards.Where(CardLayouts.IsMultiFace))
				foreach (var name in card.Names)
					foreach (var faceVariant in card.Set.CardsByName[name])
						Assert.That(faceVariant.Names.SequenceEqual(card.Names), card.ToString);
		}
	}
}