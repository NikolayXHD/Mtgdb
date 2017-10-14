using System.Collections.Generic;

namespace Mtgdb.Gui
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