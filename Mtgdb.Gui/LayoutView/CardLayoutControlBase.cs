using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class CardLayoutControlBase : LayoutControl
	{
		public UiModel Ui { get; set; }

		public override void CopyFrom(LayoutControl other)
		{
			base.CopyFrom(other);
			Ui = ((CardLayoutControlBase) other).Ui;
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// CardLayoutControlBase
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Name = "CardLayoutControlBase";
			this.ResumeLayout(false);

		}
	}
}