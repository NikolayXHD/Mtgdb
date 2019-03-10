using System.ComponentModel;
using System.Drawing;
using JetBrains.Annotations;

namespace Mtgdb.Controls
{
	public class ButtonOptions
	{
		[UsedImplicitly] // to search usages in IDE
		public ButtonOptions()
		{
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
				IconTransp = value?.SetOpacity(1 - HotTrackOpacityDelta);
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