using System;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Dal;
using Mtgdb.Downloader;

namespace Mtgdb.Gui
{
	public class DownloaderSubsystem
	{
		public bool NeedToSuggestDownloader { get; private set; }

		public DownloaderSubsystem(
			SuggestImageDownloaderConfig config,
			UpdateForm updateForm)
		{
			_config = config;
			_updateForm = updateForm;
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

			ProgressCalculated?.Invoke();
		}

		public void ShowDownloader(Form owner, bool auto)
		{
			if (auto)
			{
				if (_wasAutoShown)
					return;

				_wasAutoShown = true;
			}

			_updateForm.IsShownAutomatically = auto;

			_updateForm.SetWindowLocation(owner);

			if (!_updateForm.Visible)
				_updateForm.Show(owner);

			if (!_updateForm.Focused)
				_updateForm.Focus();
		}

		public bool IsProgressCalculated => _updateForm.IsProgressCalculated;

		public event Action ProgressCalculated;

		private bool _wasAutoShown;

		private readonly SuggestImageDownloaderConfig _config;
		private readonly UpdateForm _updateForm;
	}
}