using System;
using System.Text.RegularExpressions;

namespace Mtgdb.Controls.Statistics
{
	public class FieldBuilder<TObj>
	{
		public Field<TObj, TVal> Get<TVal>(string fieldName, Func<TObj, TVal> getter, string alias = null)
		{
			return new Field<TObj, TVal>(getter, fieldName, alias ?? fieldName.FromCamelCase().ToLowerInvariant());
		}
	}

	public static class FieldNameFormatter
	{
		private static readonly Regex CamelPattern = new Regex("(\\B[A-Z])", RegexOptions.Compiled);

		public static string FromCamelCase(this string name)
		{
			return CamelPattern.Replace(name, "_$1").ToLowerInvariant();
		}
	}
}
