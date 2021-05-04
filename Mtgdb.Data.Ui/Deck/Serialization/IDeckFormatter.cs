namespace Mtgdb.Ui
{
	public interface IDeckFormatter
	{
		string Description { get; }
		string FileNamePattern { get; }

		Deck ImportDeck(string serialized, bool exact = false);

		string ExportDeck(string name, Deck current, bool exact = false);

		bool ValidateFormat(string serialized);

		bool SupportsExport { get; }
		bool SupportsImport { get; }
		bool SupportsFile { get; }

		bool UseBom { get; }
		string FormatHint { get; }
	}
}
