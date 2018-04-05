using System;
using System.Collections.Generic;

namespace Mtgdb
{
	public class CustomComparer<TKey> : IComparer<TKey>
	{
		public CustomComparer(Func<TKey, TKey, int> func) => _func = func;
		public int Compare(TKey x, TKey y) => _func(x, y);
		private readonly Func<TKey, TKey, int> _func;
	}
}