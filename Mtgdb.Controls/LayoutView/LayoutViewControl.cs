﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public partial class LayoutViewControl : UserControl, IMessageFilter
	{
		public event Action<object> CardIndexChanged;
		public event Action<object, CustomDrawArgs> CustomDrawField;
		public event Action<object, int> RowDataLoaded;
		public event Action<object> SortChanged;

		public LayoutViewControl()
		{
			InitializeComponent();

			Cards = new List<LayoutControl>();
			CardIndex = 0;

			LayoutControlType = typeof(LayoutControl);

			Load += load;
			Paint += paint;
			Layout += layout;
			Disposed += disposed;

			SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
		}

		private void layout(object sender, LayoutEventArgs e)
		{
			applySize();
			ApplyCardIndex();
			updateScrollbar();
		}

		private void paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			e.Graphics.Clear(BackColor);

			for (int i = 0; i < Cards.Count; i++)
			{
				var card = Cards[i];

				if (!card.Visible || card.DataSource == null)
					continue;

				var cardArea = new Rectangle(card.Location, card.Size);
				if (!e.ClipRectangle.IntersectsWith(cardArea))
					continue;
				
				e.Graphics.FillRectangle(new SolidBrush(BackColor), cardArea);
				
				foreach (var field in card.Fields)
				{
					var fieldArea = new Rectangle(card.Location.X + field.Location.X, card.Location.Y + field.Location.Y, field.Width, field.Height);

					if (!fieldArea.IntersectsWith(e.ClipRectangle))
						continue;

					if (field.IsHotTracked)
					{
						var area = fieldArea;
						e.Graphics.FillRectangle(new SolidBrush(HotTrackBackgroundColor), area);
						e.Graphics.DrawRectangle(new Pen(HotTrackBorderColor),
							new Rectangle(area.Location, new Size(area.Width - 1, area.Height - 1)));
					}

					var customFieldArgs = new CustomDrawArgs
					{
						RowHandle = getRowHandle(i),
						Graphics = e.Graphics,
						Bounds = fieldArea,
						FieldName = field.FieldName,
						Handled = false,
						Font = field.Font,
						ForeColor = field.ForeColor,
						DisplayText = field.Text,
						HAlignment = field.HorizontalAlignment,
					};

					CustomDrawField?.Invoke(this, customFieldArgs);

					if (!customFieldArgs.Handled)
						field.PaintSelf(e.Graphics, card.Location, card.HighlightSettings);

					if (field.IsSortVisible)
					{
						var sortIcon = SortOptions.GetSortIcon(field);
						if (sortIcon != null)
						{
							var sortIconBounds = SortOptions.GetSortButtonBounds(field, card);
							e.Graphics.DrawImageUnscaled(sortIcon, sortIconBounds.Location);
						}
					}
				}
			}
		}

		public void RefreshData()
		{
			_rowHandleByObject.Clear();
			for (int i = 0; i < DataSource.Count; i++)
				_rowHandleByObject[DataSource[i]] = i;

			if (CardIndex < 0 || CardIndex >= Count)
				CardIndex = snapCardIndex(CardIndex, Count);

			ApplyCardIndex();
			updateScrollbar();
		}

		private void updateScrollbar()
		{
			int pageSize = getRowsCount() * getColumnsCount();
			_scrollBar.Enabled = Count > pageSize;

			int largeChange = Math.Max(
				Math.Max(2, pageSize),
				(int) Math.Round(Math.Pow(10, Math.Round(Math.Log10(Count + 1d) - 2.5d))));
			int maximum = Math.Max(0, PageCount + largeChange - 2);
			int value = CardIndex / pageSize;

			_scrollBar.LargeChange = largeChange;
			_scrollBar.Maximum = maximum;
			_scrollBar.Value = value;
		}

		private void load(object sender, EventArgs e)
		{
			ApplyIconRecognizer();

			_scrollBar.Scroll += scrolled;
			MouseWheel += mouseWheel;
			KeyDown += keyDown;

			MouseMove += mouseMove;
			MouseClick += mouseClick;

			Application.AddMessageFilter(this);
		}

		private void disposed(object sender, EventArgs e)
		{
			Application.RemoveMessageFilter(this);
		}



		private LayoutControl createCard()
		{
			var result = (LayoutControl) Activator.CreateInstance(LayoutControlType);
			result.SetIconRecognizer(IconRecognizer);
			result.Size = ProbeCard.Size;

			var probeEnumerator = ProbeCard.Fields.GetEnumerator();
			var enumerator = result.Fields.GetEnumerator();

			while (probeEnumerator.MoveNext())
			{
				enumerator.MoveNext();

				var probeField = probeEnumerator.Current;
				var field = enumerator.Current;

				field.Location = probeField.Location;
				field.Size = probeField.Size;
				field.Font = probeField.Font;
				field.BackColor = probeField.BackColor;
				field.ForeColor = probeField.ForeColor;
				field.HorizontalAlignment = probeField.HorizontalAlignment;

				updateSort(field);
			}

			result.Invalid += cardInvalidated;
			return result;
		}

		private LayoutControl createProbeCard()
		{
			var result = (LayoutControl)Activator.CreateInstance(LayoutControlType);
			result.SetIconRecognizer(IconRecognizer);
			return result;
		}

		private void updateSort(FieldControl field)
		{
			int sortIndex;
			if (_sortIndexByField.TryGetValue(field.FieldName, out sortIndex))
				field.SortOrder = _sortInfos[sortIndex].SortOrder;
			else
				field.SortOrder = SortOrder.None;
		}

		private void cardInvalidated(LayoutControl layoutControl, FieldControl fieldControl)
		{
			var rect = fieldControl.Bounds;
			rect.Offset(layoutControl.Location);
			Invalidate(rect);
		}

		private int getColumnsCount() => CardLayoutUtil.GetVisibleCount(
			Width - _scrollBar.Width,
			CardSize.Width,
			CardInterval.Width,
			PartialCardsThreshold.Width,
			AllowPartialCards);

		private int getRowsCount() => CardLayoutUtil.GetVisibleCount(
			Height,
			CardSize.Height,
			CardInterval.Height,
			PartialCardsThreshold.Height,
			AllowPartialCards);

		private void scrolled(object sender, EventArgs e)
		{
			int pageSize = getRowsCount() * getColumnsCount();
			CardIndex = _scrollBar.Value * pageSize;
			ApplyCardIndex();
		}

		private void keyDown(object sender, KeyEventArgs e)
		{
			if (!isUnderMouse())
				return;

			e.SuppressKeyPress = true;
			e.Handled = true;

			if (e.KeyData == Keys.PageDown)
				scrollAdd(getRowsCount()*getColumnsCount());
			else if (e.KeyData == Keys.PageUp)
				scrollAdd(-(getRowsCount()*getColumnsCount()));
			else if (e.KeyData == Keys.End)
				scrollAdd(Count);
			else if (e.KeyData == Keys.Home)
				scrollAdd(-Count);
			else
			{
				e.SuppressKeyPress = false;
				e.Handled = false;
			}
		}

		private void mouseWheel(object sender, MouseEventArgs e)
		{
			if (!isUnderMouse())
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
			int columnsCount = getColumnsCount();
			int rowsCount = getRowsCount();
			for (int j = 0; j < rowsCount; j++)
				for (int i = 0; i < columnsCount; i++)
				{
					var location = getCardLocation(i, j);

					int index = getCardIndex(i, j, columnsCount);
					if (index == Cards.Count)
						Cards.Add(createCard());

					Cards[index].Location = location;
					Cards[index].Visible = Cards[index].DataSource != null;
				}

			for (int index = rowsCount * columnsCount; index < Cards.Count; index++)
				Cards[index].Visible = false;
		}



		private void mouseMove(object sender, MouseEventArgs e)
		{
			var location = e.Location;

			var prevHitInfo = _hitInfo;
			_hitInfo = CalcHitInfo(location);
			
			if (prevHitInfo?.Field != null && prevHitInfo.Field != _hitInfo.Field)
			{
				prevHitInfo.Field.IsHotTracked = false;
				prevHitInfo.Field.IsSortHotTracked = false;
			}

			if (_hitInfo.Field != null)
			{
				_hitInfo.Field.IsHotTracked = true;
				_hitInfo.Field.IsSortHotTracked = _hitInfo.IsSortButton;
			}

			if (prevHitInfo?.Card != null && prevHitInfo.Card != _hitInfo.Card)
				foreach (var field in prevHitInfo.Card.Fields)
					field.IsSortVisible = false;

			if (_hitInfo.Card != null)
				foreach (var field in _hitInfo.Card.Fields)
					field.IsSortVisible = SortOptions.AllowSort && field.AllowSort  && (field.IsHotTracked || field.SortOrder != SortOrder.None);
		}

		private void mouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			var hitInfo = CalcHitInfo(e.Location);
			if (!hitInfo.IsSortButton)
				return;

			int sortIndex;
			FieldSortInfo sortInfo;

			if (!_sortIndexByField.TryGetValue(hitInfo.FieldName, out sortIndex))
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
				RowHandle = -1,
				InBounds = new Rectangle(new Point(0, 0), Size).Contains(location)
			};

			if (!hitInfo.InBounds)
				return hitInfo;

			var cardCell = getCardCell(location);

			int rowsCount = getRowsCount();
			int columnsCount = getColumnsCount();

			if (cardCell.X < 0 || cardCell.X >= columnsCount || cardCell.Y < 0 || cardCell.Y >= rowsCount)
				return hitInfo;

			var index = getCardIndex(cardCell.X, cardCell.Y, columnsCount);

			if (index < 0 || index >= Cards.Count)
				return hitInfo;

			var card = Cards[index];

			if (!card.Visible)
				return hitInfo;

			hitInfo.RowHandle = getRowHandle(index);
			hitInfo.CardBounds = card.Bounds;
			hitInfo.Card = card;

			var locationInField = new Point(location.X - card.Bounds.X, location.Y - card.Bounds.Y);

			foreach (var field in card.Fields)
			{
				bool inField = field.Bounds.Contains(locationInField);
				if (inField)
				{
					var rectField = field.Bounds;
					rectField.Offset(card.Location);
					hitInfo.FieldBounds = rectField;

					hitInfo.FieldName = field.FieldName;
					hitInfo.Field = field;

					hitInfo.IsSortButton =
						SortOptions.AllowSort &&
						hitInfo.Field.AllowSort &&
						SortOptions.GetSortButtonBounds(field, card).Contains(location);

					return hitInfo;
				}
			}

			return hitInfo;
		}

		private Point getCardLocation(int i, int j)
		{
			return new Point(
				CardInterval.Width/2 + i*(CardSize.Width + CardInterval.Width),
				CardInterval.Height/2 + j*(CardSize.Height + CardInterval.Height));
		}

		private Point getCardCell(Point location)
		{
			return new Point(
				(location.X - CardInterval.Width/2)/(CardSize.Width + CardInterval.Width),
				(location.Y - CardInterval.Height/2)/(CardSize.Height + CardInterval.Height));
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
				int pageSize = columnsCount*rowsCount;
				value = pageSize*(value/pageSize);
			}

			return value;
		}

		public void InvalidateLayout()
		{
			Cards.Clear();
		}

		public void ApplyCardIndex()
		{
			int columnsCount = getColumnsCount();
			int rowsCount = getRowsCount();

			for (int j = 0; j < rowsCount; j++)
				for (int i = 0; i < columnsCount; i++)
				{
					int index = j*columnsCount + i;
					int rowHandle = getRowHandle(index);
					var row = FindRow(rowHandle);

					if (row != null)
					{
						Cards[index].DataSource = row;
						RowDataLoaded?.Invoke(this, rowHandle);
						Cards[index].Visible = true;
					}
					else
					{
						Cards[index].DataSource = null;
						Cards[index].Visible = false;
					}
				}

			for (int index = rowsCount*columnsCount; index < Cards.Count; index++)
			{
				Cards[index].DataSource = null;
				Cards[index].Visible = false;
			}
		}

		public void ApplyIconRecognizer()
		{
			foreach (var card in Cards)
				card.SetIconRecognizer(IconRecognizer);
		}

		public void SetHighlihgtTextRanges(IList<TextRange> ranges, int rowHandle, string fieldName)
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

			if (result < 0 || result > columnsCount*rowsCount)
				return -1;

			return result;
		}

		private int getRowHandle(int displayIndex)
		{
			return CardIndex + displayIndex;
		}

		public IList<TextRange> GetHighlihgtTextRanges(int rowHandle, string fieldName)
		{
			var card = Cards[getDisplayIndex(rowHandle)];
			var field = card.Fields.Single(_ => _.FieldName == fieldName);
			return field.HighlightRanges;
		}

		[Category("Settings")]
		[DefaultValue(typeof (LayoutControl)), TypeConverter(typeof (LayoutControlTypeConverter))]
		public Type LayoutControlType
		{
			get { return _layoutControlType; }
			set
			{
				_layoutControlType = value;
				ProbeCard = createProbeCard();
				InvalidateLayout();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof (Size), "0, 0")]
		public Size CardInterval
		{
			get { return _cardInterval; }
			set
			{
				_cardInterval = value;
				OnLayout(new LayoutEventArgs(this, nameof(CardInterval)));
			}
		}

		[Category("Settings")]
		[DefaultValue(false)]
		public bool AllowPartialCards
		{
			get { return _allowPartialCards; }
			set
			{
				_allowPartialCards = value;
				OnLayout(new LayoutEventArgs(this, nameof(AllowPartialCards)));
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof (Size), "0, 0")]
		public Size PartialCardsThreshold
		{
			get { return _partialCardsThreshold; }
			set
			{
				_partialCardsThreshold = value;
				OnLayout(new LayoutEventArgs(this, nameof(PartialCardsThreshold)));
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof (Color), "Transparent")]
		public Color HotTrackBackgroundColor { get; set; }

		[Category("Settings")]
		[DefaultValue(typeof (Color), "Transparent")]
		public Color HotTrackBorderColor { get; set; }

		[Category("Settings")]
		[TypeConverter(typeof (ExpandableObjectConverter))]
		public SortOptions SortOptions { get; set; } = new SortOptions();



		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CardIndex
		{
			get { return _cardIndex; }
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
			get { return _sortInfos.ToList(); }

			set
			{
				_sortInfos.Clear();
				_sortInfos.AddRange(value);

				updateSort();
				SortChanged?.Invoke(this);
			}
		}

		[Browsable(false)]
		public int ScrollWidth => _scrollBar.Width;

		[Browsable(false)]
		public int Count => DataSource?.Count ?? 0;

		[Browsable(false)]
		private int PageCount
		{
			get
			{
				int pageSize = getRowsCount()*getColumnsCount();

				int pageCount = Count/pageSize;

				if (Count%pageSize > 0)
					pageCount++;

				return pageCount;
			}
		}

		[Browsable(false)]
		public Size CardSize => ProbeCard.Size;

		[Browsable(false)]
		private IList<LayoutControl> Cards { get; }

		public int FindRow(object row)
		{
			int rowHandle;
			if (!_rowHandleByObject.TryGetValue(row, out rowHandle))
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
			return field.Text;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LayoutControl ProbeCard { get; private set; }

		public IEnumerable<string> FieldNames
		{
			get { return ProbeCard.Fields.Select(_ => _.FieldName); }
		}

		private Size _cardInterval;
		private Type _layoutControlType;
		private bool _allowPartialCards;
		private Size _partialCardsThreshold;
		private int _cardIndex;

		private readonly Dictionary<object, int> _rowHandleByObject = new Dictionary<object, int>();
		private readonly List<FieldSortInfo> _sortInfos = new List<FieldSortInfo>();
		private Dictionary<string, int> _sortIndexByField = new Dictionary<string, int>();

		private HitInfo _hitInfo;

		#region mouse wheel without focus

		public bool PreFilterMessage(ref Message m)
		{
			// WM_MOUSEWHEEL, WM_KEYDOWN
			if (m.Msg != 0x020a && m.Msg != 0x0100)
				return false;

			if (m.Msg == 0x0100)
			{
				var keyCode = m.WParam.ToInt32();
				if (keyCode < NavigationKeys[0] || keyCode > NavigationKeys[NavigationKeys.Length - 1])
					return false;
			}

			if (!Focused && isUnderMouse())
				// Отправить событие в данный контрол
				ControlHelpers.SendMessage(Handle, m.Msg, m.WParam, m.LParam);

			return false;
		}

		private bool isUnderMouse()
		{
			return Handle.Equals(ControlHelpers.WindowFromPoint(Cursor.Position));
		}

		private static readonly int[] NavigationKeys = 
		{
			0x21, // pgup
			0x22, // pgdn
			0x23, // end
			0x24 //  home
		};

		#endregion
	}
}