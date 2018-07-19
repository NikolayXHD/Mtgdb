using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Dal
{
	public class DeckZone
	{
		public DeckZone()
		{
		}

		public Dictionary<string, int> Count { get; set; }
		public List<string> Order { get; set; }

		public bool IsEquivalentTo(DeckZone other)
		{
			return
				Order.SequenceEqual(other.Order) &&
				Order.Select(_ => Count[_]).SequenceEqual(other.Order.Select(_ => other.Count[_]));
		}
	}
}