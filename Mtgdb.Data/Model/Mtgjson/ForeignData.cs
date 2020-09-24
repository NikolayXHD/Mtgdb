using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class ForeignData
	{
		/// <summary>
		/// Foreign language of card. Can be English, Spanish, French, German, Italian, Portuguese, Japanese, Korean, Russian, Simplified Chinese, or Traditional Chinese. Promo cards can be Hebrew, Latin, Ancient Greek, Arabic, Sanskrit, or Phyrexian.
		/// </summary>
		[JsonProperty("language")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Language { get; set; }

		[JsonProperty("faceName", DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(InternedStringConverter))]
		public string FaceName { get; set; }

		[JsonProperty("name")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Name { get; set; }

		[JsonProperty("flavorText")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Flavor { get; set; }

		[JsonProperty("text")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Text { get; set; }

		[JsonProperty("type")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Type { get; set; }

		/// <summary>
		/// Multiverse ID of the card.
		/// </summary>
		[JsonProperty("multiverseId")]
		public int MultiverseId { get; set; }

		/* flavorText, text, type */
	}
}
