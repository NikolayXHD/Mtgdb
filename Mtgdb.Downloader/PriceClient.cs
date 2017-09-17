using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Dal;

namespace Mtgdb.Downloader
{
	public class PriceClient
	{
		public PriceClient()
		{
			_webClient = new System.Net.WebClient();
			_webClient.Headers.Add("User-Agent", "Mozilla/5.0");
		}

		public PriceId DownloadSid(string mciSetCode, string mciCardNumber)
		{
			if (string.IsNullOrEmpty(mciSetCode))
				return null;

			if (string.IsNullOrEmpty(mciCardNumber))
				return null;

			string html = download(MagiccardsUrl + "/" + mciSetCode.ToLowerInvariant() + "/en/" + mciCardNumber + ".html");

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
			string script = download(MagiccardsUrl + "/tcgplayer/hl?sid=" + priceId.Sid);

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
				return float.Parse(value.Substring(1), CultureInfo.InvariantCulture);

			return null;
		}

		private string download(string pageUrl)
		{
			using (var stream = _webClient.OpenRead(pageUrl))
			{
				if (stream == null)
					return null;

				using (var streamReader = new StreamReader(stream, Encoding.UTF8))
				{
					string result = streamReader.ReadToEnd();
					return result;
				}
			}
		}

		private const string MagiccardsUrl = "https://magiccards.info";
		private static readonly Regex _sidRegex = new Regex(@"[\?\&]sid=(?<sid>[\w\d]+)", RegexOptions.Compiled);
		private static readonly Regex _priceRegex = new Regex(@"L:.+>(?<low>NA|\$\d+(?:\.\d\d?)?)<.+M:.+>(?<mid>NA|\$\d+(?:\.\d\d?)?)<.+H:.+>(?<hig>NA|\$\d+(?:\.\d\d?)?)<",
			RegexOptions.Compiled);

		private System.Net.WebClient _webClient;
	}
}