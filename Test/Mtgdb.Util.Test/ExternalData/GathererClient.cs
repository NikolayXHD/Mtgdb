using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Mtgdb.Dal;
using Mtgdb.Downloader;
using Newtonsoft.Json;
using NLog;

namespace Mtgdb.Util
{
	public class GathererClient : WebClientBase
	{
		public GathererClient()
		{
		}

		public GathererClient(CardRepository repo)
		{
			_repo = repo;
		}

		public void DownloadTranslations(string targetDir)
		{
			var idsArray = getMultiverseIds();

			for (int i = 0; i < idsArray.Length; i++)
			{
				int id = idsArray[i];
				DownloadTranslation(targetDir, id);

				if (i % 1000 == 0)
					_log.Info($"downloaded {i} / {idsArray.Length}");
			}
		}

		public void DownloadTranslation(string targetDir, int id)
		{
			string targetFile = Path.Combine(targetDir, id + ".html");

			if (!File.Exists(targetFile))
			{
				var page = DownloadCardPage(id);
				File.WriteAllText(targetFile, page);
			}
		}

		public void ParseTranslations(string downloadedDir, string parsedDir)
		{
			var files = Directory.GetFiles(downloadedDir, "*.html");
			var failedIds = new List<int>();

			for (int i = 0; i < files.Length; i++)
			{
				var file = files[i];
				int id = int.Parse(Path.GetFileNameWithoutExtension(file));

				if (id == 0)
					continue;

				var resultFile = Path.Combine(parsedDir, id + ".json");
				if (File.Exists(resultFile))
					continue;

				string targetFile = GetSavedTranslationHtmlFile(downloadedDir, id);
				var content = File.ReadAllText(targetFile);
				var translations = ParseCardPage(content);

				var result = new List<Translation>();

				foreach (var translation in translations)
				{
					if (translation == null)
					{
						failedIds.Add(id);
						break;
					}

					translation.Id = id;
					result.Add(translation);
				}

				if (result.Count > 0)
				{
					var serialized = JsonConvert.SerializeObject(result);
					File.WriteAllText(resultFile, serialized);
				}

				if (i % 1000 == 0)
					_log.Info($"parsed {i} / {files.Length}");
			}

			File.WriteAllLines(getFailedListFile(parsedDir), failedIds.Select(id => id.ToString()));
		}

		public void MergeTranslations(string parsedDir, string resultDir)
		{
			var files = Directory.GetFiles(parsedDir, "*.json", SearchOption.TopDirectoryOnly);
			var result = new List<Translation>();

			foreach (var file in files)
			{
				var list = JsonConvert.DeserializeObject<List<Translation>>(File.ReadAllText(file));
				result.AddRange(list);
			}

			var serialized = JsonConvert.SerializeObject(result);
			File.WriteAllText(Path.Combine(resultDir, "translations.json"), serialized);
		}

		public void SaveNonEnglishTranslations(string resultDir)
		{
			var engMultiverseIds = new HashSet<int>(_repo.Cards.Select(card => card.MultiverseId)
				.Where(_ => _.HasValue)
				.Cast<int>());

			var serialized = File.ReadAllText(Path.Combine(resultDir, "translations.json"));
			var translations = JsonConvert.DeserializeObject<List<Translation>>(serialized);

			var nonEnglishTranslations = translations.Where(_ => !engMultiverseIds.Contains(_.Id)).ToList();
			serialized = JsonConvert.SerializeObject(nonEnglishTranslations);

			File.WriteAllText(Path.Combine(resultDir, "translations-non-en.json"), serialized);
		}

		public string GetSavedTranslationHtmlFile(string targetDir, int id)
		{
			return Path.Combine(targetDir, id + ".html");
		}

		public int[] GetFailedIds(string resultDir)
		{
			var file = getFailedListFile(resultDir);
			var result = File.ReadAllLines(file).Select(int.Parse).ToArray();
			return result;
		}

		private static string getFailedListFile(string resultDir)
		{
			return Path.Combine(Path.GetDirectoryName(resultDir), "failed");
		}

		private int[] getMultiverseIds()
		{
			var ids = new HashSet<int>();

			foreach (var card in _repo.Cards)
			{
				if (card.MultiverseId.HasValue)
					ids.Add(card.MultiverseId.Value);

				if (card.ForeignNames != null)
					foreach (var fName in card.ForeignNames)
						ids.Add(fName.MultiverseId);
			}

			var idsArray = ids.ToArray();
			return idsArray;
		}

		public void DownloadCardImage(int multiverseId, string targetFile) =>
			DownloadFile(BaseUrl + ImagePath + multiverseId, targetFile);

		public string DownloadCardPage(int multiverseId)
		{
			string pageText = DownloadString(BaseUrl + TranslationPath + multiverseId);
			return pageText;
		}

		public IEnumerable<Translation> ParseCardPage(string pageText)
		{
			var startIndex = pageText.IndexOf(_translationBegin[0], 0, Str.Comparison);

			if (startIndex >= 0)
			{
				var endIndex = pageText.IndexOf(TranslationEnd, startIndex, Str.Comparison);

				var content = pageText.Substring(startIndex, endIndex - startIndex + TranslationEnd.Length);
				var result = parse(content);

				yield return result;
				yield break;
			}

			for (int i = 1; i < _translationBegin.Length - 1; i++)
			{
				startIndex = pageText.IndexOf(_translationBegin[i], 0, Str.Comparison);

				if (startIndex < 0)
					continue;

				var endIndex = pageText.IndexOf(TranslationEnd, startIndex, Str.Comparison);

				var content = pageText.Substring(startIndex, endIndex - startIndex + TranslationEnd.Length);
				var result = parse(content);

				yield return result;

				startIndex = pageText.IndexOf(_translationBegin[i + 1], endIndex, Str.Comparison);

				endIndex = pageText.IndexOf(TranslationEnd, startIndex, Str.Comparison);
				content = pageText.Substring(startIndex, endIndex - startIndex + TranslationEnd.Length);
				result = parse(content);

				yield return result;
				yield break;
			}

			yield return null;
		}

		private static Translation parse(string content)
		{
			content = _hrefFixRegex.Replace(content, "href=\"${url}\"");
			content = content.Replace(GamerFlavor, _gamerFlavorEncoded);
			content = _ampFixRegex.Replace(content, "&amp;");
			content = content.Replace("\"class=\"", "\" class=\"");
			// ReSharper disable StringLiteralTypo
			content = content.Replace("\"><< Les elfes ont raison", "\">« Les elfes ont raison");
			// ReSharper restore StringLiteralTypo
			content = content.Replace("<i>", string.Empty).Replace("</i>", string.Empty);

			var tdElement = XElement.Parse(content);
			var fieldDivs = tdElement.Elements("div").Elements("div").ToArray();
			string type = getFieldValue(fieldDivs, "Types:", multiline: false);
			string text = getFieldValue(fieldDivs, "Card Text:", multiline: true);
			string flavor = getFieldValue(fieldDivs, "Flavor Text:", multiline: true);
			string cardNumber = getFieldValue(fieldDivs, "Card Number:", multiline: false);

			var result = new Translation
			{
				Type = type,
				Text = text,
				Flavor = flavor,
				CardNumber = cardNumber
			};

			return result;
		}

		private static string getFieldValue(XElement[] fieldDivs, string fieldName, bool multiline)
		{
			var valueEl = fieldDivs
				.FirstOrDefault(fDiv => Str.Equals(fDiv.Element("div")?.Value.Trim(), fieldName))
				?.Elements("div")
				.Skip(1)
				.First();

			if (valueEl == null)
				return null;

			if (!multiline)
				return valueEl.Value.Trim();

			var lineDivs = valueEl.Elements("div");
			var resultBuilder = new StringBuilder();

			foreach (var lineDiv in lineDivs)
			{
				var lineElements = lineDiv.Nodes().ToArray();
				foreach (var lineNode in lineElements)
				{
					if (lineNode.NodeType == XmlNodeType.Text)
						resultBuilder.Append(lineNode);
					else if (lineNode.NodeType == XmlNodeType.Whitespace | lineNode.NodeType == XmlNodeType.SignificantWhitespace)
						resultBuilder.Append(' ');
					else if (lineNode.NodeType == XmlNodeType.Element)
					{
						var lineElement = (XElement)lineNode;

						if (Str.Equals(lineElement.Name.ToString(), "img"))
							resultBuilder.Append(getSymbol(lineElement));
						else if (Str.Equals(lineElement.Name.ToString(), "i"))
						{
							foreach (var iNode in lineElement.Nodes())
							{
								if (iNode.NodeType == XmlNodeType.Text)
									resultBuilder.Append(iNode);
								else if (iNode.NodeType == XmlNodeType.Whitespace | lineNode.NodeType == XmlNodeType.SignificantWhitespace)
									resultBuilder.Append(' ');
								else if (iNode.NodeType == XmlNodeType.Element)
								{
									var iElement = (XElement)iNode;
									if (Str.Equals(iElement.Name.ToString(), "img"))
										resultBuilder.Append(getSymbol(iElement));
								}
							}
						}
					}
				}

				resultBuilder.AppendLine();
			}

			return resultBuilder.ToString().Trim();
		}

		private static string getSymbol(XElement imgElement)
		{
			var srcAttr = imgElement.Attribute("src");
			string src = srcAttr.Value;
			var match = _symbolRegex.Match(src);
			var value = match.Groups["name"].Value;
			if (Str.Equals(value, "tap"))
				value = "T";
			else if (Str.Equals(value, "untap"))
				value = "Q";

			return $"{{{value}}}";
		}

		private const string BaseUrl = "http://gatherer.wizards.com/";
		private const string ImagePath = "Handlers/Image.ashx?type=card&multiverseid=";

		private const string TranslationPath = "Pages/Card/Details.aspx?printed=true&multiverseid=";

		private static readonly string[] _translationBegin =
		{
			"<td id=\"ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_rightCol\"",
			"<td id=\"ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_ctl01_rightCol\"",
			"<td id=\"ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_ctl02_rightCol\"",
			"<td id=\"ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_ctl03_rightCol\"",
			"<td id=\"ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_ctl04_rightCol\""
		};

		private static readonly Regex _symbolRegex = new Regex(@"&name=(?<name>[\w\d]+)&type=symbol",
			RegexOptions.IgnoreCase);

		private static readonly Regex _hrefFixRegex = new Regex(@"\bhref='(?<url>[^']+)'");
		private static readonly Regex _ampFixRegex = new Regex(@"&(?!amp;)");

		private const string TranslationEnd = "</td>";

		private readonly CardRepository _repo;
		private const string GamerFlavor = "1|= y()u (4|\\| r3@d 7#5, y0|_| /\\r3 @ IVI0/\\/$+3|2 &33|<";
		private static readonly string _gamerFlavorEncoded = System.Web.HttpUtility.HtmlEncode(GamerFlavor);

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}