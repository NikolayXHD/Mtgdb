using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Controls
{
	public abstract class DropDownBase : Popup
	{
		protected DropDownBase()
		{
			BackColor = SystemColors.Window;
			ForeColor = SystemColors.WindowText;

			AutoSize = false;
			AutoCheck = false;

			HighlightCheckedOpacity = 64;
			HighlightMouseOverOpacity = 64;

			VisibleAllBorders = true;

			Menu = new BorderedFlowLayoutPanel // because it's double-buffered
			{
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink,

				BackColor = SystemColors.Window,
				FlowDirection = FlowDirection.TopDown,

				Margin = new Padding(0),
				Padding = new Padding(0),

				Visible = false,
				VisibleBorders = AnchorStyles.None
			};

			MenuControl = Menu;
			MarginTop = -1;

			_menuItemsAccessor = new MenuItemsAccessor(Menu);
			_menuValuesAccessor = new MenuValuesAccessor(_menuItemsAccessor);
		}



		public void SetMenuValues(IEnumerable<string> values) =>
			SetMenuValues(values.ToReadOnlyList());

		public void SetMenuValues(IList<string> values) =>
			SetMenuValues(values.AsReadOnlyList());

		public void SetMenuValues(params string[] values) =>
			SetMenuValues(values.AsReadOnlyList());

		public void SetMenuValues(IReadOnlyList<string> values)
		{
			if (MenuValues.SequenceEqual(values))
				return;

			Menu.SuspendLayout();

			clear();

			for (int i = 0; i < values.Count; i++)
			{
				string text = values[i];
				Menu.Controls.Add(CreateMenuItem(text, i));
			}

			UpdateMenuSize();
			updateItemBorders();

			MenuItemsCreated?.Invoke(this, EventArgs.Empty);

			Menu.ResumeLayout(false);
			Menu.PerformLayout();

			SelectedIndex = Math.Min(SelectedIndex, values.Count - 1);

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

		internal void UpdateMenuSize()
		{
			var sizes = MenuItems.Select(MeasureMenuItem)
				.ToArray();

			var maxWidth = sizes.DefaultIfEmpty(default).Max(_ => _.Width);

			MenuItems.ForEach((item, i) =>
				item.Size = new Size(maxWidth, sizes[i].Height));
		}


		protected override void Show(bool focus)
		{
			Menu.Parent = this.ParentForm();
			base.Show(focus);
		}

		protected override void Hide(bool focus)
		{
			base.Hide(focus);
			Menu.Parent = null;
		}



		protected virtual Size MeasureMenuItem(ButtonBase menuItem) =>
			menuItem.MeasureContent();

		protected override void Dispose(bool disposing)
		{
			clear();
			Menu.Dispose();
			base.Dispose(disposing);
		}

		protected virtual void OnSelectedIndexChanged() =>
			SelectedIndexChanged?.Invoke(this, EventArgs.Empty);

		private void clear()
		{
			foreach (var item in MenuItems.ToArray())
			{
				Menu.Controls.Remove(item);
				item.Dispose();
			}
		}

		protected virtual ButtonBase CreateMenuItem(string value, int index)
		{
			var result = new ButtonBase
			{
				Text = value,
				AutoCheck = false,
				TextPosition = StringAlignment.Near,
				Margin = new Padding(0),
				Padding = new Padding(2),
				ForeColor = SystemColors.WindowText,
				Checked = index == SelectedIndex
			};

			result.KeyUp += keyUp;
			result.Disposed += disposed;

			void keyUp(object sender, KeyEventArgs e) =>
				MenuItemKeyUp?.Invoke(sender, e);

			void disposed(object s, EventArgs e)
			{
				result.Disposed -= disposed;
				result.KeyUp -= keyUp;
			}

			MenuItemCreated?.Invoke(this, new ControlEventArgs(result));
			return result;
		}

		protected override void HandlePopupItemPressed(ButtonBase sender)
		{
			int index = MenuItems.IndexOf(sender);
			SelectedIndex = index;
			MenuItemPressed?.Invoke(this, new MenuItemEventArgs(MenuItems[index]));
			
			if (CloseMenuOnClick)
				Hide(focus: true);
		}


		private int _selectedIndex = -1;
		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{
				if (_selectedIndex == value)
					return;

				((ButtonBase) Menu.Controls.TryGet(_selectedIndex))
					?.Invoke0(_ => _.Checked = false);

				_selectedIndex = value;

				if (_selectedIndex.IsWithin(0, Menu.Controls.Count - 1))
					((ButtonBase) Menu.Controls[_selectedIndex]).Checked = true;

				MenuItems.ForEach((m, i) => m.Checked = i == _selectedIndex);

				OnSelectedIndexChanged();
			}
		}

		public string SelectedValue =>
			MenuValues.TryGet(SelectedIndex);

		[DefaultValue(typeof(Color), "Window")]
		public override Color BackColor
		{
			get => base.BackColor;
			set => base.BackColor = value;
		}

		[DefaultValue(typeof(Color), "WindowText")]
		public override Color ForeColor
		{
			get => base.ForeColor;
			set => base.ForeColor = value;
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

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IReadOnlyList<ButtonBase> MenuItems =>
			_menuItemsAccessor;



		public event EventHandler MenuItemsCreated;
		public event ControlEventHandler MenuItemCreated;
		public event EventHandler SelectedIndexChanged;
		public event EventHandler<MenuItemEventArgs> MenuItemPressed;
		public event KeyEventHandler MenuItemKeyUp;

		protected readonly FlowLayoutPanel Menu;

		private readonly MenuValuesAccessor _menuValuesAccessor;
		private readonly MenuItemsAccessor _menuItemsAccessor;
	}
}