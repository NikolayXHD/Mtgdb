using Mtgdb.Dal;
using Mtgdb.Index;

namespace Mtgdb.Controls
{
	public class DeckQueryParser : MtgQueryParser
	{
		public DeckQueryParser(MtgAnalyzer analyzer, DeckDocumentAdapter adapter) 
			: base(analyzer, adapter, CardLocalization.DefaultLanguage)
		{
		}
	}
}