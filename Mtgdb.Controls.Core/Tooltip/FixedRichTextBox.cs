using System;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class FixedRichTextBox : RichTextBox
	{
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			if (Runtime.IsMono)
				return;

			if (AutoWordSelection)
				return;

			AutoWordSelection = true;
			AutoWordSelection = false;
		}

		protected override void WndProc(ref Message m)
		{
			const int WM_MOUSEACTIVATE = 0x21;
			if (m.Msg == WM_MOUSEACTIVATE)
				Focus();

			base.WndProc(ref m);
		}
	}
}
