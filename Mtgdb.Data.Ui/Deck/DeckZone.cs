using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Mtgdb.Ui
{
	public class DeckZone
	{
		[UsedImplicitly] // to find usages in IDE
		public DeckZone()
		{
		}

		public bool ShouldSerializeCount() => false;
		public Dictionary<string, int> Count { get; set; }

		public List<string> Order { get; set; }

		[UsedImplicitly] // for serialization
		public List<int> CountList
		{
			get => Count == null
				? null
				: Order.Select(_ => Count[_]).ToList();
			set
			{
				if (value == null)
					return;

				Count = Enumerable.Range(0, Order.Count)
					.ToDictionary(i => Order[i], i => value[i]);
			}
		}

		public bool IsEquivalentTo(DeckZone other)
		{
			return
				Order.SequenceEqual(other.Order) &&
				Order.Select(_ => Count[_]).SequenceEqual(other.Order.Select(_ => other.Count[_]));
		}
	}
}