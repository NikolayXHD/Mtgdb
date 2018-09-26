using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DrawingEx.IconEncoder
{
	/// <summary>
	/// header of multiple icon file
	/// </summary>
	[StructLayout(LayoutKind.Sequential,Pack=2)]
	public struct ICONDIR
	{
		#region fields
		public short idReserved;
		public short idType;
		public short idCount;
		#endregion
		public ICONDIR(short idcount)
		{
			this.idCount=idcount;
			this.idReserved=0;
			this.idType=1;
		}
		public unsafe ICONDIR(Stream str)
		{
			if(str==null)
				throw new ArgumentNullException("str","null reference in ICONDIR.ctor");
			byte[] buffer=new byte[sizeof(ICONDIR)];
			str.Read(buffer,0,buffer.Length);
			fixed(byte* ptbuffer=buffer)
			{
				this=*(ICONDIR*)ptbuffer;
			}
		}
		public unsafe void Write(Stream str)
		{
			if(str==null)
				throw new ArgumentNullException("str","null reference in ICONDIR.Write");
			byte[] buffer=new byte[sizeof(ICONDIR)];
			fixed(ICONDIR* ptthis=&this)
			{
				Marshal.Copy((IntPtr)ptthis,buffer,0,buffer.Length);
			}
			str.Write(buffer,0,buffer.Length);
		}
	}
	/// <summary>
	/// header of icon image inside .ico
	/// </summary>
	[StructLayout(LayoutKind.Sequential,Pack=2)]
	public struct ICONDIRENTRY
	{
		#region fields
		public byte Width;
		public byte Height;
		public byte ColorCount;
		public byte Reserved;
		public short Planes;
		public short BitsPerPixel;
		public int SizeInBytes;
		public int FileOffset;
		#endregion
		public ICONDIRENTRY(
			byte width,
			byte height,
			byte colorcount,
			short bitsperpixel,
			int sizeinbytes)
		{
			this.Width=width;
			this.Height=height;
			this.ColorCount=colorcount;
			this.Reserved=0;
			this.Planes=1;
			this.BitsPerPixel=bitsperpixel;
			this.SizeInBytes=sizeinbytes;
			this.FileOffset=0;
		}
		public unsafe ICONDIRENTRY(Stream str)
		{
			if(str==null)
				throw new ArgumentNullException("str","null reference in ICONDIRENTRY.ctor");
			byte[] buffer=new byte[sizeof(ICONDIRENTRY)];
			str.Read(buffer,0,buffer.Length);
			fixed(byte* ptbuffer=buffer)
			{
				this=*(ICONDIRENTRY*)ptbuffer;
			}
		}
		public unsafe void Write(Stream str)
		{
			if(str==null)
				throw new ArgumentNullException("str","null reference in ICONDIRENTRY.Write");
			byte[] buffer=new byte[sizeof(ICONDIRENTRY)];
			fixed(ICONDIRENTRY* ptthis=&this)
			{
				Marshal.Copy((IntPtr)ptthis,buffer,0,buffer.Length);
			}
			str.Write(buffer,0,buffer.Length);
		}
	}
	/// <summary>
	/// header of icon image inside .dll
	/// </summary>
	[StructLayout(LayoutKind.Sequential,Pack=2)]
	public struct MEMICONDIRENTRY
	{
		#region fields
		public byte Width;
		public byte Height;
		public byte ColorCount;
		public byte Reserved;
		public short Planes;
		public short BitsPerPixel;
		public int SizeInBytes;
		public short ID;
		#endregion	
		public unsafe MEMICONDIRENTRY(Stream str)
		{
			if(str==null)
				throw new ArgumentNullException("str","null reference in MEMICONDIRENTRY.ctor");
			byte[] buffer=new byte[sizeof(MEMICONDIRENTRY)];
			str.Read(buffer,0,buffer.Length);
			fixed(byte* ptbuffer=buffer)
			{
				this=*(MEMICONDIRENTRY*)ptbuffer;
			}		
		}
		public unsafe void Write(Stream str)
		{
			if(str==null)
				throw new ArgumentNullException("str","null reference in MEMICONDIRENTRY.Write");
			byte[] buffer=new byte[sizeof(MEMICONDIRENTRY)];
			fixed(MEMICONDIRENTRY* ptthis=&this)
			{
				Marshal.Copy((IntPtr)ptthis,buffer,0,buffer.Length);
			}
			str.Write(buffer,0,buffer.Length);
		}
	}
	/// <summary>
	/// header of bitmap data
	/// </summary>
	[StructLayout(LayoutKind.Sequential,Pack=2)]
	public struct BITMAPINFOHEADER
	{
		#region fields
		public int Size;
		public int Width;
		public int Height;
		public short Planes;
		public short BitCount;
		public int Compression;
		public int SizeImage;
		public int XPixelsPerMeter;
		public int YPixelsPerMeter;
		public int ColorsUsed;
		public int ColorsImportant;
		#endregion
		public BITMAPINFOHEADER(
			System.Drawing.Size sz,
			short bitcount,
			int sizeimage,
			int colorcount)
		{
			this.Size=40;
			this.Width=sz.Width;
			this.Height=sz.Height*2;
			this.Planes=1;
			this.BitCount=bitcount;
			this.Compression=0;
			this.SizeImage=sizeimage;
			this.XPixelsPerMeter=0;
			this.YPixelsPerMeter=0;
			this.ColorsUsed=
				this.ColorsImportant=colorcount;
		}
		public unsafe BITMAPINFOHEADER(Stream str)
		{
			if(str==null)
				throw new ArgumentNullException("str","null reference in BITMAPINFOHEADER.ctor");
			byte[] buffer=new byte[sizeof(BITMAPINFOHEADER)];
			str.Read(buffer,0,buffer.Length);
			fixed(byte* ptbuffer=buffer)
			{
				this=*(BITMAPINFOHEADER*)ptbuffer;
			}
		}
		public unsafe void Write(Stream str)
		{
			if(str==null)
				throw new ArgumentNullException("str","null reference in BITMAPINFOHEADER.Write");
			byte[] buffer=new byte[sizeof(BITMAPINFOHEADER)];
			fixed(BITMAPINFOHEADER* ptthis=&this)
			{
				Marshal.Copy((IntPtr)ptthis,buffer,0,buffer.Length);
			}
			str.Write(buffer,0,buffer.Length);
		}
	}
}
