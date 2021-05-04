namespace Mtgdb.Ui
{
	public class DeckFile
	{
		public DeckFile(FsPath file, int formatIndex)
		{
			File = file;
			FormatIndex = formatIndex;
		}

		public FsPath File { get; }
		public int FormatIndex { get; }
	}
}
