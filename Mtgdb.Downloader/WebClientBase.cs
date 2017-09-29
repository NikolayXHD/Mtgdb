using System;
using System.IO;
using System.Text;

namespace Mtgdb.Downloader
{
	public class WebClientBase : IDisposable
	{
		public WebClientBase()
		{
			_webClient = new System.Net.WebClient();

			_webClient.Headers.Add(
				"User-Agent",
				"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
		}

		protected string DownloadString(string pageUrl)
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

		protected Stream DownloadStream(string pageUrl)
		{
			return _webClient.OpenRead(pageUrl);
		}

		public void DownloadFile(string downloadUrl, string downloadTarget)
		{
			using (var webStream = DownloadStream(downloadUrl))
			using (var fileStream = File.Open(downloadTarget, FileMode.Create))
				webStream.CopyTo(fileStream);
		}

		private readonly System.Net.WebClient _webClient;

		public void Dispose()
		{
			_webClient?.Dispose();
		}
	}
}