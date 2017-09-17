using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public partial class TabHeaderControl : UserControl
	{
		private readonly object _syncRoot = new object();

		public event Action<TabHeaderControl, int> SelectedIndexChanging;
		public event Action<TabHeaderControl, int> SelectedIndexChanged;
		public event Action<TabHeaderControl, int> TabAdded;
		public event Action<TabHeaderControl, int> TabRemoving;
		public event Action<TabHeaderControl> TabRemoved;
		public event Action<TabHeaderControl> TabReordered;

		public TabHeaderControl()
		{
			InitializeComponent();

			clearTabs();
			
			Paint += paint;
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
		}

		public void ApplyOrderFrom(TabHeaderControl other)
		{
			lock (_syncRoot)
			{
				var indices = TabIds.Select(id => other.TabIds.IndexOf(id))
					.ToArray();

				if (indices.Any(i=>i < 0 || i >= Count))
					return;

				Texts = Enumerable.Range(0, Count).Select(i => Texts[indices[i]]).ToList();
				TabIds = Enumerable.Range(0, Count).Select(i => TabIds[indices[i]]).ToList();
				Widths = Enumerable.Range(0, Count).Select(i => Widths[indices[i]]).ToList();
				Icons = Enumerable.Range(0, Count).Select(i => Icons[indices[i]]).ToList();
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

					TabSettings setting;
					if (settingsByTabId.TryGetValue(TabIds[i], out setting))
					{
						if (setting.HasText)
							Texts[i] = setting.Text;

						if (setting.HasIcon)
							Icons[i] = setting.Icon;
					}

					Widths[i] = getTabWidth(Texts[i], Icons[i], CloseIcon);
				}
			}

			OnLayout(new LayoutEventArgs(this, null));
			Invalidate();
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
				TabId = TabIds[i]
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
			}
		}

		private void setIds(List<object> tabIds)
		{
			clearTabs();

			for (int i = 0; i < tabIds.Count; i++)
				AddTab(tabIds[i], GetDefaultText(i), _defaultIcon);

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

				if (SelectedIndex >= index)
					SelectedIndex--;
			}

			OnLayout(new LayoutEventArgs(this, null));
			Invalidate();

			TabRemoved?.Invoke(this);
		}

		public void AddTab(object tabId = null, string tabText = null, Bitmap icon = null)
		{
			lock (_syncRoot)
			{
				tabText = tabText ?? GetDefaultText(Count);
				var tabIcon = icon ?? DefaultIcon;

				TabIds.Add(tabId);
				Texts.Add(tabText);
				Icons.Add(tabIcon);
				Widths.Add(getTabWidth(tabText, tabIcon, CloseIcon));
			}

			OnLayout(new LayoutEventArgs(this, null));
			Invalidate();

			TabAdded?.Invoke(this, Count - 1);
			SelectedIndex = Count - 1;
		}

		public void RelocateTab(int fromIndex, int toIndex, bool selectRelocated)
		{
			if (fromIndex != toIndex && toIndex >= 0 && toIndex < Count)
				lock (_syncRoot)
				{
					Texts = Texts.Reorder(fromIndex, toIndex);
					TabIds = TabIds.Reorder(fromIndex, toIndex);
					Widths = Widths.Reorder(fromIndex, toIndex);
					Icons = Icons.Reorder(fromIndex, toIndex);
				}

			if (selectRelocated)
				SelectedIndex = toIndex;

			TabReordered?.Invoke(this);
		}



		private void mouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			if (IsDragging() || isReadyToDrag())
			{
				abortDrag();
				return;
			}

			int hoveredIndex;
			bool hoveredClose;
			getTabIndex(e.Location, out hoveredIndex, out hoveredClose);

			if (hoveredIndex < 0 || hoveredIndex >= Count || hoveredClose)
			{
				abortDrag();
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
				int draggingIndex = _draggingIndex.Value;
				_dragCurrentX = e.Location.X;
				int draggingOverIndex = getDraggingOverIndex(_dragCurrentX.Value, _dragStartedX.Value, draggingIndex);

				RelocateTab(draggingIndex, draggingOverIndex, selectRelocated: true);

				abortDrag();
				return;
			}

			if (isReadyToDrag())
				abortDrag();
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

			int hoveredIndex;
			bool hoveredClose;
			getTabIndex(e.Location, out hoveredIndex, out hoveredClose);

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
				_draggingOverIndex = getDraggingOverIndex(_dragCurrentX.Value, _dragStartedX.Value, _draggingIndex.Value);
				Invalidate();
			}
			else
			{
				int hoveredIndex;
				bool hoveredClose;
				getTabIndex(e.Location, out hoveredIndex, out hoveredClose);
				HoveredIndex = hoveredIndex;
				HoveredCloseIndex = hoveredClose ? hoveredIndex : -1;
			}
		}

		private void paint(object sender, PaintEventArgs e)
		{
			lock (_syncRoot)
			{
				e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				e.Graphics.Clear(BackColor);

				IList<string> texts = Texts;
				IList<int> widths = Widths;
				IList<Bitmap> icons = Icons;

				if (DrawBottomBorder)
					e.Graphics.DrawLine(new Pen(ColorTabBorder), 0, Height - 1, Width - 1, Height - 1);

				if (IsDragging())
				{
					texts = texts.Reorder(_draggingIndex.Value, _draggingOverIndex.Value);
					widths = widths.Reorder(_draggingIndex.Value, _draggingOverIndex.Value);
					icons = icons.Reorder(_draggingIndex.Value, _draggingOverIndex.Value);

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



		private int getDraggingOverIndex(int dragCurrentX, int dragStartedX, int draggingIndex)
		{
			int delta = dragCurrentX - dragStartedX;

			IEnumerable<int> widths;
			int deltaAbs = Math.Abs(delta);
			int signum = Math.Sign(delta);

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
					passedCount += signum;
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

			_draggingIndex = index;
			_draggingOverIndex = null;
		}

		private bool isReadyToDrag()
		{
			return _dragCurrentX.HasValue && !_dragStartedX.HasValue;
		}

		private void beginDrag(int x)
		{
			_dragStartedX = _dragCurrentX;
			_dragCurrentX = x;

			SelectedIndex = HoveredIndex = _draggingIndex.Value;
			_draggingOverIndex = getDraggingOverIndex(_dragCurrentX.Value, _dragStartedX.Value, _draggingIndex.Value);
		}

		public bool IsDragging()
		{
			return _dragStartedX.HasValue;
		}

		private void abortDrag()
		{
			_dragStartedX =
			_dragCurrentX = null;

			_draggingIndex = null;
			_draggingOverIndex = null;

			Invalidate();
		}

		private void getTabIndex(Point location, out int hoveredIndex, out bool hoveredClose)
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
			var width = SlopeSize.Width*(Count + 1) + Widths.Sum();

			if (AllowAddingTabs)
				width = width - SlopeSize.Width + 2*AddButtonSlopeSize.Width + AddButtonWidth;

			Width = width;
		}

		
		private void paintTab(int i, Graphics g, IList<string> texts, IList<int> widths, IList<Bitmap> icons)
		{
			var color = getTabColor(i);
			var closeIcon = getCloseIcon(i);
			var points = getTabPolygon(i, widths);

			var brush = new SolidBrush(color);
			var pen = new Pen(ColorTabBorder, TabBorderWidth);

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
				var iconTop = Height - (icon.Height + SlopeSize.Height)/2;
				g.DrawImageUnscaled(icon, left, iconTop);
			}

			int iconWidth = icon?.Width ?? 0;

			var textSize = !string.IsNullOrEmpty(text) 
				? measureText(text)
				: new Size(0, 0);

			if (!string.IsNullOrEmpty(text))
			{
				int textLeft = left + TextPadding + iconWidth;
				var textTop = Height - (textSize.Height + SlopeSize.Height)/2;
				var textBounds = new Rectangle(new Point(textLeft, textTop), textSize);

				TextRenderer.DrawText(g, text, Font, textBounds, ForeColor, _textFormatFlags);
			}

			if (closeIcon != null)
			{
				int closeLeft = left + iconWidth + TextPadding + textSize.Width + TextPadding;
				var closeTop = Height - (closeIcon.Height + SlopeSize.Height)/2;
				g.DrawImageUnscaled(closeIcon, closeLeft, closeTop);
			}
		}

		private void paintAddButtonElements(Graphics g, Point[] points, Bitmap addIcon)
		{
			if (addIcon == null)
				return;

			var left = points[2].X - addIcon.Width;
			var top = Height - (addIcon.Height + AddButtonSlopeSize.Height) / 2;

			g.DrawImageUnscaled(addIcon, left, top);
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

		private Point[] getTabPolygon(int i, Size slopeSize, int width, IList<int> widths, Size addButtonSlopeSize)
		{
			int x1;
			if (!isDraggedOver(i))
				x1 = getTabLeft(i, slopeSize, widths);
			else
				x1 = getTabLeft(_draggingIndex.Value, slopeSize, Widths) + _dragCurrentX.Value - _dragStartedX.Value;

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
			return slopeSize.Width*i + widths.Take(i).Sum();
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
			return measureText(tabText).Width + TextPadding*2 + (icon?.Width ?? 0) + Convert.ToInt32(AllowRemovingTabs)*(closeIcon?.Width ?? 0);
		}

		private Size measureText(string text)
		{
			var image = new Bitmap(100, 100);
			var g = Graphics.FromImage(image);

			var size = TextRenderer.MeasureText(g, text, Font, new Size(1024, 100), _textFormatFlags);

			return size;
		}

		public string GetDefaultText(int i)
		{
			return $"Tab no.{i + 1}";
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
			var property = typeof(Control).GetProperty("IsLayoutSuspended", BindingFlags.NonPublic | BindingFlags.Instance);
			bool result = (bool) property.GetValue(this, null);
			return result;
		}


		[Category("Settings"), DefaultValue(typeof(Size), "15, 21")]
		public Size SlopeSize
		{
			get { return _slopeSize; }
			set
			{
				_slopeSize = value;
				OnLayout(new LayoutEventArgs(this, nameof(SlopeSize)));
			}
		}

		[Category("Settings"), DefaultValue(typeof(Size), "10, 14")]
		public Size AddButtonSlopeSize
		{
			get { return _addButtonSlopeSize; }
			set
			{
				_addButtonSlopeSize = value;
				OnLayout(new LayoutEventArgs(this, nameof(AddButtonSlopeSize)));
			}
		}

		[Category("Settings"), DefaultValue(30)]
		public int AddButtonWidth
		{
			get { return _addButtonWidth; }
			set
			{
				_addButtonWidth = value;
				OnLayout(new LayoutEventArgs(this, nameof(AddButtonWidth)));
			}
		}

		[Category("Settings"), DefaultValue(-1)]
		public int SelectedIndex
		{
			get { return _selectedIndex; }
			set
			{
				if (value >= Count)
					value = Count - 1;

				if (_selectedIndex != value)
				{
					SelectedIndexChanging?.Invoke(this, _selectedIndex);
					_selectedIndex = value;
					Invalidate();
					SelectedIndexChanged?.Invoke(this, value);
				}
			}
		}

		[Category("Settings"), DefaultValue(-1)]
		public int HoveredIndex
		{
			get { return _hoveredIndex; }
			private set
			{
				if (_hoveredIndex != value)
				{
					_hoveredIndex = value;
					Invalidate();
				}
			}
		}

		[Category("Settings"), DefaultValue(false)]
		public int HoveredCloseIndex
		{
			get { return _hoveredCloseIndex; }
			private set
			{
				if (_hoveredCloseIndex != value)
				{
					_hoveredCloseIndex = value;
					Invalidate();
				}
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Color), "232, 232, 232")]
		public Color ColorUnselected
		{
			get { return _colorUnselected; }
			set
			{
				_colorUnselected = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Color), "WhiteSmoke")]
		public Color ColorSelected
		{
			get { return _colorSelected; }
			set
			{
				_colorSelected = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Color), "WhiteSmoke")]
		public Color ColorUnselectedHovered
		{
			get { return _colorUnselectedHovered; }
			set
			{
				_colorUnselectedHovered = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Color), "White")]
		public Color ColorSelectedHovered
		{
			get { return _colorSelectedHovered; }
			set
			{
				_colorSelectedHovered = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color ColorTabBorder
		{
			get { return _colorTabBorder; }
			set
			{
				_colorTabBorder = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(1)]
		public int TabBorderWidth
		{
			get { return _tabBorderWidth; }
			set
			{
				_tabBorderWidth = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(true)]
		public bool AllowAddingTabs
		{
			get { return _allowAddingTabs; }
			set
			{
				_allowAddingTabs = value;
				OnLayout(new LayoutEventArgs(this, nameof(AllowAddingTabs)));
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(true)]
		public bool AllowRemovingTabs
		{
			get { return _allowRemovingTabs; }
			set
			{
				_allowRemovingTabs = value;
				OnLayout(new LayoutEventArgs(this, nameof(AllowRemovingTabs)));
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(true)]
		public bool AllowReorderTabs { get; set; } = true;

		[Category("Settings"), DefaultValue(6)]
		public int TextPadding
		{
			get { return _textPadding; }
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
			get { return TabIds.Count; }
			set { setIds(Enumerable.Range(0, value).Cast<object>().ToList()); }
		}

		[Category("Settings"), DefaultValue(false)]
		public bool DrawBottomBorder
		{
			get { return _drawBottomBorder; }
			set
			{
				_drawBottomBorder = value;
				Invalidate();
			}
		}

		[Category("Settings"), DefaultValue(null)]
		public Bitmap CloseIcon
		{
			get { return _closeIcon; }
			set
			{
				_closeIcon = value;
				OnLayout(new LayoutEventArgs(this, nameof(CloseIcon)));
				Invalidate();
			}
		}

		[Category("Settings"), DefaultValue(null)]
		public Bitmap CloseIconHovered { get; set; }

		[Category("Settings"), DefaultValue(null)]
		public Bitmap AddIcon
		{
			get { return _addIcon; }
			set
			{
				_addIcon = value;
				Invalidate();
			}
		}

		[Category("Settings"), DefaultValue(null)]
		public Bitmap DefaultIcon
		{
			get { return _defaultIcon; }
			set
			{
				_defaultIcon = value;
				OnLayout(new LayoutEventArgs(this, nameof(DefaultIcon)));
				Invalidate();
			}
		}



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
			set { SelectedIndex = TabIds.IndexOf(value); }
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
		public List<Bitmap> Icons { get; set; }


		private Size _slopeSize = new Size(15, 21);
		private Size _addButtonSlopeSize = new Size(10, 14);
		private int _addButtonWidth = 24;
		private int _selectedIndex = -1;
		private int _hoveredIndex = -1;
		private int _hoveredCloseIndex = -1;

		private Color _colorUnselected = Color.FromArgb(232, 232, 232);
		private Color _colorSelected = Color.WhiteSmoke;
		private Color _colorUnselectedHovered = Color.WhiteSmoke;
		private Color _colorSelectedHovered = Color.White;
		private Color _colorTabBorder = Color.DarkGray;
		private int _tabBorderWidth = 1;
		private int _textPadding = 6;
		private bool _allowAddingTabs = true;

		private int? _draggingIndex;
		private int? _draggingOverIndex;
		private int? _dragStartedX;
		private int? _dragCurrentX;
		private bool _drawBottomBorder;
		private Bitmap _closeIcon;
		private Bitmap _defaultIcon;
		private bool _allowRemovingTabs = true;
		private Bitmap _addIcon;

		private static readonly TextFormatFlags _textFormatFlags = new StringFormat(default(StringFormatFlags)).ToTextFormatFlags();
	}
}
