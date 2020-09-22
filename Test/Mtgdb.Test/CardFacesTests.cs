using System.Linq;
using FluentAssertions;
using Mtgdb.Data;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class CardLayoutTests : TestsBase
	{
		[OneTimeSetUp]
		public void OneTimeSetup() =>
			LoadCards();

		[SetUp]
		public void Setup()
		{
			Formatter.CustomLayout = new[]
			{
				nameof(Card.SetCode),
				nameof(Card.NameEn),
			};
		}

		[TearDown]
		public void Teardown()
		{
			Formatter.CustomLayout = null;
		}

		[Test]
		public void Single_faced_cards_dont_have_multiple_values_in_names_list()
		{
			foreach (var card in Repo.Cards.Where(CardLayouts.IsSingleFace))
			{
				if (card.OtherFaceIds != null && card.OtherFaceIds.Count > 0)
					Assert.Fail("card with single-face layout {0} has {1} sides: {2}", card.Layout, card.OtherFaceIds.Count, card);
			}
		}

		[Test]
		public void Meld_cards_have_side_a_and_b()
		{
			foreach (var card in Repo.Cards.Where(CardLayouts.IsMeld))
				Assert.That(card.Side, Is.EqualTo(CardSides.A).Or.EqualTo(CardSides.B));
		}

		[Test]
		public void Meld_side_a_only_references_meld_result()
		{
			foreach (var card in Repo.Cards.Where(CardLayouts.IsMeld).Where(CardSides.IsSideA))
			{
				Assert.That(card.OtherFaceIdsMtgjson, Has.Count.EqualTo(1));
				Assert.That(card.Set.MapById(card.IsToken)[card.OtherFaceIdsMtgjson[0]].Side, Is.EqualTo(CardSides.B));
			}
		}

		[Test]
		public void Meld_side_b_references_both_meld_parts()
		{
			foreach (var card in Repo.Cards.Where(CardLayouts.IsMeld).Where(CardSides.IsSideB))
			{
				Assert.That(card.OtherFaceIdsMtgjson, Has.Count.EqualTo(2));
				Assert.That(card.OtherFaceIdsMtgjson[0], Is.Not.EqualTo(card.OtherFaceIdsMtgjson[1]));
				for (int i = 0; i < 2; i++)
					Assert.That(card.Set.MapById(card.IsToken)[card.OtherFaceIdsMtgjson[i]].Side, Is.EqualTo(CardSides.A));
			}
		}

		[Test]
		public void Multiface_cards_should_specify_other_faces() =>
			Repo.Cards.Where(CardLayouts.IsMultiFace)
				.Should().NotContain(c => c.OtherFaceIds == null || c.OtherFaceIds.Count == 0);

		[Test]
		public void Meld_cards_should_specify_two_other_faces() =>
			Repo.Cards.Where(CardLayouts.IsMeld)
				.Should().OnlyContain(c => c.OtherFaceIds.Count == 2);

		[Test]
		public void Split_cards_should_specify_other_faces() =>
			Repo.Cards.Where(CardLayouts.IsSplit)
				.Should().OnlyContain(c => c.OtherFaceIds.Count > 0);

		[Test]
		public void Typical_multiface_layouts_have_2_faces() =>
			Repo.Cards.Where(CardLayouts.IsMultiFace).Where(c => !c.IsMeld() && !c.IsSplit())
				.Should().OnlyContain(c => c.OtherFaceIds.Count == 1);

		[Test]
		public void Multiface_tokens_do_not_exist() =>
			Repo.Cards.Where(CardLayouts.IsMultiFace).Where(c => c.IsToken)
				.Should().BeEmpty();

		[Test]
		public void Aftermath_card_sides_Have_layout_aftermath()
		{
			var aftermathCards = Repo.Cards
				.Where(_ => _.GetCastKeywords().Contains(KeywordDefinitions.AftermathKeyword))
				.ToArray();

			Assert.That(aftermathCards, Is.Not.Null.And.Not.Empty);

			foreach (var card in aftermathCards)
			{
				Assert.That(card.Layout, Is.EqualTo(CardLayouts.Aftermath).IgnoreCase);

				foreach (var rotatedCard in card.OtherFaces)
					Assert.That(rotatedCard.Layout, Is.EqualTo(CardLayouts.Aftermath).IgnoreCase);
			}
		}
	}
}
