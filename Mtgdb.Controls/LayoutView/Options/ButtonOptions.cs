using System.ComponentModel;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class ButtonOptions
	{
		public ButtonOptions Clone()
		{
			return new ButtonOptions
			{
				Icon = Icon,
				UnfocusedOpacity = UnfocusedOpacity,
				Alignment = Alignment,
				Margin = Margin,
				ShowOnlyWhenHotTracked = ShowOnlyWhenHotTracked
			};
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap IconTransp { get; private set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap Icon
		{
			get => _icon;
			set
			{
				_icon = value;
				updateTranspIcon(value);
			}
		}

		private void updateTranspIcon(Bitmap value)
		{
			IconTransp = value?.SetOpacity(UnfocusedOpacity);
		}

		[Category("Settings")]
		[DefaultValue(0.5f)]
		public float UnfocusedOpacity
		{
			get => _unfocusedOpacity;
			set
			{
				_unfocusedOpacity = value;

				if (_icon != null)
					updateTranspIcon(_icon);
			}
		}

		[Category("Settings")]
		[DefaultValue(null)]
		public ContentAlignment? Alignment { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Size? Margin { get; set; }

		[Category("Settings"), DefaultValue(null)]
		public bool? ShowOnlyWhenHotTracked { get; set; }

		private Bitmap _icon;
		private float _unfocusedOpacity = 0.5f;
	}
}