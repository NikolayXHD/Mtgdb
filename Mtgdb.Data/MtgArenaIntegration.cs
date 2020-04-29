using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Mtgdb.Data
{
	public class MtgArenaIntegration
	{
		[UsedImplicitly] // by ninject
		public MtgArenaIntegration(MtgaIntegrationConfig config)
		{
			_cardLibraryFile = resolveCardLibraryPath();
			_logFile = resolveLogPath();

			FsPath resolveCardLibraryPath()
			{
				var path = config.CardLibraryFile.ExpandEnvironmentVariables();
				var directory = path.Parent();
				var filePattern = path.Basename().Replace("[guid]", "*");

				if (!directory.IsDirectory())
					return FsPath.None;

				var file = directory.EnumerateFiles(filePattern).FirstOrDefault();
				return file;
			}

			FsPath resolveLogPath()
			{
				var path = config.LogFile.ExpandEnvironmentVariables();
				if (!path.IsFile())
					return FsPath.None;

				return path;
			}
		}

		public IEnumerable<(string Set, string Number, int Count)> ImportCollection()
		{
			if (!MtgaInstallationFound)
				return null;

			string log = tryReadLog();
			if (log == null)
				return null;

			int inventoryJsonIndex = findInventoryJsonIndex();
			if (inventoryJsonIndex < 0)
				return null;

			var countByMtgaId = parseCountByMtgaId();
			if (countByMtgaId == null)
				return null;

			string cardLibrary = tryReadCardLibrary();
			if (cardLibrary == null)
				return null;

			int libraryJsonIndex = findCardLibraryJsonIndex();
			if (libraryJsonIndex < 0)
				return null;

			var cardByMtgaId = parseCardByMtgaId();
			if (cardByMtgaId == null)
				return null;

			return resolveCollectionCards();

			string tryReadLog()
			{
				try
				{
					var result = _logFile.ReadAllText();
					return result;
				}
				catch (Exception ex)
				{
					_log.Warn(ex);
					return null;
				}
			}

			int findInventoryJsonIndex()
			{
				string inventoryRequestSignature = "PlayerInventory.GetPlayerCards";
				var lastInventoryRequestIndex = log.LastIndexOf(inventoryRequestSignature, Str.Comparison);
				if (lastInventoryRequestIndex < 0)
					return -1;

				var openingBraceIndex = log.IndexOf("{", lastInventoryRequestIndex + inventoryRequestSignature.Length, Str.Comparison);
				return openingBraceIndex;
			}

			Dictionary<string, int> parseCountByMtgaId()
			{
				var serializer = new JsonSerializer();
				using var textReader = new StringReader(log.Substring(inventoryJsonIndex));
				using var jsonReader = new JsonTextReader(textReader);
				try
				{
					var parsed = serializer.Deserialize<Dictionary<string, int>>(jsonReader);
					return parsed;
				}
				catch (Exception ex)
				{
					_log.Warn(ex);
					return null;
				}
			}

			string tryReadCardLibrary()
			{
				try
				{
					var result = _cardLibraryFile.ReadAllText(Encoding.Default);
					return result;
				}
				catch (Exception ex)
				{
					_log.Warn(ex);
					return null;
				}
			}

			int findCardLibraryJsonIndex()
			{
				var openJsonPattern = new Regex(@"\[\s*\{\s*""[^""]+""\s*:");
				var match = openJsonPattern.Match(cardLibrary);
				if (!match.Success)
					return -1;

				return match.Index;
			}

			Dictionary<string, (string Set, string Number)> parseCardByMtgaId()
			{
				var serializer = new JsonSerializer();
				using var textReader = new StringReader(cardLibrary.Substring(libraryJsonIndex));
				using var jsonReader = new JsonTextReader(textReader);
				try
				{
					var parsed = serializer.Deserialize<JArray>(jsonReader);

					Dictionary<string, (string Set, string Number)> result = parsed.Children().ToDictionary(
						t => t.Value<string>("grpid"),
						t => (Set: t.Value<string>("set"), Number: t.Value<string>("CollectorNumber")));

					return result;
				}
				catch (Exception ex)
				{
					_log.Warn(ex);
					return null;
				}
			}

			IEnumerable<(string Set, string Number, int Count)> resolveCollectionCards()
			{
				foreach (var pair in countByMtgaId)
				{
					if (!cardByMtgaId.TryGetValue(pair.Key, out var card))
						continue;

					yield return (card.Set, card.Number, pair.Value);
				}
			}
		}

		public bool MtgaInstallationFound =>
			_cardLibraryFile != FsPath.None && _logFile != FsPath.None;

		private readonly FsPath _cardLibraryFile;
		private readonly FsPath _logFile;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
