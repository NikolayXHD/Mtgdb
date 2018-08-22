namespace Mtgdb.Gui
{
	public class DeckFile
	{
		public DeckFile(string file, int formatIndex)
		{
			File = file;
			FormatIndex = formatIndex;
		}

		public string File { get; }
		public int FormatIndex { get; }
	}
}