using System;
using System.Reflection;

namespace Mtgdb
{
	public static class AppDir
	{
		public static FsPath Executable { get; }
		public static FsPath BinVersion { get; }

		public static FsPath Root { get; }

		public static FsPath Update => Root.Join("update");
		public static FsPath Data => Root.Join("data");
		public static FsPath Save => Root.Join("save");
		public static FsPath ColorSchemes => Root.Join("color-schemes");
		public static FsPath History => Root.Join("history");
		public static FsPath Charts => Root.Join("charts");

		public static FsPath Etc => Root.Join("etc");
		public static FsPath GeneralConfigXml => Root.Join("etc", "Mtgdb.Gui.xml");
		public static FsPath DisplayConfigXml => Root.Join("etc", "Mtgdb.Gui.Display.xml");
		public static FsPath BinShadowCopy => Root.Join("bin", "_shadow_copy");

		static AppDir()
		{
			var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
			Executable = getAssemblyFile(assembly);
			BinVersion = Executable.Parent();
			Root = Executable.Parent().Parent().Parent();
		}

		private static FsPath getAssemblyFile(Assembly assembly)
		{
			if (assembly == null)
				return FsPath.None;

			string dllPath = new Uri(assembly.CodeBase).LocalPath;
			return new FsPath(dllPath);
		}

		public static FsPath ToAppRootedPath(this FsPath path)
		{
			if (path.IsPathRooted())
				return path;

			FsPath result = Root.Join(path);
			return result;
		}

		public static string GetVersion() =>
			Assembly.GetExecutingAssembly().GetName().Version.ToString();
	}
}
