using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class MtgjsonPrices
	{
		[JsonProperty("paper")]
		public Dictionary<string, float> Paper { get; set; }
	}
}