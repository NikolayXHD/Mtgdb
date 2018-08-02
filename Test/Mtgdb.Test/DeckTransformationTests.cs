using System;
using Mtgdb.Controls;
using Mtgdb.Dal;
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
				FilterSetCode = F.IsEqualTo("ISD", Str.Comparer)
			};

			_repo.LoadFile();
			_repo.Load();

			_priceRepo = new PriceRepository();
			_priceRepo.Load();

			_repo.FillPrices(_priceRepo);
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
			const string name = "plains";
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
			const string name = "plains";
			const int count = 1;

			var cardKnownPrice = _repo.CardsByName[name][0];
			var cardUnknownPrice = _repo.CardsByName[name][1];

			_deckEditor.Add(cardUnknownPrice, count);

			var deck = _deckEditor.Snapshot();
			var collection = _collectionEditor.Snapshot();

			using (TemporaryPrice.Clear(cardUnknownPrice))
			{
				var transformed = _transformation.Transform(deck, collection);

				Assert.That(transformed.MainDeck.Count[cardKnownPrice.Id], Is.EqualTo(count));
				Assert.That(transformed.MainDeck.Count.ContainsKey(cardUnknownPrice.Id), Is.False);
			}
		}

		[Test]
		public void Collected_is_preferred_over_Known_price()
		{
			const string name = "plains";

			var cardKnownPrice = _repo.CardsByName[name][0];
			var cardCollectedUnknownPrice = _repo.CardsByName[name][1];

			_collectionEditor.Add(cardCollectedUnknownPrice, 1);
			_deckEditor.Add(cardKnownPrice, 2);

			var deck = _deckEditor.Snapshot();
			var collection = _collectionEditor.Snapshot();

			using (TemporaryPrice.Clear(cardCollectedUnknownPrice))
			{
				var transformed = _transformation.Transform(deck, collection);

				Assert.That(transformed.MainDeck.Count[cardKnownPrice.Id], Is.EqualTo(1));
				Assert.That(transformed.MainDeck.Count[cardCollectedUnknownPrice.Id], Is.EqualTo(1));
			}
		}

		private CardRepository _repo;
		private PriceRepository _priceRepo;
		private CollectedCardsDeckTransformation _transformation;
		private DeckEditorModel _deckEditor;
		private CollectionEditorModel _collectionEditor;

		private class TemporaryPrice : IDisposable
		{
			public static TemporaryPrice Clear(Card c) =>
				new TemporaryPrice(c, null);

			public static TemporaryPrice Set(Card c, PriceValues p) =>
				new TemporaryPrice(c, p);

			private TemporaryPrice(Card c, PriceValues p)
			{
				_originalPrice = c.PricesValues;
				_card = c;
				c.PricesValues = p;
			}

			public void Dispose() =>
				_card.PricesValues = _originalPrice;

			private readonly PriceValues _originalPrice;
			private readonly Card _card;
		}
	}
}
