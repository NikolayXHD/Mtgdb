using System;
using System.ComponentModel;
using System.Drawing;
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
		private readonly DraggingSubsystem _draggingSubsystem;
		private readonly FormZoom _formZoom;
		private readonly Cursor _zoomCursor;
		private readonly Cursor _textSelectionCursor;

		public DeckEditingSubsystem(
			LayoutView layoutViewCards,
			LayoutView layoutViewDeck,
			DeckModel deckModel,
			CollectionModel collectionModel,
			DraggingSubsystem draggingSubsystem,
			Cursor cursor,
			FormZoom formZoom)
		{
			_layoutViewCards = layoutViewCards;
			_layoutViewDeck = layoutViewDeck;
			_cursor = cursor;
			_deckModel = deckModel;
			_collectionModel = collectionModel;
			_draggingSubsystem = draggingSubsystem;
			_draggingSubsystem.DraggedLikeClick += draggedLikeClick;
			_draggingSubsystem.DragRemoved += dragRemoved;
			_draggingSubsystem.DragAdded += dragAdded;
			_layoutViewCards.SelectionStarted += selectionStarted;

			_formZoom = formZoom;

			var hotSpot = Size.Empty.ByDpi();
			_zoomCursor = CursorHelper.CreateCursor(Resources.zoom_48.HalfResizeDpi(), hotSpot);

			var iBeamIcon = Resources.text_selection_24.ResizeDpi();
			var iBeamHotSpot = new Size(iBeamIcon.Width / 2, iBeamIcon.Height / 2);
			_textSelectionCursor = CursorHelper.CreateCursor(iBeamIcon, iBeamHotSpot);
		}

		private void dragRemoved(Card card, Zone fromDeckZone)
		{
			int count = Control.ModifierKeys == Keys.Control ? 4 : 1;
			_deckModel.Add(card, -count, fromDeckZone);
		}

		private void dragAdded(Card card, Zone? fromDeckZone)
		{
			int count = Control.ModifierKeys == Keys.Control ? 4 : 1;

			if (fromDeckZone.HasValue && _deckModel.Zone != Zone.SampleHand)
				_deckModel.Add(card, -count, zone: fromDeckZone, changeTerminatesBatch: false);

			_deckModel.Add(card, +count);
		}

		public void SubscribeToEvents()
		{
			_layoutViewCards.MouseLeave += gridMouseLeave;
			_layoutViewDeck.MouseLeave += gridMouseLeave;

			_layoutViewCards.MouseMove += gridMouseMove;
			_layoutViewDeck.MouseMove += gridMouseMove;

			_layoutViewCards.MouseClicked += gridMouseClick;
			_layoutViewDeck.MouseClicked += gridMouseClick;
		}

		private void gridMouseLeave(object sender, EventArgs e)
		{
			if (_deckModel.IsDragging())
				return;

			updateCursor(getView(sender).Control, outside: true);
		}

		private void gridMouseMove(object sender, MouseEventArgs e)
		{
			var view = getView(sender);

			if (_deckModel.IsDragging())
				return;

			var hitInfo = view.CalcHitInfo(e.Location);
			var card = (Card) view.GetRow(hitInfo.RowHandle);

			if (card != null)
			{
				bool isOverImage = hitInfo.IsOverImage();

				updateCursor(view.Control,
					overImage: isOverImage,
					overText: hitInfo.FieldBounds != null && !isOverImage,
					overButton: hitInfo.IsSearchButton || hitInfo.IsSortButton);
			}
			else
			{
				updateCursor(view.Control);
			}
		}

		private void updateCursor(Control control, bool overImage = false, bool overText = false, bool overButton = false, bool outside = false)
		{
			if (outside)
				control.Cursor = _cursor;
			else if (overButton)
				control.Cursor = Cursors.Default;
			else if (overImage)
				control.Cursor = _zoomCursor;
			else if (overText)
				control.Cursor = _textSelectionCursor;
			else
				control.Cursor = Cursors.Default;
		}

		private void gridMouseClick(object sender, HitInfo hitInfo, MouseEventArgs e)
		{
			if (_draggingSubsystem.IsDragging())
				return;

			var view = getView(sender);

			if (hitInfo.AlignButtonDirection.HasValue)
				return;

			var card = getCard(view, hitInfo);

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
			else
				return;

			if ((Control.ModifierKeys & Keys.Control) > 0)
				countDelta *= 4;

			if ((Control.ModifierKeys & Keys.Alt) > 0)
				changeCountInCollection(card, countDelta);
			else
				_deckModel.Add(card, countDelta);
		}

		private static Card getCard(LayoutView view, HitInfo hitInfo)
		{
			bool overImage = hitInfo.IsOverImage();

			if (!overImage)
				return null;

			var card = (Card) view.GetRow(hitInfo.RowHandle);
			return card;
		}

		private void zoomCard(Card card)
		{
			if (!card.HasImage(Ui))
				return;

			_formZoom.LoadImages(card, Ui);
			_formZoom.ShowImages();
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

		private static void selectionStarted(object sender, HitInfo hitInfo, CancelEventArgs cancelArgs)
		{
			if (hitInfo.IsOverImage())
				cancelArgs.Cancel = true;
		}

		private LayoutView getView(object view)
		{
			if (_layoutViewCards.Wraps(view))
				return _layoutViewCards;

			if (_layoutViewDeck.Wraps(view))
				return _layoutViewDeck;

			throw new Exception(@"wrapper not found");
		}



		public UiModel Ui { get; set; }
	}
}