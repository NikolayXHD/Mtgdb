using System.Collections.Generic;
using Lucene.Net.Documents;

namespace Mtgdb.Data
{
	public interface IDocumentAdapter<out TId, in TObj> : IDocumentAdapterBase
	{
		TId GetId(Document doc);
		IEnumerable<string> GetSpellcheckerValues(TObj obj, string userField, string language);
		Document ToDocument(TObj obj);
	}
}