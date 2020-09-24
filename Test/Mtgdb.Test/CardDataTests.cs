using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Mtgdb.Data;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class CardDataTests : TestsBase
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			LoadCards();
		}

		[Test]
		public void Id_is_unique()
		{
			Repo.Cards.Should().OnlyHaveUniqueItems(card => card.Id);
		}

		[Test]
		public void Token_ids_do_not_intersect_with_card_ids()
		{
			var cardIds = Repo.Cards.Where(_ => !_.IsToken).Select(_ => _.Id);
			var tokenIds = Repo.Cards.Where(_ => _.IsToken).Select(_ => _.Id);
			cardIds.Should().NotIntersectWith(tokenIds);
		}

		[Test]
		public void Tokens_have_exclusive_type_or_name()
		{
			foreach (Set set in Repo.SetsByCode.Values)
			{
				set.Tokens.Should().NotContain(card => !shouldBeToken(card));
				set.ActualCards.Should().NotContain(card => shouldBeToken(card));
			}
		}

		[Test]
		public void Tokens_have_IsToken_flag_set()
		{
			foreach (Set set in Repo.SetsByCode.Values)
			{
				set.Tokens.Should().NotContain(card => !card.IsToken);
				set.ActualCards.Should().NotContain(card => card.IsToken);
			}
		}

		[Test]
		public void Tokens_in_set_are_in_separate_map()
		{
			var cardNamesakeLists = new HashSet<ICollection<Card>>();
			var tokenNamesakeLists = new HashSet<ICollection<Card>>();

			foreach (Set set in Repo.SetsByCode.Values)
			{
				foreach (var (_, cards) in set.ActualCardsByName)
				foreach (Card card in cards)
				{
					card.IsToken.Should().BeFalse();
					if (cardNamesakeLists.Add(card.Namesakes))
						card.Namesakes.Should().NotContain(_ => _.IsToken);
				}

				foreach (var (_, tokens) in set.TokensByName)
				{
					foreach (Card token in tokens)
					{
						token.IsToken.Should().BeTrue();
						if (tokenNamesakeLists.Add(token.Namesakes))
							token.Namesakes.Should().NotContain(_ => !_.IsToken);
					}
				}
			}
		}

		[Test]
		public void Namesakes_within_set_have_same_printings_list()
		{
			foreach (Set set in Repo.SetsByCode.Values)
			{
				foreach (var (_, cards) in set.ActualCardsByName)
				foreach (var card in cards)
					card.Printings.Should().BeSameAs(cards[0].Printings);

				foreach (var (_, tokens) in set.TokensByName)
				foreach (var token in tokens)
					token.Printings.Should().BeSameAs(tokens[0].Printings);
			}
		}

		[Test]
		public void Printings_point_to_sets_with_namesakes()
		{
			foreach (Set set in Repo.SetsByCode.Values)
			{
				foreach (var (_, cards) in set.ActualCardsByName)
				{
					var card = cards[0];
					foreach (string setcode in card.Printings)
						Repo.SetsByCode[setcode].ActualCardsByName[card.NameNormalized].Should().NotBeEmpty();
				}

				foreach (var (_, tokens) in set.TokensByName)
				{
					var token = tokens[0];
					foreach (string setcode in token.Printings)
						Repo.SetsByCode[setcode].TokensByName[token.NameNormalized].Should().NotBeEmpty();
				}
			}
		}

		[Test]
		public void Punctuation_plus_minus_are_not_preceding_or_following_text_without_whitespace()
		{
			var precedingSignPattern = new Regex(@"(?<![\w']+)[+\-](?![xXyY]\b)\p{L}");
			Repo.CardsByName.Select(_ => _.Value[0].TextEn)
				.Where(_ => _ != null && precedingSignPattern.IsMatch(_))
				.Should().BeEmpty();

			var followingSignPattern = new Regex(@"\p{L}(?<![xXyY]\b)[+\-](?![\w']+)");
			Repo.CardsByName.Select(_ => _.Value[0].TextEn)
				.Where(_ => _ != null && followingSignPattern.IsMatch(_))
				.Should().BeEmpty();
		}

		private static bool shouldBeToken(Card c) =>
			CardCardTypes.ByName.ContainsKey(c.NameEn) || c.TypesArr.Any(CardCardTypes.ByType.ContainsKey);
	}
}
