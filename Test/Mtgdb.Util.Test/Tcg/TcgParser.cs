using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Mtgdb.Test
{
	public class TcgParser
	{
		private readonly string _resourcesDir = Path.Combine(
			TestContext.CurrentContext.TestDirectory,
			@"D:\Distrib\games\mtg\tcg");

		public Dictionary<string, Dictionary<string, int>> GetOrderByCard()
		{
			string sortFile = _resourcesDir.AddPath("tcg.sort.json");

			var json = File.ReadAllText(sortFile);
			var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(json);
			return result;
		}

		public Dictionary<string, string> GetTcgSetBySet()
		{
			string setMapFile = _resourcesDir.AddPath("tcg.sets.map.txt");

			var tcgSetBySet = File.ReadAllLines(setMapFile)
				.Where(l => l != string.Empty)
				.Select(l => l.Split('\t'))
				.ToDictionary(p => p[0], p => p[1]);

			return tcgSetBySet;
		}

		public List<TcgSet> ParseSets()
		{
			string setListFile = _resourcesDir.AddPath("tcg.sets.xml");

			var text = File.ReadAllText(setListFile);
			var root = XElement.Parse(text);
			var options = root.Elements("option").ToArray();

			var result = options
				.Select(option => new TcgSet
				{
					Name = option.Value,
					Code = option.Attribute("value").Value
				})
				.ToList();

			return result;
		}

		public Dictionary<string, Dictionary<string, TcgCard>> GetTcgCardsByTcgSet()
		{
			string cardsFile = _resourcesDir.AddPath("tcg.json");
			var json = File.ReadAllText(cardsFile);
			var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, TcgCard>>>(json);
			return result;
		}

		public Dictionary<string, Dictionary<string, TcgCard>> ParseCards()
		{
			var result = new Dictionary<string, Dictionary<string, TcgCard>>();

			string dir = _resourcesDir.AddPath("pages");

			var files = Directory.GetFiles(dir, "*.html", SearchOption.TopDirectoryOnly);
			foreach (var file in files)
			{
				var content = File.ReadAllText(file);
				var setCode = Path.GetFileNameWithoutExtension(file);

				var tableBeginPosition = content.IndexOf("<table class=\"priceGuideTable", 0, Str.Comparison);
				string closingTag = "</table>";
				var tableEndPosition = content.IndexOf(closingTag, tableBeginPosition, Str.Comparison);
				var tableElement = XElement.Parse(content
					.Substring(tableBeginPosition, tableEndPosition + closingTag.Length - tableBeginPosition)
					.Replace("&mdash;", string.Empty));

				var trs = tableElement.Element("tbody").Elements("tr").ToArray();

				var cards = new Dictionary<string, TcgCard>();
				result.Add(setCode, cards);

				foreach (var tr in trs)
				{
					var tds = tr.Elements("td").ToArray();

					var cellWrapperDivs = tds[0].Element("div").Elements("div").ToArray();

					var thumbnailUrl = cellWrapperDivs[0].Element("a").Element("img").Attribute("data-original").Value;
					var sidBegin = thumbnailUrl.LastIndexOf("/", Str.Comparison);
					var sidEnd = thumbnailUrl.IndexOf("_", sidBegin, Str.Comparison);

					var a = cellWrapperDivs[1].Element("a");
					var url = a.Attribute("href").Value;
					var codeBegin = url.LastIndexOf("/", Str.Comparison);

					cards.Add(url.Substring(codeBegin + 1),
						new TcgCard
						{
							Id = thumbnailUrl.Substring(sidBegin + 1, sidEnd - sidBegin - 1),
							Name = a.Value,
							Number = tds[2].Element("div").Value.Trim().NullIfEmpty(),
							MarketPrice = parsePrice(tds[3].Element("div")),
							BuylistMarketPrice = parsePrice(tds[5].Element("div")),
							ListedMedianPrice = parsePrice(tds[6].Element("div")),
							Rarity = tds[1].Element("div").Value.Trim()
						});
				}
			}

			return result;
		}

		private static float? parsePrice(XElement priceElement)
		{
			var priceRaw = priceElement.Value.Trim();

			float? price = null;

			if (priceRaw.StartsWith("$"))
				price = float.Parse(priceRaw.Substring(1), CultureInfo.InvariantCulture);
			return price;
		}
	}
}
