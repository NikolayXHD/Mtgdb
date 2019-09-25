using System;
using System.IO;

namespace Mtgdb
{
	public static class FileUtil
	{
		public static void SafeCreateFile(string filename, Action<string> createFile)
		{
			if (!File.Exists(filename))
			{
				createFile(filename);
				return;
			}

			string backupName = filename + ".bak";
			if (File.Exists(backupName))
				File.Delete(backupName);
			File.Move(filename, backupName);
			try
			{
				createFile(filename);
			}
			catch (Exception)
			{
				File.Move(backupName, filename);
				throw;
			}

			File.Delete(backupName);
		}
	}
}
