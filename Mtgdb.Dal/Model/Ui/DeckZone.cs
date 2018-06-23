using System.Collections.Generic;

namespace Mtgdb.Dal
{
	public class DeckZone
	{
		public DeckZone()
		{
		}

		public Dictionary<string, int> Count { get; set; }
		public List<string> Order { get; set; }
	}
}