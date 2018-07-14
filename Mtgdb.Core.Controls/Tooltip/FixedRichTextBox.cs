using System;
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
	}
}