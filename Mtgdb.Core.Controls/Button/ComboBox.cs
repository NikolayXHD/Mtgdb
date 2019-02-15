using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls.Properties;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Controls
{
	public class ComboBox : Dropdown
	{
		public ComboBox()
		{
			BackColor = SystemColors.Window;
			ForeColor = SystemColors.WindowText;

			Padding = DefaultPadding;

			AutoSize = false;
			AutoCheck = false;

			HighlightCheckedOpacity = 64;
			HighlightMouseOverOpacity = 64;
			TextImageRelation = TextImageRelation.TextBeforeImage;
			TextAlign = StringAlignment.Near;
			VisibleAllBorders = true;

			_menu = new FlowLayoutPanel
			{
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink,

				BackColor = SystemColors.Window,
				FlowDirection = FlowDirection.TopDown,

				Margin = new Padding(0),
				Padding = new Padding(0),

				Visible = false,
			};

			var dropDownImg = Resources.drop_down_48.ScaleBy(0.5f);
			ButtonImages = new ButtonImages(dropDownImg, dropDownImg);

			MenuControl = _menu;
			MarginTop = -1;

			_menuItemsAccessor = new MenuItemsAccessor(this);
			_menuValuesAccessor = new MenuValuesAccessor(_menuItemsAccessor);
		}

		public void SetMenuContainer(Control container) =>
			container.Controls.Add(_menu);

		private void clear()
		{
			foreach (var item in MenuItems.ToArray())
			{
				_menu.Controls.Remove(item);
				item.Dispose();
			}
		}

		private ButtonBase createMenuItem(string value, int index)
		{
			var result = new ButtonBase
			{
				Text = value,
				AutoSize = true,
				AutoCheck = false,
				TextAlign = StringAlignment.Near,
				Margin = new Padding(0),
				ForeColor = SystemColors.WindowText
			};

			result.Padding = new Padding(
				result.Padding.Left - 1,
				result.Padding.Top - 1,
				result.Padding.Right - 1,
				result.Padding.Bottom - 1);

			result.MouseClick += click;
			result.KeyDown += keyDown;
			result.Disposed += disposed;

			void click(object s, MouseEventArgs e)
			{
				if (e.Button == MouseButtons.Left)
					SelectedIndex = index;
			}

			void keyDown(object sender, KeyEventArgs e)
			{
				switch (e.KeyData)
				{
					case Keys.Enter:
					case Keys.Space:
						SelectedIndex = index;
						break;
				}
			}

			void disposed(object s, EventArgs e)
			{
				result.Disposed -= disposed;
				result.MouseClick -= click;
				result.KeyDown -= keyDown;
			}

			return result;
		}

		private void updateSelectedText()
		{
			if (_selectedIndex == -1)
				Text = EmptySelectionText;
			else if (_selectedIndex < _menu.Controls.Count)
				Text = _menu.Controls[_selectedIndex].Text;
			else
				throw new IndexOutOfRangeException();
		}

		protected override void Dispose(bool disposing)
		{
			clear();
			_menu.Dispose();
			base.Dispose(disposing);
		}


		[DefaultValue(typeof(Color), "Window")]
		public override Color BackColor
		{
			get => base.BackColor;
			set => base.BackColor = value;
		}

		[DefaultValue(typeof(Color), "Text")]
		public override Color ForeColor
		{
			get => base.ForeColor;
			set => base.ForeColor = value;
		}

		protected override Padding DefaultPadding =>
			new Padding(4, 4, 0, 4);

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Bitmap Image
		{
			get => base.Image;
			set => base.Image = value;
		}

		[DefaultValue(false)]
		public override bool AutoSize
		{
			get => base.AutoSize;
			set => base.AutoSize = value;
		}

		[DefaultValue(false)]
		public override bool AutoCheck
		{
			get => base.AutoCheck;
			set => base.AutoCheck = value;
		}

		[DefaultValue(64)]
		public override int HighlightCheckedOpacity
		{
			get => base.HighlightCheckedOpacity;
			set => base.HighlightCheckedOpacity = value;
		}

		[DefaultValue(64)]
		public override int HighlightMouseOverOpacity
		{
			get => base.HighlightMouseOverOpacity;
			set => base.HighlightMouseOverOpacity = value;
		}

		[DefaultValue(typeof(TextImageRelation), "TextBeforeImage")]
		public override TextImageRelation TextImageRelation
		{
			get => base.TextImageRelation;
			set => base.TextImageRelation = value;
		}


		[DefaultValue(typeof(StringAlignment), "Near")]
		public override StringAlignment TextAlign
		{
			get => base.TextAlign;
			set => base.TextAlign = value;
		}

		[DefaultValue(true)]
		public override bool? VisibleAllBorders
		{
			get => base.VisibleAllBorders;
			set => base.VisibleAllBorders = value;
		}

		[DefaultValue(typeof(AnchorStyles), "Top, Bottom, Left, Right")]
		public override AnchorStyles VisibleBorders
		{
			get => base.VisibleBorders;
			set => base.VisibleBorders = value;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Control MenuControl
		{
			get => base.MenuControl;
			set => base.MenuControl = value;
		}

		[DefaultValue(-1)]
		public override int? MarginTop
		{
			get => base.MarginTop;
			set => base.MarginTop = value;
		}



		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IReadOnlyList<string> MenuValues =>
			_menuValuesAccessor;

		public void SetMenuValues(IEnumerable<string> values) =>
			SetMenuValues(values.ToReadOnlyList());

		public void SetMenuValues(IList<string> values) =>
			SetMenuValues(values.AsReadOnlyList());

		public void SetMenuValues(IReadOnlyList<string> values)
		{
			_menu.SuspendLayout();

			clear();

			for (int i = 0; i < values.Count; i++)
			{
				string text = values[i];
				_menu.Controls.Add(createMenuItem(text, i));
			}

			updateItemSizes();
			updateItemBorders();

			MenuItemsCreated?.Invoke(this, EventArgs.Empty);

			_menu.ResumeLayout(false);
			_menu.PerformLayout();

			SelectedIndex = Math.Min(SelectedIndex, values.Count - 1);

			void updateItemSizes()
			{
				var sizes = MenuItems
					.Select(_ => _.MeasureContent())
					.ToArray();

				var maxWidth = sizes.Append(Size - new Size(_menu.Padding.Horizontal, 0)).Max(_ => _.Width);

				MenuItems.ForEach((item, i) =>
					item.Size = new Size(maxWidth, sizes[i].Height));
			}

			void updateItemBorders()
			{
				const AnchorStyles defaultBorders = AnchorStyles.Left | AnchorStyles.Right;

				for (int i = 0; i < MenuItems.Count; i++)
				{
					var border = defaultBorders;

					if (i == 0)
						border |= AnchorStyles.Top;

					if (i == MenuItems.Count - 1)
						border |= AnchorStyles.Bottom;

					MenuItems[i].VisibleBorders = border;
				}
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IReadOnlyList<ButtonBase> MenuItems =>
			_menuItemsAccessor;

		private int _selectedIndex = -1;
		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{
				if (_selectedIndex == value)
					return;

				((ButtonBase) _menu.Controls.TryGet(_selectedIndex))
					?.Invoke0(_ => _.Checked = false);

				_selectedIndex = value;

				if (_selectedIndex.IsWithin(0, _menu.Controls.Count - 1))
					((ButtonBase) _menu.Controls[_selectedIndex]).Checked = true;

				updateSelectedText();
				MenuItems.ForEach((m, i) => m.Checked = i == _selectedIndex);
				SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		private readonly string _emptySelectionText = string.Empty;
		public string EmptySelectionText
		{
			get => _emptySelectionText;
			set
			{
				if (_emptySelectionText == value)
					return;

				if (_selectedIndex == -1)
					updateSelectedText();
			}
		}



		public event EventHandler SelectedIndexChanged;
		public event EventHandler MenuItemsCreated;

		private readonly FlowLayoutPanel _menu;
		private readonly MenuValuesAccessor _menuValuesAccessor;
		private readonly MenuItemsAccessor _menuItemsAccessor;



		private class MenuItemsAccessor : IReadOnlyList<ButtonBase>
		{
			public MenuItemsAccessor(ComboBox comboBox) =>
				_comboBox = comboBox;

			public IEnumerator<ButtonBase> GetEnumerator() =>
				_comboBox._menu.Controls.Cast<ButtonBase>().GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() =>
				GetEnumerator();

			public int Count =>
				_comboBox._menu.Controls.Count;

			public ButtonBase this[int index] =>
				(ButtonBase) _comboBox._menu.Controls[index];

			private readonly ComboBox _comboBox;
		}

		private class MenuValuesAccessor : IReadOnlyList<string>
		{
			public MenuValuesAccessor(MenuItemsAccessor items) =>
				_items = items;

			public IEnumerator<string> GetEnumerator() =>
				_items.Select(_ => _.Text).GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() =>
				GetEnumerator();

			public int Count =>
				_items.Count;

			public string this[int index] =>
				_items[index].Text;

			private readonly MenuItemsAccessor _items;
		}
	}
}