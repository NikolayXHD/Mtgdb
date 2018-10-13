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
				Alignment = Alignment,
				Margin = Margin,
				ShowOnlyWhenHotTracked = ShowOnlyWhenHotTracked,
				BreaksLayout = BreaksLayout
			};
		}



		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap IconTransp { get; set; }

		private Bitmap _icon;
		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap Icon
		{
			get => _icon;
			set
			{
				_icon = value;
				IconTransp = IconTransp ?? value?.SetOpacity(1f - HotTrackOpacityDelta);
			}
		}

		[Category("Settings")]
		[DefaultValue(null)]
		public ContentAlignment? Alignment { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public bool? BreaksLayout { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Size? Margin { get; set; }

		[Category("Settings"), DefaultValue(null)]
		public bool? ShowOnlyWhenHotTracked { get; set; }

		private const float HotTrackOpacityDelta = 0.2f;
	}
}