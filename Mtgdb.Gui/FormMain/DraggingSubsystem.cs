using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Properties;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class DraggingSubsystem: IMessageFilter
	{
		public DraggingSubsystem(
			MtgLayoutView layoutViewDeck,
			MtgLayoutView layoutViewCards,
			DeckEditorModel deckEditorModel,
			FormMain parent,
			ImageLoader imageLoader,
			Application application)
		{
			_layoutViewDeck = layoutViewDeck;
			_layoutViewCards = layoutViewCards;
			_deckEditorModel = deckEditorModel;
			_parent = parent;
			_imageLoader = imageLoader;
			_application = application;
		}



		public void SubscribeToEvents()
		{
			_layoutViewDeck.MouseDown += mouseDown;
			_layoutViewCards.MouseDown += mouseDown;

			_layoutViewDeck.MouseMove += mouseMove;
			_layoutViewCards.MouseMove += mouseMove;

			_layoutViewDeck.MouseUp += mouseUp;
			_layoutViewCards.MouseUp += mouseUp;

			_layoutViewDeck.VisibleRecordIndexChanged += deckScrolled;

			_layoutViewDeck.MouseEnter += mouseEnter;
			_layoutViewCards.MouseEnter += mouseEnter;

			_layoutViewDeck.MouseLeave += mouseLeave;
			_layoutViewCards.MouseLeave += mouseLeave;

			System.Windows.Forms.Application.AddMessageFilter(this);
		}

		public void UnsubscribeFromEvents()
		{
			_layoutViewDeck.MouseDown -= mouseDown;
			_layoutViewCards.MouseDown -= mouseDown;

			_layoutViewDeck.MouseMove -= mouseMove;
			_layoutViewCards.MouseMove -= mouseMove;

			_layoutViewDeck.MouseUp -= mouseUp;
			_layoutViewCards.MouseUp -= mouseUp;

			_layoutViewDeck.VisibleRecordIndexChanged -= deckScrolled;

			_layoutViewDeck.MouseEnter -= mouseEnter;
			_layoutViewCards.MouseEnter -= mouseEnter;

			_layoutViewDeck.MouseLeave -= mouseLeave;
			_layoutViewCards.MouseLeave -= mouseLeave;

			System.Windows.Forms.Application.RemoveMessageFilter(this);
		}

		public void SetupDrawingDraggingMarkEvent()
		{
			_layoutViewDeck.CustomDrawField += drawDraggingMark;
			_layoutViewCards.CustomDrawField += drawDraggingMark;
		}

		private void mouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			var view = getView(sender);
			var hitInfo = view.CalcHitInfo(e.Location);
			if (!hitInfo.IsOverImage())
				return;

			var card = (Card)view.FindRow(hitInfo.RowHandle);

			if (card == null)
				return;

			_mouseDownLocation = e.Location;
			_cardMouseDown = card;
			_dragFromView = view;
		}

		private void mouseMove(object sender, MouseEventArgs e)
		{
			var view = getView(sender);

			if (_deckEditorModel.IsDragging())
			{
				if (view == _layoutViewDeck)
					updateCardBelowDragged(view);

				return;
			}

			// double click leads to an empty mousemove
			if (!_mouseDownLocation.HasValue || _mouseDownLocation == e.Location)
				return;

			DragBegin(_cardMouseDown, _dragFromView);
		}

		private void mouseUp(object sender, MouseEventArgs e)
		{
			_mouseDownLocation = null;
			var cursorPosition = Cursor.Position;

			if (e.Button != MouseButtons.Left)
				return;

			if (!_deckEditorModel.IsDragging())
				return;

			var draggedCard = _deckEditorModel.DraggedCard;
			var cardBelowDragged = _deckEditorModel.CardBelowDragged;

			var cardHitInfo = getHitInfo(_layoutViewCards, cursorPosition);
			var deckHitInfo = getHitInfo(_layoutViewDeck, cursorPosition);

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
					DragAdded?.Invoke(draggedCard, _deckEditorModel.IsDraggingFromZone);
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
			if (!_deckEditorModel.IsDragging())
				return;

			updateCardBelowDragged(_layoutViewDeck);
		}

		private void mouseEnter(object sender, EventArgs e)
		{
			if (!_deckEditorModel.IsDragging())
				return;

			updateCursor();

			var view = getView(sender);
			if (view == _layoutViewDeck)
				updateCardBelowDragged(view);
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			if (_deckEditorModel.IsDragging())
				setCardBelowDragged(_deckEditorModel.DraggedCard);
		}



		public bool IsDragging()
		{
			return _deckEditorModel.IsDragging();
		}

		public void DragBegin(Card card, MtgLayoutView dragFromView)
		{
			dragFromView.Control.Capture = false;
			_cardMouseDown = card;
			_dragFromView = dragFromView;

			_dragStartedTime = DateTime.Now;

			_deckEditorModel.DragStart(card, fromDeck: dragFromView == _layoutViewDeck);

			createDragCursor(card);
			updateCursor();

			// Because the card we are dragging received a mark
			_layoutViewCards.InvalidateCard(card);
			_layoutViewDeck.InvalidateCard(card);
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
					g.DrawRectangle(new Pen(SystemColors.WindowText, width: 2), new Rectangle(Point.Empty, cardIconSize));
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
			_layoutViewCards.InvalidateCard(draggedCard);
			_layoutViewDeck.Invalidate();
		}

		private void setCardBelowDragged(Card card)
		{
			if (_deckEditorModel.CardBelowDragged != card)
				_deckEditorModel.CardBelowDragged = card;

			_layoutViewDeck.Invalidate();
		}

		private void updateCardBelowDragged(MtgLayoutView view)
		{
			var card = getCardBelowDragged(view) ?? _deckEditorModel.CardBelowDragged;
			setCardBelowDragged(card);
		}

		private void updateCursor()
		{
			Cursor cursor;

			if (_deckEditorModel.IsDragging())
				cursor = _dragCursor;
			else
				cursor = Cursors.Default;

			_parent.Cursor =
			_layoutViewDeck.Control.Cursor =
			_layoutViewCards.Control.Cursor =
				cursor;
		}

		private Card getCardBelowDragged(MtgLayoutView view)
		{
			var hitInfo = getHitInfo(view, Cursor.Position);

			if (hitInfo.IsOverImage())
				return (Card) _layoutViewDeck.FindRow(hitInfo.RowHandle);

			return null;
		}

		private static HitInfo getHitInfo(MtgLayoutView view, Point position)
		{
			var clientLocation = view.Control.PointToClient(position);
			var hitInfo = view.CalcHitInfo(clientLocation);
			return hitInfo;
		}

		private void drawDraggingMark(object sender, CustomDrawArgs e)
		{
			var view = getView(sender);
			var card = GetCard(view, e.RowHandle);

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

			e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.Location, _imageLoader.CardSize));
		}



		private MtgLayoutView getView(object view)
		{
			if (_layoutViewCards.Wraps(view))
				return _layoutViewCards;

			if (_layoutViewDeck.Wraps(view))
				return _layoutViewDeck;

			throw new Exception(@"wrapper not found");
		}

		public Card GetCard(MtgLayoutView view, int rowHandle)
		{
			if (view == _layoutViewCards)
				return (Card)view.FindRow(rowHandle);

			if (view != _layoutViewDeck)
				throw new ArgumentOutOfRangeException();

			int visibleIndex = view.GetVisibleIndex(rowHandle);

			if (visibleIndex < 0)
				return null;

			return _deckEditorModel.GetVisibleCards()[visibleIndex];
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
			if (IsDragging())
				return;

			bool underMouse = _parent.IsChildUnderMouse();

			if (!underMouse)
				return;

			var draggingForm = _application.FindCardDraggingForm();

			if (draggingForm == null || draggingForm == _parent)
				return;

			var draggedCard = draggingForm.DraggedCard;
			draggingForm.StopDragging();

			if (IsDragging())
				DragAbort();

			DragBegin(draggedCard, _layoutViewCards);
		}



		public event Action<Card> DraggedLikeClick;
		public event Action<Card, Zone> DragRemoved;
		public event Action<Card, Zone?> DragAdded;


		public UiModel Ui { get; set; }


		private DateTime? _dragStartedTime;
		private Point? _mouseDownLocation;
		private Card _cardMouseDown;

		private MtgLayoutView _dragFromView;
		private readonly MtgLayoutView _layoutViewDeck;
		private readonly MtgLayoutView _layoutViewCards;
		private readonly DeckEditorModel _deckEditorModel;
		private readonly FormMain _parent;
		private readonly ImageLoader _imageLoader;
		private readonly Application _application;
		private Cursor _dragCursor;
	}
}