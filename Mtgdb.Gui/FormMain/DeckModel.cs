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

		public Card DraggedCard { get; set; }
		public Card CardBelowDragged { get; set; }
		public Card TouchedCard { get; set; }
		public Zone? IsDraggingFromZone { get; set; }

		public DeckZoneModel MainDeck { get; }
		public DeckZoneModel SideDeck { get; }
		public DeckZoneModel SampleHand { get; }

		public Zone Zone { get; private set; }

		public void SetZone(Zone value, CardRepository repo)
		{
			Zone = value;
			LoadDeck(repo);
		}

		public DeckZoneModel Deck => getDeckZone(Zone);

		private DeckZoneModel getDeckZone(Zone zone)
		{
			switch (zone)
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

		public int MainDeckSize => MainDeck.CountById.Values.Sum();
		public int SideDeckSize => SideDeck.CountById.Values.Sum();
		public int SampleHandSize => SampleHand.CountById.Values.Sum();

		public IList<Card> GetVisibleCards()
		{
			if (IsDraggingFromZone == Zone)
				return getReorderedCards(DraggedCard, CardBelowDragged);

			lock (DataSource)
				return DataSource.ToList();
		}


		public DeckModel()
		{
			MainDeck = new DeckZoneModel();
			SideDeck = new DeckZoneModel();
			SampleHand = new DeckZoneModel();
		}

		public void SetDeck(Deck deck, CardRepository repo)
		{
			lock (DataSource)
				DataSource.Clear();

			MainDeck.SetDeck(deck.MainDeck);
			SideDeck.SetDeck(deck.SideDeck);

			LoadDeck(repo);
		}

		public void LoadDeck(CardRepository cardRepository)
		{
			if (!cardRepository.IsLoadingComplete)
				return;

			lock (DataSource)
			{
				DataSource.Clear();
				DataSource.AddRange(Deck.CardsIds.Select(id => cardRepository.CardsById[id]));
			}

			DeckChanged?.Invoke(
				listChanged: true,
				countChanged: true,
				card: null,
				touchedChanged: false,
				changedZone: Zone,
				changeTerminatesBatch: true);
		}

		public void Add(Card card, int increment, Zone? zone = null, bool changeTerminatesBatch = true)
		{
			var specificZone = zone ?? Zone;
			var deckZone = getDeckZone(specificZone);

			int previousCount = deckZone.GetCount(card.Id);

			var count = (previousCount + increment)
				.WithinRange(
					card.MinCountInDeck(),
					card.MaxCountInDeck());

			if (increment > 0)
				add(card, newCount: count, zone: specificZone);
			else if (increment < 0)
				remove(card, newCount: count, zone: specificZone);

			var previousTouchedCard = TouchedCard;
			
			// уменьшение количества карты, которой в колоде уже 0 приводит к снятию отметки о последнем прикосновении
			if (previousCount == 0 && increment < 0)
				TouchedCard = null;
			else
				TouchedCard = card;

			bool listChanged = previousCount == 0 || GetCount(card) == 0;
			bool countChanged = previousCount != GetCount(card);
			bool touchedChanged = previousTouchedCard != TouchedCard;

			DeckChanged?.Invoke(
				listChanged,
				countChanged,
				card,
				touchedChanged,
				Zone,
				changeTerminatesBatch);
		}

		private void remove(Card card, int newCount, Zone zone)
		{
			getDeckZone(zone).Remove(card.Id, newCount);

			if (zone != Zone)
				return;

			if (newCount == 0)
				lock (DataSource)
					DataSource.Remove(card);
		}

		private void add(Card card, int newCount, Zone zone)
		{
			getDeckZone(zone).Add(card.Id, newCount);

			if (zone != Zone)
				return;

			lock (DataSource)
				if (!DataSource.Contains(card))
					DataSource.Add(card);
		}


		public void Clear()
		{
			switch (Zone)
			{
				case Zone.SampleHand:
					SampleHand.Clear();
					break;
				case Zone.Side:
					SideDeck.Clear();
					break;
				case Zone.Main:
					MainDeck.Clear();
					SideDeck.Clear();
					break;
				default:
					return;
			}

			lock (DataSource)
				DataSource.Clear();

			DeckChanged?.Invoke(
				listChanged: true,
				countChanged: true,
				card: null,
				touchedChanged: false,
				changedZone: Zone,
				changeTerminatesBatch: true);
		}

		private List<Card> getReorderedCards(Card cardDragged, Card cardBelowDragged)
		{
			List<Card> copy;

			lock (DataSource)
				copy = DataSource.ToList();

			if (cardBelowDragged == null || cardDragged == null)
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
			// possible if some keyboard shortcuts (e.g. undo / redo or paste)
			// modified the deck while dragging the card
			if (!Deck.Contains(cardDragged))
				return;

			var cards = getReorderedCards(cardDragged, cardBelowDragged);
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
				touchedChanged: false,
				changedZone: Zone,
				changeTerminatesBatch: true);
		}

		public void DragStart(Card card, bool fromDeck)
		{
			DraggedCard = CardBelowDragged = card;
			IsDraggingFromZone = fromDeck ? Zone : (Zone?) null;
		}

		public bool IsDragging()
		{
			return DraggedCard != null;
		}

		public void DragAbort()
		{
			DraggedCard = null;
			CardBelowDragged = null;
			IsDraggingFromZone = null;
		}

		public int GetCount(Card c)
		{
			return Deck.GetCount(c.Id);
		}


		public void Paste(Deck deck, bool append, CardRepository repo)
		{
			var operations = new Dictionary<DeckZone, DeckZoneModel>();

			switch (Zone)
			{
				case Zone.Main:
					operations.Add(deck.MainDeck, MainDeck);
					if (deck.SideDeck.Order.Count > 0)
						operations.Add(deck.SideDeck, SideDeck);
					break;
				case Zone.Side:
					operations.Add(deck.MainDeck, SideDeck);
					break;
				case Zone.SampleHand:
					operations.Add(deck.MainDeck, SampleHand);
					break;
				default:
					return;
			}

			if (!append && (Zone == Zone.Main || Zone == Zone.Side))
				lock (DataSource)
					DataSource.Clear();

			foreach (var operation in operations)
			{
				if (!append)
					operation.Value.Clear();

				foreach (var cardId in operation.Key.Order)
				{
					var prevCount = operation.Value.CountById.TryGet(cardId);
					var increment = operation.Key.Count[cardId];

					var card = repo.CardsById[cardId];

					var newCount = (prevCount + increment)
						.WithinRange(card.MinCountInDeck(), card.MaxCountInDeck());

					operation.Value.Add(cardId, newCount);
				}
			}

			LoadDeck(repo);
		}

		public void NewSampleHand(CardRepository cardRepository)
		{
			if (!cardRepository.IsLoadingComplete || Zone != Zone.SampleHand)
				return;

			createSampleHand(7, cardRepository);
		}

		public void Mulligan(CardRepository cardRepository)
		{
			if (!cardRepository.IsLoadingComplete || Zone != Zone.SampleHand)
				return;

			int count = getMulliganCount();
			createSampleHand(count, cardRepository);
		}

		public void Draw(CardRepository cardRepository)
		{
			if (!cardRepository.IsLoadingComplete || Zone != Zone.SampleHand)
				return;

			var drawn = draw(cardRepository);

			if (drawn == null)
				return;

			TouchedCard = drawn;

			lock (DataSource)
				if (!DataSource.Contains(drawn))
					DataSource.Add(drawn);

			DeckChanged?.Invoke(
				listChanged: SampleHand.GetCount(drawn.Id) == 1,
				countChanged: true,
				card: drawn,
				touchedChanged: true,
				changedZone: Zone,
				changeTerminatesBatch: true);
		}

		private int getMulliganCount()
		{
			return Math.Max(0, SampleHand.CountById.Sum(_ => _.Value) - 1);
		}

		private void createSampleHand(int handSize, CardRepository cardRepository)
		{
			SampleHand.Clear();

			Shuffle();

			for (int i = 0; i < handSize; i++)
				draw(cardRepository);

			SampleHand.SetOrder(SampleHand.CardsIds
				.OrderBy(id => cardRepository.CardsById[id].Cmc)
				.ThenBy(id => cardRepository.CardsById[id].TypeEn)
				.ThenBy(id => cardRepository.CardsById[id].Color)
				.ToList());

			lock (DataSource)
			{
				DataSource.Clear();
				DataSource.AddRange(SampleHand.CardsIds.Select(id => cardRepository.CardsById[id]));
			}

			DeckChanged?.Invoke(
				listChanged: true,
				countChanged: true,
				card: null,
				touchedChanged: false,
				changedZone: Zone,
				changeTerminatesBatch: true);
		}

		public void Shuffle()
		{
			var library = new List<string>();

			foreach (var pair in MainDeck.CountById)
				for (int i = 0; i < pair.Value; i++)
					library.Add(pair.Key);

			_library = library;
		}

		private Card draw(CardRepository cardRepository)
		{
			if (_library == null || _library.Count == 0)
				return null;

			var index = _random.Next(_library.Count);
			var id = _library[index];
			_library.RemoveAt(index);

			SampleHand.Add(id, SampleHand.GetCount(id) + 1);

			return cardRepository.CardsById[id];
		}

		private static readonly Random _random = new Random();
		private List<string> _library;
	}
}