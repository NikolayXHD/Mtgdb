using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class FixedRichTextBox : RichTextBox
	{
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			if (!AutoWordSelection)
			{
				AutoWordSelection = true;
				AutoWordSelection = false;
			}

			SelectionEnabled = true;

			MouseDown += text_MouseDown;
			MouseUp += text_MouseUp;
			MouseMove += text_MouseMove;
		}

		protected override void Dispose(bool disposing)
		{
			MouseDown -= text_MouseDown;
			MouseUp -= text_MouseUp;
			MouseMove -= text_MouseMove;

			base.Dispose(disposing);
		}

		private void text_MouseDown(object sender, MouseEventArgs e)
		{
			if (!SelectionEnabled)
				return;

			if (e.Button == MouseButtons.Left)
			{
				int current = this.GetTrueIndexPositionFromPoint(e.Location);

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
				var current = this.GetTrueIndexPositionFromPoint(e.Location);

				if (current != _selectionStart)
				{
					int start = Math.Min(current, _selectionStart);

					SelectionStart = start;
					SelectionLength = Math.Abs(current - _selectionStart);
				}
			}
			else if (!Focused)
			{
				SelectionStart = _selectionStart =
					this.GetTrueIndexPositionFromPoint(e.Location);
			}
		}

		public void ResetSelection()
		{
			_selectionStart = -1;
			_selectionManual = false;
		}

		public bool SelectionEnabled { get; set; }

		private int _selectionStart;
		private bool _selectionManual;

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_LBUTTONDBLCLK && handleDoubleClick())
				return;

			base.WndProc(ref m);

			bool handleDoubleClick()
			{
				int start = SelectionStart;
				if (start < 0)
					return false;

				var text = Text;

				var leftMatch = _leftDelimiterRegex.Match(text, 0, start);

				int left = leftMatch.Success
					? leftMatch.Index + leftMatch.Length
					: 0;

				var rightMatch = _rightDelimiterRegex.Match(text, start);
				int right = rightMatch.Success
					? rightMatch.Index
					: text.Length;

				SelectionStart = left;
				SelectionLength = right - left;

				return true;
			}
		}

		private static readonly Regex _leftDelimiterRegex = new Regex(@"\W", RegexOptions.RightToLeft);
		private static readonly Regex _rightDelimiterRegex = new Regex(@"\W");

		private const int WM_LBUTTONDBLCLK = 0x0203;
	}
}