using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using JetBrains.Annotations;
using NLog;

namespace Mtgdb.Downloader
{
	public class NewsService
	{
		[UsedImplicitly] // by ninject
		public NewsService(AppSourceConfig appSourceConfig)
		{
			_appSourceConfig = appSourceConfig;
			string newsDir = AppDir.Update.AddPath("notifications");

			_newsArchive = newsDir.AddPath("archive.zip");

			_unzippedNewsDir = newsDir.AddPath("archive");
			_unreadNewsDir = newsDir.AddPath("new");
			_readNewsDir = newsDir.AddPath("read");

			Directory.CreateDirectory(_unzippedNewsDir);
			Directory.CreateDirectory(_unreadNewsDir);
			Directory.CreateDirectory(_readNewsDir);
		}

		public void DisplayNews()
		{
			if (_unreadNews == null)
				return;

			foreach (var file in _unreadNews)
			{
				string readAnnounceFile = getReadNewsFile(file);

				string text = File.ReadAllText(file).Trim();

				bool isLocked = file.IndexOf("[locked]", Str.Comparison) >= 0;

				if (!isLocked && !File.Exists(readAnnounceFile))
					File.Move(file, readAnnounceFile);

				if (string.IsNullOrEmpty(text))
					continue;

				Console.WriteLine();

				Console.WriteLine(text);
				Console.WriteLine();
			}

			_unreadNews.Clear();

			NewsDisplayed?.Invoke();
		}


		public Task FetchNews(CancellationToken token) =>
			FetchNews(repeatViewed: false, token);

		public async Task FetchNews(bool repeatViewed, CancellationToken token)
		{
			await downloadNews(token);
			unpackNews();

			_unreadNews = Directory.GetFiles(_unreadNewsDir, "*.txt", SearchOption.TopDirectoryOnly)
				.Where(file => repeatViewed || !File.Exists(getReadNewsFile(file)))
				.OrderByDescending(Path.GetFileNameWithoutExtension, _versionComparer)
				.ToList();

			NewsFetched?.Invoke();
		}

		private async Task downloadNews(CancellationToken token)
		{
			Console.Write("Fetching update news... ");

			try
			{
				var webClient = new WebClientBase();
				await webClient.DownloadFile(_appSourceConfig.NewsUrl, _newsArchive, token);

				Console.WriteLine("done");
			}
			catch (Exception ex)
			{
				_log.Info(ex, "Download news failed");
				Console.WriteLine("failed");
			}

			Console.WriteLine();
		}

		private void unpackNews()
		{
			if (File.Exists(_newsArchive))
			{
				_unzippedNewsDir.EmptyDirectory();

				new FastZip().ExtractZip(_newsArchive, _unzippedNewsDir, fileFilter: null);
				var unzipped = Directory.GetFiles(_unzippedNewsDir, "*.txt", SearchOption.AllDirectories);

				_unreadNewsDir.EmptyDirectory();

				foreach (var file in unzipped)
					File.Copy(file, getUnreadNewsFile(file));
			}
		}

		private string getUnreadNewsFile(string file)
		{
			return _unreadNewsDir.AddPath(Path.GetFileName(file));
		}

		private string getReadNewsFile(string file)
		{
			return _readNewsDir.AddPath(Path.GetFileName(file));
		}

		public event Action NewsFetched;
		public event Action NewsDisplayed;

		public bool NewsLoaded => _unreadNews != null;
		public bool HasUnreadNews => _unreadNews != null && _unreadNews.Count > 0;

		private List<string> _unreadNews;

		private readonly string _unzippedNewsDir;
		private readonly string _unreadNewsDir;
		private readonly string _readNewsDir;
		private readonly string _newsArchive;
		private readonly AppSourceConfig _appSourceConfig;

		private readonly VersionComparer _versionComparer = new VersionComparer();

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
