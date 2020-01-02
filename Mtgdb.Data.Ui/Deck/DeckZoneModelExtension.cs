using System.Collections.Generic;
using System.Linq;
using Mtgdb.Data;

namespace Mtgdb.Ui
{
	public static class DeckZoneModelExtension
	{
		public static IEnumerable<string> NamesakeIds(this DeckZoneModel zone, Card card) =>
			zone.CardsIds.Where(id => id == card.Id || card.NamesakeIds.Contains(id));

		public static bool Contains(this DeckZoneModel zone, Card c) =>
			zone.CountById.ContainsKey(c.Id);
	}
}
