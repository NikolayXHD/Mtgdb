namespace Mtgdb.Gui
{
	public class DeckFile
	{
		public DeckFile(string file, int formatIndex)
		{
			File = file;
			FormatIndex = formatIndex;
		}

		public string File { get; private set; }
		public int FormatIndex { get; private set; }
	}
}