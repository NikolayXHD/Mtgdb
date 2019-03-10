using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Mtgdb.Downloader
{
	public class GdriveWebClient : WebClientBase
	{
		public void DownloadFromGdrive(string downloadUrl, string targetDirectory)
		{
			var originalRequest = CreateRequest(downloadUrl);
			originalRequest.CookieContainer = new CookieContainer();

			var response = GetResponse(originalRequest);

			string contentDisposition = response.Headers["content-disposition"];
			if (contentDisposition == null || !contentDisposition.StartsWith("attachment;", Str.Comparison))
			{
				if (response.StatusCode != HttpStatusCode.OK || !response.ContentType.StartsWith("text/html", Str.Comparison))
					throw new WebException($"Unexpected response from {downloadUrl}: {response}");

				using (var stream = response.GetResponseStream())
				using (var reader = new StreamReader(stream))
				{
					var content = reader.ReadToEnd();

					var continuationUrlPattern = new Regex(@"href ?= ?""([^""]*confirm=[^""]+)""");
					var urlPatternMatch = continuationUrlPattern.Match(content);

					if (!urlPatternMatch.Success)
						throw new WebException($"Continuation URL not found in: {content}");

					var continuationUrl = WebUtility.HtmlDecode(urlPatternMatch.Groups[1].Value);

					if (!Uri.TryCreate(continuationUrl, UriKind.Absolute, out _))
					{
						var originalUrl = new UriBuilder(downloadUrl);

						var builder = new UriBuilder
						{
							Scheme = originalUrl.Scheme,
							Host = originalUrl.Host
						};

						continuationUrl = builder.ToString().TrimEnd('/') + '/' + continuationUrl.TrimStart('/');
					}

					var request = CreateRequest(continuationUrl);

					request.Referer = downloadUrl;

					request.CookieContainer = new CookieContainer();
					foreach (Cookie cookie in response.Cookies)
						request.CookieContainer.Add(cookie);

					response = GetResponse(request);
				}
			}

			contentDisposition = response.Headers["content-disposition"];
			
			if (string.IsNullOrEmpty(contentDisposition))
				throw new WebException($"content-disposition header not found in response from {response.ResponseUri}");

			var fileNamePattern = new Regex("filename=\"([^\"]+)\"");

			var fileNameMatch = fileNamePattern.Match(contentDisposition);

			if (!fileNameMatch.Success)
				throw new WebException($"File name not found in content-disposition header {contentDisposition} from {response.ResponseUri}");

			var fileName = fileNameMatch.Groups[1].Value;

			var downloadTarget = targetDirectory.AddPath(fileName);

			using (var stream = response.GetResponseStream())
			using (var fileStream = File.Open(downloadTarget, FileMode.Create))
				stream.CopyTo(fileStream);
		}
	}
}