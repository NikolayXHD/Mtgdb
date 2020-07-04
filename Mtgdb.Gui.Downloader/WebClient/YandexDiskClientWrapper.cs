using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mtgdb.Downloader
{
	public class YandexDiskClientWrapper
	{
		public YandexDiskClientWrapper(YandexDiskClient client, string key, object syncOutput = null)
		{
			_key = key;
			_client = client;
			_syncOutput = syncOutput ?? new object();
		}

		public async Task<bool> TryDownloadFile(string remotePath, FsPath targetPath, CancellationToken token)
		{
			string link;
			lock (_syncOutput)
				Console.Write("{0}: get YandexDisk download url  ... ", remotePath);
			try
			{
				link = await _client.GetDownloadLink(_key, remotePath, token);
			}
			catch (Exception ex)
			{
				lock (_syncOutput)
				{
					Console.WriteLine("failed");
					Console.WriteLine(ex.ToString());
				}

				return false;
			}

			lock (_syncOutput)
				Console.Write("downloading ... ");
			try
			{
				await _client.DownloadFile(link, targetPath, token);
			}
			catch (Exception ex)
			{
				lock (_syncOutput)
				{
					Console.WriteLine("failed");
					Console.WriteLine(ex.ToString());
				}
				return false;
			}

			lock (_syncOutput)
				Console.WriteLine("done");
			return true;
		}

		public async Task<bool> DownloadAndExtract(string remotePath, FsPath targetDirectory, FsPath fileName,  CancellationToken token)
		{
			if (!Str.Equals(".7z", fileName.Extension()))
				throw new ArgumentException();

			FsPath archiveFileName = targetDirectory.Join(fileName);

			if (archiveFileName.IsFile())
			{
				try
				{
					archiveFileName.DeleteFile();
				}
				catch (Exception ex)
				{
					lock (_syncOutput)
						Console.WriteLine($"Failed to remove {archiveFileName}: {ex.Message}");
					return false;
				}
			}

			bool downloaded = await TryDownloadFile(remotePath, archiveFileName, token);
			if (!downloaded)
				return false;

			if (!archiveFileName.IsFile())
			{
				lock (_syncOutput)
					Console.WriteLine($"Failed to download {archiveFileName} from {remotePath}");
				return false;
			}

			var sevenZip = new SevenZip(silent: true);
			sevenZip.Extract(archiveFileName, targetDirectory, Enumerable.Empty<FsPath>());

			try
			{
				archiveFileName.DeleteFile();
			}
			catch (Exception ex)
			{
				lock (_syncOutput)
					Console.WriteLine($"Failed to remove {archiveFileName}: {ex.Message}");
			}

			return true;
		}

		private readonly string _key;
		private readonly YandexDiskClient _client;
		private readonly object _syncOutput;
	}
}

