using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Mtgdb.Downloader
{
	public class GdriveWebClient : WebClientBase
	{
		public override async Task DownloadFile(string downloadUrl, [NotNull] FsPath targetFile, CancellationToken token)
		{
			var originalCookieContainer = new CookieContainer();
			using var originalHandler = new HttpClientHandler
			{
				CookieContainer = originalCookieContainer
			};

			using var client = new HttpClient();

			client.DefaultRequestHeaders.Add("User-Agent",
				"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36");
			client.DefaultRequestHeaders.Add("Accept-Language", "ru,en;q=0.9");

			var response = await GetResponse(client, HttpMethod.Get, downloadUrl, token);
			if (response.Content.Headers.ContentDisposition?.ToString()
				    .StartsWith("attachment;", Str.Comparison) != true)
			{
				if (!response.Content.Headers.ContentType.ToString().StartsWith("text/html", Str.Comparison))
					throw new WebException($"Unexpected response from {downloadUrl}: {response}");

				using var memoryStream = await ReadToMemoryStream(response, token);
				using var reader = new StreamReader(memoryStream);
				var content = await reader.ReadToEndAsync();

				var continuationUrlPattern = new Regex("href ?= ?\"([^\"]*confirm=[^\"]+)\"");
				var urlPatternMatch = continuationUrlPattern.Match(content);

				if (!urlPatternMatch.Success)
					throw new HttpRequestException($"Continuation URL not found in: {content}");

				var continuationUrl = WebUtility.HtmlDecode(urlPatternMatch.Groups[1].Value);
				var originalUrl = new UriBuilder(downloadUrl);
				if (!Uri.TryCreate(continuationUrl, UriKind.Absolute, out _))
				{
					var builder = new UriBuilder
					{
						Scheme = originalUrl.Scheme,
						Host = originalUrl.Host
					};

					continuationUrl = builder.ToString().TrimEnd('/') + '/' + continuationUrl.TrimStart('/');
				}

				response = await GetResponse(client, HttpMethod.Get, continuationUrl, token, h => h.Referrer = originalUrl.Uri);
			}

			var contentDisposition = response.Content.Headers.ContentDisposition?.ToString();
			if (contentDisposition == null)
				throw new HttpRequestException($"content-disposition header not found in response from {response.RequestMessage.RequestUri}");

			var fileNamePattern = new Regex("filename=\"([^\"]+)\"");
			var fileNameMatch = fileNamePattern.Match(contentDisposition);
			if (!fileNameMatch.Success)
				throw new HttpRequestException($"File name not found in content-disposition header {contentDisposition} from {response.RequestMessage.RequestUri}");

			var fileName = fileNameMatch.Groups[1].Value;
			if (!Str.Equals(fileName, targetFile.Basename()))
				throw new ApplicationException(
					$"Attempted to download file {fileName} to different filename {targetFile}");

			using var stream = await response.Content.ReadAsStreamAsync();
			using var fileStream = targetFile.OpenFile(FileMode.Create);
			await stream.CopyToAsync(fileStream, BufferSize, token);
		}
	}
}
