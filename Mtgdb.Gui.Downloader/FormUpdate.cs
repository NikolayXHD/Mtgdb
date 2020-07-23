using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Controls;
using Mtgdb.Data;
using ButtonBase = Mtgdb.Controls.ButtonBase;

namespace Mtgdb.Downloader
{
	public sealed partial class FormUpdate : CustomBorderForm
	{
		public FormUpdate()
		{
			InitializeComponent();
			Icon = Icon.ExtractAssociatedIcon(AppDir.Executable.Value);
		}

		[UsedImplicitly]
		public FormUpdate(
			Installer installer,
			ImageDownloader imageDownloader,
			ImageDownloadProgressReader imageDownloadProgressReader,
			NewsService newsService,
			ImageRepository imageRepository,
			CardRepository cardRepository,
			ImageLoader imageLoader,
			IApplication app)
			:this()
		{
			_installer = installer;
			_imageDownloader = imageDownloader;
			_imageDownloadProgressReader = imageDownloadProgressReader;
			_newsService = newsService;
			_imageRepository = imageRepository;
			_cardRepository = cardRepository;
			_imageLoader = imageLoader;
			_app = app;

			_buttonApp.Pressed += appClick;
			_buttonImgLq.Pressed += imgLqClick;
			_buttonImgMq.Pressed += imgMqClick;
			_buttonImgArt.Pressed += imgArtClick;
			_buttonMtgjson.Pressed += mtgjsonClick;
			_buttonPrices.Pressed += pricesClick;
			_buttonEditConfig.Pressed += editConfigClick;
			_buttonNotifications.Pressed += notificationsClick;

			Closing += closing;
			Closed += closed;
			Load += load;
			DoubleBuffered = true;

			_imageDownloader.ProgressChanged += downloadImageProgressChanged;
			ColorSchemeController.SystemColorsChanging += systemColorsChanged;

			RegisterDragControl(_labelTitle);

			scale();

			OnInitializationComplete();
		}

		public override string Text
		{
			get => base.Text;
			set
			{
				if (_labelTitle != null)
					_labelTitle.Text = value;

				base.Text = value;
			}
		}

		private void scale()
		{
			this.ScaleDpiSize();
			this.ScaleDpi();

			_labelTitle.ScaleDpiFont();

			_progressBar.ScaleDpiHeight();
			_tableLayoutButtons.ScaleDpi();
			_textBoxLog.ScaleDpiFont();

			_tableLayoutButtons.Controls
				.Cast<ButtonBase>()
				.ForEach(ButtonBaseScaler.ScaleDpiAuto);
		}

		private void systemColorsChanged() =>
			_textBoxLog.TouchColorProperties();

		public void CalculateProgress() =>
			ImageDownloadProgress = _imageDownloadProgressReader.GetProgress();

		private void load(object sender, EventArgs e)
		{
			_consoleWriter = new RichTextBoxWriter(_textBoxLog);
			Console.SetOut(_consoleWriter);

			if (IsShownAutomatically)
			{
				Console.WriteLine("Here you can download card images. After downloading images this window will stop showing up.");
				Console.WriteLine("This notification can be disabled in Advanced settings menu");
				Console.WriteLine("  in main window's title, left to color scheme icon,");
				Console.WriteLine("  select Missing images notification: No");
				Console.WriteLine();
			}

			_appVersionInstalled = getAppVersionInstalled();
			Text = $"{_appVersionInstalled.Replace(".zip", string.Empty)} - updates and downloads";

			Console.WriteLine("Downloaded images:");
			write(ImageDownloadProgress);

			_newsService.DisplayNews();
		}

		private void closing(object sender, CancelEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

		private void closed(object sender, EventArgs e) =>
			_consoleWriter.Dispose();

		private static void editConfigClick(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(AppDir.GeneralConfigXml.Value);
		}

		private void mtgjsonClick(object sender, EventArgs e)
		{
			_app.CancellationToken.Run(async token =>
			{
				setButtonsEnabled(false);
				Console.WriteLine();
				await _installer.DownloadMtgjson(token);
				setButtonsEnabled(true);
			});
		}

		private void pricesClick(object sender, EventArgs e)
		{
			_app.CancellationToken.Run(async token =>
			{
				setButtonsEnabled(false);
				Console.WriteLine();
				await _installer.DownloadPrices(token);
				setButtonsEnabled(true);
			});
		}

		private void appClick(object sender, EventArgs e)
		{
			if (!_appVersionOnlineChecked)
				_app.CancellationToken.Run(checkNewVersion);
			else if (_appVersionDownloaded == null)
				_app.CancellationToken.Run(downloadNewVersion);
			else
				_app.CancellationToken.Run(token => installNewVersion());
		}

		private void notificationsClick(object sender, EventArgs e)
		{
			setButtonsEnabled(false);
			_app.CancellationToken.Run(async token =>
			{
				await _newsService.FetchNews(repeatViewed: true, token);
				_newsService.DisplayNews();
				setButtonsEnabled(true);
			});
		}



		private async Task checkNewVersion(CancellationToken token)
		{
			setButtonsEnabled(false);

			Console.WriteLine();
			Console.WriteLine("Checking version online");

			var appVersionOnline = await getAppVersionOnline(token);
			var appVersionDownloaded = getAppVersionDownloaded();

			setButtonsEnabled(true);

			if (appVersionOnline == null && appVersionDownloaded == null)
				suggestCheckAppVersionOnline();
			else if (appVersionOnline != null && _installer.AppOnlineSignature.Md5Hash != _installer.AppDownloadedSignature?.Md5Hash )
				suggestDownloadApp(appVersionOnline);
			else
				suggestInstallApp(appVersionDownloaded);
		}

		private async Task downloadNewVersion(CancellationToken token)
		{
			if (_appVersionOnline == null)
				throw new InvalidOperationException("There is no online version to download");

			setButtonsEnabled(false);

			Console.WriteLine();
			await _installer.DownloadApp(token);
			bool downloadSuccess = _installer.ValidateDownloadedApp();

			setButtonsEnabled(true);

			if (downloadSuccess)
			{
				var appVersionDownloaded = getAppVersionDownloaded();
				suggestInstallApp(appVersionDownloaded);
			}
			else
			{
				suggestCheckAppVersionOnline();
			}

			Console.WriteLine();
		}

		private void installNewVersion()
		{
			if (_appVersionDownloaded == null)
				throw new InvalidOperationException("There is no downloaded version to install");

			setButtonsEnabled(false);

			_installer.Install(() => _appVersionInstalled = getAppVersionInstalled());

			setButtonsEnabled(true);
			suggestCheckAppVersionOnline();

			Console.WriteLine();
		}



		private void suggestCheckAppVersionOnline()
		{
			_appVersionOnlineChecked = false;

			this.Invoke(delegate
			{
				_buttonApp.Text = (string) _buttonApp.Tag;
			});
		}

		private void suggestInstallApp(string appVersionDownloaded)
		{
			if (appVersionDownloaded == null)
				throw new ArgumentNullException(nameof(appVersionDownloaded));

			this.Invoke(delegate
			{
				if (_buttonApp.Tag == null)
					_buttonApp.Tag = _buttonApp.Text;

				int versionDelta = _versionComparer.Compare(
					appVersionDownloaded.ToLower(Str.Culture).Replace(".zip", string.Empty),
					_appVersionInstalled.ToLower(Str.Culture).Replace(".zip", string.Empty));

				if (versionDelta > 0)
				{
					Console.WriteLine("A new version {0} is downloaded and ready to install. To proceed press 'Upgrade'...", appVersionDownloaded);
					_buttonApp.Text = "Upgrade";
				}
				else if (versionDelta == 0)
				{
					Console.WriteLine("Version {0} is already installed. To repeat installation press 'Reinstall'...", appVersionDownloaded);
					_buttonApp.Text = "Reinstall";
				}
				else
				{
					Console.WriteLine(
						"Currently installed version {0} is newer than downloaded {1}. To install older version press 'Downgrade'...",
						_appVersionInstalled,
						appVersionDownloaded);

					_buttonApp.Text = "Downgrade";
				}
			});

			_appVersionOnlineChecked = true;
			_appVersionDownloaded = appVersionDownloaded;
		}

		private void suggestDownloadApp(string appVersionOnline)
		{
			this.Invoke(delegate
			{
				if (_buttonApp.Tag == null)
					_buttonApp.Tag = _buttonApp.Text;

				int versionDelta = _versionComparer.Compare(appVersionOnline, _appVersionInstalled);

				if (versionDelta > 0)
					Console.WriteLine("A new version {0} is ready to download. To start downloading press 'Download'...", appVersionOnline);
				else if (versionDelta == 0)
					Console.WriteLine("The version {0} available online is already installed. To re-download it press 'Download'...", appVersionOnline);
				else
					Console.WriteLine(
						"Currently installed version {1} is newer than {0} available online. To start downloading press 'Download'...",
						appVersionOnline,
						_appVersionInstalled);

				_buttonApp.Text = "Download";
			});

			_appVersionOnlineChecked = true;
			_appVersionOnline = appVersionOnline;
		}



		private async Task<string> getAppVersionOnline(CancellationToken token)
		{
			await _installer.DownloadAppSignature(token);
			string result = _installer.AppOnlineSignature?.Path.Value;
			Console.WriteLine("Online version: {0}", result ?? "None");
			return result;
		}

		private string getAppVersionDownloaded()
		{
			string result = _installer.AppDownloadedSignature?.Path.Value;
			Console.WriteLine("Downloaded version: {0}", result ?? "None");
			return result;
		}

		private string getAppVersionInstalled()
		{
			string result = _installer.GetAppVersionInstalled();
			Console.WriteLine("Installed version: {0}", result ?? "None");
			return result;
		}



		private void downloadImageClick(object sender, string quality)
		{
			if (_downloadingImages)
			{
				setButtonsEnabled(false);
				Console.WriteLine("Interrupting...");
				_imageDownloader.Abort();
			}
			else
			{
				_app.CancellationToken.Run(async token =>
				{
					setButtonsEnabled(false);
					suggestAbortImageDownloading((ButtonBase) sender);

					Console.WriteLine();

					await _imageDownloadProgressReader.DownloadSignatures(quality, token);
					ImageDownloadProgress = _imageDownloadProgressReader.GetProgress();

					await _imageDownloader.Download(quality, ImageDownloadProgress, token);

					Console.WriteLine("Looking up downloaded images...");
					ImageDownloadProgress = _imageDownloadProgressReader.GetProgress();
					write(ImageDownloadProgress);

					Console.Write("Reloading images...");

					switch (quality)
					{
						case ImageQuality.Low:
							_imageRepository.LoadFilesSmall();
							_imageRepository.LoadSmall();
							resetCardImages();
							break;

						case ImageQuality.Medium:
							_imageRepository.LoadFilesZoom();
							_imageRepository.LoadZoom();
							resetCardImages();
							break;

						case ImageQuality.Art:
							_imageRepository.LoadFilesArt();
							_imageRepository.LoadArt();
							break;
					}

					Console.WriteLine(" Done");

					suggestImageDownloading((ButtonBase) sender);
					setButtonsEnabled(true);
				});
			}
		}

		private void resetCardImages()
		{
			_imageLoader.ClearCache();

			foreach (var card in _cardRepository.Cards)
				card.ResetImageModel();
		}

		private static void write(IReadOnlyList<ImageDownloadProgress> imageDownloadProgresses)
		{
			var progressByGroup = imageDownloadProgresses
				.GroupBy(_ => _.QualityGroup.Name)
				.ToDictionary(
					grQ => grQ.Key,
					grQ => grQ.ToList());

			foreach ((string quality, var progress) in progressByGroup)
			{
				int totalDirs = progress.Count;
				var downloadedDirs = progress.Where(_ => _.MayBeComplete).Select(_ => _.Dir.Subdir).ToArray();
				int downloadedDirsCount = downloadedDirs.Length;
				int totalFiles = progress.Sum(_ => _.FilesOnline?.Count ?? 0);
				var downloadedFiles = progress.Sum(_ => _.FilesDownloaded?.Count);

				Console.WriteLine($"{quality}: {downloadedDirsCount}/{totalDirs} directories, {downloadedFiles}/{totalFiles} files");
			}

			Console.WriteLine();
		}

		private void imgLqClick(object sender, EventArgs e)
		{
			if (Dpi.ScalePercent > 100)
			{
				var dlgResult = MessageBox.Show($@"Your screen DPI is {Dpi.ScalePercent}% from default. Small images will look blurry on your screen.
It is recommended to download zoom images only.

Are you sure you need small images? (Recommended answer is NO)",
					"Warning",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning);

				if (dlgResult != DialogResult.Yes)
					return;
			}

			downloadImageClick(sender, ImageQuality.Low);
		}

		private void imgMqClick(object sender, EventArgs e)
		{
			downloadImageClick(sender, ImageQuality.Medium);
		}

		private void imgArtClick(object sender, EventArgs e)
		{
			downloadImageClick(sender, ImageQuality.Art);
		}



		private void suggestAbortImageDownloading(ButtonBase button)
		{
			this.Invoke(delegate
			{
				_downloadingImages = true;

				_progressBar.Value = 0;
				_progressBar.Visible = true;

				_labelProgress.Text = null;
				_labelProgress.Visible = true;

				button.Enabled = true;
				button.Tag = button.Text;
				button.Text = "Abort";
			});
		}

		private void suggestImageDownloading(ButtonBase button)
		{
			this.Invoke(delegate
			{
				_downloadingImages = false;
				_progressBar.Visible = false;
				_labelProgress.Visible = false;

				button.Text = (string) button.Tag;
			});
		}

		private void setButtonsEnabled(bool enabled)
		{
			this.Invoke(delegate
			{
				_buttonApp.Enabled =
				_buttonMtgjson.Enabled =
				_buttonImgArt.Enabled =
				_buttonImgLq.Enabled =
				_buttonImgMq.Enabled =
				_buttonNotifications.Enabled = enabled;
			});
		}



		private void buttonDesktopShortcut(object sender, EventArgs e)
		{
			_installer.CreateApplicationShortcut(new FsPath(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)));
		}

		private void downloadImageProgressChanged()
		{
			this.Invoke(delegate
			{
				setProgress(
					_imageDownloader.TotalCount,
					_imageDownloader.DownloadedCount,
					"{0} / {1} files ready");
			});
		}

		private void setProgress(int total, int current, string messageFormat)
		{
			_progressBar.Maximum = total;
			_progressBar.Value = Math.Min(current, total);
			_labelProgress.Text = string.Format(messageFormat, current, total);
		}

		public void SetWindowLocation(Form owner)
		{
			StartPosition = FormStartPosition.Manual;

			Location = new Point(
				owner.Left + (owner.Width - Width) / 2,
				owner.Top + (owner.Height - Height) / 2);
		}

		public bool IsShownAutomatically { get; set; }

		public IReadOnlyList<ImageDownloadProgress> ImageDownloadProgress { get; private set; }
		public bool IsProgressCalculated => ImageDownloadProgress != null;

		public bool AreSignaturesDownloaded(QualityGroupConfig qualityGroup) =>
			_imageDownloadProgressReader.SignaturesFileExist(qualityGroup);


		private bool _downloadingImages;
		private bool _appVersionOnlineChecked;
		private string _appVersionInstalled;
		private string _appVersionOnline;
		private string _appVersionDownloaded;

		private readonly Installer _installer;
		private readonly ImageDownloader _imageDownloader;
		private readonly ImageDownloadProgressReader _imageDownloadProgressReader;
		private readonly NewsService _newsService;
		private readonly ImageRepository _imageRepository;
		private readonly CardRepository _cardRepository;
		private readonly ImageLoader _imageLoader;
		private readonly IApplication _app;
		private readonly VersionComparer _versionComparer = new VersionComparer();
		private RichTextBoxWriter _consoleWriter;
	}
}
