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
				HotTrackOpacityDelta = HotTrackOpacityDelta,
				Icon = Icon,
				Alignment = Alignment,
				Margin = Margin,
				ShowOnlyWhenHotTracked = ShowOnlyWhenHotTracked,
			};
		}



		private void updateTranspIcon(Bitmap value)
		{
			IconTransp = value?.SetOpacity(1f - _hotTrackOpacityDelta);
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

		[Category("Settings")]
		[DefaultValue(0.25f)]
		public float HotTrackOpacityDelta
		{
			get => _hotTrackOpacityDelta;
			set
			{
				_hotTrackOpacityDelta = value;
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
		private float _hotTrackOpacityDelta = 0.25f;
	}
}