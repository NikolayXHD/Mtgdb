namespace Mtgdb.Gui
{
	public class FileDialogState
	{
		public string LastFile { get; set; }

		public string LastSavedFile
		{
			get { return _lastSavedFile ?? LastFile; }

			set
			{
				_lastSavedFile = value;
				LastFile = value;
			}
		}

		public string LastLoadedFile
		{
			get { return _lastLoadedFile ?? LastFile; }

			set
			{
				_lastLoadedFile = value;
				LastFile = value;
			}
		}

		private string _lastSavedFile;
		private string _lastLoadedFile;
	}
}