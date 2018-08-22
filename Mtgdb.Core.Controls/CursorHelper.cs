using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace Mtgdb.Controls
{
	public static class CursorHelper
	{
		public struct IconInfo
		{
			[UsedImplicitly]
			public bool FIcon;
			[UsedImplicitly]
			public int XHotspot;
			[UsedImplicitly]
			public int YHotspot;
			[UsedImplicitly]
			public IntPtr HbmMask;
			[UsedImplicitly]
			public IntPtr HbmColor;
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

		[DllImport("user32.dll")]
		public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

		public static Cursor CreateCursor(Bitmap bmp, Size hotSpot)
		{
			return CreateCursor(bmp, hotSpot.Width, hotSpot.Height);
		}

		public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
		{
			IntPtr ptr = bmp.GetHicon();
			IconInfo tmp = new IconInfo();
			GetIconInfo(ptr, ref tmp);
			tmp.XHotspot = xHotSpot;
			tmp.YHotspot = yHotSpot;
			tmp.FIcon = false;
			ptr = CreateIconIndirect(ref tmp);
			return new Cursor(ptr);
		}
	}
}