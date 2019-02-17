using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Mtgdb.Controls.Properties;

namespace Mtgdb.Controls
{
	public class CheckBox : ButtonBase
	{
		public CheckBox()
		{
			AutoSize = true;
			AutoCheck = true;
			TextImageRelation = TextImageRelation.ImageBeforeText;
			HighlightCheckedOpacity = 0;

			ButtonImages = new ButtonImages(
				Resources.unchecked_32.ScaleBy(0.5f),
				Resources.checked_32.ScaleBy(0.5f));
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Bitmap Image
		{
			get => base.Image;
			set => base.Image = value;
		}

		[DefaultValue(typeof(TextImageRelation), "ImageBeforeText")]
		public override TextImageRelation TextImageRelation
		{
			get => base.TextImageRelation;
			set => base.TextImageRelation = value;
		}

		[DefaultValue(true)]
		public override bool AutoSize
		{
			get => base.AutoSize;
			set => base.AutoSize = value;
		}

		[DefaultValue(true)]
		public override bool AutoCheck
		{
			get => base.AutoCheck;
			set => base.AutoCheck = value;
		}

		[DefaultValue(0)]
		public override int HighlightCheckedOpacity
		{
			get => base.HighlightCheckedOpacity;
			set => base.HighlightCheckedOpacity = value;
		}
	}
}