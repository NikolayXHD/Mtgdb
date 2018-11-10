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
		}

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

		// ReSharper disable once InconsistentNaming
		// ReSharper disable once IdentifierTypo
		private const int WM_LBUTTONDBLCLK = 0x0203;
	}
}