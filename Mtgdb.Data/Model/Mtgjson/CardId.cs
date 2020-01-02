namespace Mtgdb.Data
{
	public static class CardId
	{
		public static string Generate(Card card)
		{
			string discriminator = card.IsToken ? card.Side : card.NameEn;
			return generate(card.ScryfallId, discriminator);
		}

		private static string generate(string scryfallId, string name) =>
			GuidV5.Base64(scryfallId + name);
	}
}
