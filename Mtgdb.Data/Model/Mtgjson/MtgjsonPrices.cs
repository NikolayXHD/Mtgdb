using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class MtgjsonPrices
	{
		[JsonProperty("paper"), JsonConverter(typeof(PriceHistoryConverter))]
		public List<KeyValuePair<string, float?>> Paper { get; set; }
	}
}
