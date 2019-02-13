using System;
using System.ComponentModel;
using System.Windows.Forms;
using Mtgdb.Controls.Properties;

namespace Mtgdb.Controls
{
	public class PseudoCheckBox : IComponent
	{
		public PseudoCheckBox(ButtonBase box, ButtonSubsystem buttonSubsystem)
		{
			Box = box;
			ButtonSubsystem = buttonSubsystem;

			Box.AutoSize = true;
			Box.AutoCheck = true;
			Box.TextImageRelation = TextImageRelation.ImageBeforeText;

			box.ButtonImages = new ButtonImages(
				Resources.unchecked_32.ScaleBy(0.5f),
				Resources.checked_32.ScaleBy(0.5f));
		}

		internal readonly ButtonBase Box;
		internal readonly ButtonSubsystem ButtonSubsystem;
		
		/// <summary>
		/// Does nothing because this class owns nothing
		/// </summary>
		public void Dispose() =>
			Disposed?.Invoke(this, EventArgs.Empty);

		public ISite Site { get; set; }
		public event EventHandler Disposed;
	}
}