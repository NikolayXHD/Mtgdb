using System;
using System.ComponentModel;
using System.Windows.Forms;
using Mtgdb.Controls.Properties;

namespace Mtgdb.Controls
{
	public class CheckBox : ButtonBase
	{
		public CheckBox()
			:base()
		{
			AutoSize = true;
			AutoCheck = true;
			TextImageRelation = TextImageRelation.ImageBeforeText;

			ButtonImages = new ButtonImages(
				Resources.unchecked_32.ScaleBy(0.5f),
				Resources.checked_32.ScaleBy(0.5f));
		}
	}
}