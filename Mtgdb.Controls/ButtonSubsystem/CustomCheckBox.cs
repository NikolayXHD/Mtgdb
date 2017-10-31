using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class CustomCheckBox : CheckBox
	{
		public CustomCheckBox()
		{
			TabStop = false;
			FlatStyle = FlatStyle.Flat;
			FlatAppearance.CheckedBackColor = BackColor;
		}

		public sealed override Color BackColor
		{
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}

		protected override bool ShowFocusCues => false;
		protected override bool ShowKeyboardCues => false;
	}
}
