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

			SelectionEnabled = true;

			MouseDown += text_MouseDown;
			MouseUp += text_MouseUp;
			MouseMove += text_MouseMove;

			AutoWordSelection = false;
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
				_selectionStart = this.GetTrueIndexPositionFromPoint(e.Location);
		}

		private void text_MouseUp(object sender, MouseEventArgs e)
		{
			if (!SelectionEnabled)
				return;

			_selectionStart = -1;
		}

		private void text_MouseMove(object sender, MouseEventArgs e)
		{
			if (!SelectionEnabled)
				return;

			if (_selectionStart == -1)
				return;

			var location = e.Location;
			if (location.Y < 0 || location.Y >= Height)
				location.Y = 0;
			var current = this.GetTrueIndexPositionFromPoint(location);
			int start = Math.Min(current, _selectionStart);
			int length = Math.Abs(current - _selectionStart);

			if (start == SelectionStart && length == SelectionLength)
				return;

			SelectionStart = start;
			SelectionLength = length;
		}

		public void ResetSelection()
		{
			_selectionStart = -1;
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_LBUTTONDBLCLK && handleDoubleClick(m))
				return;

			base.WndProc(ref m);

			bool handleDoubleClick(Message msg)
			{
				int start = this.GetTrueIndexPositionFromPoint(msg.LParam.ToPoint());
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

		public bool SelectionEnabled { get; set; }

		private int _selectionStart = -1;
		private static readonly Regex _leftDelimiterRegex = new Regex(@"\W", RegexOptions.RightToLeft);
		private static readonly Regex _rightDelimiterRegex = new Regex(@"\W");

		private const int WM_LBUTTONDBLCLK = 0x0203;
	}
}
