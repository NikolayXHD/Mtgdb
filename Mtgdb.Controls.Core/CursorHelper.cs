using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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

		public static Cursor CreateCursor(Bitmap bmp, Point hotSpot) =>
			Runtime.IsMono
				? createCursorMono(bmp, hotSpot)
				: createCursorWindows(bmp, hotSpot);

		private static Cursor createCursorWindows(Bitmap bmp, Point hotSpot)
		{
			IntPtr ptr = bmp.GetHicon();
			IconInfo tmp = new IconInfo();
			GetIconInfo(ptr, ref tmp);
			tmp.XHotspot = hotSpot.X;
			tmp.YHotspot = hotSpot.Y;
			tmp.FIcon = false;
			ptr = CreateIconIndirect(ref tmp);
			return new Cursor(ptr);
		}

		private static Cursor createCursorMono(Bitmap bmp, Point hotSpot)
		{
			// https://en.wikipedia.org/wiki/ICO_(file_format)
			var bmpData = bmp.LockBits(new Rectangle(default, bmp.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

			try
			{
				int numBytes = bmpData.Stride * bmpData.Height;
				var bgraValues = new byte[numBytes];
				Marshal.Copy(bmpData.Scan0, bgraValues, 0, numBytes);

				int max = Math.Max(bmp.Width, bmp.Height);

				if (max > 256)
					throw new NotSupportedException();

				byte iconSizeByte = _sizes.FirstOrDefault(s => s >= max); // 0 means 256
				int iconSizeI = iconSizeByte == 0 ? 256 : iconSizeByte;
				const int bytesPerPixel = 4;
				const int bytesPerPixelSource = 4;
				byte[] emptyPixel = new byte[bytesPerPixel];

				using (var stream = new MemoryStream())
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write((ushort)0); // idReserved
					writer.Write((ushort)2); // idType, 1 = .ico 2 = .cur
					writer.Write((ushort)1); // idCount

					writer.Write(iconSizeByte);
					writer.Write(iconSizeByte);
					writer.Write((byte)0); // colorCount
					writer.Write((byte)0); // reserved
					writer.Write((ushort)hotSpot.X);
					writer.Write((ushort)hotSpot.Y);

					var pixelsCount = iconSizeI * iconSizeI;
					var xorLength = pixelsCount * bytesPerPixel;
					var andLength = pixelsCount / 8 * 2;

					writer.Write((uint)(40 + xorLength + andLength)); // sizeInBytes
					writer.Write((uint)stream.Position + sizeof(uint)); // fileOffset = 22 = 0x16

					writer.Write(40u); // cursorInfoHeader.biSize
					writer.Write((int)iconSizeI); // cursorInfoHeader.biWidth
					writer.Write((int)iconSizeI * 2); // cursorInfoHeader.biHeight
					writer.Write((ushort)1); // cursorInfoHeader.biPlanes
					writer.Write((ushort)(8 * bytesPerPixel)); // cursorInfoHeader.biBitCount
					writer.Write(0u); // cursorInfoHeader.biCompression
					writer.Write(0u); // cursorInfoHeader.biSizeImage
					writer.Write(0); // cursorInfoHeader.biXPelsPerMeter;
					writer.Write(0); // cursorInfoHeader.biYPelsPerMeter;
					writer.Write(0u); // cursorInfoHeader.biClrUsed = binaryReader2.ReadUInt32();
					writer.Write(0u); // cursorInfoHeader.biClrImportant = binaryReader2.ReadUInt32();

					using (var andMask = new MemoryStream(andLength))
					{
						byte def = 255;

						for (int j = 0; j < iconSizeI; j++)
						{
							int y = iconSizeI - 1 - j;
							byte curByte = def;

							for (int i = 0; i < iconSizeI; i++)
							{
								var bitIndex = 7 - i % 8;

								if (i < bmp.Width && y < bmp.Height)
								{
									var p = y * bmpData.Stride + i * bytesPerPixelSource;
									stream.Write(bgraValues, p, bytesPerPixel);

									if (bgraValues[p + 3] > 0)
										curByte = (byte)(curByte & ~(1 << bitIndex));
								}
								else
									stream.Write(emptyPixel, 0, emptyPixel.Length);

								if (bitIndex == 0)
								{
									andMask.WriteByte(curByte);
									curByte = def;
								}
							}
						}

						for (int j = 0; j < iconSizeI; j++)
							for (int b = 0; b < iconSizeI / 8; b++)
								andMask.WriteByte(def);

						andMask.Seek(0, SeekOrigin.Begin);
						andMask.CopyTo(stream);
					}

					stream.Seek(0, SeekOrigin.Begin);

					// File.WriteAllBytes("/home/kolia/Documents/stream", stream.ToArray());
					// stream.Seek(0, SeekOrigin.Begin);

					var cursor = new Cursor(stream);
					return cursor;
				}
			}
			finally
			{
				bmp.UnlockBits(bmpData);
			}
		}

		private static readonly byte[] _sizes = { 16, 32, 64, 128 };
	}
}
