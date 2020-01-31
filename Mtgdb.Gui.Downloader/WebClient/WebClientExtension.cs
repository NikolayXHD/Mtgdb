using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Mtgdb.Downloader
{
	public static class WebClientExtension
	{
		public static async Task<bool> DownloadAndExtract(this WebClientBase webClient, string url, string targetDirectory, string fileName, CancellationToken token)
		{
			if (!Str.Equals(".7z", Path.GetExtension(fileName)))
				throw new ArgumentException();

			string archiveFileName = targetDirectory.AddPath(fileName);

			if (File.Exists(archiveFileName))
			{
				try
				{
					File.Delete(archiveFileName);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to remove {archiveFileName}: {ex.Message}");
					return false;
				}
			}

			try
			{
				await webClient.DownloadFile(url, archiveFileName, token);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error downloading {archiveFileName} from {url}: {ex.Message}");
				_log.Warn(ex, $"Failed download {archiveFileName} from {url}");
				return false;
			}

			if (!File.Exists(archiveFileName))
			{
				Console.WriteLine($"Failed to download {archiveFileName} from {url}");
				return false;
			}

			var sevenZip = new SevenZip(silent: true);
			sevenZip.Extract(archiveFileName, targetDirectory, Enumerable.Empty<string>());

			try
			{
				File.Delete(archiveFileName);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to remove {archiveFileName}: {ex.Message}");
			}

			return true;
		}

		public static async Task<bool> TryDownload(this WebClientBase webClient, string url, string targetFile, CancellationToken token)
		{
			Console.Write($"Download {targetFile} from {url} ...");

			try
			{
				await webClient.DownloadFile(url, targetFile, token);
			}
			catch (Exception ex)
			{
				Console.WriteLine($" failed: {ex.Message}");
				_log.Warn(ex, $"Failed download {targetFile} from {url}");
				return false;
			}

			Console.WriteLine(" done");
			return true;
		}

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
