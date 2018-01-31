using System.Linq;
using System.Windows.Forms;
using Mtgdb.Dal;
using Mtgdb.Downloader;
using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	public class DownloaderSubsystem
	{
		public bool NeedToSuggestDownloader { get; private set; }

		public DownloaderSubsystem(
			SuggestImageDownloaderConfig config,
			UpdateForm updateForm,
			NewsService newsService)
		{
			_config = config;
			_updateForm = updateForm;
			_newsService = newsService;
		}

		public void CalculateProgress()
		{
			_updateForm.CalculateProgress();

			var progress = _updateForm.ImageDownloadProgress
				.Where(p => !Str.Equals(p.QualityGroup.Quality, "Art"))
				.ToArray();

			var countTotal = progress
				.Sum(_ => _.FilesOnline?.Count ?? 0);

			var countDownloaded = progress
				.Where(_ => _.FilesOnline != null)
				.Sum(_ => _.FilesDownloaded.Count);

			var notDownloaded = countTotal - countDownloaded;

			NeedToSuggestDownloader = notDownloaded > 0 && _config.Enabled != false;
		}
		
		public void ShowDownloader(Form owner, bool auto)
		{
			_updateForm.IsShownAutomatically = auto;
			owner.Invoke(delegate
			{
				_updateForm.SetWindowLocation(owner);

				if (!_updateForm.Visible)
					_updateForm.Show(owner);

				if (!_updateForm.Focused)
					_updateForm.Focus();
			});
		}

		public void FetchNews(bool repeatViewed) => _newsService.FetchNews(repeatViewed);
		public bool NewsLoaded => _newsService.NewsLoaded;
		public bool HasUnreadNews => _newsService.HasUnreadNews;

		private readonly SuggestImageDownloaderConfig _config;
		private readonly UpdateForm _updateForm;
		private readonly NewsService _newsService;
	}
}