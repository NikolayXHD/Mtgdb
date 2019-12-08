using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using Mtgdb.Controls.Properties;
using Mtgdb.Data;

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

			_selectionCaretTimer = new Timer
			{
				Interval = SystemInformation.CaretBlinkTime
			};

			_selectionCaretTimer.Tick += tick;

			Scrollbar.ChannelColor = SystemColors.Control;
			Scrollbar.BorderColor = SystemColors.Control;

			Scrollbar.UpArrowImage = Resources.uparrow.ScaleBy(0.5f);
			Scrollbar.ThumbTopImage = Resources.ThumbTop.ScaleBy(0.5f);
			Scrollbar.ThumbTopSpanImage = Resources.ThumbSpanTop.ScaleBy(0.5f);
			Scrollbar.ThumbMiddleImage = Resources.ThumbMiddle.ScaleBy(0.5f);
			Scrollbar.ThumbBottomSpanImage = Resources.ThumbSpanBottom.ScaleBy(0.5f);
			Scrollbar.ThumbBottomImage = Resources.ThumbBottom.ScaleBy(0.5f);
			Scrollbar.DownArrowImage = Resources.downarrow.ScaleBy(0.5f);
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

		private void layout(object sender, LayoutEventArgs e)
		{
			applySize();
			loadVisibleData();
			updateScrollbar();
		}



		private void paint(object sender, PaintEventArgs eArgs)
		{
			var paintActions = new PaintActions();

			// implicit connection: data_source_sync
			lock (DataSource)
			{
				using var hotTrackBgBrush = new SolidBrush(SelectionOptions.HotTrackBackColor);
				using var hotTrackBgPen = new Pen(SelectionOptions.HotTrackBorderColor);
				paintActions.Back.Add(e => e.Graphics.Clear(BackColor));
				addPaintCardActions(paintActions, eArgs.ClipRectangle, hotTrackBgBrush, hotTrackBgPen);
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
				using var brush = new SolidBrush(Color.FromArgb(alpha, SelectionOptions.RectFillColor));
				e.Graphics.FillRectangle(brush, rectangle);
			}

			if (SelectionOptions.RectBorderColor != Color.Transparent)
			{
				using var pen = new Pen(Color.FromArgb(alpha, SelectionOptions.RectBorderColor));
				e.Graphics.DrawRectangle(pen, rectangle);
			}
		}

		private void addPaintCardActions(PaintActions actions, Rectangle clipRectangle, SolidBrush hotTrackBgBrush, Pen hotTrackBgPen)
		{
			for (int i = 0; i < Cards.Count; i++)
			{
				var card = Cards[i];
				int rowHandle = getRowHandle(displayIndex: i);

				if (!card.Visible || card.DataSource == null)
					continue;

				if (!clipRectangle.IntersectsWith(card.Bounds))
					continue;

				actions.Back.Add(e => card.PaintSelf(e.Graphics, card.Location, BackColor));

				foreach (var field in card.Fields)
				{
					var fieldArea = new Rectangle(card.Location.Plus(field.Location), field.Size);
					if (!clipRectangle.IntersectsWith(fieldArea))
						continue;

					actions.Back.Add(e =>
						paintFieldBg(e, field, fieldArea, hotTrackBgBrush, hotTrackBgPen));
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
				using var bgBrush = new SolidBrush(field.BackColor);
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
					Selection = _selection,
					HotTracked = field.IsHotTracked
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



		public void ScrollTo(int rowHandle)
		{
			if (rowHandle < 0)
				return;

			int index = snapCardIndex(rowHandle);
			if (index == CardIndex)
				return;

			CardIndex = index;
			loadVisibleData();
			updateScrollbar();
		}

		public void ScrollTo(object row) =>
			ScrollTo(FindIndex(row));

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
			}

			CardIndex = snapCardIndex(CardIndex);
			loadVisibleData();
			updateScrollbar();
		}

		public int GetPageSize() =>
			getRowsCount() * getColumnsCount();

		private void updateScrollbar()
		{
			int pageSize = GetPageSize();
			Scrollbar.Enabled = Count > pageSize;

			int largeChange = ((int) Math.Round(Math.Pow(10, Math.Round(Math.Log10(Count + 1d) - 2.5d))))
				.WithinRange(1, PageCount);

			int maximum = Math.Max(0, PageCount - 1);
			int value = CardIndex / pageSize;

			_updatingScrollValue = true;

			Scrollbar.SmallChange = 1;
			Scrollbar.LargeChange = largeChange;
			Scrollbar.Minimum = 0;
			Scrollbar.Maximum = maximum;
			Scrollbar.Value = value;

			_updatingScrollValue = false;
		}

		private void load(object sender, EventArgs e)
		{
			applyIconRecognizer();

			Scrollbar.Scroll += scrolled;
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

			result.SetIconRecognizer(IconRecognizer);
			result.Font = Font;
			result.HighlightOptions = HighlightOptions;

			CardCreating?.Invoke(this, result);

			return result;
		}

		private void updateSort(FieldControl field)
		{
			if (field.FieldName != null && _sortIndexByField.TryGetValue(field.FieldName, out int sortIndex))
				field.SortOrder = _sortInfo[sortIndex].SortOrder;
			else
				field.SortOrder = SortDirection.No;
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

			int index = snapCardIndex(Scrollbar.Value * GetPageSize());
			if (index == CardIndex)
				return;

			CardIndex = index;
			loadVisibleData();
		}

		private static void previewKeyDown(object sender, PreviewKeyDownEventArgs e) =>
			e.IsInputKey = true;

		private void keyDown(object sender, KeyEventArgs e)
		{
			void resetTimer() =>
				_selectionCaretTimerSkip = true;

			bool handled = true;

			if (e.KeyData == Keys.PageDown)
			{
				if (IsUnderMouse)
					scrollAdd(GetPageSize());
				else
					handled = false;
			}
			else if (e.KeyData == Keys.PageUp)
			{
				if (IsUnderMouse)
					scrollAdd(-GetPageSize());
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
				scrollAdd(GetPageSize());
			else if (e.Delta > 0)
				scrollAdd(-GetPageSize());
		}

		private void scrollAdd(int pageSize)
		{
			int index = snapCardIndex(CardIndex + pageSize);
			if (index == CardIndex)
				return;

			CardIndex = index;
			loadVisibleData();
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
						Cards.Add(setupCard());

					Cards[index].Location = location;
					Cards[index].Visible = Cards[index].DataSource != null;
				}

			for (int index = rowsCount * columnsCount; index < Cards.Count; index++)
				Cards[index].Visible = false;

			LayoutControl setupCard()
			{
				var result = createCard();

				foreach (var field in result.Fields)
					updateSort(field);

				result.Invalid += cardInvalidated;
				return result;
			}
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
					IsShiftModifier = ModifierKeys == Keys.Shift
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
				sortInfo = _sortInfo[sortIndex];


			if (ModifierKeys == Keys.None)
			{
				_sortInfo.Clear();
				_sortIndexByField.Clear();

				if (sortInfo == null)
				{
					sortInfo = new FieldSortInfo(hitInfo.FieldName, SortDirection.Asc);

					_sortInfo.Add(sortInfo);
					_sortIndexByField.Add(hitInfo.FieldName, _sortInfo.Count - 1);
				}
				else if (sortInfo.SortOrder == SortDirection.Asc)
				{
					sortInfo.SortOrder = SortDirection.Desc;
					_sortInfo.Add(sortInfo);
					_sortIndexByField.Add(hitInfo.FieldName, _sortInfo.Count - 1);
				}

				updateSort();
				SortChanged?.Invoke(this);
			}
			else if (ModifierKeys == Keys.Shift)
			{
				if (sortInfo == null)
				{
					sortInfo = new FieldSortInfo(hitInfo.FieldName, SortDirection.Asc);
					_sortInfo.Add(sortInfo);
					_sortIndexByField.Add(hitInfo.FieldName, _sortInfo.Count - 1);
				}
				else if (sortInfo.SortOrder == SortDirection.Asc)
				{
					sortInfo.SortOrder = SortDirection.Desc;
				}
				else if (sortInfo.SortOrder == SortDirection.Desc)
				{
					_sortInfo.RemoveAt(sortIndex);
					_sortIndexByField.Remove(hitInfo.FieldName);
				}

				updateSort();
				SortChanged?.Invoke(this);
			}
			else if (ModifierKeys == Keys.Control)
			{
				if (sortInfo != null)
				{
					_sortInfo.RemoveAt(sortIndex);
					_sortIndexByField.Remove(hitInfo.FieldName);

					updateSort();
					SortChanged?.Invoke(this);
				}
			}
		}

		private void handleAlignClick(HitInfo hitInfo)
		{
			if (hitInfo.AlignButtonDirection.HasValue)
				LayoutOptions.Alignment = hitInfo.AlignButtonDirection.Value;
		}

		private void updateSort()
		{
			_sortIndexByField = Enumerable.Range(0, _sortInfo.Count)
				.ToDictionary(i => _sortInfo[i].FieldName);

			foreach (var card in Cards)
				foreach (var field in card.Fields)
					updateSort(field);
		}



		public void InvalidateCard(object row)
		{
			var card = getCard(row);
			if (card != null)
				Invalidate(card.Bounds);
		}

		public Rectangle? GetFieldBounds(object row, string fieldName)
		{
			if (row == null)
				return null;

			var index = FindIndex(row);
			if (index < 0)
				return null;

			return GetFieldBounds(index, fieldName);
		}

		public Rectangle GetFieldBounds(int index, string fieldName)
		{
			var card = Cards[getDisplayIndex(index)];
			var field = card.Fields.Single(_ => _.FieldName == fieldName);
			var bounds = new Rectangle(card.Location.Plus(field.Location), field.Size);
			return bounds;
		}

		private LayoutControl getCard(object row)
		{
			if (row == null)
				return null;

			int index = FindIndex(row);
			if (index < 0)
				return null;

			var cardIndex = getDisplayIndex(index);

			if (cardIndex < 0)
				return null;

			return Cards[cardIndex];
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

		private Rectangle getCardBounds(int i, int j, Point alignmentShift) =>
			new Rectangle(getCardLocation(i, j, alignmentShift), CardSize);

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


		private int snapCardIndex(int value)
		{
			var count = Count;
			if (count == 0)
				return 0;

			if (value >= count)
				value = count - 1;
			else if (value < 0 && count > 0)
				value = 0;

			if (value >= 0)
			{
				int pageSize = GetPageSize();
				value = pageSize * (value / pageSize);
			}

			return value;
		}

		private void loadVisibleData()
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
					Cards[index].DataSource = row;
					bool hasData = row != null;
					Cards[index].Visible = hasData;
					if (hasData)
						RowDataLoaded?.Invoke(this, rowHandle);
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

			Invalidate();
		}

		private void applyIconRecognizer()
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
			int result = rowHandle - CardIndex;
			if (result < 0 || result >= GetPageSize() || !Cards[result].Visible)
				return -1;

			return result;
		}

		private int getRowHandle(int displayIndex) =>
			CardIndex + displayIndex;

		public IList<TextRange> GetHighlightTextRanges(int rowHandle, string fieldName)
		{
			var card = Cards[getDisplayIndex(rowHandle)];
			var field = card.Fields.Single(_ => _.FieldName == fieldName);
			return field.HighlightRanges;
		}

		public int FindIndex(object row)
		{
			if (row == null || !_rowHandleByObject.TryGetValue(row, out int rowHandle))
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


		public void ResetLayout()
		{
			ProbeCard = createCard();
			Cards.Clear();
			Invalidate();
			OnLayout(new LayoutEventArgs(this, nameof(LayoutControlType)));
		}

		private Type _layoutControlType;

		[Category("Settings")]
		[DefaultValue(typeof(LayoutControl)), TypeConverter(typeof(LayoutControlTypeConverter))]
		public Type LayoutControlType
		{
			get => _layoutControlType;
			set
			{
				_layoutControlType = value;
				ResetLayout();
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
			Scrollbar.Visible = !_layoutOptions.HideScroll;
			Invalidate();
			OnLayout(new LayoutEventArgs(this, nameof(LayoutOptions)));
		}

		private int _cardIndex;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CardIndex
		{
			get => _cardIndex;
			private set
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

		private readonly List<FieldSortInfo> _sortInfo = new List<FieldSortInfo>();
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<FieldSortInfo> SortInfo
		{
			get => _sortInfo.ToList();

			set
			{
				_sortInfo.Clear();
				_sortInfo.AddRange(value);

				updateSort();
				SortChanged?.Invoke(this);
			}
		}

		[Browsable(false)]
		public int ScrollWidth => _layoutOptions.HideScroll ? 0 : Scrollbar.Width;

		[Browsable(false)]
		public int Count => DataSource?.Count ?? 0;

		[Browsable(false)]
		private int PageCount
		{
			get
			{
				int pageSize = GetPageSize();

				int pageCount = Count / pageSize;

				if (Count % pageSize > 0)
					pageCount++;

				return pageCount;
			}
		}

		[Browsable(false)]
		public Size CardSize => ProbeCard.Size;

		[Browsable(false)]
		public HighlightOptions HighlightOptions { get; } = new HighlightOptions();

		[Browsable(false)]
		private IList<LayoutControl> Cards { get; }

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		private LayoutControl ProbeCard { get; set; }

		public IEnumerable<string> FieldNames =>
			ProbeCard.Fields.Select(_ => _.FieldName).Where(F.IsNotNull);

		private bool IsUnderMouse =>
			this.IsUnderMouse() || Scrollbar.IsUnderMouse();

		/// <summary>
		/// mouse wheel without focus
		/// </summary>
		public bool PreFilterMessage(ref Message m)
		{
			if (Disposing || IsDisposed)
				return false;

			if (ContainsFocus || !IsUnderMouse)
				return false;

			switch (m.Msg)
			{
				case 0x020a: // WM_MOUSEWHEEL

					// win10 scrolls unfocused window under mouse itself
					if (_isWin10)
						return false;

					break;

				case 0x0100: // WM_KEYDOWN
					var keyCode = m.WParam.ToInt32();

					// 0x21: pgUp 0x22: pgDn 0x23: end 0x24: home
					if (keyCode < 0x21 || keyCode > 0x24)
						return false;

					break;
			}

			// send the event to this control
			ControlHelpers.SendMessage(Handle, m.Msg, m.WParam, m.LParam);
			return false;
		}

		private static bool isWin10()
		{
			try
			{
				var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

				if (reg == null)
					return false;

				string productName = (string) reg.GetValue("ProductName");
				var result = Regex.IsMatch(productName, @"^Windows 10\b");
				return result;
			}
			catch
			{
				return false;
			}
		}

		public event Action<object> CardIndexChanged;
		public event Action<object, CustomDrawArgs> CustomDrawField;
		public event Action<object, int> RowDataLoaded;
		public event Action<object> SortChanged;
		public event Action<object, SearchArgs> SearchClicked;
		public event Action<object, LayoutControl> CardCreating;
		public event Action<object, HitInfo, MouseEventArgs> MouseClicked;
		public event Action<object, HitInfo, CancelEventArgs> SelectionStarted;


		private readonly RectangularSelection _selection = new RectangularSelection();
		private readonly Timer _selectionCaretTimer;

		private readonly Dictionary<object, int> _rowHandleByObject = new Dictionary<object, int>();
		private readonly EventFiringMap<Direction, bool> _alignButtonVisible = new EventFiringMap<Direction, bool>();
		private readonly EventFiringMap<Direction, bool> _alignButtonHotTracked = new EventFiringMap<Direction, bool>();

		private Dictionary<string, int> _sortIndexByField = new Dictionary<string, int>();
		private HitInfo _hitInfo;
		private bool _selectionCaretTimerSkip;
		private bool _updatingScrollValue;

		private static readonly bool _isWin10 = isWin10();
	}
}