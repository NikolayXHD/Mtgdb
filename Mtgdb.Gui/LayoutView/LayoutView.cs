using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class LayoutView
	{
		public LayoutView(LayoutViewControl view)
		{
			_view = view;
		}

		public bool Wraps(object view)
		{
			return ReferenceEquals(_view, view);
		}

		public HitInfo CalcHitInfo(Point clientLocation)
		{
			return _view.CalcHitInfo(clientLocation);
		}

		public List<FieldSortInfo> SortInfo
		{
			get { return _view.SortInfo; }
			set { _view.SortInfo = value; }
		}

		public Control Control => _view;
		public Size CardMinSize => _view.CardSize;
		public int CardHorzInterval => _view.CardInterval.Width;
		public int CardVertInterval => _view.CardInterval.Height;
		public int RowCount => _view.Count;
		public IEnumerable<string> FieldNames => _view.FieldNames;

		public bool AllowPartialCards
		{
			get { return _view.AllowPartialCards; }
			set { _view.AllowPartialCards = value; }
		}

		public int PartialCardVertical
		{
			get { return _view.PartialCardsThreshold.Height; }
			set { _view.PartialCardsThreshold = new Size(_view.PartialCardsThreshold.Width, value); }
		}

		public int PartialCardHorizontal
		{
			get { return _view.PartialCardsThreshold.Width; }
			set { _view.PartialCardsThreshold = new Size(value, _view.PartialCardsThreshold.Height); }
		}

		public int VisibleRecordIndex
		{
			get { return _view.CardIndex; }
			set
			{
				if (_view.CardIndex == value)
					return;

				_view.CardIndex = value;
				_view.ApplyCardIndex();
				_view.Invalidate();
			}
		}

		public event Action<object> VisibleRecordIndexChanged
		{
			add { _view.CardIndexChanged += value; }
			remove { _view.CardIndexChanged -= value; }
		}

		public event Action<object, CustomDrawArgs> CustomDrawField
		{
			add { _view.CustomDrawField += value; }
			remove { _view.CustomDrawField -= value; }
		}

		public event Action<object, int> RowDataLoaded
		{
			add { _view.RowDataLoaded += value; }
			remove { _view.RowDataLoaded -= value; }
		}

		public event Action<object> SortChanged
		{
			add { _view.SortChanged += value; }
			remove { _view.SortChanged -= value; }
		}

		public event MouseEventHandler MouseDown
		{
			add { _view.MouseDown += value; }
			remove { _view.MouseDown -= value; }
		}

		public event MouseEventHandler MouseUp
		{
			add { _view.MouseUp += value; }
			remove { _view.MouseUp -= value; }
		}

		public event MouseEventHandler MouseMove
		{
			add { _view.MouseMove += value; }
			remove { _view.MouseMove -= value; }
		}

		public event EventHandler MouseEnter
		{
			add { _view.MouseEnter += value; }
			remove { _view.MouseEnter -= value; }
		}

		public event EventHandler MouseLeave
		{
			add { _view.MouseLeave += value; }
			remove { _view.MouseLeave -= value; }
		}

		public event EventHandler LostFocus
		{
			add { _view.LostFocus += value; }
			remove { _view.LostFocus -= value; }
		}
		

		public object GetRow(int cardIndex)
		{
			return _view.FindRow(cardIndex);
		}

		public int GetVisibleIndex(int cardIndex)
		{
			if (_view.DataSource == null || cardIndex < 0 || cardIndex >= _view.DataSource.Count)
				return -1;

			return cardIndex;
		}

		public int FindRow(object row)
		{
			return _view.FindRow(row);
		}

		public int GetVisibleRowHandle(int rowVisibleIndex)
		{
			if (_view.DataSource == null || rowVisibleIndex < 0 || rowVisibleIndex >= _view.DataSource.Count)
				return -1;

			return rowVisibleIndex;
		}

		public void Focus()
		{
			_view.Focus();
		}

		public void Invalidate()
		{
			_view.Invalidate();
		}

		public void InvalidateCard(object row)
		{
			_view.InvalidateCard(row);
		}

		public void RefreshData()
		{
			_view.RefreshData();
		}

		public void SetImageSize(Size size)
		{
			var imageField = _view.ProbeCard.Fields.Single(_ => _.FieldName == nameof(Card.Image));
			var textField = _view.ProbeCard.Fields.SingleOrDefault(_ => _.FieldName == nameof(Card.Text));

			if (textField == null)
				_view.ProbeCard.Size = size;
			else
			{
				var xOffset = size.Width - imageField.Width;

				_view.ProbeCard.Size = new Size(
					_view.ProbeCard.Width + xOffset,
					size.Height);

				foreach (var field in _view.ProbeCard.Fields)
					if (field != imageField)
						field.Location = new Point(field.Location.X + xOffset, field.Location.Y);
			}

			imageField.Size = size;
			_view.InvalidateLayout();
		}

		public void HideTextualFields()
		{
			_view.LayoutControlType = typeof (DeckLayout);
		}

		public string GetFieldText(int cardIndex, string fieldName)
		{
			return _view.GetText(cardIndex, fieldName);
		}

		public void SetDataSource(object dataSource)
		{
			_view.DataSource = (IList) dataSource;
		}

		public void SetIconRecognizer(IconRecognizer recognizer)
		{
			_view.IconRecognizer = recognizer;
		}

		public void SetHighlightTextRanges(IList<TextRange> ranges, int rowHandle, string fieldName)
		{
			_view.SetHighlihgtTextRanges(ranges, rowHandle, fieldName);
		}

		public IList<TextRange> GetHiglightRanges(int rowHandle, string fieldName)
		{
			return _view.GetHighlihgtTextRanges(rowHandle, fieldName);
		}

		public HighlightSettings GetHighlightSettings()
		{
			return _view.ProbeCard.HighlightSettings;
		}

		public int ScrollWidth => _view.ScrollWidth;

		private readonly LayoutViewControl _view;
	}
}