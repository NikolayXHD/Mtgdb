using System.Collections.Generic;

namespace Mtgdb
{
	public static class Sequence
	{
		public static IEnumerable<T> From<T>(T value)
		{
			yield return value;
		}

		public static IEnumerable<T> From<T>(T value1, T value2)
		{
			yield return value1;
			yield return value2;
		}

		public static IEnumerable<T> From<T>(T value1, T value2, T value3)
		{
			yield return value1;
			yield return value2;
			yield return value3;
		}

		public static IEnumerable<T> From<T>(T value1, T value2, T value3, T value4)
		{
			yield return value1;
			yield return value2;
			yield return value3;
			yield return value4;
		}

		public static IEnumerable<T> From<T>(T value1, T value2, T value3, T value4, T value5)
		{
			yield return value1;
			yield return value2;
			yield return value3;
			yield return value4;
			yield return value5;
		}

		public static int IndexOf<T>(this T[] values, T val) =>
			((IList<T>)values).IndexOf(val);

		public static T[] Array<T>(params T[] values) =>
			values;
	}
}
