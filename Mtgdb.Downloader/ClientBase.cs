using System;
using System.IO;
using System.Net.Cache;
using System.Text;

namespace Mtgdb.Downloader
{
	public class ClientBase
	{
		protected ClientBase()
		{
			_webClient = new System.Net.WebClient();
			_webClient.Headers.Add(
				"User-Agent",
				"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
			_webClient.Headers.Add(
				"Accept",
				"text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,image / apng,*/*;q=0.8");

			_webClient.Headers.Add("Accept-Language", "en,ru;q=0.8");
			_webClient.Headers.Add("Accept-Encoding", "gzip, deflate");
			_webClient.Headers.Add("Referer", "http://prices.tcgplayer.com/price-guide/magic/box-sets");
			_webClient.Headers.Add("Upgrade-Insecure-Requests", "1");
			_webClient.CachePolicy = new HttpRequestCachePolicy(HttpCacheAgeControl.MaxAge, TimeSpan.Zero);
			_webClient.Headers.Add("Host", " prices.tcgplayer.com");
		}

		protected string Download(string pageUrl)
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

		private readonly System.Net.WebClient _webClient;
	}
}