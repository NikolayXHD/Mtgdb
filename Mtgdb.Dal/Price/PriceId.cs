using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class PriceId
	{
		[JsonProperty("s")]
		public string Set { get; set; }

		[JsonProperty("c")]
		public string Card { get; set; }

		[JsonProperty("id")]
		public string Sid { get; set; }
	}
}