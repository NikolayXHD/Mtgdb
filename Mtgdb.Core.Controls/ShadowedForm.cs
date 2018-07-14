using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class ShadowedForm : Form
	{
		//[DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
		//private static extern IntPtr CreateRoundRectRgn
		//(
		//	int nLeftRect, // x-coordinate of upper-left corner
		//	int nTopRect, // y-coordinate of upper-left corner
		//	int nRightRect, // x-coordinate of lower-right corner
		//	int nBottomRect, // y-coordinate of lower-right corner
		//	int nWidthEllipse, // height of ellipse
		//	int nHeightEllipse // width of ellipse
		// );

		[DllImport("dwmapi.dll")]
		private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

		[DllImport("dwmapi.dll")]
		private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

		[DllImport("dwmapi.dll")]
		public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

		private bool _aeroEnabled = true; // variables for box shadow
		private const int CS_DROPSHADOW = 0x00020000;
		private const int WM_NCPAINT = 0x0085;
		//private const int WM_ACTIVATEAPP = 0x001C;

		private struct MARGINS // struct for box shadow
		{
			public int leftWidth;
			public int rightWidth;
			public int topHeight;
			public int bottomHeight;
		}

		protected override CreateParams CreateParams
		{
			get
			{
				_aeroEnabled = checkAeroEnabled();

				var result = base.CreateParams;
				if (!_aeroEnabled)
					result.ClassStyle |= CS_DROPSHADOW;

				return result;
			}
		}

		private static bool checkAeroEnabled()
		{
			if (Environment.OSVersion.Version.Major >= 6)
			{
				int enabled = 0;
				DwmIsCompositionEnabled(ref enabled);
				return enabled == 1;
			}

			return false;
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_NCPAINT)
				setShadow();

			base.WndProc(ref m);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			setShadow();
			base.OnSizeChanged(e);
		}

		private void setShadow()
		{
			if (!_aeroEnabled)
				return;

			var v = 2;
			DwmSetWindowAttribute(Handle, 2, ref v, 4);
			var margins = new MARGINS
			{
				bottomHeight = 1,
				leftWidth = 1,
				rightWidth = 1,
				topHeight = 1
			};

			DwmExtendFrameIntoClientArea(Handle, ref margins);
		}
	}
}