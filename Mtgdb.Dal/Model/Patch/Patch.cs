using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mtgdb.Dal
{
	internal class Patch
	{
		[UsedImplicitly]
		public Dictionary<string, CardPatch> Cards { get; set; }
		public Dictionary<string, LegalityPatch> Legality { get; [UsedImplicitly] set; }
	}
}