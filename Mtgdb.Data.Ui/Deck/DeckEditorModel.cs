using System;
using System.Collections.Generic;
using System.Linq;
using Mtgdb.Data;

namespace Mtgdb.Ui
{
	public class DeckEditorModel : ICardCollection
	{
		public DeckEditorModel()
		{
			MainDeck = new DeckZoneModel();
			SideDeck = new DeckZoneModel();
			MaybeDeck = new DeckZoneModel();
			SampleHand = new DeckZoneModel();

			CurrentZone = Zone.Main;
		}

		public void SetZone(Zone? value, CardRepository repo)
		{
			CurrentZone = value;
			LoadDeck(repo);
			ZoneChanged?.Invoke();
		}

		private DeckZoneModel getDeckZone(Zone zone)
		{
			switch (zone)
			{
				case Zone.Main:
					return MainDeck;

				case Zone.Side:
					return SideDeck;

				case Zone.Maybe:
					return MaybeDeck;

				case Zone.SampleHand:
					return SampleHand;

				default:
					throw new NotSupportedException();
			}
		}

		public List<Card> GetVisibleCards() =>
			getReorderedCards(DraggedCard, CardBelowDragged);


		public void SetDeck(Deck deck, CardRepository repo)
		{
			lock (DataSource)
				DataSource.Clear();

			MainDeck.SetDeck(deck.MainDeck);
			SideDeck.SetDeck(deck.Sideboard);
			MaybeDeck.SetDeck(deck.Maybeboard);

			LoadDeck(repo);
		}

		public void LoadDeck(CardRepository cardRepository)
		{
			if (!cardRepository.IsLoadingComplete.Signaled)
				return;

			lock (DataSource)
			{
				DataSource.Clear();

				Card getCard(string id) =>
					cardRepository.CardsById[id];

				if (CurrentZone.HasValue)
					DataSource.AddRange(Deck.CardsIds.Select(getCard));
				else
					DataSource.AddRange(MainDeck.CardsIds.Union(SideDeck.CardsIds).Union(MaybeDeck.CardsIds).Select(getCard));
			}

			DeckChanged?.Invoke(
				listChanged: true,
				countChanged: true,
				card: null,
				touchedChanged: false,
				changedZone: CurrentZone,
				changeTerminatesBatch: true);
		}

		public void Add(Card card, int increment, Card at = null, Zone? zone = null, bool changeTerminatesBatch = true)
		{
			var specificZone = zone ?? CurrentZone ?? Zone.Main;
			var deckZone = getDeckZone(specificZone);

			int previousCount = deckZone.GetCount(card.Id);

			var count = (previousCount + increment)
				.WithinRange(
					card.MinCountInDeck(),
					card.MaxCountInDeck());

			if (increment > 0)
				add(card, at, newCount: count, zone: specificZone);
			else if (increment < 0)
				remove(card, newCount: count, zone: specificZone);

			var previousTouchedCard = TouchedCard;

			// reducing the amount of cards in deck when there is already 0 leads to removing last touched mark from card
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
				CurrentZone,
				changeTerminatesBatch);
		}

		private void remove(Card card, int newCount, Zone zone)
		{
			getDeckZone(zone).Remove(card.Id, newCount);

			if (zone != CurrentZone)
				return;

			if (newCount == 0)
				lock (DataSource)
					DataSource.Remove(card);
		}

		private void add(Card card, Card at, int newCount, Zone zone)
		{
			var zoneModel = getDeckZone(zone);
			int index = getInsertionIndex(card, at, zoneModel);

			zoneModel.Insert(card.Id, index, newCount);

			if (zone != CurrentZone)
				return;

			lock (DataSource)
				if (!DataSource.Contains(card))
					DataSource.Insert(index, card);
		}

		private static int getInsertionIndex(Card card, Card at, DeckZoneModel zoneModel)
		{
			int index = zoneModel.CardsIds.IndexOf((at ?? card).Id);
			if (index < 0)
				return zoneModel.CardsIds.Count;
			return index;
		}


		public void Clear()
		{
			switch (CurrentZone)
			{
				case Zone.SampleHand:
					SampleHand.Clear();
					break;
				case Zone.Side:
					SideDeck.Clear();
					break;
				case Zone.Maybe:
					MaybeDeck.Clear();
					break;
				case Zone.Main:
					MainDeck.Clear();
					SideDeck.Clear();
					MaybeDeck.Clear();
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
				changedZone: CurrentZone,
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
				changedZone: CurrentZone,
				changeTerminatesBatch: true);
		}

		public void DragStart(Card card, bool fromDeck)
		{
			DraggedCard = CardBelowDragged = card;
			IsDraggingFromZone = fromDeck ? CurrentZone : null;
		}

		public bool IsDragging =>
			DraggedCard != null;

		public void DragAbort()
		{
			DraggedCard = null;
			CardBelowDragged = null;
			IsDraggingFromZone = null;
		}

		public int GetCount(Card c) =>
			Deck.GetCount(c.Id);

		public Deck Paste(Deck source, bool append, CardRepository repo)
		{
			var zones = new[] { Zone.Main, Zone.Side, Zone.Maybe, Zone.SampleHand };
			if (zones.All(_=> source.GetZone(_).Order.Count == 0))
				return null;

			if (!append && (CurrentZone == Zone.Main || CurrentZone == Zone.Side || CurrentZone == Zone.Maybe))
				lock (DataSource)
					DataSource.Clear();

			var pastedDeck = source.Copy();

			foreach (var zone in zones)
			{
				var sourceZone = source.GetZone(zone);
				if (sourceZone.Order.Count == 0)
					continue;

				var targetZone = getDeckZone(zone);
				if (!append)
					targetZone.Clear();

				var pastedZone = pastedDeck.GetZone(zone);
				for (int i = 0; i < sourceZone.Order.Count; i++)
				{
					string cardId = sourceZone.Order[i];
					int prevCount = targetZone.CountById.TryGet(cardId);
					int increment = sourceZone.Count[cardId];

					var card = repo.CardsById[cardId];

					int newCount = (prevCount + increment)
						.WithinRange(card.MinCountInDeck(), card.MaxCountInDeck());

					int delta = newCount - prevCount;

					pastedZone.Count[cardId] = delta;
					pastedZone.CountList[i] = delta;

					targetZone.Add(cardId, newCount);
				}
			}

			LoadDeck(repo);
			return pastedDeck;
		}

		public void NewSampleHand(CardRepository cardRepository)
		{
			if (!cardRepository.IsLoadingComplete.Signaled || CurrentZone != Zone.SampleHand)
				return;

			createSampleHand(7, cardRepository);
		}

		public void Mulligan(CardRepository cardRepository)
		{
			if (!cardRepository.IsLoadingComplete.Signaled || CurrentZone != Zone.SampleHand)
				return;

			int count = getMulliganCount();
			createSampleHand(count, cardRepository);
		}

		public void Draw(CardRepository cardRepository)
		{
			if (!cardRepository.IsLoadingComplete.Signaled || CurrentZone != Zone.SampleHand)
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
				changedZone: CurrentZone,
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
				changedZone: CurrentZone,
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

		public Deck Snapshot()
		{
			var result = Ui.Deck.Create(
				MainDeck.CountById.ToDictionary(),
				MainDeck.CardsIds.ToList(),
				SideDeck.CountById.ToDictionary(),
				SideDeck.CardsIds.ToList(),
				MaybeDeck.CountById.ToDictionary(),
				MaybeDeck.CardsIds.ToList(),
				SampleHand.CountById.ToDictionary(),
				SampleHand.CardsIds.ToList());

			result.Name = DeckName;
			result.File = DeckFile;

			return result;
		}

		public DeckZoneSnapshot SnapshotZone() =>
			new DeckZoneSnapshot(this);

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


		public event DeckChangedEventHandler DeckChanged;
		public event Action ZoneChanged;

		public readonly List<Card> DataSource = new List<Card>();

		public DeckZoneModel Deck => getDeckZone(CurrentZone ?? Zone.Main);

		public int MainDeckSize(CardRepository repo) =>
			countNonTokens(repo, MainDeck);

		public int SideDeckSize(CardRepository repo) =>
			countNonTokens(repo, SideDeck);

		public int MaybeDeckSize(CardRepository repo) =>
			countNonTokens(repo, MaybeDeck);

		public int SampleHandSize(CardRepository repo) =>
			countNonTokens(repo, SampleHand);

		private static int countNonTokens(CardRepository repo, DeckZoneModel zone)
		{
			if (repo.IsLoadingComplete.Signaled)
				return zone.CountById
					.Where(_ => !repo.CardsById[_.Key].IsToken)
					.Sum(_ => _.Value);

			return zone.CountById.Values.Sum();
		}

		public Card DraggedCard { get; set; }
		public Card CardBelowDragged { get; set; }
		public Card TouchedCard { get; set; }
		public Zone? IsDraggingFromZone { get; set; }

		public DeckZoneModel MainDeck { get; }
		public DeckZoneModel SideDeck { get; }
		public DeckZoneModel MaybeDeck { get; }
		public DeckZoneModel SampleHand { get; }

		public Zone? CurrentZone { get; private set; }

		public FsPath DeckFile { get; set; }
		public string DeckName { get; set; }

		private static readonly Random _random = new Random();
		private List<string> _library;
	}
}
