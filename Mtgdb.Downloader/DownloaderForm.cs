using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Updater;

namespace Mtgdb.Downloader
{
	public sealed partial class DownloaderForm : CustomBorderForm
	{
		private readonly Installer _installer;
		private readonly ImageDownloader _imageDownloader;
		private readonly ImageDownloadProgressReader _imageDownloadProgressReader;
		private static readonly VersionComparer _versionComparer = new VersionComparer();

		private bool _downloadingImages;
		private bool _appVersionOnlineChecked;
		private string _appVersionInstalled;
		private string _appVersionOnline;
		private string _appVersionDownloaded;
		public IList<ImageDownloadProgress> ImageDownloadProgress { get; private set; }

		public bool IsShownAutomatically { get; set; }

		public DownloaderForm()
		{
			InitializeComponent();
		}

		public DownloaderForm(
			Installer installer,
			ImageDownloader imageDownloader,
			ImageDownloadProgressReader imageDownloadProgressReader)
			:this()
		{
			_installer = installer;
			_imageDownloader = imageDownloader;
			_imageDownloadProgressReader = imageDownloadProgressReader;

			_buttonApp.Click += appClick;
			_buttonImgLq.Click += imgLqClick;
			_buttonImgMq.Click += imgMqClick;
			_buttonImgArt.Click += imgArtClick;
			_buttonsMtgjson.Click += mtgsonClick;
			_buttonEditConfig.Click += editConfigClick;

			Closing += closing;
			Load += load;
			DoubleBuffered = true;

			_imageDownloader.FileDownloaded += imageDownloaded;
		}

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

		private void mtgsonClick(object sender, EventArgs e)
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



		private void checkNewVersion(object _)
		{
			setButtonsEnabled(false);

			Console.WriteLine();
			Console.WriteLine("Checking version online...");

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
			Console.WriteLine("Downloading {0}...", _appVersionOnline);

			_installer.DownloadApp();
			bool downloadSuccess = _installer.ValidateDownloadedApp();

			setButtonsEnabled(true);

			if (downloadSuccess)
			{
				var appVersionDownloaded = getAppVersionDownloaded();
				suggestInstallApp(appVersionDownloaded);
			}
			else
				suggestCheckAppVersionOnline();
		}

		private void installNewVersion(object _)
		{
			if (_appVersionDownloaded == null)
				throw new InvalidOperationException("There is no downloaded version to install");

			setButtonsEnabled(false);

			Console.WriteLine();
			_installer.Install();
			_installer.CreateApplicationShortcut(AppDir.Root);
			_installer.UpdateApplicationShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

			setButtonsEnabled(true);

			_appVersionInstalled = getAppVersionInstalled();
			Console.WriteLine("The new version will be launched on next start.");

			suggestCheckAppVersionOnline();
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
					appVersionDownloaded.ToLowerInvariant().Replace(".zip", string.Empty),
					_appVersionInstalled.ToLowerInvariant().Replace(".zip", string.Empty));

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
					
					suggestImageDownloading((Button) sender);
					setButtonsEnabled(true);
				});
			}
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
				var downloadedDirs = pair.Value.Where(_ => _.MayBeComplete).Select(_ => _.MegaDir.Subdirectory).ToArray();
				var downloadedDirsCount = downloadedDirs.Length;
				var totalFiles = pair.Value.Sum(_ => _.FilesOnline?.Count ?? 0);
				var downloadedFiles = pair.Value.Sum(_ => _.FilesDownloaded?.Count);

				Console.WriteLine($"{quality}: {downloadedDirsCount}/{totalDirs} directories, {downloadedFiles}/{totalFiles} files");
			}
		}

		private void imgLqClick(object sender, EventArgs e)
		{
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
				_buttonsMtgjson.Enabled =
				_buttonImgArt.Enabled =
				_buttonImgLq.Enabled =
				_buttonImgMq.Enabled = enabled;
			});
		}

		private void buttonDesktopShortcut(object sender, EventArgs e)
		{
			_installer.CreateApplicationShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
		}

		private void imageDownloaded()
		{
			this.Invoke(delegate
			{
				_progressBar.Maximum = _imageDownloader.TotalCount;
				_progressBar.Value = Math.Min(_imageDownloader.DownloadedCount, _imageDownloader.TotalCount);

				_labelProgress.Text = $"{_imageDownloader.DownloadedCount} / {_imageDownloader.TotalCount} files ready";
			});
		}

		public void SetWindowLocation(Form owner)
		{
			this.StartPosition = FormStartPosition.Manual;

			this.Location = new Point(
				owner.Left + (owner.Width - this.Width) / 2,
				owner.Top + (owner.Height - this.Height) / 2);
		}
	}
}
