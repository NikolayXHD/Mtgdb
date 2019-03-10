namespace Mtgdb.Data.Index
{
	public class DeckQueryParser : MtgQueryParser
	{
		public DeckQueryParser(MtgAnalyzer analyzer, DeckDocumentAdapter adapter) 
			: base(analyzer, adapter, CardLocalization.DefaultLanguage)
		{
		}
	}
}