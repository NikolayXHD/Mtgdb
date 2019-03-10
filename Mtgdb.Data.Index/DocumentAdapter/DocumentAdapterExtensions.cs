namespace Mtgdb.Data
{
	public static class DocumentAdapterExtensions
	{
		public static bool IsNumericField(this IDocumentAdapterBase adapter, string userField) =>
			adapter.IsFloatField(userField) || adapter.IsIntField(userField);

		public static string GetActualField(this IDocumentAdapterBase adapter, string userInputField)
		{
			if (userInputField == null)
				return string.Empty;

			if (adapter.FieldByAlias.TryGetValue(userInputField, out var result))
				return result;

			return userInputField;
		}
	}
}