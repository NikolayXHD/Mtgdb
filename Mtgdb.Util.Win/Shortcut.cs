using System;
using IWshRuntimeLibrary;

namespace Mtgdb.Util
{
	public static class Shortcut
	{
		public static void CreateApplicationShortcut(FsPath exePath, FsPath iconPath, FsPath shortcutPath)
		{
			if (shortcutPath.IsFile())
				shortcutPath.DeleteFile();

			var wsh = new WshShell();
			IWshShortcut shortcut;
			try
			{
				shortcut = wsh.CreateShortcut(shortcutPath.Value) as IWshShortcut;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(
					"Failed to create shortcut object {0} at {1}: {2}",
					exePath, shortcutPath, ex);
				return;
			}

			FsPath bin = exePath.Parent();

			if (shortcut == null)
			{
				Console.Error.WriteLine("Failed to create shortcut {0} at {1}: {2}.{3} returned null",
					exePath, shortcutPath, nameof(WshShell), nameof(wsh.CreateShortcut));
				return;
			}

			shortcut.Arguments = "";
			shortcut.TargetPath = exePath.Value;
			shortcut.WindowStyle = 1;

			shortcut.Description = "Application to search MTG cards and build decks";
			shortcut.WorkingDirectory = bin.Value;

			if (iconPath.HasValue())
				shortcut.IconLocation = iconPath.Value;

			try
			{
				shortcut.Save();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Failed to create shortcut {0} at {1}: {2}", exePath, shortcutPath, ex);
			}
		}
	}
}
