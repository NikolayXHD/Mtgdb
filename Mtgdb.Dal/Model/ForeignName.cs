using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class ForeignName
	{
		[JsonProperty("language")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Language { get; set; }

		[JsonProperty("name")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Name { get; set; }

		[JsonProperty("multiverseId")]
		public int MultiverseId { get; set; }
	}
}