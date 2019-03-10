namespace Mtgdb.Data
{
	public static class CardId
	{
		public static string Generate(Card card) =>
			 Generate(card.ScryfallId, card.NameEn);

		public static string Generate(string scryfallId, string name) =>
			GuidV5.Base64(scryfallId + name);
	}
}