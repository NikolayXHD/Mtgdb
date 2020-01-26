using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Mtgdb.Data
{
	public class ListPool<TElement>
	{
		public List<TElement> GetEmptyList() =>
			_listPool.TryPop(out var result) ? result : new List<TElement>();

		public void StoreUnusedList(List<TElement> list)
		{
			list.Clear();
			_listPool.Push(list);
		}

		private readonly ConcurrentStack<List<TElement>> _listPool = new ConcurrentStack<List<TElement>>();
	}
}
