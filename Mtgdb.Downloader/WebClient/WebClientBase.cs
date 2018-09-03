using System;
using System.IO;
using System.Net;
using System.Text;
using NLog;

namespace Mtgdb.Downloader
{
	public class WebClientBase
	{
		static WebClientBase()
		{
			try
			{
				ServicePointManager.SecurityProtocol |= (SecurityProtocolType) 3072;
			}
			catch (NotSupportedException ex)
			{
				_log.Warn(ex, "Failed to setup HTTPS");
			}
		}

		public void DownloadFile(string downloadUrl, string downloadTarget)
		{
			using (var webStream = DownloadStream(downloadUrl))
			using (var fileStream = File.Open(downloadTarget, FileMode.Create))
				webStream.CopyTo(fileStream);
		}

		public string DownloadString(string pageUrl)
		{
			Stream stream;
			try
			{
				stream = DownloadStream(pageUrl);
			}
			catch (WebException)
			{
				return null;
			}
			catch (ProtocolViolationException)
			{
				return null;
			}

			using (stream)
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

		public Stream DownloadStream(string url)
		{
			var response = GetResponse(url);

			Stream result;

			try
			{
				result = response.GetResponseStream();
			}
			catch (ProtocolViolationException ex)
			{
				_log.Error(ex, $"{nameof(HttpWebRequest)} to {url} failed");
				throw;
			}

			return result;
		}

		protected static HttpWebResponse GetResponse(string url)
		{
			var request = CreateRequest(url);
			return GetResponse(request);
		}

		protected static HttpWebResponse GetResponse(HttpWebRequest request)
		{
			HttpWebResponse response;
			try
			{
				response = (HttpWebResponse) request.GetResponse();
			}
			catch (WebException ex)
			{
				_log.Error(ex, $"{nameof(HttpWebRequest)} to {request.RequestUri} failed");
				throw;
			}
			catch (ProtocolViolationException ex)
			{
				_log.Error(ex, $"{nameof(HttpWebRequest)} to {request.RequestUri} failed");
				throw;
			}

			return response;
		}

		protected static HttpWebRequest CreateRequest(string url)
		{
			var request = (HttpWebRequest) WebRequest.Create(url);

			request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
			return request;
		}

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}