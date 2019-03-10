using Newtonsoft.Json;

namespace Mtgdb.Data
{
	[JsonObject]
	public class Ruling
	{
		/// <summary>
		/// Date (OBDC standard) of ruling for the card.
		/// </summary>
		[JsonProperty("date")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Date { get; set; }

		/// <summary>
		/// Text of ruling for the card.
		/// </summary>
		[JsonProperty("text")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Text { get; set; }
	}
}