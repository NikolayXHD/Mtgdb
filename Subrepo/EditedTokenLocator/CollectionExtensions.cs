using System.Collections.Generic;

namespace Lucene.Net.Contrib
{
	internal static class CollectionExtensions
	{
		public static TVal TryPeek<TVal>(this Stack<TVal> stack) =>
			stack.Count == 0
				? default
				: stack.Peek();

		public static bool ContainsString(this ICollection<char> chars, string value) =>
			value.Length == 1 && chars.Contains(value[0]);

		public static TVal TryGetLast<TVal>(this IList<TVal> list) =>
			list.Count == 0
				? default
				: list[list.Count - 1];
	}
}
