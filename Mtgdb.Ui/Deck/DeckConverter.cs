using System;

namespace Mtgdb.Ui
{
	public static class DeckConverter
	{
		public static Deck ConvertDeck(Deck deck, Func<string, string> mapId)
		{
			var result = Deck.Create();
			addDeckZone(deck.MainDeck, result.MainDeck, mapId);
			addDeckZone(deck.Sideboard, result.Sideboard, mapId);

			result.File = deck.File;
			result.Name = deck.Name;
			result.Id = deck.Id;
			result.Error = deck.Error;
			result.Saved = deck.Saved;

			return result;
		}

		private static void addDeckZone(DeckZone source, DeckZone target, Func<string, string> mapId)
		{
			foreach (var originalId in source.Order)
			{
				var id = mapId(originalId);
				if (id != null)
					add(target, id, source, originalId);
			}
		}

		private static void add(DeckZone target, string id, DeckZone old, string oldId)
		{
			if (target.Count.TryGetValue(id, out var count))
				target.Count[id] = count + old.Count[oldId];
			else
			{
				target.Order.Add(id);
				target.Count[id] = old.Count[oldId];
			}
		}
	}
}