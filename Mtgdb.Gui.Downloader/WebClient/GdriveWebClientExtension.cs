using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mtgdb.Downloader
{
	public static class GdriveWebClientExtension
	{
		public static async Task<bool> DownloadAndExtract(this GdriveWebClient webClient, string gdriveUrl, string targetDirectory, string fileName, CancellationToken token)
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
				await webClient.DownloadFromGdrive(gdriveUrl, targetDirectory, token);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error downloading {archiveFileName} from {gdriveUrl}: {ex.Message}");
				return false;
			}

			if (!File.Exists(archiveFileName))
			{
				Console.WriteLine($"Failed to download {archiveFileName} from {gdriveUrl}");
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

		public static async Task TryDownload(this GdriveWebClient webClient, string url, string targetDirectory, string description, CancellationToken token)
		{
			Console.Write($"Download {description} from {url} ...");

			try
			{
				await webClient.DownloadFromGdrive(url, targetDirectory, token);
			}
			catch (Exception ex)
			{
				Console.WriteLine($" {ex}");
				return;
			}

			Console.WriteLine(" done");
		}
	}
}
