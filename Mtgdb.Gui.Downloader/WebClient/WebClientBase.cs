using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Mtgdb.Downloader
{
	public class WebClientBase
	{
		protected const int BufferSize = 1 << 20; // 1MB

		public async Task DownloadFile(string downloadUrl, string downloadTarget, CancellationToken token)
		{
			using var stream = await DownloadStream(downloadUrl, token);
			using var fileStream = File.Open(downloadTarget, FileMode.Create);
			await stream.CopyToAsync(fileStream, BufferSize, token);
		}

		public async Task<string> DownloadString(string pageUrl, CancellationToken token)
		{
			try
			{
				using var client = new HttpClient();
				using var request = new HttpRequestMessage(HttpMethod.Get, pageUrl);
				using var response = await GetResponse(client, request, token);
				using var memoryStream = await ReadToMemoryStream(response, token);
				using var streamReader = new StreamReader(memoryStream);
				return streamReader.ReadToEnd();
			}
			catch (HttpRequestException)
			{
				return null;
			}
		}

		public async Task<Stream> DownloadStream(string url, CancellationToken token)
		{
			using var client = new HttpClient();
			using var request = new HttpRequestMessage(HttpMethod.Get, url);
			using var response = await GetResponse(client, request, token);
			var memoryStream = await ReadToMemoryStream(response, token);
			return memoryStream;
		}

		public async Task<HttpResponseMessage> GetResponse(HttpClient client, HttpRequestMessage request, CancellationToken token)
		{
			HttpResponseMessage response;
			try
			{
				response = await client.SendAsync(request, token);
				response.EnsureSuccessStatusCode();
			}
			catch (HttpRequestException ex)
			{
				_log.Info(ex, $"{nameof(HttpRequestMessage)} to {request.RequestUri} failed");
				throw;
			}

			return response;
		}

		protected static async Task<MemoryStream> ReadToMemoryStream(HttpResponseMessage response, CancellationToken token)
		{
			using var stream = await response.Content.ReadAsStreamAsync();
			var memoryStream = new MemoryStream();
			await stream.CopyToAsync(memoryStream, BufferSize, token);
			memoryStream.Seek(0, SeekOrigin.Begin);
			return memoryStream;
		}

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
