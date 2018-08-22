using System;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class RichTextBoxSelectionSubsystem
	{
		public RichTextBoxSelectionSubsystem(FixedRichTextBox textbox)
		{
			_textbox = textbox;
			SelectionEnabled = true;
		}

		public void SubscribeToEvents()
		{
			_textbox.MouseDown += text_MouseDown;
			_textbox.MouseUp += text_MouseUp;
			_textbox.MouseMove += text_MouseMove;
		}

		public void UnsubscribeFromEvents()
		{
			_textbox.MouseDown -= text_MouseDown;
			_textbox.MouseUp -= text_MouseUp;
			_textbox.MouseMove -= text_MouseMove;
		}

		private void text_MouseDown(object sender, MouseEventArgs e)
		{
			if (!SelectionEnabled)
				return;

			if (e.Button == MouseButtons.Left)
			{
				int current = _textbox.GetTrueIndexPositionFromPoint(e.Location);

				if (current == _selectionStart)
					_selectionManual = true;
			}
		}

		private void text_MouseUp(object sender, MouseEventArgs e)
		{
			if (!SelectionEnabled)
				return;

			_selectionStart = -1;
			_selectionManual = false;
		}

		private void text_MouseMove(object sender, MouseEventArgs e)
		{
			if (!SelectionEnabled)
				return;

			if (_selectionManual)
			{
				var current = _textbox.GetTrueIndexPositionFromPoint(e.Location);

				if (current != _selectionStart)
				{
					int start = Math.Min(current, _selectionStart);

					_textbox.SelectionStart = start;
					_textbox.SelectionLength = Math.Abs(current - _selectionStart);
				}
			}
			else if (!_textbox.Focused)
			{
				_textbox.SelectionStart = _selectionStart =
					_textbox.GetTrueIndexPositionFromPoint(e.Location);
			}
		}

		public void Reset()
		{
			_selectionStart = -1;
			_selectionManual = false;
		}

		public bool SelectionEnabled { get; set; }

		private int _selectionStart;
		private bool _selectionManual;

		private readonly FixedRichTextBox _textbox;
	}
}