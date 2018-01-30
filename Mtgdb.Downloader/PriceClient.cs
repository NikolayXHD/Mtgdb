using System.Globalization;
using System.Text.RegularExpressions;
using Mtgdb.Dal;

namespace Mtgdb.Downloader
{
	public class PriceClient : WebClientBase
	{
		public PriceId DownloadSid(string mciSetCode, string mciCardNumber)
		{
			if (string.IsNullOrEmpty(mciSetCode))
				return null;

			if (string.IsNullOrEmpty(mciCardNumber))
				return null;

			string html = DownloadString(MagiccardsUrl + "/" + mciSetCode.ToLowerInvariant() + "/en/" + mciCardNumber + ".html");

			if (html == null)
				return null;

			var sidMatch = _sidRegex.Match(html);

			return new PriceId
			{
				Set = mciSetCode,
				Card = mciCardNumber,
				Sid = sidMatch.Success ? sidMatch.Groups["sid"].Value : null
			};
		}

		public PriceValues DownloadPrice(PriceId priceId)
		{
			string script = DownloadString(MagiccardsUrl + "/tcgplayer/hl?sid=" + priceId.Sid);

			var result = new PriceValues
			{
				Sid = priceId.Sid,
			};

			if (script == null)
				return result;

			var priceMatch = _priceRegex.Match(script);
			if (!priceMatch.Success)
				return result;


			result.Low = parsePrice(priceMatch.Groups["low"].Value);
			result.Mid = parsePrice(priceMatch.Groups["mid"].Value);
			result.High = parsePrice(priceMatch.Groups["hig"].Value);

			return result;
		}

		private static float? parsePrice(string value)
		{
			if (value[0] == '$')
				return float.Parse(value.Substring(1), Str.Culture);

			return null;
		}

		private const string MagiccardsUrl = "https://magiccards.info";
		private static readonly Regex _sidRegex = new Regex(@"[\?\&]sid=(?<sid>[\w\d]+)", RegexOptions.Compiled);
		private static readonly Regex _priceRegex = new Regex(@"L:.+>(?<low>NA|\$\d+(?:\.\d\d?)?)<.+M:.+>(?<mid>NA|\$\d+(?:\.\d\d?)?)<.+H:.+>(?<hig>NA|\$\d+(?:\.\d\d?)?)<",
			RegexOptions.Compiled);
	}
}