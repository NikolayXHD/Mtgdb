using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mtgdb.Data;
using Mtgdb.Data.Model;
using Mtgdb.Ui;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	[Parallelizable(ParallelScope.Self)]
	public class DeckTransformationTests : TestsBase
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			LoadModules();

			_repo = new CardRepository(Kernel.Get<CardFormatter>(), () => null)
			{
				FilterSetCode = F.IsWithin(Sequence.From("ME2", "ICE"), Str.Comparer),
			};

			var priceRepo = new PriceRepository(() => null);

			_repo.LoadFile();
			_repo.Load();

			priceRepo.FillPrice(_repo);

			_transformation = new CollectedCardsDeckTransformation(_repo, priceRepo);
		}

		[SetUp]
		public void Setup()
		{
			_deckEditor = new DeckEditorModel();
			_collectionEditor = new CollectionEditorModel();
		}

		[Test]
		public void Price_can_be_temporarily_set()
		{
			var card = _repo.CardsByName["plains"][0];
			using var tempPrice = TemporaryPrice.Set(card, 1f);
			Assert.That(card.Price, Is.Not.Null);
		}

		[Test]
		public void Price_can_be_temporarily_cleared()
		{
			var card = _repo.CardsByName["plains"][1];
			using (TemporaryPrice.Set(card, 1f))
			{
				Assert.That(card.Price, Is.Not.Null);
				using (TemporaryPrice.Clear(card))
					Assert.That(card.Price, Is.Null);
			}
		}

		[Test]
		public void Collected_cards_are_preferred()
		{
			const string name = "Plains";
			const int count = 1;

			var cardCollected = _repo.CardsByName[name][0];
			var cardOther = _repo.CardsByName[name][1];

			_collectionEditor.Add(cardCollected, count);
			_deckEditor.Add(cardOther, count);

			var deck = _deckEditor.Snapshot();
			var collection = _collectionEditor.Snapshot();

			var transformed = _transformation.Transform(deck, collection);

			Assert.That(transformed.MainDeck.Count[cardCollected.Id], Is.EqualTo(count));
			Assert.That(transformed.MainDeck.Count.ContainsKey(cardOther.Id), Is.False);
		}

		[Test]
		public void Known_price_is_preferred()
		{
			const string name = "Snow-Covered Plains";
			const int count = 1;

			var cardVariants = _repo.CardsByName[name];

			var cardKnownPrice = cardVariants[0];
			using var tempPrice = TemporaryPrice.Set(cardKnownPrice, 1f);
			var cardUnknownPrice = cardVariants[1];
			cardKnownPrice.Price.Should().NotBeNull();
			cardUnknownPrice.Price.Should().BeNull();

			_deckEditor.Add(cardUnknownPrice, count);

			var deck = _deckEditor.Snapshot();
			var collection = _collectionEditor.Snapshot();

			var transformed = _transformation.Transform(deck, collection);
			Assert.That(transformed.MainDeck.Count.ContainsKey(cardKnownPrice.Id));
			Assert.That(transformed.MainDeck.Count[cardKnownPrice.Id], Is.EqualTo(count));
			Assert.That(transformed.MainDeck.Count.ContainsKey(cardUnknownPrice.Id), Is.False);
		}

		[Test]
		public void Collected_is_preferred_over_Known_price()
		{
			const string name = "Snow-Covered Plains";

			var cardVariants = _repo.CardsByName[name];

			var cardKnownPrice = cardVariants[0];
			using var tempPrice = TemporaryPrice.Set(cardKnownPrice, 1f);
			var cardCollectedUnknownPrice = cardVariants[1];
			cardKnownPrice.Price.Should().NotBeNull();
			cardCollectedUnknownPrice.Price.Should().BeNull();

			_collectionEditor.Add(cardCollectedUnknownPrice, 1);
			_deckEditor.Add(cardKnownPrice, 3);

			var deck = _deckEditor.Snapshot();
			var collection = _collectionEditor.Snapshot();

			var transformed = _transformation.Transform(deck, collection);

			Assert.That(transformed.MainDeck.Count[cardKnownPrice.Id], Is.EqualTo(2));
			Assert.That(transformed.MainDeck.Count[cardCollectedUnknownPrice.Id], Is.EqualTo(1));
		}


		private static IEnumerable<TestCaseData> Count_is_preserved_cases()
		{
			int[] countValues = {0, 1, 2, 4};
			const string nameWithNamesakes = "Plains";
			const string nameUnique = "Abyssal Specter";
			foreach (var name in new[] {nameWithNamesakes, nameUnique})
			foreach (int countCollected in countValues)
			foreach (int countInDeck in countValues)
			foreach (int countInDeckOther in countValues)
			{
				if (countInDeck + countInDeckOther == 0 || countInDeck + countInDeckOther > 4 || name == nameUnique && countInDeckOther > 0)
					continue;
				yield return new TestCaseData(name, countCollected, countInDeck, countInDeckOther);
			}

		}

		[TestCaseSource(nameof(Count_is_preserved_cases))]
		public void Count_is_preserved(
			string name,
			int countCollected,
			int countInDeck,
			int countInDeckOther)
		{
			var cardCollected = _repo.CardsByName[name][0];

			if (countCollected > 0)
				_collectionEditor.Add(cardCollected, countCollected);
			if (countInDeck > 0)
				_deckEditor.Add(cardCollected, countInDeck);
			if (countInDeckOther > 0)
			{
				var cardOther = _repo.CardsByName[name][1];
				_deckEditor.Add(cardOther, countInDeckOther);
			}

			var deck = _deckEditor.Snapshot();
			var collection = _collectionEditor.Snapshot();

			var transformed = _transformation.Transform(deck, collection);

			var countInTransformed = transformed.MainDeck.Count
				.Where(entry => _repo.CardIdsByName[name].Contains(entry.Key))
				.Sum(entry => entry.Value);

			countInTransformed.Should().Be(countInDeck + countInDeckOther);
		}

		private CardRepository _repo;
		private CollectedCardsDeckTransformation _transformation;
		private DeckEditorModel _deckEditor;
		private CollectionEditorModel _collectionEditor;

		private class TemporaryPrice : IDisposable
		{
			public static TemporaryPrice Clear(Card c) =>
				new TemporaryPrice(c, null);

			public static TemporaryPrice Set(Card c, float? p) =>
				new TemporaryPrice(c, p);

			private TemporaryPrice(Card c, float? p)
			{
				_originalPrice = c.Price;
				_card = c;
				c.Price = p;
			}

			public void Dispose() =>
				_card.Price = _originalPrice;

			private readonly float? _originalPrice;
			private readonly Card _card;
		}
	}
}
