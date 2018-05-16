using System;

namespace Mtgdb
{
	public static class F
	{
		public static bool IsNull<T>(T val)
			where T : class =>
			val == null;

		public static bool IsNull<T>(T? val)
			where T : struct =>
			val == null;

		public static bool IsNotNull<T>(T val)
			where T : class =>
			val != null;

		public static bool IsNotNull<T>(T? val)
			where T : struct =>
			val != null;

		public static Func<T, bool> Not<T>(Func<T, bool> func) =>
			elem => !func(elem);

		public static bool False(bool val) => !val;
		public static bool True(bool val) => val;

		public static Func<T, bool> IsEqualTo<T>(T val) =>
			elem => Equals(elem, val);

		public static Func<T, bool> IsNotEqualTo<T>(T val) =>
			elem => !Equals(elem, val);
	}
}