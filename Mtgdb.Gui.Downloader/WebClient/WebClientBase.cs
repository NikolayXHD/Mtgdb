using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Mtgdb.Downloader
{
	public class WebClientBase
	{
		protected const int BufferSize = 1 << 20; // 1MB

		public virtual async Task DownloadFile(string downloadUrl, FsPath downloadTarget, CancellationToken token)
		{
			using var stream = await DownloadStream(downloadUrl, token);
			using var fileStream = downloadTarget.OpenFile(FileMode.Create);
			await stream.CopyToAsync(fileStream, BufferSize, token);
		}

		public async Task<string> DownloadString(string pageUrl, CancellationToken token)
		{
			try
			{
				using var client = new HttpClient();
				using var response = await GetResponse(client, HttpMethod.Get, pageUrl, token);
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
			using var response = await GetResponse(client, HttpMethod.Get, url, token);
			var memoryStream = await ReadToMemoryStream(response, token);
			return memoryStream;
		}

		protected async Task<HttpResponseMessage> GetResponse(
			HttpClient client, HttpMethod method, string uri, CancellationToken token,
			Action<HttpRequestHeaders> setHeaders = null)
		{
			int attempts = 0;
			const int maxAttempts = 5;
			const int delayMs = 500;
			while (true)
			{
				try
				{
					var request = new HttpRequestMessage(method, uri);
					setHeaders?.Invoke(request.Headers);
					var response = await client.SendAsync(request, token);
					response.EnsureSuccessStatusCode();
					return response;
				}
				catch (Exception ex) when (
					getInnerExceptionsChain(ex).OfType<SocketException>().Any()
					&& attempts++ < maxAttempts)
				{
					_log.Debug(ex, "");
					await Task.Delay(delayMs, token);
				}
				catch (Exception ex)
				{
					_log.Error(ex, $"HTTP {method.Method} {uri} failed");
					throw;
				}
			}
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

		private static IEnumerable<Exception> getInnerExceptionsChain(Exception ex)
		{
			while (ex != null)
			{
				yield return ex;
				ex = ex.InnerException;
			}
		}
	}
}
