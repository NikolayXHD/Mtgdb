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



		public static Func<T, (T1, T2)> Combine<T, T1, T2>(
			Func<T, T1> func1,
			Func<T, T2> func2)
		{
			return arg => (
				func1(arg),
				func2(arg));
		}

		public static Action<T, (T1, T2)> Combine<T, T1, T2>(
			Action<T, T1> action1,
			Action<T, T2> action2)
		{
			return (arg, par) =>
			{
				action1(arg, par.Item1);
				action2(arg, par.Item2);
			};
		}

		public static Func<(T1, T2), (T1, T2)> Combine<T1, T2>(
			Func<T1, T1> func1,
			Func<T2, T2> func2)
		{
			return arg => (
				func1(arg.Item1),
				func2(arg.Item2));
		}


		public static Func<T, (T1, T2, T3)> Combine<T, T1, T2, T3>(
			Func<T, T1> func1,
			Func<T, T2> func2,
			Func<T, T3> func3)
		{
			return arg => (
				func1(arg),
				func2(arg),
				func3(arg));
		}

		public static Action<T, (T1, T2, T3)> Combine<T, T1, T2, T3>(
			Action<T, T1> action1,
			Action<T, T2> action2,
			Action<T, T3> action3)
		{
			return (arg, par) =>
			{
				action1(arg, par.Item1);
				action2(arg, par.Item2);
				action3(arg, par.Item3);
			};
		}

		public static Func<(T1, T2, T3), (T1, T2, T3)> Combine<T1, T2, T3>(
			Func<T1, T1> func1,
			Func<T2, T2> func2,
			Func<T3, T3> func3)
		{
			return arg => (
				func1(arg.Item1),
				func2(arg.Item2),
				func3(arg.Item3));
		}


		public static Func<T, (T1, T2, T3, T4)> Combine<T, T1, T2, T3, T4>(
			Func<T, T1> func1,
			Func<T, T2> func2,
			Func<T, T3> func3,
			Func<T, T4> func4)
		{
			return arg => (
				func1(arg),
				func2(arg),
				func3(arg),
				func4(arg));
		}

		public static Action<T, (T1, T2, T3, T4)> Combine<T, T1, T2, T3, T4>(
			Action<T, T1> action1,
			Action<T, T2> action2,
			Action<T, T3> action3,
			Action<T, T4> action4)
		{
			return (arg, par) =>
			{
				action1(arg, par.Item1);
				action2(arg, par.Item2);
				action3(arg, par.Item3);
				action4(arg, par.Item4);
			};
		}

		public static Func<(T1, T2, T3, T4), (T1, T2, T3, T4)> Combine<T1, T2, T3, T4>(
			Func<T1, T1> func1,
			Func<T2, T2> func2,
			Func<T3, T3> func3,
			Func<T4, T4> func4)
		{
			return arg => (
				func1(arg.Item1),
				func2(arg.Item2),
				func3(arg.Item3),
				func4(arg.Item4));
		}


		public static Func<T, (T1, T2, T3, T4, T5)> Combine<T, T1, T2, T3, T4, T5>(
			Func<T, T1> func1,
			Func<T, T2> func2,
			Func<T, T3> func3,
			Func<T, T4> func4,
			Func<T, T5> func5)
		{
			return arg => (
				func1(arg),
				func2(arg),
				func3(arg),
				func4(arg),
				func5(arg));
		}

		public static Action<T, (T1, T2, T3, T4, T5)> Combine<T, T1, T2, T3, T4, T5>(
			Action<T, T1> action1,
			Action<T, T2> action2,
			Action<T, T3> action3,
			Action<T, T4> action4,
			Action<T, T5> action5)
		{
			return (arg, par) =>
			{
				action1(arg, par.Item1);
				action2(arg, par.Item2);
				action3(arg, par.Item3);
				action4(arg, par.Item4);
				action5(arg, par.Item5);
			};
		}

		public static Func<(T1, T2, T3, T4, T5), (T1, T2, T3, T4, T5)> Combine<T1, T2, T3, T4, T5>(
			Func<T1, T1> func1,
			Func<T2, T2> func2,
			Func<T3, T3> func3,
			Func<T4, T4> func4,
			Func<T5, T5> func5)
		{
			return arg => (
				func1(arg.Item1),
				func2(arg.Item2),
				func3(arg.Item3),
				func4(arg.Item4),
				func5(arg.Item5));
		}

		public static Func<TArgSpecific, TResult> Specific<TArg, TResult, TArgSpecific>(this Func<TArg, TResult> func)
			where TArgSpecific : TArg =>
			val => func(val);

		public static Action<TArgSpecific, TResult> Specific<TArg, TResult, TArgSpecific>(this Action<TArg, TResult> action)
			where TArgSpecific : TArg =>
			(val, arg) => action(val, arg);

		public static T Self<T>(this T value) =>
			value;
	}
}