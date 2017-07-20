namespace Mtgdb.Gui
{
	public interface IDeckFormatter
	{
		string Description { get; }
		string FileNamePattern { get; }

		GuiSettings ImportDeck(string serialized);
		string ExportDeck(string name, GuiSettings current);

		bool ValidateFormat(string serialized);

		bool SupportsExport { get; }
		bool SupportsImport { get; }
	}
}