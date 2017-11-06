using System.Collections.Generic;

namespace Mtgdb.Dal
{
	internal class Patch
	{
		public Dictionary<string, CardPatch> Cards { get; set; }
		public Dictionary<string, LegalityPatch> Legality { get; set; }
	}
}