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
	public class PseudoComboBox : IComponent
	{
		public PseudoComboBox(ButtonSubsystem buttonSubsystem, ButtonBase owner, Control parent)
		{
			Owner = owner;

			Owner.AutoSize = false;
			Owner.AutoCheck = false;
			Owner.HighlightCheckedOpacity = 64;
			Owner.HighlightMouseOverOpacity = 64;
			Owner.TextImageRelation = TextImageRelation.TextBeforeImage;
			Owner.TextAlign = StringAlignment.Near;
			Owner.VisibleAllBorders = true;
			Owner.Padding = new Padding(owner.Padding.Left, 0, 0, 0);
			Owner.BackColor = SystemColors.Window;
			Owner.ForeColor = SystemColors.WindowText;

			var dropDownImg = Resources.drop_down_48.ScaleBy(0.5f);
			Owner.ButtonImages = new ButtonImages(dropDownImg, dropDownImg);

			_menu = new BorderedFlowLayoutPanel
			{
				FlowDirection = FlowDirection.TopDown,
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink,
				Visible = false,
				Margin = new Padding(0, 0, 0, 0),
				BackColor = SystemColors.Window
			};

			parent.Controls.Add(_menu);

			buttonSubsystem.SetupPopup(new Popup(_menu, owner));

			_menuItemsAccessor = new MenuItemsAccessor(this);
			_menuValuesAccessor = new MenuValuesAccessor(_menuItemsAccessor);
		}

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
				Owner.Text = EmptySelectionText;
			else if (_selectedIndex < _menu.Controls.Count)
				Owner.Text = _menu.Controls[_selectedIndex].Text;
			else
				throw new IndexOutOfRangeException();
		}

		private void updateItemSizes()
		{
			var sizes = MenuItems
				.Select(_ => _.MeasureContent())
				.ToArray();

			var maxWidth = sizes.Append(Owner.Size).Max(_ => _.Width);

			MenuItems.ForEach((item, i) =>
				item.Size = new Size(maxWidth, sizes[i].Height));
		}

		public void Dispose()
		{
			clear();
			_menu.Dispose();
			Disposed?.Invoke(this, EventArgs.Empty);
		}



		public ButtonBase Owner { get; }

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
			MenuItemsCreated?.Invoke(this, EventArgs.Empty);

			_menu.ResumeLayout(false);
			_menu.PerformLayout();

			SelectedIndex = Math.Min(SelectedIndex, values.Count - 1);
		}

		internal IReadOnlyList<ButtonBase> MenuItems =>
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

		private readonly string _emptySelectionText = String.Empty;
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

		public ISite Site { get; set; }

		public event EventHandler SelectedIndexChanged;
		public event EventHandler MenuItemsCreated;
		public event EventHandler Disposed;

		private readonly BorderedFlowLayoutPanel _menu;
		private readonly MenuValuesAccessor _menuValuesAccessor;
		private readonly MenuItemsAccessor _menuItemsAccessor;



		private class MenuItemsAccessor : IReadOnlyList<ButtonBase>
		{
			public MenuItemsAccessor(PseudoComboBox comboBox) =>
				_comboBox = comboBox;

			public IEnumerator<ButtonBase> GetEnumerator() =>
				_comboBox._menu.Controls.Cast<ButtonBase>().GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() =>
				GetEnumerator();

			public int Count =>
				_comboBox._menu.Controls.Count;

			public ButtonBase this[int index] =>
				(ButtonBase) _comboBox._menu.Controls[index];

			private readonly PseudoComboBox _comboBox;
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