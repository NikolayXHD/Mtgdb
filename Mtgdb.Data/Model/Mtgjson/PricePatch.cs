using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class PricePatch
	{
		[JsonProperty("prices")]
		public MtgjsonPrices Prices;
	}
}
