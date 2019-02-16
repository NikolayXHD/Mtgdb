using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public static class ManualMenuPainter
	{
		public static void SetupComboBox(ComboBox menu, bool allowScroll)
		{
			menu.DrawMode = DrawMode.OwnerDrawVariable;
			menu.FlatStyle = FlatStyle.Flat;
			menu.IntegralHeight = false;

			menu.MeasureItem += (s, e) =>
			{
				var comboBox = (ComboBox) s;
				var size = comboBox.MeasureText(comboBox.Items[e.Index], e.Graphics);
				e.ItemWidth = size.Width;
				e.ItemHeight = size.Height;
			};

			menu.DrawItem += (s, e) =>
			{
				var comboBox = (ComboBox) s;

				bool isHighlighted = (e.State & (DrawItemState.HotLight | DrawItemState.Focus)) > 0;
				var backColor = isHighlighted ? SystemColors.Highlight : comboBox.BackColor;
				var foreColor = isHighlighted ? SystemColors.HighlightText : comboBox.ForeColor;

				e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);

				if (e.Index >= 0 && e.Index < comboBox.Items.Count)
					e.Graphics.DrawText((string) comboBox.Items[e.Index], comboBox.Font, e.Bounds, foreColor, TextFormat);

				e.DrawFocusRectangle();
			};

			if (!allowScroll)
				menu.MouseWheel += (s, e) => { ((HandledMouseEventArgs) e).Handled = true; };
		}

		public static Size MeasureText(this Control listBox, object item, Graphics g) =>
			g.MeasureText((string) item, listBox.Font, listBox.Size, TextFormat);

		public static void SetHeightByContent(this ListBox listBox)
		{
			listBox.Height = getHeight();

			int getHeight()
			{
				if (listBox.Items.Count == 0)
					return 0;

				var g = listBox.CreateGraphics();

				int contentHeight = listBox.Items
					.Cast<object>()
					.Sum(item => listBox.MeasureText(item, g).Height);

				int height = 2 + contentHeight;
				return height;
			}
		}

		private const TextFormatFlags TextFormat =
			TextFormatFlags.NoClipping |
			TextFormatFlags.NoPrefix |
			TextFormatFlags.VerticalCenter |
			TextFormatFlags.TextBoxControl;
	}
}