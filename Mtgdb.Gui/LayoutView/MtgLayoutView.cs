using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class MtgLayoutView
	{
		public MtgLayoutView(LayoutViewControl view)
		{
			_view = view;
		}

		public bool Wraps(object view) =>
			ReferenceEquals(_view, view);

		public HitInfo CalcHitInfo(Point clientLocation) =>
			_view.CalcHitInfo(clientLocation);

		public List<FieldSortInfo> SortInfo
		{
			get => _view.SortInfo;
			set => _view.SortInfo = value;
		}

		public Control Control => _view;
		public Size CardMinSize => _view.CardSize;
		public int CardHorzInterval => _view.LayoutOptions.CardInterval.Width;
		public int CardVertInterval => _view.LayoutOptions.CardInterval.Height;
		public int RowCount => _view.Count;
		public IEnumerable<string> FieldNames => _view.FieldNames;

		public bool AllowPartialCards
		{
			get => _view.LayoutOptions.AllowPartialCards;
			set => _view.LayoutOptions.AllowPartialCards = value;
		}

		public Size PartialCardSize
		{
			get => _view.LayoutOptions.PartialCardsThreshold;
			set => _view.LayoutOptions.PartialCardsThreshold = value;
		}

		public int VisibleRecordIndex
		{
			get => _view.CardIndex;
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
			add => _view.CardIndexChanged += value;
			remove => _view.CardIndexChanged -= value;
		}

		public event Action<object, CustomDrawArgs> CustomDrawField
		{
			add => _view.CustomDrawField += value;
			remove => _view.CustomDrawField -= value;
		}

		public event Action<object, int> RowDataLoaded
		{
			add => _view.RowDataLoaded += value;
			remove => _view.RowDataLoaded -= value;
		}

		public event Action<object> SortChanged
		{
			add => _view.SortChanged += value;
			remove => _view.SortChanged -= value;
		}

		public event Action<object, SearchArgs> SearchClicked
		{
			add => _view.SearchClicked += value;
			remove => _view.SearchClicked -= value;
		}

		public event MouseEventHandler MouseDown
		{
			add => _view.MouseDown += value;
			remove => _view.MouseDown -= value;
		}

		public event MouseEventHandler MouseUp
		{
			add => _view.MouseUp += value;
			remove => _view.MouseUp -= value;
		}

		public event MouseEventHandler MouseMove
		{
			add => _view.MouseMove += value;
			remove => _view.MouseMove -= value;
		}

		public event EventHandler MouseEnter
		{
			add => _view.MouseEnter += value;
			remove => _view.MouseEnter -= value;
		}

		public event EventHandler MouseLeave
		{
			add => _view.MouseLeave += value;
			remove => _view.MouseLeave -= value;
		}

		public event Action<object, HitInfo, MouseEventArgs> MouseClicked
		{
			add => _view.MouseClicked += value;
			remove => _view.MouseClicked -= value;
		}

		public event Action<object, HitInfo, CancelEventArgs> SelectionStarted
		{
			add => _view.SelectionStarted += value;
			remove => _view.SelectionStarted -= value;
		}

		public object GetRow(int rowHandle) => 
			_view.FindRow(rowHandle);

		public int GetVisibleIndex(int cardIndex)
		{
			if (_view.DataSource == null || cardIndex < 0 || cardIndex >= _view.DataSource.Count)
				return -1;

			return cardIndex;
		}

		public int FindRow(object row) => 
			_view.FindRow(row);

		public int GetVisibleRowHandle(int rowVisibleIndex)
		{
			if (_view.DataSource == null || rowVisibleIndex < 0 || rowVisibleIndex >= _view.DataSource.Count)
				return -1;

			return rowVisibleIndex;
		}

		public void Focus() => 
			_view.Focus();

		public void Invalidate() => 
			_view.Invalidate();

		public void InvalidateCard(object row) => 
			_view.InvalidateCard(row);

		public void RefreshData() => 
			_view.RefreshData();

		public bool TextualFieldsVisible
		{
			get => _view.LayoutControlType == typeof(CardLayout);
			set
			{
				var layout = value ? typeof(CardLayout) : typeof(DeckLayout);
				_view.LayoutControlType = layout;

				var interval = _view.LayoutOptions.CardInterval;

				if (value)
					_view.LayoutOptions.CardInterval = new Size(interval.Height * 2, interval.Height);
				else
					_view.LayoutOptions.CardInterval = new Size(interval.Height, interval.Height);

				var threshold = _view.LayoutOptions.PartialCardsThreshold;

				_view.LayoutOptions.PartialCardsThreshold = new Size(
					_view.ProbeCard.Width * threshold.Height / _view.ProbeCard.Height,
					threshold.Height);
			}
		}

		public string GetFieldText(int rowHandle, string fieldName) => 
			_view.GetText(rowHandle, fieldName);

		public string GetFieldTooltipText(int rowHandle, string field)
		{
			var text = new StringBuilder(_view.GetText(rowHandle, field));

			var card = (Card) GetRow(rowHandle);

			if (Str.Equals(field, nameof(Card.Text)))
			{
				if (!string.IsNullOrEmpty(card.OriginalText))
					text
						.AppendLine()
						.AppendLine()
						.Append(nameof(Card.OriginalText)).Append(":")
						.AppendLine()
						.Append(card.OriginalText);

				text
					.AppendLine()
					.AppendLine()
					.Append(nameof(KeywordDefinitions.Layout)).Append(": ").Append(card.Layout);

				text
					.AppendLine()
					.AppendLine()
					.Append(nameof(Card.GeneratedMana)).Append(": ").Append(card.GeneratedMana);

				var keywords = card.GetKeywords()
					.Concat(card.GetCastKeywords())
					.ToHashSet(Str.Comparer);

				if (keywords.Count > 0)
					text
						.AppendLine()
						.AppendLine()
						.Append(nameof(KeywordDefinitions.Keywords)).Append(": ")
						.Append(string.Join(", ", keywords));
			}
			else if (Str.Equals(field, nameof(Card.Type)) && !string.IsNullOrEmpty(card.OriginalType))
				text
					.AppendLine()
					.AppendLine()
					.Append(nameof(Card.OriginalType)).Append(":")
					.AppendLine()
					.Append(card.OriginalType);

			return text.ToString();
		}

		public void SetDataSource(object dataSource) =>
			_view.DataSource = (IList) dataSource;

		public void SetIconRecognizer(IconRecognizer recognizer) => 
			_view.IconRecognizer = recognizer;

		public void SetHighlightTextRanges(IList<TextRange> ranges, int rowHandle, string fieldName) => 
			_view.SetHighlihgtTextRanges(ranges, rowHandle, fieldName);

		public IList<TextRange> GetHiglightRanges(int rowHandle, string fieldName) => 
			_view.GetHighlihgtTextRanges(rowHandle, fieldName);

		public HighlightOptions GetHighlightSettings() =>
			_view.ProbeCard.HighlightOptions;

		public int ScrollWidth => _view.ScrollWidth;

		public Rectangle GetAlignButtonBounds(HitInfo hitInfo) =>
			_view.GetAlignButtonBounds(hitInfo);

		public bool IsSelectingText() =>
			_view.IsSelectingText();

		private readonly LayoutViewControl _view;
	}
}