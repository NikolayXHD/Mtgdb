using System;

namespace Mtgdb
{
	public static class FileUtil
	{
		public static void SafeCreateFile(FsPath filename, Action<FsPath> createFile)
		{
			if (!filename.IsFile())
			{
				createFile(filename);
				return;
			}

			FsPath backupName = filename.Concat(".bak");
			if (backupName.IsFile())
				backupName.DeleteFile();
			filename.MoveFileTo(backupName);
			try
			{
				createFile(filename);
			}
			catch (Exception)
			{
				backupName.MoveFileTo(filename);
				throw;
			}

			backupName.DeleteFile();
		}
	}
}
