using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace Mtgdb.Controls
{
	public class ShadowedForm : Form
	{
		[DllImport("dwmapi.dll")]
		private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

		[DllImport("dwmapi.dll")]
		private static extern int DwmSetWindowAttribute(IntPtr hWnd, int attr, ref int attrValue, int attrSize);

		[DllImport("dwmapi.dll")]
		public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

		private bool _aeroEnabled = true; // variables for box shadow
		private const int CsDropshadow = 0x00020000;
		private const int WmNcpaint = 0x0085;

		protected struct Margins // struct for box shadow
		{
			[UsedImplicitly]
			public int LeftWidth;

			[UsedImplicitly]
			public int RightWidth;

			[UsedImplicitly]
			public int TopHeight;

			[UsedImplicitly]
			public int BottomHeight;
		}

		protected override CreateParams CreateParams
		{
			get
			{
				_aeroEnabled = checkAeroEnabled();

				var result = base.CreateParams;
				if (!_aeroEnabled)
					result.ClassStyle |= CsDropshadow;

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
			if (m.Msg == WmNcpaint)
				setShadow();

			base.WndProc(ref m);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			setShadow();
			base.OnSizeChanged(e);
		}

		protected virtual bool FixShadowTransparency => false;

		private void setShadow()
		{
			if (!_aeroEnabled)
				return;

			int v = 2;
			DwmSetWindowAttribute(Handle, 2, ref v, 4);

			int marginVal = FixShadowTransparency ? -1 : 1;
			var margins = new Margins
			{
				BottomHeight = marginVal,
				LeftWidth = marginVal,
				RightWidth = marginVal,
				TopHeight = marginVal,
			};

			DwmExtendFrameIntoClientArea(Handle, ref margins);
		}
	}
}