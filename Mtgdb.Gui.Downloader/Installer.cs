using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using IWshRuntimeLibrary;
using JetBrains.Annotations;
using Mtgdb.Data;
using Mtgdb.Data.Index;
using File = System.IO.File;

namespace Mtgdb.Downloader
{
	public class Installer : IDataDownloader
	{
		[UsedImplicitly] // by ninject
		public Installer(
			AppSourceConfig appSourceConfig,
			MtgjsonSourceConfig mtgjsonSourceConfig,
			CardSearcher cardSearcher,
			KeywordSearcher keywordSearcher)
		{
			_appSourceConfig = appSourceConfig;
			_mtgjsonSourceConfig = mtgjsonSourceConfig;
			_updateAppDir = AppDir.Update.AddPath("app");

			_appOnlineSignatureFile = Path.Combine(_updateAppDir, Signer.SignaturesFile);
			_appDownloadedSignatureFile = AppDir.Update.AddPath(Signer.SignaturesFile);
			_appInstalledVersionFile = AppDir.Update.AddPath("version.txt");

			AppDownloadedSignature = getAppDownloadedSignature();

			_webClient = new WebClientBase();

			_protectedFiles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
			{
				AppDir.GeneralConfigXml,
				AppDir.DisplayConfigXml,
				cardSearcher.IndexDirectory.AddPath("*"),
				cardSearcher.Spellchecker.IndexDirectory.AddPath("*"),
				keywordSearcher.IndexDirectory.AddPath("*")
			};
		}

		public Task DownloadMtgjson(CancellationToken token) =>
			downloadDataZip(_mtgjsonSourceConfig.Url, token);

		public Task DownloadPrices(CancellationToken token) =>
			downloadDataZip(_mtgjsonSourceConfig.PriceUrl, token);

		private async Task downloadDataZip(string url, CancellationToken token)
		{
			Console.WriteLine("GET {0}", url);

			try
			{
				var stream = await _webClient.DownloadStream(url, token);
				if (stream == null)
				{
					Console.WriteLine("Failed request to mtgjson.com: empty response");
					Console.WriteLine();
					return;
				}

				Console.WriteLine("Downloading complete.");
				using (stream)
				{
					Console.WriteLine("Extracting to {0}", AppDir.Data);
					new FastZip().ExtractZip(
						stream,
						AppDir.Data,
						FastZip.Overwrite.Always,
						name => true,
						fileFilter: null,
						directoryFilter: null,
						restoreDateTime: true,
						isStreamOwner: false);

					MtgjsonFileUpdated?.Invoke();

					Console.WriteLine("Done. New data will be available after restart.");
					Console.WriteLine();
				}
			}
			catch (AggregateException ex)
			{
				Console.WriteLine("Failed request to mtgjson.com: {0}",
					string.Join(", ", ex.InnerExceptions.Select(_ => _.Message)));
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed request to mtgjson.com: {0}", ex.Message);
			}
		}

		public string GetAppVersionInstalled()
		{
			if (!File.Exists(_appInstalledVersionFile))
			{
				string versionFromAssembly = getVersionFromAssembly();
				return versionFromAssembly;
			}

			var result = File.ReadAllText(_appInstalledVersionFile);
			return result;
		}

		public async Task DownloadAppSignature(CancellationToken token)
		{
			ensureFileDeleted(_appOnlineSignatureFile);
			Directory.CreateDirectory(_updateAppDir);
			await new GdriveWebClient().TryDownload(
				_appSourceConfig.FileListUrl, _appOnlineSignatureFile, token);
			AppOnlineSignature = getAppOnlineSignature();
		}

		public async Task DownloadApp(CancellationToken token)
		{
			var expectedSignature = AppOnlineSignature;

			string appOnline = Path.Combine(_updateAppDir, expectedSignature.Path);
			ensureFileDeleted(appOnline);
			Directory.CreateDirectory(_updateAppDir);
			await new GdriveWebClient().TryDownload(
				_appSourceConfig.ZipUrl, appOnline, token);
		}

		public bool ValidateDownloadedApp()
		{
			if (AppOnlineSignature == null)
				throw new InvalidOperationException("Cannot validate downloaded app. Online signature must be downloaded first");

			var expectedSignature = AppOnlineSignature;
			var appOnline = _updateAppDir.AddPath(expectedSignature.Path);
			var appDownloaded = AppDir.Update.AddPath(expectedSignature.Path);

			if (!File.Exists(appOnline))
			{
				Console.WriteLine("Failed to download {0}", expectedSignature.Path);
				return false;
			}

			var signature = Signer.CreateSignature(appOnline);
			bool signatureMatch = signature.Md5Hash == expectedSignature.Md5Hash;

			if (signatureMatch)
			{
				move(appOnline, appDownloaded, overwrite: true);
				move(_appOnlineSignatureFile, _appDownloadedSignatureFile, overwrite: true);

				Console.WriteLine("Downloading complete.");

				AppDownloadedSignature = getAppDownloadedSignature();
				return true;
			}

			Console.WriteLine("The downloaded file {0} is corrupted.", expectedSignature.Path);
			return false;
		}

		private static void move(string appOnline, string appDownloaded, bool overwrite)
		{
			if (File.Exists(appDownloaded) && overwrite)
				File.Delete(appDownloaded);

			File.Move(appOnline, appDownloaded);
		}

		private static void ensureFileDeleted(string file)
		{
			if (!File.Exists(file))
				return;

			File.Delete(file);
		}

		public void Install(Action onComplete)
		{
			var expectedSignature = AppOnlineSignature;
			var appDownloaded = AppDir.Update.AddPath(expectedSignature.Path);

			BeginInstall?.Invoke();

			if (new SevenZip(silent: false).Extract(appDownloaded, AppDir.Root, _protectedFiles))
			{
				EndInstall?.Invoke();
				writeInstalledVersion(expectedSignature.Path);
				CreateApplicationShortcut(AppDir.Root);
				updateApplicationShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
				Console.WriteLine();

				onComplete?.Invoke();

				Console.WriteLine("Installed version will run next time you start Mtgdb.Gui");
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine($"Failed to extract new version files from {appDownloaded}.");
				Console.WriteLine("It should never happen, but here we are :(");
				Console.WriteLine();
				Console.WriteLine("However you should be able to run the current version normally.");
				Console.WriteLine("If you can't:");
				Console.WriteLine("\t* Re-download and manually install Mtgdb.Gui");
				Console.WriteLine("\t* Report the problem and get help at https://www.slightlymagic.net/forum/viewtopic.php?f=15&t=19298&sid=02dfce1282b368b1b8f40d452ac0af18");
				Console.WriteLine();
			}
		}

		private FileSignature getAppOnlineSignature()
		{
			if (!File.Exists(_appOnlineSignatureFile))
			{
				Console.WriteLine("Failed to download current version signature");
				return null;
			}

			var fileList = Signer.ReadFromFile(_appOnlineSignatureFile);

			// may be empty if downloading signature failed
			var lastMetadata = fileList.LastOrDefault();
			return lastMetadata;
		}

		private FileSignature getAppDownloadedSignature()
		{
			if (!File.Exists(_appDownloadedSignatureFile))
				return null;

			var fileList = Signer.ReadFromFile(_appDownloadedSignatureFile);
			var lastMetadata = fileList[fileList.Length - 1];
			return lastMetadata;
		}

		private void writeInstalledVersion(string version)
		{
			File.WriteAllText(_appInstalledVersionFile, version);
		}

		private void updateApplicationShortcut(string shortcutLocation)
		{
			string shortcutPath = Path.Combine(shortcutLocation, ShortcutFileName);
			if (File.Exists(shortcutPath))
				CreateApplicationShortcut(shortcutLocation);
		}

		public void CreateApplicationShortcut(string shortcutLocation)
		{
			string shortcutPath = Path.Combine(shortcutLocation, ShortcutFileName);

			string appVersionInstalled = GetAppVersionInstalled();
			// Mtgdb.Gui.v1.3.5.10.zip
			var prefix = "Mtgdb.Gui.";
			var postfix = ".zip";
			var versionDir = appVersionInstalled.Substring(prefix.Length, appVersionInstalled.Length - prefix.Length - postfix.Length);

			string currentBin = AppDir.BinVersion.Parent().AddPath(versionDir);
			string execPath = currentBin.AddPath(ExecutableFileName);
			string iconPath = currentBin.AddPath("mtg64.ico");

			createApplicationShortcut(shortcutPath, execPath, iconPath);
		}

		private static string getVersionFromAssembly()
		{
			string version = Assembly.GetEntryAssembly().GetName().Version.ToString();
			return "Mtgdb.Gui.v" + version + ".zip";
		}

		private static void createApplicationShortcut(string shortcutPath, string exePath, string iconPath)
		{
			var wsh = new WshShell();
			var shortcut = wsh.CreateShortcut(shortcutPath) as IWshShortcut;

			string bin = Path.GetDirectoryName(exePath);

			if (shortcut != null)
			{
				shortcut.Arguments = "";
				shortcut.TargetPath = exePath;
				shortcut.WindowStyle = 1;

				shortcut.Description = "Application to search MTG cards and build decks";
				shortcut.WorkingDirectory = bin;

				if (iconPath != null)
					shortcut.IconLocation = iconPath;

				shortcut.Save();
				Console.WriteLine("Created shortcut {0}", shortcutPath);
			}
		}

		public event Action MtgjsonFileUpdated;

		public event Action BeginInstall;
		public event Action EndInstall;

		public const string ShortcutFileName = "Mtgdb.Gui.lnk";
		private const string ExecutableFileName = "Mtgdb.Gui.exe";

		private readonly HashSet<string> _protectedFiles;

		private readonly AppSourceConfig _appSourceConfig;
		private readonly MtgjsonSourceConfig _mtgjsonSourceConfig;

		private readonly string _updateAppDir;
		private readonly string _appOnlineSignatureFile;
		private readonly string _appDownloadedSignatureFile;
		private readonly string _appInstalledVersionFile;
		public FileSignature AppOnlineSignature { get; private set; }
		public FileSignature AppDownloadedSignature { get; private set; }
		private readonly WebClientBase _webClient;
	}
}
