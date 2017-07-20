using System.Linq;
using System.Windows.Forms;
using Mtgdb.Dal;
using Mtgdb.Downloader;
using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	public class DownloaderSubsystem
	{
		private readonly SuggestImageDownloaderConfig _config;
		private readonly DownloaderForm _downloaderForm;

		public bool NeedToSuggestDownloader { get; private set; }

		public DownloaderSubsystem(
			SuggestImageDownloaderConfig config,
			DownloaderForm downloaderForm)
		{
			_config = config;
			_downloaderForm = downloaderForm;
		}

		public void CalculateProgress()
		{
			_downloaderForm.CalculateProgress();

			var progress = _downloaderForm.ImageDownloadProgress;

			var countTotal = progress
				.Sum(_ => _.FilesOnline?.Count ?? 0);

			var countDownloaded = progress
				.Where(_ => _.FilesOnline != null)
				.Sum(_ => _.FilesDownloaded.Count);

			var notDownloaded = countTotal - countDownloaded;

			NeedToSuggestDownloader = notDownloaded > 0 && _config.Enabled != false;
		}
		
		public void ShowDownloader(Form parent, bool auto)
		{
			_downloaderForm.IsShownAutomatically = auto;
			parent.Invoke(delegate
			{
				_downloaderForm.Show(parent);

				if (!_downloaderForm.Focused)
					_downloaderForm.Focus();
			});
		}
	}
}