using System;
using System.IO;
using System.Linq;

namespace Mtgdb.Downloader
{
	public static class GdriveWebClientExtension
	{
		public static bool DownloadAndExtract(this GdriveWebClient webClient, string gdriveUrl, string targetDirectory, string fileName)
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
				webClient.DownloadFromGdrive(gdriveUrl, targetDirectory);
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
	}
}