using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace Mtgdb.Controls
{
	public class ShadowedForm : Form
	{
		public ShadowedForm()
			: this(Mtgdb.Controls.EnableShadow.Yes)
		{
		}

		public ShadowedForm(EnableShadow enableShadow)
		{
			EnableShadow = enableShadow == Mtgdb.Controls.EnableShadow.Yes;
		}

		private bool EnableShadow { get; set; }

		protected override CreateParams CreateParams
		{
			get
			{
				_aeroEnabled = isAeroEnabled();

				var result = base.CreateParams;

				const int csDropshadow = 0x00020000;

				if (!_aeroEnabled && EnableShadow)
					result.ClassStyle |= csDropshadow;

				return result;
			}
		}

		private static bool isAeroEnabled()
		{
			if (!Runtime.IsMono && Environment.OSVersion.Version.Major >= 6)
			{
				int enabled = 0;
				DwmIsCompositionEnabled(ref enabled);
				return enabled == 1;
			}

			return false;
		}

		protected override void WndProc(ref Message m)
		{
			const int wmNcpaint = 0x0085;

			if (m.Msg == wmNcpaint)
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
			if (!_aeroEnabled || !EnableShadow)
				return;

			int v = 2;
			DwmSetWindowAttribute(Handle, 2, ref v, 4);

			int marginVal = FixShadowTransparency ? -1 : 1;
			var margins = new Margins
			{
				BottomHeight = marginVal,
				LeftWidth = marginVal,
				RightWidth = marginVal,
				TopHeight = marginVal
			};

			DwmExtendFrameIntoClientArea(Handle, ref margins);
		}

		private bool _aeroEnabled = true; // variables for box shadow

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

		[DllImport("dwmapi.dll")]
		private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

		[DllImport("dwmapi.dll")]
		private static extern int DwmSetWindowAttribute(IntPtr hWnd, int attr, ref int attrValue, int attrSize);

		[DllImport("dwmapi.dll")]
		private static extern int DwmIsCompositionEnabled(ref int pfEnabled);
	}
}