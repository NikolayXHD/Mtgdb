using System;
using System.Collections.Generic;
using System.Linq;

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

		public static Func<T, bool> IsEqualTo<T>(T val, IEqualityComparer<T> cmp) =>
			elem => cmp.Equals(elem, val);

		public static Func<T, bool> IsEqualTo<T>(T val) =>
			elem => Equals(elem, val);

		public static Func<string, bool> IsWithin(IEnumerable<string> values, StringComparer comparer) =>
			elem => values.Any(IsEqualTo(elem, comparer));

		public static Func<T, bool> IsNotEqualTo<T>(T val) =>
			elem => !Equals(elem, val);

		public static Func<T, bool> IsGreaterThan<T>(T val) =>
			elem => Comparer<T>.Default.Compare(elem, val) > 0;

		public static Func<T, bool> IsLessThan<T>(T val) =>
			elem => Comparer<T>.Default.Compare(elem, val) < 0;

		public static TResult Invoke0<TObj, TResult>(this TObj target, Func<TObj, TResult> getter) =>
			getter(target);

		public static TResult Invoke1<TObj, TParam, TResult>(this TObj target, Func<TObj, TParam, TResult> getter, TParam param) =>
			getter(target, param);

		public static TResult Invoke1<TObj, TParam1, TParam2, TResult>(
			this TObj target, Func<TObj, TParam1, TParam2, TResult> getter, TParam1 param1, TParam2 param2) =>
			getter(target, param1, param2);

		public static TResult Invoke2<TObj, TParam, TResult>(this TObj target, Func<TParam, TObj, TResult> getter, TParam param) =>
			getter(param, target);
	}
}