using Newtonsoft.Json;

namespace Mtgdb.Test
{
	public class TcgCard
	{
		[JsonProperty("a")]
		public string Name { get; set; }

		[JsonProperty("n")]
		public string Number { get; set; }

		[JsonProperty("i")]
		public string Id { get; set; }

		[JsonProperty("l")]
		public float? BuylistMarketPrice { get; set; }

		[JsonProperty("m")]
		public float? MarketPrice { get; set; }

		[JsonProperty("h")]
		public float? ListedMedianPrice { get; set; }

		[JsonProperty("r")]
		public string Rarity { get; set; }

		public override string ToString()
		{
			return $"{Name} {Rarity} {Number} {Id} ${BuylistMarketPrice:F2}-{MarketPrice:F2}-{ListedMedianPrice:F2}";
		}
	}
}
