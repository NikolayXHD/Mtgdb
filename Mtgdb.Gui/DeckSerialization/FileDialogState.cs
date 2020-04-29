namespace Mtgdb.Gui
{
	public class FileDialogState
	{
		public FsPath LastFile { get; set; }

		public FsPath LastSavedFile
		{
			get => _lastSavedFile.Or(LastFile);

			set
			{
				_lastSavedFile = value;
				LastFile = value;
			}
		}

		public FsPath LastLoadedFile
		{
			get => _lastLoadedFile.Or(LastFile);

			set
			{
				_lastLoadedFile = value;
				LastFile = value;
			}
		}

		private FsPath _lastSavedFile;
		private FsPath _lastLoadedFile;
	}
}
