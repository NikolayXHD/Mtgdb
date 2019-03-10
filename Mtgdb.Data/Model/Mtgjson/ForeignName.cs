using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class ForeignName
	{
		/// <summary>
		/// Foreign language of card. Can be English, Spanish, French, German, Italian, Portuguese, Japanese, Korean, Russian, Simplified Chinese, or Traditional Chinese. Promo cards can be Hebrew, Latin, Ancient Greek, Arabic, Sanskrit, or Phyrexian.
		/// </summary>
		[JsonProperty("language")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Language { get; set; }

		/// <summary>
		/// Name of the card in foreign language.
		/// </summary>
		[JsonProperty("name")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Name { get; set; }

		/// <summary>
		/// Multiverse ID of the card.
		/// </summary>
		[JsonProperty("multiverseId")]
		public int MultiverseId { get; set; }

		/* flavorText, text, type */
	}
}