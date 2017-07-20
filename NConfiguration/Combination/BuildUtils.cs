using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NConfiguration.Combination
{
	internal static partial class BuildUtils
	{
		private static readonly HashSet<Type> _simplySystemStructs = new HashSet<Type>
			{
				typeof(string),
				typeof(Enum), typeof(DateTime), typeof(DateTimeOffset),
				typeof(bool), typeof(byte), typeof(char), typeof(decimal),
				typeof(double), typeof(Guid), typeof(short), typeof(int),
				typeof(long), typeof(sbyte), typeof(float), typeof(TimeSpan),
				typeof(ushort), typeof(uint), typeof(ulong)
			};

		private static bool isSimplyStruct(Type type)
		{
			if (type.IsEnum)
				return true;

			var ntype = Nullable.GetUnderlyingType(type);
			if (ntype != null) // is Nullable<>
			{
				type = ntype;
				if (type.IsEnum)
					return true;
			}

			return _simplySystemStructs.Contains(type);
		}

		private static Type getEnumerableType(Type type)
		{
			foreach (Type intType in type.GetInterfaces())
			{
				if (intType.IsGenericType
					&& intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					return intType.GetGenericArguments()[0];
				}
			}
			return null;
		}

		public static object CreateFunction(Type targetType)
		{
			var supressValue = createDefaultSupressor(targetType);

			return
				TryCreateAsCombinable(targetType) ??
				TryCreateAsAttribute(targetType) ??
				TryCreateForSimplyStruct(targetType, supressValue) ??
				TryCreateRecursiveNullableCombiner(targetType, supressValue) ??
				tryCreateCollectionCombiner(targetType) ??
				tryCreateComplexCombiner(targetType) ??
				CreateForwardCombiner(targetType, supressValue);
		}

		private static object tryCreateCollectionCombiner(Type targetType)
		{
			var itemType = getEnumerableType(targetType);
			if (itemType == null)
				return null;

			var funcType = typeof(Combine<>).MakeGenericType(targetType);

			return Delegate.CreateDelegate(funcType, CollectionCombineMi.MakeGenericMethod(targetType, itemType));
		}

		private static object tryCreateComplexCombiner(Type targetType)
		{
			try
			{
				var builder = new ComplexFunctionBuilder(targetType);

				foreach (var fi in targetType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
					builder.Add(fi);

				foreach (var pi in targetType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
					builder.Add(pi);

				return builder.Compile();
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(string.Format("can't create combiner for type {0}", targetType.FullName), ex);
			}
		}

		internal static object TryCreateAsAttribute(Type targetType)
		{
			var combinerAttr = targetType.GetCustomAttributes(false).OfType<ICombinerFactory>().SingleOrDefault();
			
			if(combinerAttr == null)
				return null;
			
			var combiner = combinerAttr.CreateInstance(targetType);
			
			var mi = typeof(ICombiner<>).MakeGenericType(targetType).GetMethod("Combine");
			var funcType = typeof(Combine<>).MakeGenericType(targetType);
			return Delegate.CreateDelegate(funcType, combiner, mi);
		}

		internal static object TryCreateAsCombinable(Type targetType)
		{
			Type combinerType;
			if (typeof(ICombinable<>).MakeGenericType(targetType).IsAssignableFrom(targetType))
				combinerType = targetType.IsValueType ? typeof(GenericStructCombiner<>) : typeof(GenericClassCombiner<>);
			else if (typeof(ICombinable).IsAssignableFrom(targetType))
				combinerType = targetType.IsValueType ? typeof(StructCombiner<>) : typeof(ClassCombiner<>);
			else
				return null;

			combinerType = combinerType.MakeGenericType(targetType);
			return CreateByCombinerInterfaceMi.MakeGenericMethod(combinerType, targetType).Invoke(null, new object[0]);
		}

		internal static object TryCreateForSimplyStruct(Type targetType, object supressValue)
		{
			if (isSimplyStruct(targetType))
			{
				return CreateForwardCombiner(targetType, supressValue);
			}
			else
				return null;
		}

		internal static object TryCreateRecursiveNullableCombiner(Type targetType, object supressValue)
		{
			var ntype = Nullable.GetUnderlyingType(targetType);
			if (ntype == null) // is not Nullable<>
				return null;

			var funcType = typeof(Combine<>).MakeGenericType(targetType);

			return Delegate.CreateDelegate(funcType, supressValue, RecursiveNullableCombineMi.MakeGenericMethod(ntype));
		}

		internal static readonly MethodInfo CollectionCombineMi = getMethod("CollectionCombine");
		internal static T CollectionCombine<T, I>(ICombiner combiner, T x, T y) where T: IEnumerable<I>
		{
			if (x == null)
				return y;

			if (y == null)
				return x;

			if (!x.Any())
				return y;

			if (!y.Any())
				return x;

			return y;
		}

		internal static readonly MethodInfo RecursiveNullableCombineMi = getMethod("RecursiveNullableCombine");
		internal static T? RecursiveNullableCombine<T>(Predicate<T?> supressValue, ICombiner combiner, T? x, T? y) where T : struct
		{
			if (supressValue(x)) return y;
			if (supressValue(y)) return x;
			return combiner.Combine<T>(x.Value, y.Value);
		}

		internal static readonly MethodInfo CreateByCombinerInterfaceMi = getMethod("CreateByCombinerInterface");
		internal static Combine<T> CreateByCombinerInterface<TC, T>() where TC: ICombiner<T>
		{
			var combiner = Activator.CreateInstance<TC>();
			return combiner.Combine;
		}

		internal static object CreateForwardCombiner(Type type, object supressValue)
		{
			var funcType = typeof(Combine<>).MakeGenericType(type);

			return Delegate.CreateDelegate(funcType, supressValue, ForwardCombineMi.MakeGenericMethod(type));
		}

		internal static readonly MethodInfo ForwardCombineMi = getMethod("ForwardCombine");
		internal static T ForwardCombine<T>(Predicate<T> supressValue, ICombiner combiner, T x, T y)
		{
			return supressValue(y) ? x : y;
		}

		private static object createDefaultSupressor(Type type)
		{
			var funcType = typeof(Predicate<>).MakeGenericType(type);

			var ntype = Nullable.GetUnderlyingType(type);
			if (ntype != null) // is Nullable<>
				return Delegate.CreateDelegate(funcType, NullableStructSupressMi.MakeGenericMethod(ntype));

			if (type.IsValueType)
				return Delegate.CreateDelegate(funcType, SelectStructSupresssor(type).MakeGenericMethod(type));
			else
				return Delegate.CreateDelegate(funcType, ClassSupressMi.MakeGenericMethod(type));
		}

		internal static readonly MethodInfo NullableStructSupressMi = getMethod("NullableStructSupress");
		internal static bool NullableStructSupress<T>(T? item) where T : struct
		{
			return item == null;
		}

		internal static readonly MethodInfo ClassSupressMi = getMethod("ClassSupress");
		internal static bool ClassSupress<T>(T item) where T : class
		{
			return item == null;
		}

		internal static MethodInfo SelectStructSupresssor(Type type)
		{
			var eqInterface = typeof(IEquatable<>).MakeGenericType(type);
			if (eqInterface.IsAssignableFrom(type))
				return EquatableStructSupressMi;

			var comInterface = typeof(IComparable<>).MakeGenericType(type);
			if (comInterface.IsAssignableFrom(type))
				return ComparableStructSupressMi;

			return OthersStructSupressMi;
		}

		internal static readonly MethodInfo EquatableStructSupressMi = getMethod("EquatableStructSupress");
		internal static bool EquatableStructSupress<T>(T item) where T : struct, IEquatable<T>
		{
			return item.Equals(default(T));
		}

		internal static readonly MethodInfo ComparableStructSupressMi = getMethod("ComparableStructSupress");
		internal static bool ComparableStructSupress<T>(T item) where T : struct, IComparable<T>
		{
			return item.CompareTo(default(T)) == 0;
		}

		internal static readonly MethodInfo OthersStructSupressMi = getMethod("OthersStructSupress");
		internal static bool OthersStructSupress<T>(T item) where T : struct
		{
			return item.Equals(default(T));
		}

		private static MethodInfo getMethod(string name)
		{
			return typeof(BuildUtils).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);
		}
	}
}
