using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public partial class LayoutViewControl : UserControl, IMessageFilter
	{
		public LayoutViewControl()
		{
			InitializeComponent();

			DataSource = new List<object>();

			Cards = new List<LayoutControl>();
			CardIndex = 0;

			LayoutControlType = typeof(LayoutControl);

			Load += load;
			Paint += paint;
			Layout += layout;
			Disposed += disposed;

			_alignButtonVisible.Changed += alignButtonStateChanged;
			_alignButtonHotTracked.Changed += alignButtonStateChanged;

			SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

			scaleScrollbar();

			_selectionCaretTimer = new Timer
			{
				Interval = SystemInformation.CaretBlinkTime
			};

			_selectionCaretTimer.Tick += tick;
		}

		private void tick(object sender, EventArgs e)
		{
			if (Focused)
			{
				if (_selectionCaretTimerSkip)
					_selectionCaretTimerSkip = false;
				else
					getNonEmptySelection()?.Tick();
			}
			else
				getNonEmptySelection()?.Hide();
		}

		private void scaleScrollbar()
		{
			_scrollBar.ChannelColor = SystemColors.Control;
			_scrollBar.BorderColor = SystemColors.Control;

			int scrollbarWidth = _scrollBar.Width.ByDpiWidth();

			_scrollBar.Left = _scrollBar.Right - scrollbarWidth;
			_scrollBar.Width = scrollbarWidth;

			_scrollBar.UpArrowImage = ((Bitmap) _scrollBar.UpArrowImage)?.ResizeDpi();
			_scrollBar.ThumbTopImage = ((Bitmap) _scrollBar.ThumbTopImage)?.ResizeDpi();
			_scrollBar.ThumbTopSpanImage = ((Bitmap) _scrollBar.ThumbTopSpanImage)?.ResizeDpi();

			_scrollBar.ThumbMiddleImage = ((Bitmap) _scrollBar.ThumbMiddleImage)?.ResizeDpi();

			_scrollBar.ThumbBottomSpanImage = ((Bitmap) _scrollBar.ThumbBottomSpanImage)?.ResizeDpi();
			_scrollBar.ThumbBottomImage = ((Bitmap) _scrollBar.ThumbBottomImage)?.ResizeDpi();
			_scrollBar.DownArrowImage = ((Bitmap) _scrollBar.DownArrowImage)?.ResizeDpi();
		}

		private void layout(object sender, LayoutEventArgs e)
		{
			applySize();
			ApplyCardIndex();
			updateScrollbar();
		}



		private void paint(object sender, PaintEventArgs eArgs)
		{
			var paintActions = new PaintActions();

			// implicit connection: data_source_sync
			lock (DataSource)
			{
				paintActions.Back.Add(e => e.Graphics.Clear(BackColor));
				addPaintCardActions(paintActions, eArgs.ClipRectangle);
				paintActions.AlignButtons.Add(paintAlignButtons);
				paintActions.Selection.Add(paintSelection);

				eArgs.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				eArgs.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

				paintActions.Back.Paint(eArgs);
				paintActions.FieldData.Paint(eArgs);
				paintActions.Selection.Paint(eArgs);

				paintActions.AlignButtons.Paint(eArgs);
				// paint field buttons over align buttons
				paintActions.FieldButtons.Paint(eArgs);
			}
		}

		private void paintSelection(PaintEventArgs e)
		{
			if (!_selection.Selecting)
				return;

			if (!e.ClipRectangle.IntersectsWith(_selection.Rectangle))
				return;

			byte alpha = SelectionOptions.RectAlpha;

			if (alpha == 0)
				return;

			var rectangle = new Rectangle(_selection.Rectangle.Location, _selection.Rectangle.Size.Minus(new Size(1, 1)));

			if (SelectionOptions.RectFillColor != Color.Transparent)
			{
				e.Graphics.FillRectangle(
					new SolidBrush(Color.FromArgb(alpha, SelectionOptions.RectFillColor)),
					rectangle);
			}

			if (SelectionOptions.RectBorderColor != Color.Transparent)
			{
				e.Graphics.DrawRectangle(
					new Pen(Color.FromArgb(alpha, SelectionOptions.RectBorderColor)),
					rectangle);
			}
		}

		private void addPaintCardActions(PaintActions actions, Rectangle clipRectangle)
		{
			var hotTrackBgBrush = new SolidBrush(SelectionOptions.HotTrackBackColor);
			var hotTrackBgPen = new Pen(SelectionOptions.HotTrackBorderColor);

			for (int i = 0; i < Cards.Count; i++)
			{
				var card = Cards[i];
				int rowHandle = getRowHandle(displayIndex: i);

				if (!card.Visible || card.DataSource == null)
					continue;

				var cardArea = new Rectangle(card.Location, card.Size);
				if (!clipRectangle.IntersectsWith(cardArea))
					continue;

				actions.Back.Add(e => card.PaintSelf(e.Graphics, card.Location, BackColor));

				foreach (var field in card.Fields)
				{
					var fieldArea = new Rectangle(card.Location.Plus(field.Location), field.Size);

					if (!clipRectangle.IntersectsWith(fieldArea))
						continue;

					actions.Back.Add(e => paintFieldBg(e, field, fieldArea, hotTrackBgBrush, hotTrackBgPen));
					actions.FieldData.Add(e => paintFieldData(e, card, field, fieldArea, rowHandle));
					actions.FieldButtons.Add(e => paintButtons(e, field, card));
				}
			}
		}

		private void paintFieldBg(PaintEventArgs e, FieldControl field, Rectangle fieldArea, SolidBrush hotTrackBgBrush, Pen hotTrackBgPen)
		{
			var rect = new Rectangle(fieldArea.Location, new Size(fieldArea.Width - 1, fieldArea.Height - 1));

			if (field.IsHotTracked)
			{
				e.Graphics.FillRectangle(hotTrackBgBrush, rect);
				e.Graphics.DrawRectangle(hotTrackBgPen, rect);
			}
			else if (!field.BackColor.Equals(BackColor) && !field.BackColor.Equals(Color.Transparent))
			{
				var bgBrush = new SolidBrush(field.BackColor);
				e.Graphics.FillRectangle(bgBrush, rect);
			}
		}

		private void paintFieldData(PaintEventArgs e, LayoutControl card, FieldControl field, Rectangle fieldArea, int rowHandle)
		{
			if (CustomDrawField != null)
			{
				var customFieldArgs = new CustomDrawArgs
				{
					RowHandle = rowHandle,
					Graphics = e.Graphics,
					Bounds = fieldArea,
					FieldName = field.FieldName,
					Handled = false,
					Font = field.Font,
					ForeColor = field.ForeColor,
					DisplayText = field.DataText,
					HAlignment = field.HorizontalAlignment,
					Selection = _selection
				};

				CustomDrawField.Invoke(this, customFieldArgs);

				if (!customFieldArgs.Handled)
				{
					field.PaintSelf(
						e.Graphics,
						card.Location,
						card.HighlightOptions,
						_selection,
						SelectionOptions);
				}
			}
			else
			{
				field.PaintSelf(
					e.Graphics,
					card.Location,
					card.HighlightOptions,
					_selection,
					SelectionOptions);
			}
		}

		private void paintButtons(PaintEventArgs e, FieldControl field, LayoutControl card)
		{
			var fieldBounds = getFieldBounds(field, card);

			var buttons = card.GetFieldButtons(field, SearchOptions, SortOptions).ToList();
			buttons.LayOutIn(fieldBounds);

			foreach (var button in buttons)
			{
				if (button.Size == Size.Empty || button.Icon == null)
					continue;

				e.Graphics.DrawImage(button.Icon, button.Location);
			}
		}

		private static Rectangle getFieldBounds(FieldControl field, LayoutControl card)
		{
			var fieldBounds = field.Bounds;
			fieldBounds.Offset(card.Location);
			return fieldBounds;
		}

		private void paintAlignButtons(PaintEventArgs e)
		{
			if (!LayoutOptions.HasAlignIcons)
				return;

			foreach (var direction in getAlignDirections())
			{
				if (!_alignButtonVisible[direction])
					continue;

				var bounds = getAlignIconBounds(direction);
				if (!e.ClipRectangle.IntersectsWith(bounds))
					continue;

				var icon = LayoutOptions.GetAlignIcon(direction, _alignButtonHotTracked[direction]);
				e.Graphics.DrawImage(icon, bounds);
			}
		}



		public Rectangle GetAlignButtonBounds(HitInfo hitInfo)
		{
			if (!hitInfo.AlignButtonDirection.HasValue)
				return Rectangle.Empty;

			return getAlignIconBounds(hitInfo.AlignButtonDirection.Value);
		}

		public void RefreshData()
		{
			_rowHandleByObject.Clear();

			// implicit connection: data_source_sync
			lock (DataSource)
			{
				for (int i = 0; i < DataSource.Count; i++)
					_rowHandleByObject[DataSource[i]] = i;

				if (CardIndex < 0 || CardIndex >= Count)
					CardIndex = snapCardIndex(CardIndex, Count);
			}

			ApplyCardIndex();
			updateScrollbar();
		}

		private void updateScrollbar()
		{
			int pageSize = getRowsCount() * getColumnsCount();
			_scrollBar.Enabled = Count > pageSize;

			int largeChange = ((int) Math.Round(Math.Pow(10, Math.Round(Math.Log10(Count + 1d) - 2.5d))))
				.WithinRange(1, PageCount).AtLeast(1);

			int maximum = Math.Max(0, PageCount - 1);
			int value = CardIndex / pageSize;

			_updatingScrollValue = true;

			_scrollBar.SmallChange = 1;
			_scrollBar.LargeChange = largeChange;
			_scrollBar.Minimum = 0;
			_scrollBar.Maximum = maximum;
			_scrollBar.Value = value;

			_updatingScrollValue = false;
		}

		private void load(object sender, EventArgs e)
		{
			ApplyIconRecognizer();

			_scrollBar.Scroll += scrolled;
			MouseWheel += mouseWheel;
			KeyDown += keyDown;
			PreviewKeyDown += previewKeyDown;

			MouseDown += mouseDown;
			MouseMove += mouseMove;
			MouseLeave += mouseLeave;
			MouseUp += mouseUp;

			MouseClick += mouseClick;

			_selection.Changed += selectionChanged;

			Application.AddMessageFilter(this);

			_selectionCaretTimer.Start();
		}

		private void disposed(object sender, EventArgs e)
		{
			_selectionCaretTimer.Stop();
			Application.RemoveMessageFilter(this);
		}



		private LayoutControl createCard()
		{
			var result = (LayoutControl) Activator.CreateInstance(LayoutControlType);

			result.CopyFrom(ProbeCard);

			foreach (var field in result.Fields)
				updateSort(field);

			result.Invalid += cardInvalidated;

			return result;
		}

		private LayoutControl createProbeCard()
		{
			var result = (LayoutControl) Activator.CreateInstance(LayoutControlType);
			result.SetIconRecognizer(IconRecognizer);
			result.Font = Font;

			ProbeCardCreating?.Invoke(this, result);

			return result;
		}

		private void updateSort(FieldControl field)
		{
			if (field.FieldName != null && _sortIndexByField.TryGetValue(field.FieldName, out int sortIndex))
				field.SortOrder = _sortInfos[sortIndex].SortOrder;
			else
				field.SortOrder = SortOrder.None;
		}

		private void cardInvalidated(LayoutControl layoutControl, FieldControl fieldControl)
		{
			Rectangle rect;

			if (fieldControl != null)
			{
				rect = fieldControl.Bounds;
				rect.Offset(layoutControl.Location);
			}
			else
				rect = layoutControl.Bounds;

			Invalidate(rect);
		}

		private int getColumnsCount() => LayoutUtil.GetVisibleCardsCount(
			Width - ScrollWidth,
			CardSize.Width,
			LayoutOptions.CardInterval.Width,
			LayoutOptions.PartialCardsThreshold.Width,
			LayoutOptions.AllowPartialCards);

		private int getRowsCount() => LayoutUtil.GetVisibleCardsCount(
			Height,
			CardSize.Height,
			LayoutOptions.CardInterval.Height,
			LayoutOptions.PartialCardsThreshold.Height,
			LayoutOptions.AllowPartialCards);

		private void scrolled(object sender, EventArgs e)
		{
			if (_updatingScrollValue)
				return;

			int pageSize = getRowsCount() * getColumnsCount();
			int cardIndex = _scrollBar.Value * pageSize;

			if (cardIndex == CardIndex)
				return;

			CardIndex = cardIndex;
			ApplyCardIndex();
		}

		private static void previewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			e.IsInputKey = true;
		}

		private void keyDown(object sender, KeyEventArgs e)
		{
			void resetTimer() =>
				_selectionCaretTimerSkip = true;

			bool handled = true;

			if (e.KeyData == Keys.PageDown)
			{
				if (IsUnderMouse)
					scrollAdd(getRowsCount() * getColumnsCount());
				else
					handled = false;
			}
			else if (e.KeyData == Keys.PageUp)
			{
				if (IsUnderMouse)
					scrollAdd(-(getRowsCount() * getColumnsCount()));
				else
					handled = false;
			}
			else if (e.KeyData == Keys.End)
			{
				if (IsUnderMouse)
					scrollAdd(Count);
				else
					handled = false;
			}
			else if (e.KeyData == Keys.Home)
			{
				if (IsUnderMouse)
					scrollAdd(-Count);
				else
					handled = false;
			}
			else if (e.KeyData == Keys.Escape)
				getNonEmptySelection()?.Clear();
			else if (e.KeyData == Keys.Right)
			{
				getNonEmptySelection()?.MoveSelectionRight();
				resetTimer();
			}
			else if (e.KeyData == Keys.Left)
			{
				getNonEmptySelection()?.MoveSelectionLeft();
				resetTimer();
			}
			else if (e.KeyData == Keys.Up)
			{
				getNonEmptySelection()?.MoveSelectionUp();
				resetTimer();
			}
			else if (e.KeyData == Keys.Down)
			{
				getNonEmptySelection()?.MoveSelectionDown();
				resetTimer();
			}
			else if (e.KeyData == (Keys.Shift | Keys.Right) || e.KeyData == (Keys.Control | Keys.Shift | Keys.Right))
			{
				getNonEmptySelection()?.ShiftSelectionRight();
				resetTimer();
			}
			else if (e.KeyData == (Keys.Shift | Keys.Left) || e.KeyData == (Keys.Control | Keys.Shift | Keys.Left))
			{
				getNonEmptySelection()?.ShiftSelectionLeft();
				resetTimer();
			}
			else if (e.KeyData == (Keys.Shift | Keys.Up) || e.KeyData == (Keys.Control | Keys.Shift | Keys.Up))
			{
				getNonEmptySelection()?.ShiftSelectionUp();
				resetTimer();
			}
			else if (e.KeyData == (Keys.Shift | Keys.Down) || e.KeyData == (Keys.Control | Keys.Shift | Keys.Down))
			{
				getNonEmptySelection()?.ShiftSelectionDown();
				resetTimer();
			}
			else if (e.KeyData == (Keys.Shift | Keys.Home))
			{
				getNonEmptySelection()?.ShiftSelectionToStart();
				resetTimer();
			}
			else if (e.KeyData == (Keys.Shift | Keys.End))
			{
				getNonEmptySelection()?.ShiftSelectionToEnd();
				resetTimer();
			}
			else if (e.KeyData == (Keys.Control | Keys.A))
			{
				getNonEmptySelection()?.SelectAll();
				resetTimer();
			}
			else if (e.KeyData == (Keys.Control | Keys.C))
			{
				handled = getNonEmptySelection()?.SelectedText.TryCopyToClipboard() == true;
			}
			else
				handled = false;

			e.Handled = handled;
			e.SuppressKeyPress = handled;
		}

		private void mouseWheel(object sender, MouseEventArgs e)
		{
			if (!IsUnderMouse)
				return;

			if (e.Delta < 0)
				scrollAdd(getRowsCount() * getColumnsCount());
			else if (e.Delta > 0)
				scrollAdd(-(getRowsCount() * getColumnsCount()));
		}

		private void scrollAdd(int pageSize)
		{
			int cardIndex = CardIndex;
			cardIndex += pageSize;
			CardIndex = snapCardIndex(cardIndex, Count);

			ApplyCardIndex();
			updateScrollbar();
		}

		private void applySize()
		{
			var shift = getAlignmentShift();

			int columnsCount = getColumnsCount();
			int rowsCount = getRowsCount();
			for (int j = 0; j < rowsCount; j++)
				for (int i = 0; i < columnsCount; i++)
				{
					var location = getCardLocation(i, j, shift);

					int index = getCardIndex(i, j, columnsCount);
					if (index == Cards.Count)
						Cards.Add(createCard());

					Cards[index].Location = location;
					Cards[index].Visible = Cards[index].DataSource != null;
				}

			for (int index = rowsCount * columnsCount; index < Cards.Count; index++)
				Cards[index].Visible = false;
		}



		private void selectionChanged(
			Rectangle previousRect,
			Point previousStart,
			bool previousSelecting,
			IEnumerable<Rectangle> delta)
		{
			// update selection rectangle where its presence changed
			foreach (var rectangle in delta)
			{
				rectangle.Inflate(1, 1);
				Invalidate(rectangle);
			}

			// paint or erase entire selection rectangle
			if (previousSelecting != _selection.Selecting)
				Invalidate(previousRect);

			// paint previously selected field to reflect changed selected text
			if (previousStart != _selection.Start)
				invalidateFieldAt(previousStart);

			// paint currently selected field to show selected text
			if (_selection.Rectangle != previousRect)
				invalidateFieldAt(_selection.Start);
		}

		private void invalidateFieldAt(Point location)
		{
			var index = getCardIndex(location);

			if (index < 0)
				return;

			var card = Cards[index];

			if (!card.Visible)
				return;

			var field = card.Fields.FirstOrDefault(f => new Rectangle(f.Location.Plus(card.Location), f.Size).Contains(location));

			if (field == null)
				return;

			invalidateField(field, card);
		}

		private void invalidateField(FieldControl field, LayoutControl card)
		{
			Invalidate(new Rectangle(field.Location.Plus(card.Location), field.Size));
		}

		private void mouseDown(object sender, MouseEventArgs e)
		{
			if (!SelectionOptions.Enabled)
				return;

			if (e.Button != MouseButtons.Left)
				return;

			var hitInfo = CalcHitInfo(e.Location);

			if (_hitInfo.IsSomeButton)
				return;

			var cancelArgs = new CancelEventArgs();
			SelectionStarted?.Invoke(this, hitInfo, cancelArgs);

			if (cancelArgs.Cancel)
				return;

			_selection.StartAt(e.Location);
			hideButtons();
		}

		private void mouseUp(object sender, MouseEventArgs e)
		{
			if (SelectionOptions.Enabled && _selection.Selecting && e.Button == MouseButtons.Left)
			{
				_selection.MoveTo(e.Location);
				_selection.EndSelection();

				var hitInfo = CalcHitInfo(e.Location);
				handleMouseMove(hitInfo);
			}
		}

		private void mouseMove(object sender, MouseEventArgs e)
		{
			if (SelectionOptions.Enabled && _selection.Selecting)
			{
				_selection.MoveTo(e.Location);
				hideButtons();
			}
			else
			{
				var hitInfo = CalcHitInfo(e.Location);
				handleMouseMove(hitInfo);
			}
		}

		private void hideButtons()
		{
			if (_hitInfo?.Card != null)
				foreach (var field in _hitInfo.Card.Fields)
				{
					field.IsSortVisible = false;
					field.IsSearchVisible = false;
				}
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			var hitInfo = CalcHitInfo(new Point(-10000, -10000));
			handleMouseMove(hitInfo);
		}

		private void handleMouseMove(HitInfo hitInfo)
		{
			var prevHitInfo = _hitInfo;
			_hitInfo = hitInfo;

			var prevField = prevHitInfo?.Field;
			var newField = _hitInfo.Field;
			var prevCard = prevHitInfo?.Card;
			var newCard = _hitInfo.Card;

			bool suppressed = ModifierKeys == Keys.Alt;

			if (prevField != null && prevField != newField)
			{
				prevField.IsHotTracked = false;
				prevField.IsSortHotTracked = false;
				prevField.IsSearchHotTracked = false;
				prevField.HotTrackedCustomButtonIndex = -1;
			}

			if (newField != null)
			{
				newField.IsHotTracked = true;
				newField.IsSortHotTracked = _hitInfo.IsSortButton;
				newField.IsSearchHotTracked = _hitInfo.IsSearchButton;
				newField.HotTrackedCustomButtonIndex = _hitInfo.CustomButtonIndex;
			}

			if (prevCard != null && prevCard != newCard)
			{
				prevCard.IsHotTracked = false;
				foreach (var field in prevCard.Fields)
					field.IsSearchVisible = false;
			}

			if (newCard != null)
			{
				newCard.IsHotTracked = true;
				foreach (var field in newCard.Fields)
					field.IsSearchVisible = !suppressed && SearchOptions.IsButtonVisible(field);
			}

			int columnsCount = getColumnsCount();
			int rowsCount = getRowsCount();

			for (int j = 0; j < rowsCount; j++)
				for (int i = 0; i < columnsCount; i++)
				{
					int index = getCardIndex(i, j, columnsCount);

					var card = Cards[index];
					if (index >= Cards.Count || !card.Visible)
						continue;

					foreach (var field in card.Fields)
						field.IsSortVisible = !suppressed && SortOptions.IsButtonVisible(card, field);
				}

			foreach (var direction in getAlignDirections())
				_alignButtonHotTracked[direction] = direction == _hitInfo.AlignButtonDirection;
		}

		private void mouseClick(object sender, MouseEventArgs e)
		{
			if (_selection.Selecting)
				return;

			var hitInfo = CalcHitInfo(e.Location);

			if (e.Button == MouseButtons.Left)
			{
				if (hitInfo.IsSearchButton)
					handleSearchClick(hitInfo);
				else if (hitInfo.IsSortButton)
					handleSortClick(hitInfo);
				else if (hitInfo.AlignButtonDirection.HasValue)
					handleAlignClick(hitInfo);
			}

			MouseClicked?.Invoke(this, hitInfo, e);
		}

		private void handleSearchClick(HitInfo hitInfo)
		{
			SearchClicked?.Invoke(this,
				new SearchArgs
				{
					FieldName = hitInfo.FieldName,
					FieldValue = GetText(hitInfo.RowHandle, hitInfo.FieldName),
					UseAndOperator = ModifierKeys == Keys.Shift
				});
		}

		private void handleSortClick(HitInfo hitInfo)
		{
			FieldSortInfo sortInfo;

			if (!_sortIndexByField.TryGetValue(hitInfo.FieldName, out int sortIndex))
			{
				sortIndex = -1;
				sortInfo = null;
			}
			else
				sortInfo = _sortInfos[sortIndex];


			if (ModifierKeys == Keys.None)
			{
				_sortInfos.Clear();
				_sortIndexByField.Clear();

				if (sortInfo == null)
				{
					sortInfo = new FieldSortInfo(hitInfo.FieldName, SortOrder.Ascending);

					_sortInfos.Add(sortInfo);
					_sortIndexByField.Add(hitInfo.FieldName, _sortInfos.Count - 1);
				}
				else if (sortInfo.SortOrder == SortOrder.Ascending)
				{
					sortInfo.SortOrder = SortOrder.Descending;
					_sortInfos.Add(sortInfo);
					_sortIndexByField.Add(hitInfo.FieldName, _sortInfos.Count - 1);
				}

				updateSort();
				SortChanged?.Invoke(this);
			}
			else if (ModifierKeys == Keys.Shift)
			{
				if (sortInfo == null)
				{
					sortInfo = new FieldSortInfo(hitInfo.FieldName, SortOrder.Ascending);
					_sortInfos.Add(sortInfo);
					_sortIndexByField.Add(hitInfo.FieldName, _sortInfos.Count - 1);
				}
				else if (sortInfo.SortOrder == SortOrder.Ascending)
				{
					sortInfo.SortOrder = SortOrder.Descending;
				}
				else if (sortInfo.SortOrder == SortOrder.Descending)
				{
					_sortInfos.RemoveAt(sortIndex);
					_sortIndexByField.Remove(hitInfo.FieldName);
				}

				updateSort();
				SortChanged?.Invoke(this);
			}
			else if (ModifierKeys == Keys.Control)
			{
				if (sortInfo != null)
				{
					_sortInfos.RemoveAt(sortIndex);
					_sortIndexByField.Remove(hitInfo.FieldName);

					updateSort();
					SortChanged?.Invoke(this);
				}
			}
		}

		private void handleAlignClick(HitInfo hitInfo)
		{
			LayoutOptions.Alignment = hitInfo.AlignButtonDirection.Value;
		}

		private void updateSort()
		{
			_sortIndexByField = Enumerable.Range(0, _sortInfos.Count)
				.ToDictionary(i => _sortInfos[i].FieldName);

			foreach (var card in Cards)
				foreach (var field in card.Fields)
					updateSort(field);
		}



		public void InvalidateCard(object row)
		{
			if (row == null)
				return;

			int rowHandle = FindRow(row);
			if (rowHandle < 0)
				return;

			var cardIndex = getDisplayIndex(rowHandle);

			if (cardIndex < 0)
				return;

			var card = Cards[cardIndex];
			Invalidate(card.Bounds);
		}



		public HitInfo CalcHitInfo(Point location)
		{
			var hitInfo = new HitInfo
			{
				InBounds = getDisplayBounds().Contains(location)
			};

			if (!hitInfo.InBounds)
				return hitInfo;

			setFieldRelatedData(location, hitInfo);

			if (!hitInfo.IsSearchButton && !hitInfo.IsSortButton)
			{
				setAlignButtonDirection(location, hitInfo);

				if (hitInfo.AlignButtonDirection.HasValue)
				{
					hitInfo.ClearCard();
					hitInfo.ClearField();
				}
			}

			return hitInfo;
		}

		private void setFieldRelatedData(Point location, HitInfo hitInfo)
		{
			var index = getCardIndex(location);

			if (index < 0)
				return;

			var card = Cards[index];

			if (!card.Visible)
				return;

			int handle = getRowHandle(index);
			var rowDataSource = FindRow(handle);

			hitInfo.SetCard(card, handle, rowDataSource);

			var field = card.Fields.FirstOrDefault(_ => _.Bounds.Plus(card.Bounds.Location).Contains(location));

			if (field == null)
				return;

			var bounds = getFieldBounds(field, card);
			var buttons = card.GetFieldButtons(field, SearchOptions, SortOptions).ToList();
			buttons.LayOutIn(bounds);

			ButtonLayout button = null;
			bool isSortButton = SortOptions.Allow && field.AllowSort &&
				(button = buttons.FirstOrDefault(b => b.Type == ButtonType.Sort && b.Bounds.Contains(location))) != null;

			bool isSearchButton = button == null && SearchOptions.Allow && field.SearchOptions.Allow &&
				(button = buttons.FirstOrDefault(b => b.Type == ButtonType.Search && b.Bounds.Contains(location))) != null;

			int customButtonIndex;
			(customButtonIndex, button) = button == null
				? getCustomButtonIndex(location, buttons)
				: (-1, button);

			hitInfo.SetField(field, isSortButton, isSearchButton, customButtonIndex, button?.Bounds);
		}

		private static (int Index, ButtonLayout layout) getCustomButtonIndex(Point location, IList<ButtonLayout> buttons)
		{
			int firstCustomButtonIndex = buttons.BinarySearchFirstIndexOf(button => button.Type == ButtonType.Custom);

			if (firstCustomButtonIndex == -1)
				return (-1, null);

			for (int i = firstCustomButtonIndex; i < buttons.Count; i++)
			{
				if (buttons[i].Bounds.Contains(location))
					return (i - firstCustomButtonIndex, buttons[i]);
			}

			return (-1, null);
		}

		private int getCardIndex(Point location)
		{
			var cardCell = getCardCell(location);

			int rowsCount = getRowsCount();
			int columnsCount = getColumnsCount();

			if (cardCell.X < 0 || cardCell.X >= columnsCount || cardCell.Y < 0 || cardCell.Y >= rowsCount)
				return -1;

			var index = getCardIndex(cardCell.X, cardCell.Y, columnsCount);

			if (index < 0 || index >= Cards.Count)
				return -1;

			return index;
		}

		private void setAlignButtonDirection(Point location, HitInfo hitInfo)
		{
			var alignDirection = getAlignIconDirection(location);

			if (!alignDirection.HasValue || !_alignButtonVisible[alignDirection.Value])
				return;

			hitInfo.AlignButtonDirection = alignDirection.Value;
		}



		private void alignButtonStateChanged(Direction direction, bool prevVal, bool newVal)
		{
			var bounds = getAlignIconBounds(direction);
			Invalidate(bounds);
		}

		private static IEnumerable<Direction> getAlignDirections()
		{
			yield return Direction.TopLeft;
			yield return Direction.TopRight;
			yield return Direction.BottomRight;
			yield return Direction.BottomLeft;
		}

		private bool isAlignIconVisible(Direction direction)
		{
			return LayoutOptions.Alignment != direction;
		}

		private Point getCornerCardCell(Direction direction)
		{
			switch (direction)
			{
				case Direction.TopLeft:
					return new Point(0, 0);
				case Direction.TopRight:
					return new Point(getColumnsCount() - 1, 0);
				case Direction.BottomRight:
					return new Point(getColumnsCount() - 1, getRowsCount() - 1);
				case Direction.BottomLeft:
					return new Point(0, getRowsCount() - 1);
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		private Direction? getAlignIconDirection(Point location)
		{
			if (!LayoutOptions.HasAlignIcons)
				return null;

			foreach (var direction in getAlignDirections())
				if (getAlignIconBounds(direction).Contains(location))
					return direction;

			return null;
		}

		private Rectangle getDisplayBounds()
		{
			return new Rectangle(new Point(0, 0), new Size(Width - ScrollWidth, Height));
		}

		private Rectangle getAlignIconBounds(Direction direction)
		{
			var size = LayoutOptions.AlignIconSize;
			var bounds = getDisplayBounds();

			switch (direction)
			{
				case Direction.TopLeft:
					return new Rectangle(new Point(0, 0), size);
				case Direction.TopRight:
					return new Rectangle(new Point(bounds.Width - size.Width, 0), size);
				case Direction.BottomRight:
					return new Rectangle(new Point(bounds.Width - size.Width, bounds.Height - size.Height), size);
				case Direction.BottomLeft:
					return new Rectangle(new Point(0, bounds.Height - size.Height), size);
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		private Rectangle getCardBounds(int i, int j, Point alignmentShift)
		{
			var cardArea = new Rectangle(getCardLocation(i, j, alignmentShift), CardSize);
			return cardArea;
		}

		private Point getCardLocation(int i, int j, Point alignmentShift)
		{
			var cardInterval = LayoutOptions.CardInterval;

			var result = new Point(
				cardInterval.Width / 2 + i * (CardSize.Width + cardInterval.Width),
				cardInterval.Height / 2 + j * (CardSize.Height + cardInterval.Height));

			result = result.Plus(alignmentShift);

			return result;
		}

		private Point getCardCell(Point location)
		{
			var shift = getAlignmentShift();
			location = location.Minus(shift);

			var cardInterval = LayoutOptions.CardInterval;

			return new Point(
				(location.X - cardInterval.Width / 2) / (CardSize.Width + cardInterval.Width),
				(location.Y - cardInterval.Height / 2) / (CardSize.Height + cardInterval.Height));
		}

		private Point getAlignmentShift()
		{
			var cell = getCornerCardCell(Direction.BottomRight);

			var cardLogicalBounds = getCardBounds(cell.X, cell.Y, alignmentShift: default).RightBottom() +
				LayoutOptions.CardInterval.MultiplyBy(0.5f).Round();

			var cardToDisplayRightBottom = getDisplayBounds()
				.RightBottom()
				.Minus(cardLogicalBounds);

			var result = new Point(
				applyAlignmentSide(cardToDisplayRightBottom.X, LayoutOptions.Alignment.HasFlag(Direction.Left)),
				applyAlignmentSide(cardToDisplayRightBottom.Y, LayoutOptions.Alignment.HasFlag(Direction.Top)));

			return result;
		}

		private static int applyAlignmentSide(int cardToDisplayMaxValue, bool alignToMin)
		{
			if (alignToMin && cardToDisplayMaxValue < 0)
				return 0;

			if (cardToDisplayMaxValue > 0)
				return cardToDisplayMaxValue / 2;

			return cardToDisplayMaxValue;
		}



		private static int getCardIndex(int i, int j, int columnsCount)
		{
			return j * columnsCount + i;
		}


		private int snapCardIndex(int value, int count)
		{
			if (count == 0)
				return 0;

			if (value >= count)
				value = count - 1;

			else if (value < 0 && count > 0)
				value = 0;

			int columnsCount = getColumnsCount();
			int rowsCount = getRowsCount();

			if (value >= 0)
			{
				int pageSize = columnsCount * rowsCount;
				value = pageSize * (value / pageSize);
			}

			return value;
		}

		public void ApplyCardIndex()
		{
			if (!_selection.Selecting)
				_selection.Reset();

			int columnsCount = getColumnsCount();
			int rowsCount = getRowsCount();

			for (int j = 0; j < rowsCount; j++)
				for (int i = 0; i < columnsCount; i++)
				{
					int index = j * columnsCount + i;
					int rowHandle = getRowHandle(index);
					var row = FindRow(rowHandle);

					if (row != null)
					{
						Cards[index].DataSource = row;
						Cards[index].Visible = true;

						RowDataLoaded?.Invoke(this, rowHandle);
					}
					else
					{
						Cards[index].DataSource = null;
						Cards[index].Visible = false;
					}
				}

			for (int index = rowsCount * columnsCount; index < Cards.Count; index++)
			{
				Cards[index].DataSource = null;
				Cards[index].Visible = false;
			}

			foreach (var direction in getAlignDirections())
			{
				bool visible = isAlignIconVisible(direction);
				_alignButtonVisible[direction] = visible;
			}
		}

		public void ApplyIconRecognizer()
		{
			foreach (var card in Cards)
				card.SetIconRecognizer(IconRecognizer);
		}

		public void SetHighlightTextRanges(IList<TextRange> ranges, int rowHandle, string fieldName)
		{
			var card = Cards[getDisplayIndex(rowHandle)];
			var field = card.Fields.Single(_ => _.FieldName == fieldName);
			field.HighlightRanges = ranges;
		}

		private int getDisplayIndex(int rowHandle)
		{
			int columnsCount = getColumnsCount();
			int rowsCount = getRowsCount();
			int result = rowHandle - CardIndex;

			if (result < 0 || result >= columnsCount * rowsCount || !Cards[result].Visible)
				return -1;

			return result;
		}

		private int getRowHandle(int displayIndex)
		{
			return CardIndex + displayIndex;
		}

		public IList<TextRange> GetHighlightTextRanges(int rowHandle, string fieldName)
		{
			var card = Cards[getDisplayIndex(rowHandle)];
			var field = card.Fields.Single(_ => _.FieldName == fieldName);
			return field.HighlightRanges;
		}

		public int FindRow(object row)
		{
			if (!_rowHandleByObject.TryGetValue(row, out int rowHandle))
				return -1;

			return rowHandle;
		}

		public object FindRow(int index)
		{
			if (DataSource == null || index < 0 || index >= DataSource.Count)
				return null;

			return DataSource[index];
		}

		public string GetText(int index, string fieldName)
		{
			var dataObject = FindRow(index);

			if (dataObject == null)
				return null;

			var field = ProbeCard.Fields.FirstOrDefault(_ => _.FieldName == fieldName);
			if (field == null)
				return null;

			ProbeCard.DataSource = dataObject;
			return field.DataText;
		}



		public bool IsSelectingText() =>
			_selection.Selecting;

		private TextSelection getNonEmptySelection() =>
			Cards
				.Where(c => c.Visible)
				.SelectMany(c => c.Fields.Select(f => f.TextSelection))
				.FirstOrDefault(s => !s.IsEmpty);


		public void TransformIcons(Func<Bitmap, Bitmap> transformation)
		{
			SortOptions.Icon = SortOptions.Icon?.Invoke0(transformation);
			SortOptions.AscIcon = SortOptions.AscIcon?.Invoke0(transformation);
			SortOptions.DescIcon = SortOptions.DescIcon?.Invoke0(transformation);

			SortOptions.IconTransp = SortOptions.IconTransp?.Invoke0(transformation);
			SortOptions.AscIconTransp = SortOptions.AscIconTransp?.Invoke0(transformation);
			SortOptions.DescIconTransp = SortOptions.DescIconTransp?.Invoke0(transformation);

			SearchOptions.Button.Icon = SearchOptions.Button.Icon?.Invoke0(transformation);
			SearchOptions.Button.IconTransp = SearchOptions.Button.IconTransp?.Invoke0(transformation);

			foreach (var pair in LayoutOptions.AlignmentIconsByDirection.ToArray())
				LayoutOptions.AlignmentIconsByDirection[pair.Key] = pair.Value?.Invoke0(transformation);

			foreach (var pair in LayoutOptions.AlignmentHoveredIconsByDirection.ToArray())
				LayoutOptions.AlignmentHoveredIconsByDirection[pair.Key] = pair.Value?.Invoke0(transformation);
		}

		[Category("Settings")]
		[DefaultValue(typeof(LayoutControl)), TypeConverter(typeof(LayoutControlTypeConverter))]
		public Type LayoutControlType
		{
			get => _layoutControlType;
			set
			{
				_layoutControlType = value;
				ProbeCard = createProbeCard();
				Cards.Clear();
				Invalidate();
				OnLayout(new LayoutEventArgs(this, nameof(LayoutControlType)));
			}
		}

		[Category("Settings")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public SortOptions SortOptions { get; set; } = new SortOptions();

		[Category("Settings")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public SearchOptions SearchOptions { get; set; } = new SearchOptions();

		[Category("Settings")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public SelectionOptions SelectionOptions { get; set; } = new SelectionOptions();

		private LayoutOptions _layoutOptions = new LayoutOptions();

		[Category("Settings")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public LayoutOptions LayoutOptions
		{
			get => _layoutOptions;
			set
			{
				if (value == _layoutOptions)
					return;

				if (_layoutOptions != null)
					_layoutOptions.Changed -= layoutOptionsChanged;

				_layoutOptions = value;
				layoutOptionsChanged();
				_layoutOptions.Changed += layoutOptionsChanged;
			}
		}

		private void layoutOptionsChanged()
		{
			_scrollBar.Visible = !_layoutOptions.HideScroll;
			Invalidate();
			OnLayout(new LayoutEventArgs(this, nameof(LayoutOptions)));
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CardIndex
		{
			get => _cardIndex;
			set
			{
				if (value == _cardIndex)
					return;

				_cardIndex = value;
				CardIndexChanged?.Invoke(this);
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IconRecognizer IconRecognizer { get; set; }

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList DataSource { get; set; }

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<FieldSortInfo> SortInfo
		{
			get => _sortInfos.ToList();

			set
			{
				_sortInfos.Clear();
				_sortInfos.AddRange(value);

				updateSort();
				SortChanged?.Invoke(this);
			}
		}

		[Browsable(false)]
		public int ScrollWidth => _layoutOptions.HideScroll ? 0 : _scrollBar.Width;

		[Browsable(false)]
		public int Count => DataSource?.Count ?? 0;

		[Browsable(false)]
		private int PageCount
		{
			get
			{
				int pageSize = getRowsCount() * getColumnsCount();

				int pageCount = Count / pageSize;

				if (Count % pageSize > 0)
					pageCount++;

				return pageCount;
			}
		}

		[Browsable(false)]
		public Size CardSize => ProbeCard.Size;

		[Browsable(false)]
		private IList<LayoutControl> Cards { get; }

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LayoutControl ProbeCard { get; private set; }

		public IEnumerable<string> FieldNames =>
			ProbeCard.Fields.Select(_ => _.FieldName).Where(F.IsNotNull);

		private bool IsUnderMouse =>
			this.IsUnderMouse() || _scrollBar.IsUnderMouse();

		public event Action<object> CardIndexChanged;
		public event Action<object, CustomDrawArgs> CustomDrawField;
		public event Action<object, int> RowDataLoaded;
		public event Action<object> SortChanged;
		public event Action<object, SearchArgs> SearchClicked;
		public event Action<object, LayoutControl> ProbeCardCreating;
		public event Action<object, HitInfo, MouseEventArgs> MouseClicked;
		public event Action<object, HitInfo, CancelEventArgs> SelectionStarted;


		private Type _layoutControlType;
		private int _cardIndex;

		private readonly Dictionary<object, int> _rowHandleByObject = new Dictionary<object, int>();
		private readonly List<FieldSortInfo> _sortInfos = new List<FieldSortInfo>();
		private Dictionary<string, int> _sortIndexByField = new Dictionary<string, int>();

		private readonly EventFiringMap<Direction, bool> _alignButtonVisible = new EventFiringMap<Direction, bool>();
		private readonly EventFiringMap<Direction, bool> _alignButtonHotTracked = new EventFiringMap<Direction, bool>();

		private HitInfo _hitInfo;

		private readonly RectangularSelection _selection = new RectangularSelection();

		private readonly Timer _selectionCaretTimer;
		private bool _selectionCaretTimerSkip;

		private bool _updatingScrollValue;

		#region mouse wheel without focus

		public bool PreFilterMessage(ref Message m)
		{
			if (Disposing || IsDisposed)
				return false;

			// WM_MOUSEWHEEL, WM_KEYDOWN
			if (m.Msg != 0x020a && m.Msg != 0x0100)
				return false;

			if (m.Msg == 0x0100)
			{
				int[] navigationKeys =
				{
					0x21, // pgUp
					0x22, // pgDn
					0x23, // end
					0x24 //  home
				};

				var keyCode = m.WParam.ToInt32();
				if (keyCode < navigationKeys[0] || keyCode > navigationKeys[navigationKeys.Length - 1])
					return false;
			}

			if (!ContainsFocus && IsUnderMouse)
				// send the event to this control
				ControlHelpers.SendMessage(Handle, m.Msg, m.WParam, m.LParam);

			return false;
		}

		#endregion
	}
}