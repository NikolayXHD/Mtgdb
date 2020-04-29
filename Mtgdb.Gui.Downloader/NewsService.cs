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
			FsPath newsDir = AppDir.Update.Join("notifications");

			_newsArchive = newsDir.Join("archive.zip");

			_unzippedNewsDir = newsDir.Join("archive");
			_unreadNewsDir = newsDir.Join("new");
			_readNewsDir = newsDir.Join("read");

			_unzippedNewsDir.CreateDirectory();
			_unreadNewsDir.CreateDirectory();
			_readNewsDir.CreateDirectory();
		}

		public void DisplayNews()
		{
			if (_unreadNews == null)
				return;

			foreach (var file in _unreadNews)
			{
				FsPath readAnnounceFile = getReadNewsFile(file);

				string text = file.ReadAllText().Trim();

				bool isLocked = file.Value.IndexOf("[locked]", Str.Comparison) >= 0;

				if (!isLocked && !readAnnounceFile.IsFile())
					file.MoveFileTo(readAnnounceFile);

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

			_unreadNews = _unreadNewsDir.EnumerateFiles("*.txt")
				.Where(file => repeatViewed || !getReadNewsFile(file).IsFile())
				.OrderByDescending(_=>_.Basename(extension: false), _versionComparer)
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
			if (_newsArchive.IsFile())
			{
				_unzippedNewsDir.EnsureEmptyDirectory();

				new FastZip().ExtractZip(_newsArchive.Value, _unzippedNewsDir.Value, fileFilter: null);
				var unzipped = _unzippedNewsDir.EnumerateFiles("*.txt", SearchOption.AllDirectories);

				_unreadNewsDir.EnsureEmptyDirectory();

				foreach (var file in unzipped)
					file.CopyFileTo(getUnreadNewsFile(file));
			}
		}

		private FsPath getUnreadNewsFile(FsPath file)
		{
			return _unreadNewsDir.Join(file.Basename());
		}

		private FsPath getReadNewsFile(FsPath file)
		{
			return _readNewsDir.Join(file.Basename());
		}

		public event Action NewsFetched;
		public event Action NewsDisplayed;

		public bool NewsLoaded => _unreadNews != null;
		public bool HasUnreadNews => _unreadNews != null && _unreadNews.Count > 0;

		private List<FsPath> _unreadNews;

		private readonly FsPath _unzippedNewsDir;
		private readonly FsPath _unreadNewsDir;
		private readonly FsPath _readNewsDir;
		private readonly FsPath _newsArchive;
		private readonly AppSourceConfig _appSourceConfig;

		private readonly VersionComparer _versionComparer = new VersionComparer();

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
