using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Mtgdb.Dal
{
	internal class Patch
	{
		[UsedImplicitly]
		public Dictionary<string, CardPatch> Cards { get; set; }
		public Dictionary<string, LegalityPatch> Legality { get; set; }
		public Dictionary<string, Dictionary<string, ImageNamePatch>> ImageOrder { get; set; }

		public void IgnoreCase()
		{
			Cards = Cards?.ToDictionary(Str.Comparer);

			if (Legality != null)
			{
				Legality = Legality.ToDictionary(Str.Comparer);
				foreach (var legalityPatch in Legality.Values)
					legalityPatch.IgnoreCase();
			}

			ImageOrder = ImageOrder?.ToDictionary(
				pair => pair.Key,
				pair => pair.Value.ToDictionary(Str.Comparer),
				Str.Comparer);
		}
	}
}