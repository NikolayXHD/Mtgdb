using System;
using System.Collections.Generic;
using System.Linq;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class DeckModel : ICardCollection
	{
		public event DeckChangedEventHandler DeckChanged;

		public readonly List<Card> DataSource = new List<Card>();

		public Card CardDragged;
		public Card CardBelowDragged { get; set; }
		public Card TouchedCard { get; set; }
		public bool DraggingFromDeck;

		public DeckZoneModel MainDeck { get; }
		public DeckZoneModel SideDeck { get; }
		public DeckZoneModel SampleHand { get; }

		public Zone Zone { get; private set; }

		public void SetZone(Zone value, CardRepository repo)
		{
			Zone = value;
			LoadDeck(repo);
		}

		public DeckZoneModel Deck
		{
			get
			{
				switch (Zone)
				{
					case Zone.Main:
						return MainDeck;
					case Zone.Side:
						return SideDeck;
					case Zone.SampleHand:
						return SampleHand;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public int MainDeckSize => MainDeck.CountById.Values.Sum();
		public int SideDeckSize => SideDeck.CountById.Values.Sum();
		public int SampleHandSize => SampleHand.CountById.Values.Sum();

		public DeckModel()
		{
			MainDeck = new DeckZoneModel();
			SideDeck = new DeckZoneModel();
			SampleHand = new DeckZoneModel();
		}

		public void SetDeck(Deck deck)
		{
			Clear(loadingDeck: true);

			MainDeck.SetDeck(deck.MainDeck);
			SideDeck.SetDeck(deck.SideDeck);
		}

		public void LoadDeck(CardRepository cardRepository)
		{
			Clear(loadingDeck: true);

			foreach (var id in Deck.CardsIds)
			{
				Card card;
				if (cardRepository.CardsById == null || !cardRepository.CardsById.TryGetValue(id, out card))
					continue;

				add(card, loadingDeck: true, newCount: card.DeckCount);
			}

			DeckChanged?.Invoke(
				listChanged: true,
				countChanged: true,
				card: null,
				touchedChanged: false);
		}

		public void Add(Card card, int increment, bool touch)
		{
			int previousCount = Deck.GetCount(card.Id);
			var count = previousCount + increment;

			if (card.Rarity != @"Basic Land" && count > 4)
				count = 4;
			else if (count < 0)
				count = 0;

			if (increment > 0)
				add(card, loadingDeck: false, newCount: count);
			else if (increment < 0)
				remove(card, count);

			var previousTouchedCard = TouchedCard;

			if (touch)
			{
				// уменьшение количества карты, которой в колоде уже 0 приводит к снятию отметки о последнем прикосновении
				if (previousCount == 0 && increment < 0)
					TouchedCard = null;
				else
					TouchedCard = card;
			}

			bool listChanged = previousCount == 0 || card.DeckCount == 0;
			bool countChanged = previousCount != card.DeckCount;
			bool touchedChanged = previousTouchedCard != TouchedCard;

			DeckChanged?.Invoke(
				listChanged,
				countChanged,
				card,
				touchedChanged);
		}

		private void remove(Card card, int newCount)
		{
			Deck.Remove(card.Id, newCount);

			if (newCount == 0)
				lock (DataSource)
					DataSource.Remove(card);
		}

		private void add(Card card, bool loadingDeck, int newCount)
		{
			if (!loadingDeck)
				Deck.Add(card.Id, newCount);

			lock (DataSource)
				if (!DataSource.Contains(card))
					DataSource.Add(card);
		}


		public void Clear(bool loadingDeck)
		{
			lock (DataSource)
				DataSource.Clear();

			if (!loadingDeck)
			{
				MainDeck.Clear();
				SideDeck.Clear();

				DeckChanged?.Invoke(
					listChanged: true,
					countChanged: true,
					card: null,
					touchedChanged: false);
			}
		}

		public List<Card> GetReorderedCards(Card cardDragged, Card cardBelowDragged)
		{
			List<Card> copy;

			lock (DataSource)
				copy = DataSource.ToList();

			if (cardBelowDragged == null || cardDragged == null || !DraggingFromDeck)
				return copy;

			var cardBelowDraggedIndex = copy.IndexOf(cardBelowDragged);
			var cardDraggedIndex = copy.IndexOf(cardDragged);

			if (cardDraggedIndex >= 0)
				copy.RemoveAt(cardDraggedIndex);

			if (cardBelowDraggedIndex >= 0)
				copy.Insert(cardBelowDraggedIndex, cardDragged);
			else
				copy.Add(cardDragged);

			return copy;
		}

		private List<string> getReorderedIds(Card cardDragged, Card cardBelowDragged)
		{
			var copy = Deck.CardsIds.ToList();

			if (cardBelowDragged == null || cardDragged == null)
				return copy;

			var cardBelowDraggedIndex = copy.IndexOf(cardBelowDragged.Id);
			var cardDraggedIndex = copy.IndexOf(cardDragged.Id);

			if (cardDraggedIndex >= 0)
				copy.RemoveAt(cardDraggedIndex);

			if (cardBelowDraggedIndex >= 0)
				copy.Insert(cardBelowDraggedIndex, cardDragged.Id);
			else
				copy.Add(cardDragged.Id);

			return copy;
		}

		public void ApplyReorder(Card cardDragged, Card cardBelowDragged)
		{
			var cards = GetReorderedCards(cardDragged, cardBelowDragged);
			var cardIds = getReorderedIds(cardDragged, cardBelowDragged);

			lock (DataSource)
			{
				DataSource.Clear();
				DataSource.AddRange(cards);
			}

			Deck.SetOrder(cardIds);

			DeckChanged?.Invoke(
				listChanged: true,
				countChanged: false,
				card: null,
				touchedChanged: false);
		}

		public void DragStart(Card card, bool fromDeck)
		{
			CardDragged = CardBelowDragged = card;
			DraggingFromDeck = fromDeck;
		}

		public bool IsDragging()
		{
			return CardDragged != null;
		}

		public void DragAbort()
		{
			CardDragged = null;
			CardBelowDragged = null;
		}

		public int GetCount(Card c)
		{
			return Deck.GetCount(c.Id);
		}
	}

	public enum Zone
	{
		Main = 0, Side = 1, SampleHand = 2
	}
}