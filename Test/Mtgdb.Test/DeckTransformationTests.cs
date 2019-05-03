using System;
using System.Linq;
using Mtgdb.Data;
using Mtgdb.Data.Model;
using Mtgdb.Ui;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class DeckTransformationTests : TestsBase
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_repo = new CardRepository
			{
				FilterSetCode = F.IsWithin(Sequence.From("ME2", "ICE"), Str.Comparer)
			};

			_repo.LoadFile();
			_repo.Load();
			_transformation = new CollectedCardsDeckTransformation(_repo);
		}

		[SetUp]
		public void Setup()
		{
			_deckEditor = new DeckEditorModel();
			_collectionEditor = new CollectionEditorModel();
		}

		[Test]
		public void Plains_have_price()
		{
			var cards = _repo.CardsByName["plains"];

			foreach (var card in cards)
				Assert.That(card.PriceMid, Is.Not.Null);
		}

		[Test]
		public void Price_can_be_temporarily_cleared()
		{
			const string name = "plains";
			var card = _repo.CardsByName[name][1];

			using (TemporaryPrice.Clear(card))
				Assert.That(card.PriceMid, Is.Null);

			Assert.That(card.PriceMid, Is.Not.Null);
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

			var cardKnownPrice = cardVariants.First(c => c.PricingMid.HasValue);
			var cardUnknownPrice = cardVariants.FirstOrDefault(c => !c.PriceMid.HasValue) ??
				cardVariants.First(c => c != cardKnownPrice);

			_deckEditor.Add(cardUnknownPrice, count);

			var deck = _deckEditor.Snapshot();
			var collection = _collectionEditor.Snapshot();

			using (TemporaryPrice.Clear(cardUnknownPrice))
			{
				var transformed = _transformation.Transform(deck, collection);

				Assert.That(transformed.MainDeck.Count.ContainsKey(cardKnownPrice.Id));
				Assert.That(transformed.MainDeck.Count[cardKnownPrice.Id], Is.EqualTo(count));
				Assert.That(transformed.MainDeck.Count.ContainsKey(cardUnknownPrice.Id), Is.False);
			}
		}

		[Test]
		public void Collected_is_preferred_over_Known_price()
		{
			const string name = "Snow-Covered Plains";

			var cardVariants = _repo.CardsByName[name];

			var cardKnownPrice = cardVariants.First(c => c.PricingMid.HasValue);
			var cardCollectedUnknownPrice = cardVariants.FirstOrDefault(c => !c.PriceMid.HasValue) ??
				cardVariants.First(c => c != cardKnownPrice);

			_collectionEditor.Add(cardCollectedUnknownPrice, 1);
			_deckEditor.Add(cardKnownPrice, 3);

			var deck = _deckEditor.Snapshot();
			var collection = _collectionEditor.Snapshot();

			using (TemporaryPrice.Clear(cardCollectedUnknownPrice))
			{
				var transformed = _transformation.Transform(deck, collection);

				Assert.That(transformed.MainDeck.Count[cardKnownPrice.Id], Is.EqualTo(2));
				Assert.That(transformed.MainDeck.Count[cardCollectedUnknownPrice.Id], Is.EqualTo(1));
			}
		}

		private CardRepository _repo;
		private CollectedCardsDeckTransformation _transformation;
		private DeckEditorModel _deckEditor;
		private CollectionEditorModel _collectionEditor;

		private class TemporaryPrice : IDisposable
		{
			public static TemporaryPrice Clear(Card c) =>
				new TemporaryPrice(c, null);

			public static TemporaryPrice Set(Card c, MtgjsonPrices p) =>
				new TemporaryPrice(c, p);

			private TemporaryPrice(Card c, MtgjsonPrices p)
			{
				_originalPrice = c.MtgjsonPrices;
				_card = c;
				c.MtgjsonPrices = p;
			}

			public void Dispose() =>
				_card.MtgjsonPrices = _originalPrice;

			private readonly MtgjsonPrices _originalPrice;
			private readonly Card _card;
		}
	}
}
