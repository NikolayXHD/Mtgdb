using System;
using IWshRuntimeLibrary;

namespace Mtgdb.Util
{
	public static class Shortcut
	{
		public static bool CreateApplicationShortcut(FsPath shortcutPath, FsPath exePath, FsPath iconPath)
		{
			if (shortcutPath.IsFile())
				shortcutPath.DeleteFile();

			var wsh = new WshShell();
			var shortcut = wsh.CreateShortcut(shortcutPath.Value) as IWshShortcut;
			FsPath bin = exePath.Parent();

			if (shortcut == null)
			{
				Console.WriteLine("Failed to create shortcut at {0}: {1}.{2} returned null",
					shortcutPath, nameof(WshShell), nameof(wsh.CreateShortcut));
				return false;
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
				Console.WriteLine("Failed to create shortcut {0}: {1}", shortcutPath, ex);
				return false;
			}

			return true;
		}
	}
}
