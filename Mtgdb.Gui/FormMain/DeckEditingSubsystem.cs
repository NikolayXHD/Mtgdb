using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Properties;

namespace Mtgdb.Gui
{
	public class DeckEditingSubsystem
	{
		private readonly LayoutView _layoutViewCards;
		private readonly LayoutView _layoutViewDeck;
		private readonly Cursor _cursor;
		private readonly DeckModel _deckModel;
		private readonly CollectionModel _collectionModel;
		private readonly ScrollSubsystem _scrollSubsystem;
		private readonly DraggingSubsystem _draggingSubsystem;
		private readonly FormZoom _formZoom;
		private readonly Cursor _zoomCursor;
		
		public DeckEditingSubsystem(
			LayoutView layoutViewCards,
			LayoutView layoutViewDeck,
			DeckModel deckModel,
			CollectionModel collectionModel,
			ScrollSubsystem scrollSubsystem,
			DraggingSubsystem draggingSubsystem,
			Cursor cursor,
			FormZoom formZoom)
		{
			_layoutViewCards = layoutViewCards;
			_layoutViewDeck = layoutViewDeck;
			_cursor = cursor;
			_deckModel = deckModel;
			_collectionModel = collectionModel;
			_scrollSubsystem = scrollSubsystem;
			_draggingSubsystem = draggingSubsystem;
			_draggingSubsystem.DraggedLikeClick += draggedLikeClick;
			_draggingSubsystem.DragRemoved += dragRemoved;
			_draggingSubsystem.DragAdded += dragAdded;

			_formZoom = formZoom;

			var hotSpot = Size.Empty.ByDpi();
			_zoomCursor = CursorHelper.CreateCursor(Resources.zoom_48.HalfResizeDpi(), hotSpot);
		}

		private void dragRemoved(Card card)
		{
			if (Control.ModifierKeys == Keys.Control)
				changeCountInDeck(card, -4, touch: true);
			else
				changeCountInDeck(card, -1, touch: true);
		}

		private void dragAdded(Card card)
		{
			if (Control.ModifierKeys == Keys.Control)
				changeCountInDeck(card, 4, touch: true);
			else
				changeCountInDeck(card, 1, touch: true);
		}

		public void SubscribeToEvents()
		{
			_layoutViewCards.MouseLeave += gridMouseLeave;
			_layoutViewDeck.MouseLeave += gridMouseLeave;

			_layoutViewCards.MouseMove += gridMouseMove;
			_layoutViewDeck.MouseMove += gridMouseMove;

			_layoutViewCards.Control.MouseClick += gridMouseClick;
			_layoutViewDeck.Control.MouseClick += gridMouseClick;
		}

		private void gridMouseLeave(object sender, EventArgs e)
		{
			if (_deckModel.IsDragging())
				return;

			showZoomIcon(false, getView(sender).Control);
		}

		private void gridMouseMove(object sender, MouseEventArgs e)
		{
			var view = getView(sender);

			var hitInfo = view.CalcHitInfo(e.Location);
			var card = (Card)view.GetRow(hitInfo.RowHandle);
			var overImage = hitInfo.IsOverImage();

			
			if (_deckModel.IsDragging())
				return;

			if (card == null)
				showZoomIcon(false, view.Control);
			else
				showZoomIcon(overImage, view.Control);
		}

		private void showZoomIcon(bool overImage, Control control)
		{
			control.Cursor = overImage 
				? _zoomCursor 
				: _cursor;
		}

		private void gridMouseClick(object sender, MouseEventArgs e)
		{
			if (_draggingSubsystem.IsDragging())
				return;

			var view = getView(sender);
			var card = getCard(view, e);

			if (card == null)
				return;

			if (e.Button == MouseButtons.Left)
			{
				zoomCard(card);
				return;
			}

			int countDelta;

			if (e.Button == MouseButtons.Middle)
				countDelta = -1;
			else if (e.Button == MouseButtons.Right)
				countDelta = +1;
			else return;

			if ((Control.ModifierKeys & Keys.Control) > 0)
				countDelta *= 4;
			
			if ((Control.ModifierKeys & Keys.Alt) > 0)
				changeCountInCollection(card, countDelta);
			else
				changeCountInDeck(card, countDelta, touch: true);
		}

		private static Card getCard(LayoutView view, MouseEventArgs e)
		{
			var hitInfo = view.CalcHitInfo(e.Location);
			bool overImage = hitInfo.IsOverImage();

			if (!overImage)
				return null;

			var card = (Card)view.GetRow(hitInfo.RowHandle);
			return card;
		}

		private void zoomCard(Card card)
		{
			if (card.ImageModel == null)
				return;
			
			_formZoom.LoadImages(card);
			_formZoom.ShowImages();
		}

		private void changeCountInDeck(Card card, int increment, bool touch)
		{
			_deckModel.Add(card, increment, touch);
			
			var touchedCard = _deckModel.TouchedCard;
			if (touchedCard != null)
			{
				_scrollSubsystem.EnsureCardVisibility(touchedCard, _layoutViewCards);
				_scrollSubsystem.EnsureCardVisibility(touchedCard, _layoutViewDeck);
			}
		}

		private void changeCountInCollection(Card card, int increment)
		{
			_collectionModel.Add(card, increment);
		}

		private void draggedLikeClick(Card card)
		{
			// клик с нечаянным микродвижением мышкой был воспринят системой как запуск drag-n-drop
			// для дружелюбия отреагируем, как на нормальный клик - покажем увеличение карты

			if (card != null)
				zoomCard(card);
		}

		private LayoutView getView(object view)
		{
			if (_layoutViewCards.Wraps(view))
				return _layoutViewCards;

			if (_layoutViewDeck.Wraps(view))
				return _layoutViewDeck;

			throw new Exception(@"wrapper not found");
		}


		public void NewSampleHand(CardRepository cardRepository)
		{
			if (!cardRepository.IsLoadingComplete || _deckModel.Zone != Zone.SampleHand)
				return;

			createSampleHand(7, cardRepository);
		}

		public void Draw(CardRepository cardRepository)
		{
			if (!cardRepository.IsLoadingComplete || _deckModel.Zone != Zone.SampleHand)
				return;

			draw(cardRepository, touch: true);
		}

		public void Mulligan(CardRepository cardRepository)
		{
			if (!cardRepository.IsLoadingComplete || _deckModel.Zone != Zone.SampleHand)
				return;

			int count = getMulliganCount();
			createSampleHand(count, cardRepository);
		}

		private int getMulliganCount()
		{
			return Math.Max(0, _deckModel.SampleHand.CountById.Sum(_ => _.Value) - 1);
		}

		private void createSampleHand(int handSize, CardRepository cardRepository)
		{
			_deckModel.SampleHand.Clear();
			_deckModel.DataSource.Clear();

			Shuffle();

			for (int i = 0; i < handSize; i++)
				draw(cardRepository, touch: false);
		}

		public void Shuffle()
		{
			var library = new List<string>();

			foreach (var pair in _deckModel.MainDeck.CountById)
				for (int i = 0; i < pair.Value; i++)
					library.Add(pair.Key);

			_library = library;
		}

		private void draw(CardRepository cardRepository, bool touch)
		{
			if (_library == null || _library.Count == 0)
				return;

			var index = _random.Next(_library.Count);
			var id = _library[index];
			_library.RemoveAt(index);

			changeCountInDeck(cardRepository.CardsById[id], 1, touch);
		}


		private static readonly Random _random = new Random();
		private List<string> _library;
	}
}