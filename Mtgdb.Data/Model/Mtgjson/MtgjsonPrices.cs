using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class MtgjsonPrices
	{
		[JsonProperty("paper")]
		public Dictionary<string, ShopPriceHistory> Paper { get; set; }

		[JsonProperty("mtgo")]
		public Dictionary<string, ShopPriceHistory> Mtgo { get; set; }
	}

	public class ShopPriceHistory
	{
		[JsonProperty("buylist")]
		public PriceHistory Buylist { get; set; }

		[JsonProperty("retail")]
		public PriceHistory Retail { get; set; }
	}

	public class PriceHistory
	{
		[JsonProperty("normal"), JsonConverter(typeof(PriceHistoryConverter))]
		public List<KeyValuePair<string, float?>> Normal { get; set; }

		[JsonProperty("foil"), JsonConverter(typeof(PriceHistoryConverter))]
		public List<KeyValuePair<string, float?>> Foil { get; set; }
	}
}
