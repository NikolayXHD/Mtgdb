namespace Mtgdb.Index
{
	public static class DocumentAdapterExtensions
	{
		public static bool IsNumericField(this IDocumentAdapterBase adapter, string userField) =>
			adapter.IsFloatField(userField) || adapter.IsIntField(userField);
	}
}