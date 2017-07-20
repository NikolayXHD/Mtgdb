using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	[JsonObject]
	public class Ruling
	{
		[JsonProperty("date")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Date { get; set; }

		[JsonProperty("text")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Text { get; set; }
	}
}