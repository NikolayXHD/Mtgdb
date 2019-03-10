using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Mtgdb.Controls
{
	public static class ShowSelectedInExplorer
	{
		public static void FileOrFolder(string path, bool edit = false)
		{
			if (path == null) throw new ArgumentNullException(nameof(path));

			var pidl = pathToAbsolutePidl(path);
			try
			{
				shOpenFolderAndSelectItems(pidl, null, edit);
			}
			finally
			{
				NativeMethods.ILFree(pidl);
			}
		}

		public static void FilesOrFolders(string parentDirectory, ICollection<string> filenames)
		{
			if (filenames == null) throw new ArgumentNullException(nameof(filenames));
			if (filenames.Count == 0) return;

			var parentPidl = pathToAbsolutePidl(parentDirectory);
			try
			{
				var parent = pidlToShellFolder(parentPidl);
				var filesPidl = filenames
					.Select(filename => getShellFolderChildrenRelativePidl(parent, filename))
					.ToArray();

				try
				{
					shOpenFolderAndSelectItems(parentPidl, filesPidl, false);
				}
				finally
				{
					foreach (var pidl in filesPidl)
					{
						NativeMethods.ILFree(pidl);
					}
				}
			}
			finally
			{
				NativeMethods.ILFree(parentPidl);
			}
		}

		public static void FilesOrFolders(params string[] paths)
		{
			FilesOrFolders((IEnumerable<string>)paths);
		}

		public static void FilesOrFolders(IEnumerable<string> paths)
		{
			if (paths == null) throw new ArgumentNullException(nameof(paths));

			FilesOrFolders(pathToFileSystemInfo(paths));
		}

		public static void FilesOrFolders(IEnumerable<FileSystemInfo> paths)
		{
			if (paths == null) throw new ArgumentNullException(nameof(paths));
			var pathsArray = paths.ToArray();
			if (!pathsArray.Any()) return;

			var explorerWindows = pathsArray.GroupBy(p => Path.GetDirectoryName(p.FullName));

			foreach (var explorerWindowPaths in explorerWindows)
			{
				var parentDirectory = Path.GetDirectoryName(explorerWindowPaths.First().FullName);
				FilesOrFolders(parentDirectory, explorerWindowPaths.Select(fsi => fsi.Name).ToList());
			}
		}



		private static IntPtr getShellFolderChildrenRelativePidl(IShellFolder parentFolder, string displayName)
		{
			uint pdwAttributes = 0;
			parentFolder.ParseDisplayName(IntPtr.Zero, null, displayName, out uint _, out var ppidl, ref pdwAttributes);

			return ppidl;
		}

		private static IntPtr pathToAbsolutePidl(string path)
		{
			var desktopFolder = NativeMethods.SHGetDesktopFolder();
			return getShellFolderChildrenRelativePidl(desktopFolder, path);
		}

		private static Guid IID_IShellFolder = typeof(IShellFolder).GUID;

		private static IShellFolder pidlToShellFolder(IShellFolder parent, IntPtr pidl)
		{
			var result = parent.BindToObject(pidl, null, ref IID_IShellFolder, out var folder);
			Marshal.ThrowExceptionForHR(result);
			return folder;
		}

		private static IShellFolder pidlToShellFolder(IntPtr pidl)
		{
			return pidlToShellFolder(NativeMethods.SHGetDesktopFolder(), pidl);
		}

		private static void shOpenFolderAndSelectItems(IntPtr pidlFolder, IntPtr[] apidl, bool edit)
		{
			NativeMethods.SHOpenFolderAndSelectItems(pidlFolder, apidl, edit ? 1 : 0);
		}

		private static IEnumerable<FileSystemInfo> pathToFileSystemInfo(IEnumerable<string> paths)
		{
			foreach (var path in paths)
			{
				var fixedPath = path;
				if (fixedPath.EndsWith(Path.DirectorySeparatorChar.ToString())
					|| fixedPath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
				{
					fixedPath = fixedPath.Remove(fixedPath.Length - 1);
				}

				if (Directory.Exists(fixedPath))
				{
					yield return new DirectoryInfo(fixedPath);
				}
				else if (File.Exists(fixedPath))
				{
					yield return new FileInfo(fixedPath);
				}
				else
				{
					throw new FileNotFoundException
					(string.Format("The specified file or folder doesn't exists : {0}", fixedPath),
						fixedPath);
				}
			}
		}



		[Flags]
		private enum SHCONT : ushort
		{
			SHCONTF_CHECKING_FOR_CHILDREN = 0x0010,
			SHCONTF_FOLDERS = 0x0020,
			SHCONTF_NONFOLDERS = 0x0040,
			SHCONTF_INCLUDEHIDDEN = 0x0080,
			SHCONTF_INIT_ON_FIRST_NEXT = 0x0100,
			SHCONTF_NETPRINTERSRCH = 0x0200,
			SHCONTF_SHAREABLE = 0x0400,
			SHCONTF_STORAGE = 0x0800,
			SHCONTF_NAVIGATION_ENUM = 0x1000,
			SHCONTF_FASTITEMS = 0x2000,
			SHCONTF_FLATLIST = 0x4000,
			SHCONTF_ENABLE_ASYNC = 0x8000
		}

		[ComImport,
		Guid("000214E6-0000-0000-C000-000000000046"),
		InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
		ComConversionLoss]
		private interface IShellFolder
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			void ParseDisplayName(IntPtr hwnd, [In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, [Out] out uint pchEaten, [Out] out IntPtr ppidl, [In, Out] ref uint pdwAttributes);
			[PreserveSig]
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int EnumObjects([In] IntPtr hwnd, [In] SHCONT grfFlags, [MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenumIDList);

			[PreserveSig]
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int BindToObject([In] IntPtr pidl, [In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			void BindToStorage([In] ref IntPtr pidl, [In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In] ref Guid riid, out IntPtr ppv);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			void CompareIDs([In] IntPtr lParam, [In] ref IntPtr pidl1, [In] ref IntPtr pidl2);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			void CreateViewObject([In] IntPtr hwndOwner, [In] ref Guid riid, out IntPtr ppv);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			void GetAttributesOf([In] uint cidl, [In] IntPtr apidl, [In, Out] ref uint rgfInOut);


			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			void GetUIObjectOf([In] IntPtr hwndOwner, [In] uint cidl, [In] IntPtr apidl, [In] ref Guid riid, [In, Out] ref uint rgfReserved, out IntPtr ppv);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			void GetDisplayNameOf([In] ref IntPtr pidl, [In] uint uFlags, out IntPtr pName);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			void SetNameOf([In] IntPtr hwnd, [In] ref IntPtr pidl, [In, MarshalAs(UnmanagedType.LPWStr)] string pszName, [In] uint uFlags, [Out] IntPtr ppidlOut);
		}

		[ComImport,
		Guid("000214F2-0000-0000-C000-000000000046"),
		InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IEnumIDList
		{
			[PreserveSig]
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int Next(uint celt, IntPtr rgelt, out uint pceltFetched);

			[PreserveSig]
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int Skip([In] uint celt);

			[PreserveSig]
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int Reset();

			[PreserveSig]
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			int Clone([MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenum);
		}

		private static class NativeMethods
		{
			[DllImport("shell32.dll", EntryPoint = "SHGetDesktopFolder", CharSet = CharSet.Unicode,
				SetLastError = true)]
			private static extern int SHGetDesktopFolder_([MarshalAs(UnmanagedType.Interface)] out IShellFolder ppshf);

			public static IShellFolder SHGetDesktopFolder()
			{
				Marshal.ThrowExceptionForHR(SHGetDesktopFolder_(out var result));
				return result;
			}

			[DllImport("shell32.dll", EntryPoint = "SHOpenFolderAndSelectItems")]
			private static extern int SHOpenFolderAndSelectItems_(
				[In] IntPtr pidlFolder, uint cidl, [In, Optional, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl,
				int dwFlags);

			public static void SHOpenFolderAndSelectItems(IntPtr pidlFolder, IntPtr[] apidl, int dwFlags)
			{
				var cidl = apidl != null ? (uint)apidl.Length : 0U;
				var result = SHOpenFolderAndSelectItems_(pidlFolder, cidl, apidl, dwFlags);
				Marshal.ThrowExceptionForHR(result);
			}

			[DllImport("shell32.dll")]
			public static extern void ILFree([In] IntPtr pidl);
		}
	}
}