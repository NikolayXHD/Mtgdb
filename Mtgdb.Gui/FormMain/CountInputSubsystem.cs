using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Data;

namespace Mtgdb.Gui
{
	public class CountInputSubsystem : IComponent
	{
		public event Action<(int CountDelta, bool IsDeck, Card Card)> Input;

		public CountInputSubsystem()
		{
			_countRectSize = new Size(80, 30);
			CountBorder = 2;

			InputControl = new FixedRichTextBox
			{
				Multiline = false,
				BorderStyle = BorderStyle.None,
				Font = new Font("Arial Black", 18, GraphicsUnit.Pixel),
				BackColor = SystemColors.GradientActiveCaption,
				ForeColor = SystemColors.WindowText,
				Size = _countRectSize
			};

			ColorSchemeController.SystemColorsChanging += systemColorsChanging;
			InputControl.PreviewKeyDown += previewKeyDown;
			InputControl.KeyDown += keyDown;
			InputControl.LostFocus += lostFocus;
		}

		public void Scale()
		{
			InputControl.ScaleDpiFont();

			new DpiScaler<CountInputSubsystem, Size>(
				_ => _._countRectSize,
				(_, s) => _._countRectSize = s,
				s => s.ByDpi()
			).Setup(this);

			new DpiScaler<CountInputSubsystem, int>(
				_ => _.CountBorder,
				(_, b) => _.CountBorder = b,
				b => b.ByDpiHeight()
			).Setup(this);
		}

		public bool HandleClick(LayoutViewControl control, Card card,
			MouseEventArgs e) =>
			e.Button == MouseButtons.Left &&
			_isDeckKey.TryGetValue(Control.ModifierKeys, out _editDeck) &&
			edit(control, card, e.Location);

		public bool IsCountRectangle(HitInfo hitInfo, Point position,
			out Rectangle rect)
		{
			rect = Rectangle.Empty;
			return
				Str.Equals(hitInfo.FieldName, nameof(Card.Image)) &&
				hitInfo.FieldBounds.HasValue &&
				(
					rect = GetCountRectangle(hitInfo.FieldBounds.Value)
				).Contains(position);
		}

		public Rectangle GetCountRectangle(Rectangle imageBounds)
		{
			var rect = new Rectangle(
				imageBounds.Left +
				(Ui.ImageLoader.CardSize.Width - _countRectSize.Width) / 2,
				imageBounds.Bottom - _countRectSize.Height,
				_countRectSize.Width,
				_countRectSize.Height);

			return rect;
		}

		public int CountBorder { get; private set; }

		public Font CountFont => InputControl.Font;

		private void previewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyData == Keys.Tab || e.KeyData == (Keys.Shift | Keys.Tab))
				e.IsInputKey = true;
		}

		private void keyDown(object sender, KeyEventArgs e)
		{
			bool handled = true;
			switch (e.KeyData)
			{
				case Keys.Escape:
					hideCountEditor();
					break;

				case Keys.Enter:
					apply();
					hideCountEditor();
					break;

				case Keys.Tab:
					apply();
					hideCountEditor();
					handled = editNextCard(prev: false);
					break;

				case Keys.Shift | Keys.Tab:
					apply();
					hideCountEditor();
					handled = editNextCard(prev: true);
					break;

				default:
					handled = false;
					break;
			}

			e.Handled = e.SuppressKeyPress = handled;
		}

		private void systemColorsChanging() =>
			InputControl.TouchColorProperties();


		private void lostFocus(object sender, EventArgs e)
		{
			apply();
			hideCountEditor();
		}

		private bool editNextCard(bool prev)
		{
			if (_view == null || _card == null || _view.Count == 0)
				return false;

			int minIndex = _view.CardIndex;
			int maxIndex =
				Math.Min(_view.Count, _view.CardIndex + _view.GetPageSize()) - 1;

			int index = _view.FindIndex(_card);
			if (!index.IsWithin(minIndex, maxIndex))
				return false;

			if (prev)
				index--;
			else
				index++;

			if (!index.IsWithin(minIndex, maxIndex))
				return false;

			var card = (Card)_view.FindRow(index);
			return edit(_view, card, cursor: null);
		}

		private bool edit(LayoutViewControl view, Card card, Point? cursor)
		{
			var imageBounds = view.GetFieldBounds(card, nameof(Card.Image));
			if (!imageBounds.HasValue)
				return false;

			var countBounds = GetCountRectangle(imageBounds.Value);
			countBounds.Inflate(-CountBorder, -CountBorder);

			if (cursor.HasValue && !countBounds.Contains(cursor.Value))
				return false;

			if (view != _view)
				hideCountEditor();

			_card = card;
			_view = view;

			countBounds.Inflate(-countBounds.Height / 4, 0);
			countBounds.Size = countBounds.Size.Minus(new Size(1, 1));
			countBounds.Offset(1, 1);
			InputControl.Bounds = countBounds;
			updateText(card);

			showCountEditor();
			return true;
		}

		public void UpdateText(Card card, bool isDeck)
		{
			if (InputControl.Parent != null && _card == card && isDeck == _editDeck)
				updateText(card);
		}

		private void updateText(Card card)
		{
			InputControl.Text = getCount(card).ToString(Str.Culture);
			InputControl.SelectAll();
			InputControl.SelectionAlignment = HorizontalAlignment.Center;
		}

		private void showCountEditor()
		{
			_view.Controls.Add(InputControl);
			InputControl.BringToFront();
			InputControl.Focus();
		}

		private void hideCountEditor() =>
			_view?.Controls.Remove(InputControl);

		private void apply()
		{
			if (int.TryParse(InputControl.Text, out int count))
				Input?.Invoke((
					CountDelta: count - getCount(_card),
					IsDeck: _editDeck,
					Card: _card));
		}

		public Card GetCard(LayoutViewControl view, HitInfo hitInfo)
		{
			if (!hitInfo.IsOverImage() && hitInfo.CustomButtonIndex < 0)
				return null;

			return (Card)view.FindRow(hitInfo.RowHandle);
		}

		private int getCount(Card card)
		{
			return _editDeck
				? card.DeckCount(Ui)
				: card.CollectionCount(Ui);
		}

		public string GetTooltipTitle() =>
			$"Enter {(_editDeck ? "deck" : "collection")} count";

		public string GetTooltipText() =>
			"- Tab key to edit next card count\r\n" +
			"- Shift + Tab to edit previous card count";


		private Size _countRectSize;
		public FixedRichTextBox InputControl { get; }

		private bool _editDeck;

		private static readonly Dictionary<Keys, bool> _isDeckKey =
			new Dictionary<Keys, bool>
			{
				[Keys.None] = true,
				[Keys.Alt] = false
			};

		private Card _card;
		private LayoutViewControl _view;


		public UiModel Ui { get; set; }


		public void Dispose()
		{
			ColorSchemeController.SystemColorsChanging -= systemColorsChanging;
			InputControl.PreviewKeyDown -= previewKeyDown;
			InputControl.KeyDown -= keyDown;
			InputControl.LostFocus -= lostFocus;
			Disposed?.Invoke(this, EventArgs.Empty);
		}

		public ISite Site { get; set; }
		public event EventHandler Disposed;
	}
}