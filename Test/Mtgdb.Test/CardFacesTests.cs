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
		public void Setup() =>
			LoadCards();

		[Test]
		public void All_cards_have_known_layout()
		{
			Repo.Cards.Should().OnlyContain(card => card.IsKnownLayout());
			Repo.Cards.Should().OnlyContain(card => card.IsMultiFace() ^ card.IsSingleFace());
		}

		[Test]
		public void Single_faced_cards_dont_have_multiple_values_in_names_list()
		{
			foreach (var card in Repo.Cards.Where(CardLayouts.IsSingleFace))
				Assert.That(card.OtherFaceIds, Is.Null.Or.Count.EqualTo(0), card.ToStringShort);
		}

		[Test]
		public void Multiface_cards_have_expected_faces_count()
		{
			var multifaceCards = Repo.Cards.Where(card => card.IsMultiFace()).ToArray();
			multifaceCards
				.Should().NotContain(c => !c.IsToken && c.OtherFaceIds == null);

			multifaceCards.Where(c => c.IsMeld())
				.Should().OnlyContain(c => c.OtherFaceIds.Count == 2);

			multifaceCards.Where(c => c.IsSplit())
				.Should().OnlyContain(c => c.OtherFaceIds.Count > 0);

			multifaceCards.Where(c => !c.IsMeld() && !c.IsSplit() && !c.IsToken)
				.Should().OnlyContain(c => c.OtherFaceIds.Count == 1);

			multifaceCards.Where(c => c.IsToken)
				.Should().OnlyContain(c => c.IsTransform());

			multifaceCards.Where(c => c.IsToken && c.IsTransform())
				.Should().OnlyContain(c =>
					c.OtherFaceIds != null && c.OtherFaceIds.Count == 1 ||
					!string.IsNullOrEmpty(c.Side) &&
					c.Namesakes.Any(ns => ns.Set == c.Set && !string.IsNullOrEmpty(ns.Side) && !Str.Equals(ns.Side, c.Side)));
		}

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
