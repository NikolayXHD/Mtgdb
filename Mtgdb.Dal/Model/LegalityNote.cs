using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	[JsonObject]
	public class LegalityNote
	{
		[JsonProperty("format")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Format { get; set; }

		[JsonProperty("legality")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Legality { get; set; }
	}
}