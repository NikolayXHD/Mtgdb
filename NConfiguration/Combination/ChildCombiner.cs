using System;
using System.Collections.Generic;

namespace NConfiguration.Combination
{
	public delegate T ChildCombineDefinition<T>(ChildCombiner.Context context, T x, T y);

	public sealed class ChildCombiner: ICombiner
	{
		private ICombiner _parent;
		private Dictionary<Type, object> _funcMap = new Dictionary<Type, object>();

		public ChildCombiner(ICombiner parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			_parent = parent;
		}

		public class Context
		{
			public ICombiner Parent { get; private set; }
			public ICombiner Current { get; private set; }

			internal Context(ICombiner parent, ICombiner current)
			{
				Parent = parent;
				Current = current;
			}

			public T CurrentCombine<T>(T x, T y)
			{
				return Current.Combine(Current, x, y);
			}

			public T ParentCombine<T>(T x, T y)
			{
				return Parent.Combine(Current, x, y);
			}

			public T Combine<T>(ICombiner current, T x, T y)
			{
				return Parent.Combine(current, x, y);
			}
		}

		/// <summary>
		/// Set custom combiner
		/// </summary>
		/// <typeparam name="T">required type</typeparam>
		/// <param name="combine">combine function</param>
		public ChildCombiner SetCombiner<T>(ChildCombineDefinition<T> combine)
		{
			_funcMap[typeof(T)] = combine;
			return this;
		}

		
		/// <summary>
		/// Set custom combiner
		/// </summary>
		/// <typeparam name="T">required type</typeparam>
		/// <param name="combiner">combiner</param>
		public ChildCombiner SetCombiner<T>(ICombiner<T> combiner)
		{
			return SetCombiner<T>((ctx, prev, next) => combiner.Combine(ctx.Current, prev, next));
		}

		/// <summary>
		/// Convert any combined value of specified type
		/// </summary>
		public ChildCombiner Convert<T>(Func<T, T> converter)
		{
			return SetCombiner<T>((ctx, prev, next) => converter(ctx.ParentCombine(prev, next)));
		}

		/// <summary>
		/// Modify any combined value of specified type
		/// </summary>
		public ChildCombiner Modify<T>(Action<T> modifier)
		{
			return SetCombiner<T>((ctx, prev, next) =>
			{
				var item = ctx.ParentCombine(prev, next);
				if (item != null)
					modifier(item);
				return item;
			});
		}

		public T Combine<T>(ICombiner context, T x, T y)
		{
			object combine;
			if (_funcMap.TryGetValue(typeof(T), out combine))
				return ((ChildCombineDefinition<T>)combine)(new Context(_parent, context), x, y);

			return _parent.Combine(context, x, y);
		}
	}
}
