using System;
using System.Collections.Generic;

namespace Mtgdb
{
	public static class LinkedListExtension
	{
		public static void SwapWith<T>(this LinkedListNode<T> first, LinkedListNode<T> second)
		{
			if (first == null)
				throw new ArgumentNullException(nameof(first));

			if (second == null)
				throw new ArgumentNullException(nameof(second));

			var tmp = first.Value;
			first.Value = second.Value;
			second.Value = tmp;
		}
	}
}