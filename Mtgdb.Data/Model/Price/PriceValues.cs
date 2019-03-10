using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class PriceValues
	{
		[JsonProperty("id")]
		public string Sid { get; set; }

		[JsonProperty("l")]
		public float? Low { get; set; }

		[JsonProperty("m")]
		public float? Mid { get; set; }

		[JsonProperty("h")]
		public float? High { get; set; }
	}
}