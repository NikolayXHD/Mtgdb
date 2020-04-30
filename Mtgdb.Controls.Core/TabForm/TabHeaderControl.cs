using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Controls.Properties;

namespace Mtgdb.Controls
{
	public partial class TabHeaderControl : UserControl
	{
		private readonly object _syncRoot = new object();

		public event Action<TabHeaderControl, int> SelectedIndexChanging;
		public event Action<TabHeaderControl, int> SelectedIndexChanged;
		public event Action<TabHeaderControl, int> HoveredIndexChanged;
		public event Action<TabHeaderControl, int> TabAdded;
		public event Action<TabHeaderControl, int> TabRemoving;
		public event Action<TabHeaderControl> TabRemoved;
		public event Action<TabHeaderControl> TabReordered;

		public TabHeaderControl()
		{
			InitializeComponent();

			TabStop = false;

			clearTabs();

			Layout += layout;
			MouseMove += mouseMove;
			MouseClick += mouseClick;
			MouseDown += mouseDown;
			MouseUp += mouseUp;
			MouseLeave += mouseLeave;


			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.ResizeRedraw,
				true);

			_graphics = CreateGraphics();

			CloseIconHovered = _defaultCloseIconHovered;
			_closeIcon = _defaultCloseIcon;
			_addIcon = _defaultAddIcon;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			lock (_syncRoot)
			{
				e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

				IList<string> texts = Texts;
				IList<int> widths = Widths;
				IList<Bitmap> icons = Icons;

				if (DrawBottomBorder)
				{
					using Pen pen = new Pen(ColorTabBorder);
					e.Graphics.DrawLine(pen, 0, Height - 1, Width - 1, Height - 1);
				}

				if (IsDragging())
				{
					texts = texts.Reorder(DraggingIndex.Value, _draggingOverIndex.Value);
					widths = widths.Reorder(DraggingIndex.Value, _draggingOverIndex.Value);
					icons = icons.Reorder(DraggingIndex.Value, _draggingOverIndex.Value);

					for (int i = widths.Count - 1; i >= 0; i--)
					{
						if (isDraggedOver(i))
							continue;

						paintTab(i, e.Graphics, texts, widths, icons);
					}

					paintTab(_draggingOverIndex.Value, e.Graphics, texts, widths, icons);
				}
				else
				{
					if (AllowAddingTabs)
						paintTab(widths.Count, e.Graphics, texts, widths, icons);

					for (int i = widths.Count - 1; i >= 0; i--)
					{
						if (isSelected(i))
							continue;

						paintTab(i, e.Graphics, texts, widths, icons);
					}

					if (SelectedIndex >= 0)
						paintTab(SelectedIndex, e.Graphics, texts, widths, icons);
				}
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e) =>
			this.PaintPanelBack(e.Graphics, e.ClipRectangle, this.BackgroundImage, this.BackColor, PaintBackground);

		public void ApplyOrderFrom(TabHeaderControl other)
		{
			lock (_syncRoot)
			{
				var indices = TabIds.Select(id => other.TabIds.IndexOf(id))
					.ToArray();

				if (indices.Any(i => i < 0 || i >= Count))
					return;

				Texts = Enumerable.Range(0, Count).Select(i => Texts[indices[i]]).ToList();
				TabIds = Enumerable.Range(0, Count).Select(i => TabIds[indices[i]]).ToList();
				Widths = Enumerable.Range(0, Count).Select(i => Widths[indices[i]]).ToList();
				Icons = Enumerable.Range(0, Count).Select(i => Icons[indices[i]]).ToList();
				Tags = Enumerable.Range(0, Count).Select(i => Tags[indices[i]]).ToList();
			}

			Invalidate();
			TabReordered?.Invoke(this);
		}

		public void SetTabSettings(IDictionary<object, TabSettings> settingsByTabId)
		{
			lock (_syncRoot)
			{
				for (int i = 0; i < Count; i++)
				{
					if (TabIds[i] == null)
						continue;

					if (settingsByTabId.TryGetValue(TabIds[i], out var setting))
					{
						if (setting.HasText)
							Texts[i] = setting.Text;

						if (setting.HasIcon)
							Icons[i] = setting.Icon;
					}
				}

				recalculateWidths();
			}

			OnLayout(new LayoutEventArgs(this, null));
			Invalidate();
		}

		public void RecalculateWidths()
		{
			lock (_syncRoot)
				recalculateWidths();

			OnLayout(new LayoutEventArgs(this, null));
			Invalidate();
		}

		private void recalculateWidths()
		{
			for (int i = 0; i < Count; i++)
				Widths[i] = getTabWidth(Texts[i], Icons[i], CloseIcon);
		}

		public void SetTabSetting(object tabId, TabSettings settings)
		{
			lock (_syncRoot)
			{
				for (int i = 0; i < Count; i++)
					if (Equals(TabIds[i], tabId))
					{
						if (settings.HasText)
							Texts[i] = settings.Text;

						if (settings.HasIcon)
							Icons[i] = settings.Icon;

						Widths[i] = getTabWidth(Texts[i], Icons[i], CloseIcon);

						Tags[i] = settings.Tag;
					}
			}

			OnLayout(new LayoutEventArgs(this, null));
			Invalidate();
		}

		public TabSettings GetTabSetting(int i)
		{
			if (i < 0 || i >= Count)
				return null;

			return new TabSettings(Texts[i], Icons[i])
			{
				TabId = TabIds[i],
				Tag = Tags[i]
			};
		}

		private void clearTabs()
		{
			lock (_syncRoot)
			{
				TabIds = new List<object>();
				Texts = new List<string>();
				Widths = new List<int>();
				Icons = new List<Bitmap>();
				Tags = new List<object>();
			}
		}

		private void setIds(List<object> tabIds)
		{
			clearTabs();

			for (int i = 0; i < tabIds.Count; i++)
				AddTab(tabIds[i], GetDefaultText(i), _defaultIcon, insertToTheLeft: false);

			OnLayout(new LayoutEventArgs(this, null));
			Invalidate();
		}


		public void RemoveTab(int index)
		{
			TabRemoving?.Invoke(this, index);

			lock (_syncRoot)
			{
				Texts.RemoveAt(index);
				TabIds.RemoveAt(index);
				Widths.RemoveAt(index);
				Icons.RemoveAt(index);
				Tags.RemoveAt(index);

				var selectedIndex = SelectedIndex >= index
					? SelectedIndex - 1
					: SelectedIndex;

				selectedIndex = getValidIndex(selectedIndex);
				setSelectedIndex(selectedIndex);
			}

			OnLayout(new LayoutEventArgs(this, null));
			Invalidate();

			TabRemoved?.Invoke(this);
		}

		public void AddTab(object tabId = null, string tabText = null, Bitmap icon = null, object tag = null, bool? insertToTheLeft = null, bool select = true)
		{
			lock (_syncRoot)
			{
				tabText ??= GetDefaultText(Count);
				var tabIcon = icon ?? DefaultIcon;
				int width = getTabWidth(tabText, tabIcon, CloseIcon);

				TabIds.Add(tabId);
				Texts.Add(tabText);
				Icons.Add(tabIcon);
				Widths.Add(width);
				Tags.Add(tag);
			}

			OnLayout(new LayoutEventArgs(this, null));
			Invalidate();

			int index = Count - 1;

			TabAdded?.Invoke(this, index);

			if (select)
				SelectedIndex = index;

			if ((insertToTheLeft ?? AddNewTabsToTheLeft) && index != 0)
				RelocateTab(index, 0, selectRelocated: select);
		}

		public void RelocateTab(int fromIndex, int toIndex, bool selectRelocated)
		{
			object selectedId = TabIds[SelectedIndex];

			if (fromIndex != toIndex && toIndex >= 0 && toIndex < Count)
				lock (_syncRoot)
				{
					Texts = Texts.Reorder(fromIndex, toIndex);
					TabIds = TabIds.Reorder(fromIndex, toIndex);
					Widths = Widths.Reorder(fromIndex, toIndex);
					Icons = Icons.Reorder(fromIndex, toIndex);
					Tags = Tags.Reorder(fromIndex, toIndex);
				}

			if (selectRelocated)
				SelectedIndex = toIndex;
			else
				SelectedIndex = TabIds.IndexOf(selectedId);

			TabReordered?.Invoke(this);
		}



		private void mouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			if (IsDragging() || isReadyToDrag())
			{
				AbortDrag();
				return;
			}

			GetTabIndex(e.Location, out int hoveredIndex, out bool hoveredClose);

			if (hoveredIndex < 0 || hoveredIndex >= Count || hoveredClose)
			{
				AbortDrag();
				return;
			}

			if (AllowReorderTabs)
				getReadyToDrag(e.Location.X, hoveredIndex);
		}

		private void mouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			if (IsDragging())
			{
				int draggingIndex = DraggingIndex.Value;
				_dragCurrentX = e.Location.X;
				int draggingOverIndex =
					getDraggingOverIndex(_dragCurrentX.Value, _dragStartedX.Value, draggingIndex);

				RelocateTab(draggingIndex, draggingOverIndex, selectRelocated: true);

				AbortDrag();
				return;
			}

			if (isReadyToDrag())
				AbortDrag();
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			HoveredIndex = -1;
			HoveredCloseIndex = -1;
		}

		private void mouseClick(object sender, MouseEventArgs e)
		{
			if (IsDragging())
				return;

			GetTabIndex(e.Location, out int hoveredIndex, out bool hoveredClose);

			if (e.Button == MouseButtons.Left)
			{
				if (hoveredClose)
					RemoveTab(hoveredIndex);
				else if (hoveredIndex >= 0 && hoveredIndex < Count)
					SelectedIndex = hoveredIndex;
				else if (hoveredIndex == Count)
					AddTab(null, GetDefaultText(Count));
			}
			else if (e.Button == MouseButtons.Middle && AllowRemovingTabs)
			{
				if (hoveredIndex >= 0 && hoveredIndex < Count)
					RemoveTab(hoveredIndex);
			}
		}

		private void mouseMove(object sender, MouseEventArgs e)
		{
			if (isReadyToDrag())
				beginDrag(e.X);
			else if (IsDragging())
			{
				_dragCurrentX = e.X;
				_draggingOverIndex =
					getDraggingOverIndex(_dragCurrentX.Value, _dragStartedX.Value, DraggingIndex.Value);

				Invalidate();
			}
			else
			{
				GetTabIndex(e.Location, out int hoveredIndex, out bool hoveredClose);
				HoveredIndex = hoveredIndex;
				HoveredCloseIndex = hoveredClose ? hoveredIndex : -1;
			}
		}



		private int getDraggingOverIndex(int dragCurrentX, int dragStartedX, int draggingIndex)
		{
			int delta = dragCurrentX - dragStartedX;

			IEnumerable<int> widths;
			int deltaAbs = Math.Abs(delta);
			int sign = Math.Sign(delta);

			int passedCount = 0;

			if (delta > 0)
				widths = Widths.Skip(draggingIndex + 1);
			else if (delta < 0)
				widths = Widths.Take(draggingIndex).Reverse();
			else
				widths = Enumerable.Empty<int>();

			foreach (int width in widths)
			{
				int totalWidth = width + SlopeSize.Width;
				if (totalWidth <= deltaAbs)
				{
					passedCount += sign;
					deltaAbs -= totalWidth;
				}
				else
					break;
			}

			var tabIndex = draggingIndex + passedCount;
			return tabIndex;
		}

		private void getReadyToDrag(int x, int index)
		{
			_dragStartedX = null;
			_dragCurrentX = x;

			DraggingIndex = index;
			_draggingOverIndex = null;
		}

		private bool isReadyToDrag()
		{
			return _dragCurrentX.HasValue && !_dragStartedX.HasValue;
		}

		public void BeginDrag(int index)
		{
			var position = Cursor.Position;
			var clientPosition = PointToClient(position);

			var tabPolygon = getTabPolygon(index, Widths);
			var middleX = (tabPolygon[0].X + tabPolygon[tabPolygon.Length - 1].X) / 2;

			getReadyToDrag(middleX, index);
			beginDrag(clientPosition.X);
		}

		private void beginDrag(int x)
		{
			_dragStartedX = _dragCurrentX;
			_dragCurrentX = x;

			SelectedIndex = HoveredIndex = DraggingIndex.Value;
			_draggingOverIndex =
				getDraggingOverIndex(_dragCurrentX.Value, _dragStartedX.Value, DraggingIndex.Value);
		}

		public bool IsDragging() =>
			_draggingOverIndex.HasValue;

		public void AbortDrag()
		{
			_draggingOverIndex = null;

			_dragStartedX =
				_dragCurrentX = null;

			DraggingIndex = null;

			Invalidate();
		}

		public void GetTabIndex(Point location, out int hoveredIndex, out bool hoveredClose)
		{
			hoveredIndex = -1;
			hoveredClose = false;

			if (SelectedIndex > 0)
				if (isTabHit(SelectedIndex, location, out hoveredClose))
				{
					hoveredIndex = SelectedIndex;
					return;
				}

			for (int i = 0; i < Count; i++)
				if (isTabHit(i, location, out hoveredClose))
				{
					hoveredIndex = i;
					return;
				}

			if (AllowAddingTabs)
				if (isTabHit(Count, location, out hoveredClose))
					hoveredIndex = Count;
		}

		public Point[] GetTabPolygon(int index) =>
			getTabPolygon(index, Widths);

		private bool isTabHit(int i, Point location, out bool closeHovered)
		{
			closeHovered = false;
			var points = getTabPolygon(i, Widths);
			var result = points.ContainsPoint(location);

			var closeIcon = getCloseIcon(i);

			if (i != AddButtonIndex && AllowRemovingTabs && closeIcon != null)
			{
				int left = points[1].X;
				var icon = Icons[i];
				int iconWidth = icon?.Width ?? 0;
				var text = Texts[i];

				var textSize = !string.IsNullOrEmpty(text)
					? measureText(text)
					: new Size(0, 0);

				int closeLeft = left + iconWidth + TextPadding + textSize.Width + TextPadding;
				var closeTop = Height - (closeIcon.Height + SlopeSize.Height) / 2;
				var closeBounds = new Rectangle(new Point(closeLeft, closeTop), closeIcon.Size);

				closeHovered = closeBounds.Contains(location);
			}

			return result;
		}

		private void layout(object sender, LayoutEventArgs e)
		{
			var width = SlopeSize.Width * (Count + 1) + Widths.Sum();

			if (AllowAddingTabs)
				width = width - SlopeSize.Width + 2 * AddButtonSlopeSize.Width + AddButtonWidth;

			Width = width;
		}


		private void paintTab(int i, Graphics g, IList<string> texts, IList<int> widths, IList<Bitmap> icons)
		{
			var color = getTabColor(i);
			var closeIcon = getCloseIcon(i);
			var points = getTabPolygon(i, widths);

			using (var brush = new SolidBrush(color))
			{
				using var pen = new Pen(ColorTabBorder, TabBorderWidth);
				g.FillPolygon(brush, points);
				g.DrawPolygon(pen, points);

				if (DrawBottomBorder && !isDraggedOver(i) && !isSelected(i))
				{
					var bottomBorderPoints = new[]
					{
						new Point(points[0].X, points[0].Y - 1),
						new Point(points[3].X, points[3].Y - 1)
					};

					g.DrawLine(pen, bottomBorderPoints[0], bottomBorderPoints[1]);
				}
			}

			if (i != AddButtonIndex)
				paintTabElements(g, points, icons[i], texts[i], closeIcon);
			else
				paintAddButtonElements(g, points, AddIcon);
		}

		private void paintTabElements(Graphics g, Point[] points, Bitmap icon, string text, Bitmap closeIcon)
		{
			int left = points[1].X;

			if (icon != null)
			{
				var iconTop = Height - (icon.Height + SlopeSize.Height) / 2;
				g.DrawImage(icon, new Rectangle(left, iconTop, icon.Width, icon.Height));
			}

			int iconWidth = icon?.Width ?? 0;

			var textSize = !string.IsNullOrEmpty(text)
				? g.MeasureText(text, Font)
				: new Size(0, 0);

			if (!string.IsNullOrEmpty(text))
			{
				int textLeft = left + TextPadding + iconWidth;
				var textTop = Height - (textSize.Height + SlopeSize.Height) / 2;
				var textBounds = new Rectangle(new Point(textLeft, textTop), textSize);

				g.DrawText(text, Font, textBounds, ForeColor);
			}

			if (closeIcon != null)
			{
				int closeLeft = left + iconWidth + TextPadding + textSize.Width + TextPadding;
				var closeTop = Height - (closeIcon.Height + SlopeSize.Height) / 2;
				g.DrawImage(closeIcon, new Rectangle(closeLeft, closeTop, closeIcon.Width, closeIcon.Height));
			}
		}

		private void paintAddButtonElements(Graphics g, Point[] points, Bitmap addIcon)
		{
			if (addIcon == null)
				return;

			var left = points[2].X - addIcon.Width;
			var top = Height - (addIcon.Height + AddButtonSlopeSize.Height) / 2;

			g.DrawImage(addIcon, new Rectangle(left, top, addIcon.Width, addIcon.Height));
		}

		private Bitmap getCloseIcon(int i)
		{
			if (!AllowRemovingTabs)
				return null;

			if (IsDragging())
				return null;

			if (HoveredCloseIndex == i)
				return CloseIconHovered;

			return CloseIcon;
		}

		private Point[] getTabPolygon(int i, IList<int> widths)
		{
			if (i < widths.Count)
				return getTabPolygon(i, SlopeSize, widths[i], widths, AddButtonSlopeSize);

			if (i == widths.Count)
				return getTabPolygon(i, SlopeSize, AddButtonWidth, widths, AddButtonSlopeSize);

			throw new ArgumentOutOfRangeException();
		}

		private Point[] getTabPolygon(
			int i, Size slopeSize, int width, IList<int> widths, Size addButtonSlopeSize)
		{
			int x1;
			if (!isDraggedOver(i))
				x1 = getTabLeft(i, slopeSize, widths);
			else
				x1 = getTabLeft(DraggingIndex.Value, slopeSize, Widths) + _dragCurrentX.Value -
					_dragStartedX.Value;

			int x2;
			if (i < widths.Count)
				x2 = x1 + slopeSize.Width;
			else
				x2 = x1 + addButtonSlopeSize.Width;

			int x3 = x2 + width;

			if (i == widths.Count && i > 0)
			{
				x1 -= slopeSize.Width;
				x2 -= slopeSize.Width;
			}

			int x4;
			if (i < widths.Count)
				x4 = x3 + slopeSize.Width;
			else
				x4 = x3 + addButtonSlopeSize.Width;

			int xDelta = x4 - (Width - 1);
			if (xDelta > 0)
			{
				x1 -= xDelta;
				x2 -= xDelta;
				x3 -= xDelta;
				x4 -= xDelta;
			}

			xDelta = x1;
			if (xDelta < 0)
			{
				x1 -= xDelta;
				x2 -= xDelta;
				x3 -= xDelta;
				x4 -= xDelta;
			}

			int bottom = Height - 1 + TabBorderWidth;

			int top;
			if (i < widths.Count)
				top = bottom - slopeSize.Height;
			else
				top = bottom - addButtonSlopeSize.Height;

			var points = new[]
			{
				new Point(x1, bottom),
				new Point(x2, top),
				new Point(x3, top),
				new Point(x4, bottom)
			};

			return points;
		}

		private bool isSelected(int i)
		{
			return !IsDragging() && i == SelectedIndex;
		}

		private bool isDraggedOver(int i)
		{
			return IsDragging() && i == _draggingOverIndex.Value;
		}

		private static int getTabLeft(int i, Size slopeSize, IList<int> widths)
		{
			return slopeSize.Width * i + widths.Take(i).Sum();
		}

		private Color getTabColor(int i)
		{
			if (isDraggedOver(i))
				return ColorSelectedHovered;

			if (IsDragging())
				return ColorUnselected;

			if (isSelected(i))
			{
				if (i == HoveredIndex)
					return ColorSelectedHovered;

				return ColorSelected;
			}

			if (i == HoveredIndex)
				return ColorUnselectedHovered;

			return ColorUnselected;
		}

		private int getTabWidth(string tabText, Image icon, Image closeIcon)
		{
			return measureText(tabText).Width + TextPadding * 2 + (icon?.Width ?? 0) +
				Convert.ToInt32(AllowRemovingTabs) * (closeIcon?.Width ?? 0);
		}

		private Size measureText(string text) =>
			_graphics.MeasureText(text, Font);

		public string GetDefaultText(int i)
		{
			return (i + 1).ToString(Str.Culture);
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			if (IsLayoutSuspended())
				return;

			base.OnLayout(e);
		}

		public new void ResumeLayout()
		{
			base.ResumeLayout();
			OnLayout(new LayoutEventArgs(this, null));
		}

		protected bool IsLayoutSuspended()
		{
			return Runtime.IsMono
				? (int) _layoutSuspendedMonoField.GetValue(this) > 0
				: (bool) _layoutSuspendedProperty.GetValue(this, null);
		}

		private int getValidIndex(int value)
		{
			if (value >= Count)
				value = Count - 1;

			else if (value < 0)
				value = 0;

			return value;
		}

		private void setSelectedIndex(int value)
		{
			SelectedIndexChanging?.Invoke(this, _selectedIndex);
			_selectedIndex = value;
			Invalidate();
			SelectedIndexChanged?.Invoke(this, value);
		}



		private Size _slopeSize = new Size(15, 21);
		[Category("Settings"), DefaultValue(typeof(Size), "15, 21")]
		public Size SlopeSize
		{
			get => _slopeSize;
			set
			{
				_slopeSize = value;
				OnLayout(new LayoutEventArgs(this, nameof(SlopeSize)));
			}
		}

		private Size _addButtonSlopeSize = new Size(10, 14);
		[Category("Settings"), DefaultValue(typeof(Size), "10, 14")]
		public Size AddButtonSlopeSize
		{
			get => _addButtonSlopeSize;
			set
			{
				_addButtonSlopeSize = value;
				OnLayout(new LayoutEventArgs(this, nameof(AddButtonSlopeSize)));
			}
		}

		private int _addButtonWidth = 24;
		[Category("Settings"), DefaultValue(24)]
		public int AddButtonWidth
		{
			get => _addButtonWidth;
			set
			{
				_addButtonWidth = value;
				OnLayout(new LayoutEventArgs(this, nameof(AddButtonWidth)));
			}
		}

		private int _selectedIndex = -1;
		[Category("Settings"), DefaultValue(-1)]
		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{
				value = getValidIndex(value);

				if (_selectedIndex != value)
					setSelectedIndex(value);
			}
		}

		private int _hoveredIndex = -1;
		[Category("Settings"), DefaultValue(-1)]
		public int HoveredIndex
		{
			get => _hoveredIndex;
			private set
			{
				if (_hoveredIndex != value)
				{
					_hoveredIndex = value;
					Invalidate();
					HoveredIndexChanged?.Invoke(this, value);
				}
			}
		}

		private int _hoveredCloseIndex = -1;
		[Category("Settings"), DefaultValue(-1)]
		public int HoveredCloseIndex
		{
			get => _hoveredCloseIndex;
			private set
			{
				if (_hoveredCloseIndex != value)
				{
					_hoveredCloseIndex = value;
					Invalidate();
				}
			}
		}

		private Color _colorUnselected = SystemColors.InactiveCaption;
		[Category("Settings"), DefaultValue(typeof(Color), "InactiveCaption")]
		public Color ColorUnselected
		{
			get => _colorUnselected;
			set
			{
				_colorUnselected = value;
				Invalidate();
			}
		}

		private Color _colorUnselectedHovered = SystemColors.GradientInactiveCaption;
		[Category("Settings"), DefaultValue(typeof(Color), "GradientInactiveCaption")]
		public Color ColorUnselectedHovered
		{
			get => _colorUnselectedHovered;
			set
			{
				_colorUnselectedHovered = value;
				Invalidate();
			}
		}

		private Color _colorSelected = SystemColors.Control;
		[Category("Settings"), DefaultValue(typeof(Color), "Control")]
		public Color ColorSelected
		{
			get => _colorSelected;
			set
			{
				_colorSelected = value;
				Invalidate();
			}
		}

		private Color _colorSelectedHovered = SystemColors.Control;
		[Category("Settings"), DefaultValue(typeof(Color), "Control")]
		public Color ColorSelectedHovered
		{
			get => _colorSelectedHovered;
			set
			{
				_colorSelectedHovered = value;
				Invalidate();
			}
		}

		private Color _colorTabBorder = SystemColors.ActiveBorder;
		[Category("Settings"), DefaultValue(typeof(Color), "ActiveBorder")]
		public Color ColorTabBorder
		{
			get => _colorTabBorder;
			set
			{
				_colorTabBorder = value;
				Invalidate();
			}
		}

		private int _tabBorderWidth = 1;
		[Category("Settings"), DefaultValue(1)]
		public int TabBorderWidth
		{
			get => _tabBorderWidth;
			[UsedImplicitly]
			set
			{
				_tabBorderWidth = value;
				Invalidate();
			}
		}

		private bool _allowAddingTabs = true;
		[Category("Settings"), DefaultValue(true)]
		public bool AllowAddingTabs
		{
			get => _allowAddingTabs;
			set
			{
				_allowAddingTabs = value;
				OnLayout(new LayoutEventArgs(this, nameof(AllowAddingTabs)));
				Invalidate();
			}
		}

		private bool _allowRemovingTabs = true;
		[Category("Settings"), DefaultValue(true)]
		public bool AllowRemovingTabs
		{
			get => _allowRemovingTabs;
			set
			{
				_allowRemovingTabs = value;
				OnLayout(new LayoutEventArgs(this, nameof(AllowRemovingTabs)));
				Invalidate();
			}
		}

		[Category("Settings"), DefaultValue(true)]
		public bool AllowReorderTabs { get; set; } = true;

		private int _textPadding = 6;
		[Category("Settings"), DefaultValue(6)]
		public int TextPadding
		{
			get => _textPadding;
			set
			{
				if (_selectedIndex != value)
				{
					_textPadding = value;
					OnLayout(new LayoutEventArgs(this, nameof(TextPadding)));
					Invalidate();
				}
			}
		}

		[Category("Settings"), DefaultValue(0)]
		public int Count
		{
			get => TabIds.Count;
			set => setIds(Enumerable.Range(0, value).Cast<object>().ToList());
		}

		private bool _drawBottomBorder;
		[Category("Settings"), DefaultValue(false)]
		public bool DrawBottomBorder
		{
			get => _drawBottomBorder;
			set
			{
				_drawBottomBorder = value;
				Invalidate();
			}
		}

		private Bitmap _closeIcon;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap CloseIcon
		{
			get => DesignMode && _closeIcon == _defaultCloseIcon ? null : _closeIcon;
			set
			{
				_closeIcon = value ?? _defaultCloseIcon;
				OnLayout(new LayoutEventArgs(this, nameof(CloseIcon)));
				Invalidate();
			}
		}

		private Bitmap _closeIconHovered;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap CloseIconHovered
		{
			get => DesignMode && _closeIconHovered == _defaultCloseIconHovered ? null : _closeIconHovered;
			set => _closeIconHovered = value ?? _defaultCloseIconHovered;
		}

		private Bitmap _addIcon;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap AddIcon
		{
			get => DesignMode && _addIcon == _defaultAddIcon ? null : _addIcon;
			set
			{
				_addIcon = value ?? _defaultAddIcon;
				Invalidate();
			}
		}

		private Bitmap _defaultIcon;
		[Category("Settings"), DefaultValue(null)]
		public Bitmap DefaultIcon
		{
			get => _defaultIcon;
			set
			{
				_defaultIcon = value;
				OnLayout(new LayoutEventArgs(this, nameof(DefaultIcon)));
				Invalidate();
			}
		}

		[Category("Settings"), DefaultValue(false)]
		public bool AddNewTabsToTheLeft { get; [UsedImplicitly] set; }

		[Category("Settings"), DefaultValue(true)]
		public bool PaintBackground { get; set; } = true;

		[Browsable(false)]
		public List<object> TabIds { get; private set; }

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object SelectedTabId
		{
			get
			{
				if (SelectedIndex < 0 || SelectedIndex >= Count)
					return null;

				return TabIds[SelectedIndex];
			}
			set => SelectedIndex = TabIds.IndexOf(value);
		}

		[Browsable(false)]
		public object HoveredTabId
		{
			get
			{
				if (HoveredIndex < 0 || HoveredIndex >= Count)
					return null;

				return TabIds[HoveredIndex];
			}
		}

		[Browsable(false)]
		public int AddButtonIndex => Count;



		private List<int> Widths { get; set; }
		private List<string> Texts { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		private List<Bitmap> Icons { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<object> Tags { get; set; }

		public int? DraggingIndex { get; private set; }

		private int? _draggingOverIndex;
		private int? _dragStartedX;
		private int? _dragCurrentX;

		private readonly Graphics _graphics;

		private static readonly PropertyInfo _layoutSuspendedProperty =
			typeof(Control).GetProperty("IsLayoutSuspended", BindingFlags.NonPublic | BindingFlags.Instance);

		private static readonly FieldInfo _layoutSuspendedMonoField =
			typeof(Control).GetField("layout_suspended", BindingFlags.NonPublic | BindingFlags.Instance);

		private static readonly Bitmap _defaultCloseIconHovered = Resources.close_tab_hovered_32;
		private static readonly Bitmap _defaultCloseIcon = Resources.close_tab_32;
		private static readonly Bitmap _defaultAddIcon = Resources.add_tab_32;
	}
}
