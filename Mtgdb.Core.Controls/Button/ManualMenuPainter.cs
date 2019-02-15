using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public static class ManualMenuPainter
	{
		public static void SetupComboBox(System.Windows.Forms.ComboBox menu, bool allowScroll)
		{
			menu.DrawMode = DrawMode.OwnerDrawVariable;
			menu.FlatStyle = FlatStyle.Flat;
			menu.IntegralHeight = false;

			menu.MeasureItem += (s, e) =>
			{
				var comboBox = (System.Windows.Forms.ComboBox) s;
				var size = comboBox.MeasureText(comboBox.Items[e.Index], e.Graphics);
				e.ItemWidth = size.Width;
				e.ItemHeight = size.Height;
			};

			menu.DrawItem += (s, e) =>
			{
				var comboBox = (System.Windows.Forms.ComboBox) s;

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

		public static void SetupListBox(ListBox menu)
		{
			menu.DrawMode = DrawMode.OwnerDrawVariable;
			menu.IntegralHeight = false;

			var selectedIndices = menu.SelectedIndices.Cast<int>().ToHashSet();

			menu.SelectedIndexChanged += (s, e) =>
			{
				// list view in DrawMode.OwnerDrawVariable does not invalidate items whose selection state changed
				// so we invalidate them manually

				var newSelectedIndices = menu.SelectedIndices.Cast<int>().ToHashSet();

				var selectionChangedIndices = selectedIndices;
				selectionChangedIndices.SymmetricExceptWith(newSelectedIndices);

				var g = menu.CreateGraphics();
				int y = 0; // border

				for (int i = 0; i < menu.Items.Count; i++)
				{
					var size = menu.MeasureText(menu.Items[i], g);

					if (selectionChangedIndices.Contains(i))
						menu.Invalidate(new Rectangle(0, y, menu.Width, size.Height));

					y += size.Height;
				}

				selectedIndices = newSelectedIndices;
			};

			menu.MeasureItem += (s, e) =>
			{
				ListBox listBox = (ListBox) s;
				int index = e.Index;
				var size = listBox.MeasureText(listBox.Items[index], e.Graphics);
				e.ItemWidth = size.Width;
				e.ItemHeight = size.Height;
			};

			menu.DrawItem += (s, e) =>
			{
				var comboBox = (ListBox) s;

				bool isHighlighted = (e.State & (DrawItemState.HotLight | DrawItemState.Focus)) > 0
					|| e.Index == menu.SelectedIndex;

				var backColor = isHighlighted ? SystemColors.Highlight : comboBox.BackColor;
				var foreColor = isHighlighted ? SystemColors.HighlightText : comboBox.ForeColor;

				e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);

				if (e.Index >= 0 && e.Index < comboBox.Items.Count)
					e.Graphics.DrawText((string) comboBox.Items[e.Index], comboBox.Font, e.Bounds, foreColor, TextFormat);

				e.DrawFocusRectangle();
			};
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