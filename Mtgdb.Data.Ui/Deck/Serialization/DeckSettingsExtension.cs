namespace Mtgdb.Ui
{
	public static class DeckSettingsExtension
	{
		public static Deck GetDeck(this IDeckSettings settings)
		{
			var deck = Deck.Create(
				settings.MainDeckCount,
				settings.MainDeckOrder,
				settings.SideDeckCount,
				settings.SideDeckOrder,
				settings.MaybeDeckCount,
				settings.MaybeDeckOrder,
				sampleCountById: null,
				sampleOrder: null);

			deck.Name = settings.DeckName;
			deck.File = settings.DeckFile.OrNone();

			return deck;
		}

		public static void SetDeck(this IDeckSettings settings, Deck value)
		{
			settings.MainDeckCount = value.MainDeck.Count;
			settings.MainDeckOrder = value.MainDeck.Order;
			settings.SideDeckCount = value.Sideboard.Count;
			settings.SideDeckOrder = value.Sideboard.Order;
			settings.MaybeDeckCount = value.Maybeboard.Count;
			settings.MaybeDeckOrder = value.Maybeboard.Order;
		}
	}
}
