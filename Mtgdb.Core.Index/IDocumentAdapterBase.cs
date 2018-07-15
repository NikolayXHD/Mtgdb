using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers.Classic;

namespace Mtgdb.Index
{
	public interface IDocumentAdapterBase
	{
		IEnumerable<string> GetUserFields();
		IEnumerable<string> GetFieldLanguages(string userField);

		bool IsUserField(string userField);
		bool IsIntField(string userField);
		bool IsFloatField(string userField);
		bool IsIndexedInSpellchecker(string userField);
		bool IsStoredInSpellchecker(string userField, string lang);
		string GetSpellcheckerFieldIn(string userField, string lang);
		string GetFieldLocalizedIn(string userField, string lang);
		bool IsAnyField(string field);
		bool IsSuggestAnalyzedIn(string userField, string lang);
		bool IsNotAnalyzed(string userField);

		Analyzer CreateAnalyzer();
		QueryParser CreateQueryParser(string language, Analyzer analyzer);
	}
}