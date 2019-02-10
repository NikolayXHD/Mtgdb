using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Controls;
using Mtgdb.Dal;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Downloader
{
	public sealed partial class FormUpdate : CustomBorderForm
	{
		public FormUpdate()
		{
			InitializeComponent();
			Icon = Icon.ExtractAssociatedIcon(AppDir.Executable);
		}

		[UsedImplicitly]
		public FormUpdate(
			Installer installer,
			ImageDownloader imageDownloader,
			ImageDownloadProgressReader imageDownloadProgressReader,
			NewsService newsService,
			PriceDownloader priceDownloader,
			PriceRepository priceRepository,
			ImageRepository imageRepository,
			CardRepository cardRepository,
			ImageLoader imageLoader)
			:this()
		{
			_installer = installer;
			_imageDownloader = imageDownloader;
			_imageDownloadProgressReader = imageDownloadProgressReader;
			_newsService = newsService;
			_priceDownloader = priceDownloader;
			_priceRepository = priceRepository;
			_imageRepository = imageRepository;
			_cardRepository = cardRepository;
			_imageLoader = imageLoader;

			_buttonApp.Click += appClick;
			_buttonImgLq.Click += imgLqClick;
			_buttonImgMq.Click += imgMqClick;
			_buttonImgArt.Click += imgArtClick;
			_buttonMtgjson.Click += mtgjsonClick;
			_buttonPrices.Click += pricesClick;
			_buttonEditConfig.Click += editConfigClick;
			_buttonNotifications.Click += notificationsClick;

			Closing += closing;
			Load += load;
			DoubleBuffered = true;

			_imageDownloader.ProgressChanged += downloadImageProgressChanged;
			_priceDownloader.SidAdded += downloadPricesProgressChanged;
			_priceDownloader.PriceAdded += downloadPricesProgressChanged;

			ColorSchemeController.SystemColorsChanging += systemColorsChanged;

			scale();
		}

		private void scale()
		{
			this.ScaleDpiSize();
			this.ScaleDpi();

			_progressBar.ScaleDpiHeight();
			_tableLayoutButtons.ScaleDpi();
			_textBoxLog.ScaleDpiFont();

			_buttonImagesScaler.Setup(this);
		}

		private void systemColorsChanged() =>
			_textBoxLog.TouchColorProperties();

		public void CalculateProgress()
		{
			ImageDownloadProgress = _imageDownloadProgressReader.GetProgress();
		}

		private void load(object sender, EventArgs e)
		{
			Console.SetOut(new RichTextBoxWriter(_textBoxLog));

			if (IsShownAutomatically)
			{
				Console.WriteLine("Here you can download card images. After downloading images this window will stop showing up.");
				Console.WriteLine("If you already have card images edit <ImageLocations /> section in configuration file.");
				Console.WriteLine("To prevent showing this window set <SuggestImageDownloader Enabled=\"False\" />");
				Console.WriteLine("To edit configuration click 'Edit configuration' button...");
				Console.WriteLine();
			}

			_appVersionInstalled = getAppVersionInstalled();
			Console.WriteLine("Downloaded images:");
			write(ImageDownloadProgress);

			_newsService.DisplayNews();
		}

		private void closing(object sender, CancelEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

		private static void editConfigClick(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(AppDir.GeneralConfigXml);
		}

		private void mtgjsonClick(object sender, EventArgs e)
		{
			ThreadPool.QueueUserWorkItem(_ =>
			{
				setButtonsEnabled(false);
				Console.WriteLine();
				_installer.DownloadMtgjson();
				setButtonsEnabled(true);
			});
		}

		private void appClick(object sender, EventArgs e)
		{
			if (!_appVersionOnlineChecked)
				ThreadPool.QueueUserWorkItem(checkNewVersion);
			else if (_appVersionDownloaded == null)
				ThreadPool.QueueUserWorkItem(downloadNewVersion);
			else
				ThreadPool.QueueUserWorkItem(installNewVersion);
		}

		private void notificationsClick(object sender, EventArgs e)
		{
			setButtonsEnabled(false);

			ThreadPool.QueueUserWorkItem(_ =>
			{
				_newsService.FetchNews(repeatViewed: true);
				_newsService.DisplayNews();

				setButtonsEnabled(true);
			});
		}



		private void checkNewVersion(object _)
		{
			setButtonsEnabled(false);

			Console.WriteLine();
			Console.WriteLine("Checking version online");

			var appVersionOnline = getAppVersionOnline();
			var appVersionDownloaded = getAppVersionDownloaded();

			setButtonsEnabled(true);

			if (appVersionOnline == null && appVersionDownloaded == null)
				suggestCheckAppVersionOnline();
			else if (appVersionOnline != null && _installer.AppOnlineSignature.Md5Hash != _installer.AppDownloadedSignature?.Md5Hash )
				suggestDownloadApp(appVersionOnline);
			else
				suggestInstallApp(appVersionDownloaded);
		}

		private void downloadNewVersion(object _)
		{
			if (_appVersionOnline == null)
				throw new InvalidOperationException("There is no online version to download");

			setButtonsEnabled(false);

			Console.WriteLine();
			_installer.DownloadApp();
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

		private void installNewVersion(object _)
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



		private string getAppVersionOnline()
		{
			_installer.DownloadAppSignature();
			string result = _installer.AppOnlineSignature?.Path;
			Console.WriteLine("Online version: {0}", result ?? "None");
			return result;
		}

		private string getAppVersionDownloaded()
		{
			string result = _installer.AppDownloadedSignature?.Path;
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
				ThreadPool.QueueUserWorkItem(_ =>
				{
					setButtonsEnabled(false);
					suggestAbortImageDownloading((Button) sender);

					Console.WriteLine();

					_imageDownloadProgressReader.DownloadSignatures(quality);
					ImageDownloadProgress = _imageDownloadProgressReader.GetProgress();

					_imageDownloader.Download(quality, ImageDownloadProgress);

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
							break;
						case ImageQuality.Art:
							_imageRepository.LoadFilesArt();
							_imageRepository.LoadArt();
							break;
					}

					Console.WriteLine(" Done");

					suggestImageDownloading((Button) sender);
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

		private static void write(IList<ImageDownloadProgress> imageDownloadProgresses)
		{
			var progressByQuality = imageDownloadProgresses
				.GroupBy(_ => _.QualityGroup.Quality)
				.ToDictionary(
					grQ => grQ.Key,
					grQ => grQ.ToList());

			foreach (var pair in progressByQuality)
			{
				var quality = pair.Key;
				var totalDirs = pair.Value.Count;
				var downloadedDirs = pair.Value.Where(_ => _.MayBeComplete).Select(_ => _.Dir.Subdirectory).ToArray();
				var downloadedDirsCount = downloadedDirs.Length;
				var totalFiles = pair.Value.Sum(_ => _.FilesOnline?.Count ?? 0);
				var downloadedFiles = pair.Value.Sum(_ => _.FilesDownloaded?.Count);

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



		private void suggestAbortImageDownloading(Button button)
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

		private void suggestImageDownloading(Button button)
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
				_buttonPrices.Enabled =
				_buttonImgArt.Enabled =
				_buttonImgLq.Enabled =
				_buttonImgMq.Enabled =
				_buttonNotifications.Enabled = enabled;
			});
		}



		private void buttonDesktopShortcut(object sender, EventArgs e)
		{
			_installer.CreateApplicationShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
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


		private void pricesClick(object sender, EventArgs e)
		{
			if (_downloadingPrices)
			{
				setButtonsEnabled(false);
				_priceDownloader.Abort();
				Console.WriteLine("Interrupting...");
			}
			else
			{
				ThreadPool.QueueUserWorkItem(_ =>
				{
					setButtonsEnabled(false);
					suggestAbortPriceDownloading();

					Console.WriteLine("Downloading prices ...");

					_priceDownloader.LoadPendingProgress();
					_priceDownloader.Download();

					Console.WriteLine("Done");

					suggestPriceDownloading();
					setButtonsEnabled(true);
				});
			}
		}

		private void suggestAbortPriceDownloading()
		{
			this.Invoke(delegate
			{
				var button = _buttonPrices;

				_downloadingPrices = true;

				_progressBar.Value = 0;
				_progressBar.Visible = true;

				_labelProgress.Text = null;
				_labelProgress.Visible = true;

				button.Enabled = true;
				button.Tag = button.Text;
				button.Text = "Abort";
			});
		}

		private void suggestPriceDownloading()
		{
			this.Invoke(delegate
			{
				var button = _buttonPrices;

				_downloadingPrices = false;
				_progressBar.Visible = false;
				_labelProgress.Visible = false;

				button.Text = (string)button.Tag;
			});
		}

		private void downloadPricesProgressChanged()
		{
			this.Invoke(delegate
			{
				setProgress(
					_priceDownloader.DefinedCardsCount * 2 - _priceRepository.SidCount,
					_priceDownloader.SidCount + _priceDownloader.PricesCount - _priceRepository.SidCount,
					"{0} / {1} operations done");
			});
		}

		public void SetWindowLocation(Form owner)
		{
			StartPosition = FormStartPosition.Manual;

			Location = new Point(
				owner.Left + (owner.Width - Width) / 2,
				owner.Top + (owner.Height - Height) / 2);
		}

		public bool IsShownAutomatically { get; set; }

		public IList<ImageDownloadProgress> ImageDownloadProgress { get; private set; }

		public bool IsProgressCalculated => ImageDownloadProgress != null;

		private bool _downloadingImages;
		private bool _downloadingPrices;
		private bool _appVersionOnlineChecked;
		private string _appVersionInstalled;
		private string _appVersionOnline;
		private string _appVersionDownloaded;

		private static readonly DpiScaler<FormUpdate, IReadOnlyList<Bitmap>> _buttonImagesScaler =
			new DpiScaler<FormUpdate, IReadOnlyList<Bitmap>>(

				f => f._tableLayoutButtons.Controls
					.Cast<Button>()
					.Select(b => b.Image)
					.Cast<Bitmap>()
					.ToReadOnlyList(),
				// materialize to correctly remember the initial state
				// the remembered IEnumerable would return modified values on second Scale() call

				(f, bitmaps) => f._tableLayoutButtons.Controls
					.Cast<Button>()
					.Zip(bitmaps, (btn, bmp) => (btn, bmp))
					.ForEach(
						_ => _.btn.Image = _.bmp),

				bitmaps => bitmaps
					.Select(bmp => bmp.HalfResizeDpi())
					.ToReadOnlyList());

		private readonly Installer _installer;
		private readonly ImageDownloader _imageDownloader;
		private readonly ImageDownloadProgressReader _imageDownloadProgressReader;
		private readonly NewsService _newsService;
		private readonly PriceDownloader _priceDownloader;
		private readonly PriceRepository _priceRepository;
		private readonly ImageRepository _imageRepository;
		private readonly CardRepository _cardRepository;
		private readonly ImageLoader _imageLoader;
		private readonly VersionComparer _versionComparer = new VersionComparer();
	}
}
