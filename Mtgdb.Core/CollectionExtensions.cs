using System.Collections.Generic;
using System.Linq;

namespace Mtgdb
{
	public static class CollectionExtensions
	{
		public static TVal TryGet<TKey, TVal>(this IDictionary<TKey, TVal> dict, TKey key)
		{
			if (key == null)
				return default(TVal);

			TVal val;
			dict.TryGetValue(key, out val);

			return val;
		}

		public static TVal TryPeek<TVal>(this Stack<TVal> stack)
		{
			if (stack.Count == 0)
				return default(TVal);

			return stack.Peek();
		}

		public static TVal Get<TVal>(this HashSet<TVal> collection, TVal value)
		{
			var result = collection.First(_ => collection.Comparer.Equals(_, value));
			return result;
		}

		public static bool Contains<TVal>(this HashSet<TVal> collection, TVal? value)
			where TVal: struct
		{
			return value.HasValue && collection.Contains(value.Value);
		}

		public static char? TryGetFirst(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;

			return value[0];
		}

		public static char? TryGetLast(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;

			return value[value.Length - 1];
		}

		public static TVal TryGetLast<TVal>(this IList<TVal> list)
		{
			if (list.Count == 0)
				return default(TVal);

			return list[list.Count - 1];
		}

		public static List<TVal> SkipFirst<TVal>(this List<TVal> list, int countToSkip)
		{
			if (countToSkip <= 0)
				return list;

			var result = new List<TVal>();
			result.AddRange(list.Skip(countToSkip));

			return result;
		}

		public static List<TVal> TakeFirst<TVal>(this List<TVal> list, int countToTake)
		{
			if (countToTake == list.Count)
				return list;

			var result = new List<TVal>();
			result.AddRange(list.Take(countToTake));

			return result;
		}

		public static TVal TryGet<TVal>(this IList<TVal> list, int index)
		{
			if (index < 0 || index >= list.Count)
				return default(TVal);

			return list[index];
		}

		public static Dictionary<TKey, TVal> ToDictionary<TKey, TVal>(this IEnumerable<KeyValuePair<TKey, TVal>> dict, IEqualityComparer<TKey> equalityComparer = null)
		{
			if (equalityComparer == null)
				return dict.ToDictionary(_ => _.Key, _ => _.Value);

			return dict.ToDictionary(_ => _.Key, _ => _.Value, equalityComparer);
		}

		public static bool IsEqualTo<TKey, TVal>(this Dictionary<TKey, TVal> dict, Dictionary<TKey, TVal> dict2)
		{
			if ((dict?.Count ?? 0) != (dict2?.Count ?? 0))
				return false;

			if ((dict?.Count ?? 0) == 0)
				return true;

			// ReSharper disable PossibleNullReferenceException
			foreach (var pair in dict)
			{
				TVal value2;

				if (!dict2.TryGetValue(pair.Key, out value2))
					return false;

				// ReSharper enable PossibleNullReferenceException
				if (!value2.Equals(pair.Value))
					return false;
			}

			return true;
		}


		public static string NonEmpty(this string value)
		{
			if (value.Length == 0)
				return null;

			return value;
		}

		public static IList<TVal> NonEmpty<TVal>(this IList<TVal> value)
		{
			if (value.Count == 0)
				return null;

			return value;
		}

		public static ICollection<TVal> NonEmpty<TVal>(this ICollection<TVal> value)
		{
			if (value.Count == 0)
				return null;

			return value;
		}

		public static List<TVal> NonEmpty<TVal>(this List<TVal> value)
		{
			if (value.Count == 0)
				return null;

			return value;
		}
	}
}