using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Data;
using Mtgdb.Gui.Properties;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class DraggingSubsystem: IMessageFilter
	{
		public DraggingSubsystem(
			LayoutViewControl viewDeck,
			LayoutViewControl viewCards,
			DeckEditorModel deckEditorModel,
			FormMain parent,
			ImageLoader imageLoader,
			App app)
		{
			_viewDeck = viewDeck;
			_viewCards = viewCards;
			_deckEditorModel = deckEditorModel;
			_parent = parent;
			_imageLoader = imageLoader;
			_app = app;
		}



		public void SubscribeToEvents()
		{
			_viewDeck.MouseDown += mouseDown;
			_viewCards.MouseDown += mouseDown;

			_viewDeck.MouseMove += mouseMove;
			_viewCards.MouseMove += mouseMove;

			_viewDeck.MouseUp += mouseUp;
			_viewCards.MouseUp += mouseUp;

			_viewDeck.CardIndexChanged += deckScrolled;

			_viewDeck.MouseEnter += mouseEnter;
			_viewCards.MouseEnter += mouseEnter;

			_viewDeck.MouseLeave += mouseLeave;
			_viewCards.MouseLeave += mouseLeave;

			_deckEditorModel.ZoneChanged += zoneChanged;

			Application.AddMessageFilter(this);
		}

		public void UnsubscribeFromEvents()
		{
			_viewDeck.MouseDown -= mouseDown;
			_viewCards.MouseDown -= mouseDown;

			_viewDeck.MouseMove -= mouseMove;
			_viewCards.MouseMove -= mouseMove;

			_viewDeck.MouseUp -= mouseUp;
			_viewCards.MouseUp -= mouseUp;

			_viewDeck.CardIndexChanged -= deckScrolled;

			_viewDeck.MouseEnter -= mouseEnter;
			_viewCards.MouseEnter -= mouseEnter;

			_viewDeck.MouseLeave -= mouseLeave;
			_viewCards.MouseLeave -= mouseLeave;

			_deckEditorModel.ZoneChanged -= zoneChanged;

			Application.RemoveMessageFilter(this);
		}

		public void SetupDrawingDraggingMarkEvent()
		{
			_viewDeck.CustomDrawField += drawDraggingMark;
			_viewCards.CustomDrawField += drawDraggingMark;
		}

		private void mouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			var view = (LayoutViewControl)sender;
			var hitInfo = view.CalcHitInfo(e.Location);
			if (!hitInfo.IsOverImage())
				return;

			var card = (Card)view.FindRow(hitInfo.RowHandle);

			if (card == null)
				return;

			_mouseDownLocation = e.Location;
			_cardMouseDown = card;
			_dragFrom = view;
		}

		private void mouseMove(object sender, MouseEventArgs e)
		{
			var view = (LayoutViewControl)sender;

			if (_deckEditorModel.IsDragging)
			{
				if (view == _viewDeck)
					updateCardBelowDragged();

				return;
			}

			// double click leads to an empty mousemove
			if (!_mouseDownLocation.HasValue || _mouseDownLocation == e.Location)
				return;

			dragBegin(_cardMouseDown, _dragFrom);
		}

		private void mouseUp(object sender, MouseEventArgs e)
		{
			_mouseDownLocation = null;
			var cursorPosition = Cursor.Position;

			if (e.Button != MouseButtons.Left)
				return;

			if (!_deckEditorModel.IsDragging)
				return;

			var draggedCard = _deckEditorModel.DraggedCard;
			var cardBelowDragged = _deckEditorModel.CardBelowDragged;

			var cardHitInfo = getHitInfo(_viewCards, cursorPosition);
			var deckHitInfo = getHitInfo(_viewDeck, cursorPosition);

			if (cardHitInfo.InBounds)
			{
				if (_deckEditorModel.IsDraggingFromZone.HasValue)
					DragRemoved?.Invoke(draggedCard, _deckEditorModel.IsDraggingFromZone.Value);
				else
					handleDraggedLikeClick(cardHitInfo);
			}
			else if (deckHitInfo.InBounds)
			{
				if (_deckEditorModel.IsDraggingFromZone != _deckEditorModel.CurrentZone)
					DragAdded?.Invoke(draggedCard, cardBelowDragged, _deckEditorModel.IsDraggingFromZone);
				else if (cardBelowDragged != draggedCard)
					_deckEditorModel.ApplyReorder(draggedCard, cardBelowDragged);
				else
					handleDraggedLikeClick(deckHitInfo);
			}

			DragAbort();
		}

		private void handleDraggedLikeClick(HitInfo hitInfo)
		{
			TimeSpan dragDuration = DateTime.Now - (_dragStartedTime ?? DateTime.MinValue);
			const float dragMinDurationSec = 0.2f;

			if (dragDuration.TotalSeconds < dragMinDurationSec && hitInfo.CardBounds.HasValue)
				DraggedLikeClick?.Invoke((Card)hitInfo.RowDataSource);
			else
				DraggedLikeClick?.Invoke(null);
		}

		private void deckScrolled(object sender)
		{
			if (!_deckEditorModel.IsDragging || sender != _viewDeck)
				return;

			updateCardBelowDragged();
		}

		private void zoneChanged()
		{
			if (_deckEditorModel.IsDragging && _deckEditorModel.CurrentZone.HasValue)
				beginDisplayReorderedDeck();
		}


		private void mouseEnter(object sender, EventArgs e)
		{
			if (!_deckEditorModel.IsDragging)
				return;

			updateCursor();

			var view = (LayoutViewControl)sender;
			if (view == _viewDeck)
			{
				view.ScrollTo(_deckEditorModel.DraggedCard);
				updateCardBelowDragged();
			}
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			if (sender == _viewDeck && _deckEditorModel.IsDragging)
				setCardBelowDragged(_deckEditorModel.DraggedCard);
		}

		public bool IsDragging =>
			_deckEditorModel.IsDragging;

		private void dragBegin(Card card, LayoutViewControl dragFrom)
		{
			dragFrom.Capture = false;
			_cardMouseDown = card;
			_dragFrom = dragFrom;
			FromDeck = dragFrom == _viewDeck;

			_dragStartedTime = DateTime.Now;
			_deckEditorModel.DragStart(card, fromDeck: FromDeck);

			createDragCursor(card);
			updateCursor();

			beginDisplayReorderedDeck();

			// Because the card we are dragging received a mark
			_viewCards.InvalidateCard(card);
		}

		private void createDragCursor(Card card)
		{
			var cardIconSize = new Size(69, 96).ByDpi();
			int overlapY = 28.ByDpiHeight();
			var handImage = Resources.play_card_48.ResizeDpi();

			var cursorImage = new Bitmap(cardIconSize.Width, cardIconSize.Height + handImage.Height - overlapY);
			using (var g = Graphics.FromImage(cursorImage))
			{
				var bitmap = card?.Image(Ui);

				if (bitmap == null)
				{
					using Pen pen = new Pen(SystemColors.WindowText, width: 2);
					g.DrawRectangle(pen, new Rectangle(Point.Empty, cardIconSize));
				}
				else
					g.DrawImage(bitmap, new Rectangle(Point.Empty, cardIconSize));

				g.DrawImage(handImage,
					new Rectangle(
						new Point((cardIconSize.Width - handImage.Width) / 2, cardIconSize.Height - overlapY - 1),
						handImage.Size));
			}

			cursorImage = cursorImage.SetOpacity(0.65f);
			var hotSpot = new Size(cursorImage.Width / 2, cardIconSize.Height);
			_dragCursor = CursorHelper.CreateCursor(cursorImage, hotSpot);
		}


		public void DragAbort()
		{
			var draggedCard = _deckEditorModel.DraggedCard;

			_mouseDownLocation = null;
			_dragStartedTime = null;
			_deckEditorModel.DragAbort();
			updateCursor();

			// because there is no more mark on the card we are dragging
			_viewCards.InvalidateCard(draggedCard);

			endDisplayReorderedDeck();
		}

		private void beginDisplayReorderedDeck()
		{
			_viewDeck.DataSource = _deckEditorModel.GetVisibleCards();
			_viewDeck.RefreshData();
			_parent.UpdateDeckScrollLabel();
		}

		private void endDisplayReorderedDeck()
		{
			_viewDeck.DataSource = _deckEditorModel.DataSource;
			_viewDeck.RefreshData();
			_parent.UpdateDeckScrollLabel();
		}

		private void updateCardBelowDragged()
		{
			var hitInfo = getHitInfo(_viewDeck, Cursor.Position);
			var rowHandle = hitInfo.RowHandle;
			var deck = _deckEditorModel.DataSource;
			Card card;
			if (0 <= rowHandle && rowHandle < deck.Count)
				card = deck[rowHandle];
			else
				card = _deckEditorModel.DraggedCard;
			setCardBelowDragged(card);
		}

		private void setCardBelowDragged(Card card)
		{
			if (_deckEditorModel.CardBelowDragged == card)
				return;

			_deckEditorModel.CardBelowDragged = card;
			_viewDeck.DataSource = _deckEditorModel.GetVisibleCards();
			_viewDeck.RefreshData();
		}

		private void updateCursor()
		{
			Cursor cursor;

			if (_deckEditorModel.IsDragging)
				cursor = _dragCursor;
			else
				cursor = Cursors.Default;

			_parent.Cursor =
			_viewDeck.Cursor =
			_viewCards.Cursor =
				cursor;
		}

		private static HitInfo getHitInfo(LayoutViewControl view, Point position)
		{
			var clientLocation = view.PointToClient(position);
			var hitInfo = view.CalcHitInfo(clientLocation);
			return hitInfo;
		}

		private void drawDraggingMark(object sender, CustomDrawArgs e)
		{
			var view = (LayoutViewControl)sender;
			var card = (Card)view.FindRow(e.RowHandle);

			if (card == null)
				return;

			if (e.FieldName != nameof(Card.Image))
				return;

			if (card == _deckEditorModel.DraggedCard)
				drawDraggingMark(e);
		}

		private void drawDraggingMark(CustomDrawArgs e)
		{
			const int opacity = 127 - 32;

			var gradientRectangle = e.Bounds;
			gradientRectangle.Inflate(new Size(-70, -30).ByDpi());

			var brush = new LinearGradientBrush(
				gradientRectangle,
				Color.FromArgb(opacity, SystemColors.GradientActiveCaption),
				Color.FromArgb(opacity, SystemColors.ActiveCaption),
				LinearGradientMode.BackwardDiagonal);
			using (brush)
				e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.Location, _imageLoader.CardSize));
		}



		public bool PreFilterMessage(ref Message m)
		{
			if (_parent.IsDisposed || _parent.Disposing)
				return false;

			// WM_MOUSEMOVE
			if (m.Msg == 0x0200)
				mouseMoved();

			return false;
		}

		private void mouseMoved()
		{
			if (IsDragging)
				return;

			bool underMouse = _parent.IsUnderMouse();

			if (!underMouse)
				return;

			var draggingForm = _app.FindCardDraggingForm();

			if (draggingForm == null || draggingForm == _parent)
				return;

			var draggedCard = draggingForm.DraggedCard;
			draggingForm.StopDragging();

			if (IsDragging)
				DragAbort();

			dragBegin(draggedCard, _viewCards);
		}



		public event Action<Card> DraggedLikeClick;
		public event Action<Card, Zone> DragRemoved;
		public event Action<Card, Card, Zone?> DragAdded;


		public UiModel Ui { get; set; }

		public bool FromDeck { get; private set; }

		private DateTime? _dragStartedTime;
		private Point? _mouseDownLocation;
		private Card _cardMouseDown;

		private LayoutViewControl _dragFrom;
		private readonly LayoutViewControl _viewDeck;
		private readonly LayoutViewControl _viewCards;
		private readonly DeckEditorModel _deckEditorModel;
		private readonly FormMain _parent;
		private readonly ImageLoader _imageLoader;
		private readonly App _app;
		private Cursor _dragCursor;
	}
}