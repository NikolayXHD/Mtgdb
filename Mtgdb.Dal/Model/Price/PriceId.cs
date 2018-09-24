using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class PriceId
	{
		[JsonProperty("m")]
		public int MultiverseId { get; set; }

		[JsonProperty("t")]
		public string Sid { get; set; }
	}
}