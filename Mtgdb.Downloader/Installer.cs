using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace Mtgdb.Downloader
{
	public class Installer
	{
		private const string ExecutableFileName = "Mtgdb.Gui.exe";
		public const string ShortcutFileName = "Mtgdb.Gui.lnk";

		private static readonly HashSet<string> ProtectedFiles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
		{
			AppDir.GeneralConfigXml,
			AppDir.DisplayConfigXml
		};

		private readonly AppSourceConfig _appSourceConfig;
		private readonly MtgjsonSourceConfig _mtgjsonSourceConfig;
		
		private readonly string _updateAppDir;
		private readonly string _appOnlineSignatureFile;
		private readonly string _appDownloadedSignatureFile;
		private readonly string _appInstalledVersionFile;
		public FileSignature AppOnlineSignature { get; private set; }
		public FileSignature AppDownloadedSignature { get; private set; }
		private readonly WebClient _webClient;
		private readonly Megatools _megatools;

		public event Action MtgjsonFileUpdated;

		public Installer(AppSourceConfig appSourceConfig, MtgjsonSourceConfig mtgjsonSourceConfig)
		{
			_appSourceConfig = appSourceConfig;
			_mtgjsonSourceConfig = mtgjsonSourceConfig;
			_updateAppDir = Path.Combine(AppDir.Update, "app");
			
			_appOnlineSignatureFile = Path.Combine(_updateAppDir, Signer.SignaturesFile);
			_appDownloadedSignatureFile = Path.Combine(AppDir.Update, Signer.SignaturesFile);
			_appInstalledVersionFile = Path.Combine(AppDir.Update, "version.txt");

			AppDownloadedSignature = getAppDownloadedSignature();

			_webClient = new WebClient();
			_megatools = new Megatools();
		}

		public void DownloadMtgjson()
		{
			Console.WriteLine("GET {0}", _mtgjsonSourceConfig.Url);
			
			try
			{
				var responseStream = _webClient.OpenRead(_mtgjsonSourceConfig.Url);
				if (responseStream == null)
				{
					Console.WriteLine("Failed to send request to mtgjson.com: empty response");
					return;
				}

				using (responseStream)
				{
					var byteArray = responseStream.ReadAllBytes();
					Console.WriteLine("Downloading complete.");

					using (var stream = new MemoryStream(byteArray))
					{
						Console.WriteLine("Extracting to {0}", AppDir.Data);
						new FastZip().ExtractZip(stream, AppDir.Data, FastZip.Overwrite.Always, name => true, null, null, true, false);
					}

					MtgjsonFileUpdated?.Invoke();

					Console.WriteLine("Done. On next start new cards will be loaded and full-text index will be rebuilt.");
				}
			}
			catch (AggregateException ex)
			{
				Console.WriteLine("Failed to send request to mtgjson.com: {0}", string.Join(", ", ex.InnerExceptions.Select(_ => _.Message)));
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to send request to mtgjson.com: {0}", ex.Message);
			}
		}

		public string GetAppVersionInstalled()
		{
			if (!File.Exists(_appInstalledVersionFile))
				return null;

			var result = File.ReadAllText(_appInstalledVersionFile);
			return result;
		}

		public void DownloadAppSignature()
		{
			ensureFileDeleted(_appOnlineSignatureFile);
			_megatools.Download("current version signature", _appSourceConfig.FileListUrl, _updateAppDir);
			AppOnlineSignature = getAppOnlineSignature();
		}

		public void DownloadApp()
		{
			var expectedSignature = AppOnlineSignature;

			var appOnline = Path.Combine(_updateAppDir, expectedSignature.Path);
			ensureFileDeleted(appOnline);

			_megatools.Download(expectedSignature.Path, _appSourceConfig.ZipUrl, _updateAppDir);
		}

		public bool ValidateDownloadedApp()
		{
			if (AppOnlineSignature == null)
				throw new InvalidOperationException("Cannot validate downloaded app. Online signature must be downloaded first");

			var expectedSignature = AppOnlineSignature;
			var appOnline = Path.Combine(_updateAppDir, expectedSignature.Path);
			var appDownloaded = Path.Combine(AppDir.Update, expectedSignature.Path);

			if (!File.Exists(appOnline))
			{
				Console.WriteLine("Failed to download {0}", expectedSignature.Path);
				return false;
			}
			
			var signature = Signer.CreateSignature(appOnline);
			bool signatureMatch = signature.Md5Hash == expectedSignature.Md5Hash;

			if (signatureMatch)
			{
				File.Copy(_appOnlineSignatureFile, _appDownloadedSignatureFile, true);
				File.Copy(appOnline, appDownloaded, true);
				Console.WriteLine("Downloading complete.");

				AppDownloadedSignature = getAppDownloadedSignature();
				return true;
			}

			Console.WriteLine("The downloaded file {0} is corrupted.", expectedSignature.Path);
			return false;
		}

		private static void ensureFileDeleted(string file)
		{
			if (!File.Exists(file))
				return;

			Console.WriteLine("Deleting {0}", file);
			File.Delete(file);
		}

		public void Install()
		{
			var expectedSignature = AppOnlineSignature;
			var appDownloaded = Path.Combine(AppDir.Update, expectedSignature.Path);

			Console.WriteLine("Extracting {0} to {1}", appDownloaded, AppDir.Root);

			new FastZip().ExtractZip(
				appDownloaded,
				AppDir.Root,
				FastZip.Overwrite.Prompt,
				name => !ProtectedFiles.Contains(name),
				null,
				null,
				true);

			writeInstalledVersion(expectedSignature.Path);
		}

		private FileSignature getAppOnlineSignature()
		{
			if (!File.Exists(_appOnlineSignatureFile))
			{
				Console.WriteLine("Failed to download current version signature");
				return null;
			}

			var fileList = Signer.ReadFromFile(_appOnlineSignatureFile);
			var lastMetadata = fileList[fileList.Length - 1];
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

		public void UpdateApplicationShortcut(string shortcutLocation)
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

		private static void createApplicationShortcut(string shortcutPath, string exePath, string iconPath)
		{
			var wsh = new WshShell();
			var shortcut = wsh.CreateShortcut(shortcutPath) as IWshShortcut;

			string bin = Path.GetDirectoryName(exePath);

			if (shortcut != null)
			{
				shortcut.Arguments = "";
				shortcut.TargetPath = exePath;
				// not sure about what this is for
				shortcut.WindowStyle = 1;

				shortcut.Description = "Application to search MTG cards and build decks";
				shortcut.WorkingDirectory = bin;

				if (iconPath != null)
					shortcut.IconLocation = iconPath;

				shortcut.Save();
				Console.WriteLine("Created shortcut {0}", shortcutPath);
			}
		}
	}
}