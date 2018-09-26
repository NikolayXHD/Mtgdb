using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;

namespace DrawingEx.IconEncoder
{
	/// <summary>
	/// kernel32.dll wrapper - resource extracting
	/// </summary>
	internal class Kernel32
	{
		private Kernel32(){}

		//constants
		public const int RT_GROUP_ICON = 14;
		public const int RT_ICON = 3;
		public const int LOAD_LIBRARY_AS_DATAFILE = 0x2;

		//functions
		[DllImport("kernel32.dll", EntryPoint="FindResourceA")]
		public static extern IntPtr FindResource (IntPtr hInstance, int lpName, int lpType);
		[DllImport("kernel32.dll", EntryPoint="FindResourceA")]
		public static extern IntPtr FindResource (IntPtr hInstance, string lpName, int lpType);

		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadResource (IntPtr hInstance, IntPtr hResInfo);

		[DllImport("kernel32.dll")]
		public static extern IntPtr LockResource (IntPtr hResData);

		[DllImport("kernel32.dll")]
		public static extern int SizeofResource (IntPtr hInstance, IntPtr hResInfo);

		[DllImport("kernel32.dll", EntryPoint="EnumResourceNamesA")]
		public static extern int EnumResourceNames (IntPtr hModule, int lpType,[MarshalAs(UnmanagedType.FunctionPtr)]EnumProc lpEnumFunc, IntPtr lParam);

		[DllImport("kernel32.dll", EntryPoint="LoadLibraryExA")]
		public static extern IntPtr LoadLibraryEx (string lpLibFileName, IntPtr hFile, int dwFlags);

		[DllImport("kernel32.dll")]
		public static extern int FreeLibrary (IntPtr hLibModule);

		//delegates
		public delegate bool EnumProc(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam);

		/// <summary>
		/// gives a stream of a locked memory resource
		/// </summary>
		public static Stream GetStreamFromResource(IntPtr hlibrary, IntPtr hresinfo)
		{
			//load resource
			IntPtr hresloaded=LoadResource(hlibrary,hresinfo);
			if(hresloaded==IntPtr.Zero)
				throw new Exception("resource could not be loaded");
			//lock resource
			IntPtr hreslocked=LockResource(hresloaded);
			if(hreslocked==IntPtr.Zero)
				throw new Exception("resource could not be locked");
			//get size
			int size=SizeofResource(hlibrary,hresinfo);
			if (size<1)
				throw new ArgumentException("size invalid");
			//copy into buffer
			byte[] data=new byte[size];
			Marshal.Copy(hreslocked,data,0,size);
			return new MemoryStream(data);
		}
		/// <summary>
		/// gets if the specified pointer is an ID or a pointer to unicode string
		/// </summary>
		public static bool IS_INTRESOURCE(IntPtr value)
		{
			return value.ToInt32()<=0xFFFF;
		}
	}
}
