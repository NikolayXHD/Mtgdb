using System.Linq;
using System.Text.RegularExpressions;
using Mtgdb.Dal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mtgdb.Downloader
{
	public class PriceClient : WebClientBase
	{
		public PriceId DownloadSid(Card c)
		{
			int multiverseId = c.MultiverseId.Value;

			string json = DownloadString("https://api.scryfall.com/cards/multiverse/" + multiverseId);

			try
			{
				var jObj = JsonConvert.DeserializeObject<JObject>(json);
				var urisToken = jObj["purchase_uris"];
				var tcgplayerToken = urisToken["tcgplayer"];
				string url = tcgplayerToken.Value<string>();
				string sid = url.Split('/').Last();

				if (sid.StartsWith("show"))
					sid = null;

				return new PriceId
				{
					MultiverseId = multiverseId,
					Sid = sid
				};
			}
			catch
			{
				return new PriceId
				{
					MultiverseId = multiverseId,
					Sid = null
				};
			}
		}

		public PriceValues DownloadPrice(string sid)
		{
			string script = DownloadString("http://partner.tcgplayer.com/x3/mcpl.ashx?pk=MAGCINFO&sid=" + sid);

			var result = new PriceValues
			{
				Sid = sid
			};

			if (script == null)
				return result;

			var priceMatches = _priceRegex.Matches(script);

			foreach (Match match in priceMatches)
			{
				if (match.Groups["low"].Success)
					result.Low = parsePrice(match.Groups["low"].Value);

				if (match.Groups["mid"].Success)
					result.Mid = parsePrice(match.Groups["mid"].Value);

				if (match.Groups["high"].Success)
					result.High = parsePrice(match.Groups["high"].Value);
			}

			return result;
		}

		private static float? parsePrice(string value)
		{
			if (value[0] == '$')
				return float.Parse(value.Substring(1), Str.Culture);

			return null;
		}

		private static readonly Regex _priceRegex = new Regex(
			@"(?<=TCGPPriceLow\\'><[^>]+>)(?<low>[^<]+)(?=<)|(?<=TCGPPriceMid\\'><[^>]+>)(?<mid>[^<]+)(?=<)|(?<=TCGPPriceHigh\\'><[^>]+>)(?<high>[^<]+)(?=<)");
	}
}