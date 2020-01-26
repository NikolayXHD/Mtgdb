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
			{
				Assert.That(card.Names, Is.Null.Or.Count.LessThanOrEqualTo(1), card.ToStringShort);

				if (card.Names?.Count == 1)
					Assert.That(card.NameNormalized, Is.EqualTo(card.Names[0]), card.ToStringShort);
			}
		}

		[Test]
		public void Multiface_cards_have_expected_faces_count()
		{
			var multifaceCards = Repo.Cards.Where(card => card.IsMultiFace()).ToArray();
			multifaceCards
				.Should().NotContain(c => !c.IsToken && c.Names == null);

			multifaceCards.Where(c => c.IsMeld())
				.Should().OnlyContain(c => c.Names.Count == 3);

			multifaceCards.Where(c => c.IsSplit())
				.Should().OnlyContain(c => c.Names.Count > 1);

			multifaceCards.Where(c => !c.IsMeld() && !c.IsSplit() && !c.IsToken)
				.Should().OnlyContain(c => c.Names.Count == 2);

			multifaceCards.Where(c => c.IsToken)
				.Should().OnlyContain(c => c.IsTransform());

			multifaceCards.Where(c => c.IsToken && c.IsTransform())
				.Should().OnlyContain(c =>
					c.Names != null && c.Names.Count == 2 ||
					!string.IsNullOrEmpty(c.Side) &&
					c.Namesakes.Any(ns => ns.Set == c.Set && !string.IsNullOrEmpty(ns.Side) && ns.Side != c.Side));
		}

		[Test]
		public void Multiface_cards_report_same_faces_order()
		{
			foreach (var card in Repo.Cards.Where(c => c.IsMultiFace() && !c.IsToken))
				foreach (var name in card.Names)
					foreach (var faceVariant in card.Set.MapByName(card.IsToken)[name])
						Assert.That(faceVariant.Names.SequenceEqual(card.Names), card.ToString);
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

				foreach (var rotatedCard in card.FaceVariants.Main)
					Assert.That(rotatedCard.Layout, Is.EqualTo(CardLayouts.Aftermath).IgnoreCase);
			}
		}
	}
}
