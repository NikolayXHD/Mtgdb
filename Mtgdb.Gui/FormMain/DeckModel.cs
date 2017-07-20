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

		public DeckCards MainDeck { get; }
		public DeckCards SideDeck { get; }
		public bool IsSide { get; set; }

		public void SetIsSide(bool value, CardRepository repo)
		{
			IsSide = value;
			LoadDeck(repo);
		}

		private DeckCards Deck
		{
			get
			{
				if (IsSide)
					return SideDeck;

				return MainDeck;
			}
		}

		public int MainDeckSize => MainDeck.CountById.Values.Sum();
		public int SideDeckSize => SideDeck.CountById.Values.Sum();

		public DeckModel()
		{
			MainDeck = new DeckCards();
			SideDeck = new DeckCards();
		}

		public void SetDeck(
			Dictionary<string, int> countById, 
			List<string> order,
			Dictionary<string, int> sideCountById,
			List<string> sideOrder)
		{
			Clear(loadingDeck: true);

			MainDeck.SetDeck(countById, order);
			SideDeck.SetDeck(sideCountById, sideOrder);
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

			DeckChanged?.Invoke(true, true, false, null);
		}

		public void Add(Card card, int increment)
		{
			int previousCount = Deck.GetCount(card);
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

			// уменьшение количества карты, которой в колоде уже 0 приводит к снятию отметки о последнем прикосновении
			if (previousCount == 0 && increment < 0)
				TouchedCard = null;
			else
				TouchedCard = card;

			bool listChanged = previousCount == 0 || card.DeckCount == 0;
			bool countChanged = previousCount != card.DeckCount;
			bool touchedChanged = previousTouchedCard != TouchedCard;
			DeckChanged?.Invoke(listChanged, countChanged, touchedChanged, card);
		}

		private void remove(Card card, int newCount)
		{
			Deck.Remove(card, newCount);

			if (newCount == 0)
				lock (DataSource)
					DataSource.Remove(card);
		}

		private void add(Card card, bool loadingDeck, int newCount)
		{
			if (!loadingDeck)
				Deck.Add(card, newCount);

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
				Deck.Clear();
				DeckChanged?.Invoke(true, true, false, null);
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
			DeckChanged?.Invoke(true, false, false, null);
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
			return Deck.GetCount(c);
		}
	}
}