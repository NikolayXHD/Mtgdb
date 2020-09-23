using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace DrawingEx.ColorManagement
{
	/// <summary>
	/// Zusammenfassung für ColorDialogEx.
	/// </summary>
	public class ColorDialogEx:Component
	{
		#region variables
		private Color _color=Color.White;
		private ColorPicker.Mode _mode=ColorPicker.Mode.HSV_RGB;
		private ColorPicker.Fader _fader=ColorPicker.Fader.HSV_H;
		#endregion
		public ColorDialogEx()
		{
		}
		public DialogResult ShowDialog()
		{
			return ShowDialog(null);
		}
		public DialogResult ShowDialog(IWin32Window owner)
		{
			DialogResult res=DialogResult.Cancel;
			using(ColorPicker frm=new ColorPicker(_mode,_fader))
			{
				frm.Color=ColorModels.XYZ.FromRGB(_color);
				res=frm.ShowDialog(owner);
				if(res==DialogResult.OK)
				{
					_color=frm.Color.ToRGB();
					_mode=frm.SecondaryMode;
					_fader=frm.PrimaryFader;
				}
			}
			return res;
		}
		#region properties
		[DefaultValue(typeof(Color),"White")]
		public Color Color
		{
			get{return _color;}
			set{_color=value;}
		}
		#endregion
	}
}
