using System;
using System.IO;
using System.Reflection;

namespace Mtgdb
{
	public static class AppDir
	{
		public static string Executable { get; }
		public static string BinVersion { get; }

		public static string Root { get; }

		public static string Update => Root.AddPath("update");
		public static string Data => Root.AddPath("data");
		public static string Save => Root.AddPath("save");
		public static string ColorSchemes => Root.AddPath("color-schemes");
		public static string History => Root.AddPath("history");

		public static string Etc => Root.AddPath("etc");
		public static string GeneralConfigXml => Root.AddPath("etc").AddPath("Mtgdb.Gui.xml");
		public static string DisplayConfigXml => Root.AddPath("etc").AddPath("Mtgdb.Gui.Display.xml");
		public static string BinShadowCopy => Root.AddPath("bin").AddPath("_shadow_copy");

		static AppDir()
		{
			var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
			Executable = getAssemblyFile(assembly);
			BinVersion = Executable.Parent();
			Root = Executable.Parent().Parent().Parent();
		}

		private static string getAssemblyFile(Assembly assembly)
		{
			if (assembly == null)
				return null;

			string dllPath = new Uri(assembly.CodeBase).LocalPath;
			return dllPath;
		}

		public static string ToAppRootedPath(this string path)
		{
			if (Path.IsPathRooted(path))
				return path;

			var result = Root.AddPath(path);
			return result;
		}

		public static string GetVersion() =>
			Assembly.GetExecutingAssembly().GetName().Version.ToString();
	}
}