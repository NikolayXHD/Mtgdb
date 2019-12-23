using System;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Data;
using Mtgdb.Downloader;

namespace Mtgdb.Gui
{
	public class DownloaderSubsystem
	{
		public bool NeedToSuggestDownloader { get; private set; }

		[UsedImplicitly] // by ninject
		public DownloaderSubsystem(
			UiConfigRepository uiConfigRepository,
			FormUpdate formUpdate)
		{
			_uiConfigRepository = uiConfigRepository;
			_formUpdate = formUpdate;
		}

		public void CalculateProgress()
		{
			_formUpdate.CalculateProgress();

			var progress = _formUpdate.ImageDownloadProgress
				.Where(p => !Str.Equals(p.QualityGroup.Quality, "Art"))
				.ToArray();

			var countTotal = progress
				.Sum(_ => _.FilesOnline?.Count ?? 0);

			var countDownloaded = progress
				.Where(_ => _.FilesOnline != null)
				.Sum(_ => _.FilesDownloaded.Count);

			var notDownloaded = countTotal - countDownloaded;

			bool signaturesAreMissing = progress.Any(p => !_formUpdate.AreSignaturesDownloaded(p.QualityGroup));

			NeedToSuggestDownloader = _uiConfigRepository.Config.SuggestDownloadMissingImages &&
				(signaturesAreMissing || notDownloaded > 0);

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

			_formUpdate.IsShownAutomatically = auto;

			_formUpdate.SetWindowLocation(owner);

			if (!_formUpdate.Visible)
				_formUpdate.Show(owner);

			if (!_formUpdate.Focused)
				_formUpdate.Focus();
		}

		public bool IsProgressCalculated => _formUpdate.IsProgressCalculated;

		public event Action ProgressCalculated;

		private bool _wasAutoShown;

		private readonly UiConfigRepository _uiConfigRepository;
		private readonly FormUpdate _formUpdate;
	}
}
