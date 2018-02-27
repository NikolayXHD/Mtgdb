using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Properties;

namespace Mtgdb.Gui
{
	public class DraggingSubsystem: IMessageFilter
	{
		public DraggingSubsystem(
			LayoutView layoutViewDeck,
			LayoutView layoutViewCards,
			DeckModel deckModel,
			FormMain parent,
			ImageLoader imageLoader,
			FormManager formManager)
		{
			_layoutViewDeck = layoutViewDeck;
			_layoutViewCards = layoutViewCards;
			_deckModel = deckModel;
			_parent = parent;
			_imageLoader = imageLoader;
			_formManager = formManager;
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

			Application.AddMessageFilter(this);
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

			Application.RemoveMessageFilter(this);
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

			var card = (Card)view.GetRow(hitInfo.RowHandle);

			if (card == null)
				return;

			_mouseDownLocation = e.Location;
			_cardMouseDown = card;
			_dragFromView = view;
		}

		private void mouseMove(object sender, MouseEventArgs e)
		{
			var view = getView(sender);

			if (_deckModel.IsDragging())
			{
				if (view == _layoutViewDeck)
					updateCardBelowDragged(view);

				return;
			}

			// двойной клик приводит к пустому mousemove
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

			if (!_deckModel.IsDragging())
				return;

			var draggedCard = _deckModel.DraggedCard;
			var cardBelowDragged = _deckModel.CardBelowDragged;

			var cardHitInfo = getHitInfo(_layoutViewCards, cursorPosition);
			var deckHitInfo = getHitInfo(_layoutViewDeck, cursorPosition);
			
			if (cardHitInfo.InBounds)
			{
				if (_deckModel.IsDraggingFromZone.HasValue)
					DragRemoved?.Invoke(draggedCard, _deckModel.IsDraggingFromZone.Value);
				else
					handleDraggedLikeClick(cardHitInfo);
			}
			else if (deckHitInfo.InBounds)
			{
				if (_deckModel.IsDraggingFromZone != _deckModel.Zone)
					DragAdded?.Invoke(draggedCard, _deckModel.IsDraggingFromZone);
				else if (cardBelowDragged != draggedCard)
					_deckModel.ApplyReorder(draggedCard, cardBelowDragged);
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
			if (!_deckModel.IsDragging())
				return;

			updateCardBelowDragged(_layoutViewDeck);
		}

		private void mouseEnter(object sender, EventArgs e)
		{
			if (!_deckModel.IsDragging())
				return;

			updateCursor();

			var view = getView(sender);
			if (view == _layoutViewDeck)
				updateCardBelowDragged(view);
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			if (_deckModel.IsDragging())
				setCardBelowDragged(_deckModel.DraggedCard);
		}



		public bool IsDragging()
		{
			return _deckModel.IsDragging();
		}

		public void DragBegin(Card card, LayoutView dragFromView)
		{
			dragFromView.Control.Capture = false;
			_cardMouseDown = card;
			_dragFromView = dragFromView;
			
			_dragStartedTime = DateTime.Now;

			_deckModel.DragStart(card, fromDeck: dragFromView == _layoutViewDeck);

			createDragCursor(card);
			updateCursor();

			// Потому что появилась отметка на карте, которую мы тащим
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
					g.DrawRectangle(new Pen(Color.Black, width: 2), new Rectangle(Point.Empty, cardIconSize));
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
			var draggedCard = _deckModel.DraggedCard;
			
			_mouseDownLocation = null;
			_dragStartedTime = null;
			_deckModel.DragAbort();
			updateCursor();

			// потому что больше нет отметки на карте, которую мы тащим
			_layoutViewCards.InvalidateCard(draggedCard);
			_layoutViewDeck.Invalidate();
		}

		private void setCardBelowDragged(Card card)
		{
			if (_deckModel.CardBelowDragged != card)
				_deckModel.CardBelowDragged = card;

			_layoutViewDeck.Invalidate();
		}

		private void updateCardBelowDragged(LayoutView view)
		{
			var card = getCardBelowDragged(view) ?? _deckModel.CardBelowDragged;
			setCardBelowDragged(card);
		}

		private void updateCursor()
		{
			Cursor cursor;

			if (_deckModel.IsDragging())
				cursor = _dragCursor;
			else
				cursor = Cursors.Default;

			_parent.Cursor =
			_layoutViewDeck.Control.Cursor =
			_layoutViewCards.Control.Cursor =
				cursor;
		}

		private Card getCardBelowDragged(LayoutView view)
		{
			var hitInfo = getHitInfo(view, Cursor.Position);

			if (hitInfo.IsOverImage())
				return (Card) _layoutViewDeck.GetRow(hitInfo.RowHandle);

			return null;
		}

		private static HitInfo getHitInfo(LayoutView view, Point position)
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
			
			if (card == _deckModel.DraggedCard)
				drawDraggingMark(e);
		}

		private void drawDraggingMark(CustomDrawArgs e)
		{
			const int opacity = 66;

			var gradientRectangle = e.Bounds;
			gradientRectangle.Inflate(new Size(-70, -30).ByDpi());

			var brush = new LinearGradientBrush(
				gradientRectangle,
				Color.FromArgb(opacity, Color.LightBlue),
				Color.FromArgb(opacity, Color.AliceBlue),
				LinearGradientMode.BackwardDiagonal);

			e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.Location, _imageLoader.CardSize));
		}



		private LayoutView getView(object view)
		{
			if (_layoutViewCards.Wraps(view))
				return _layoutViewCards;

			if (_layoutViewDeck.Wraps(view))
				return _layoutViewDeck;

			throw new Exception(@"wrapper not found");
		}

		public Card GetCard(LayoutView view, int rowHandle)
		{
			if (view == _layoutViewCards)
				return (Card)view.GetRow(rowHandle);

			if (view != _layoutViewDeck)
				throw new ArgumentOutOfRangeException();

			int visibleIndex = view.GetVisibleIndex(rowHandle);

			if (visibleIndex < 0)
				return null;

			return _deckModel.GetVisibleCards()[visibleIndex];
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

			var draggingForm = _formManager.FindCardDraggingForm();

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

		private LayoutView _dragFromView;
		private readonly LayoutView _layoutViewDeck;
		private readonly LayoutView _layoutViewCards;
		private readonly DeckModel _deckModel;
		private readonly FormMain _parent;
		private readonly ImageLoader _imageLoader;
		private readonly FormManager _formManager;
		private Cursor _dragCursor;
	}
}