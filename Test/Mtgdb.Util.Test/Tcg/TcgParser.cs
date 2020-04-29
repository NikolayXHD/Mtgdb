using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Mtgdb.Dev;
using Newtonsoft.Json;

namespace Mtgdb.Util
{
	public class TcgParser
	{
		private readonly FsPath _resourcesDir = DevPaths.MtgContentDir.Join("tcg");

		public Dictionary<string, Dictionary<string, int>> GetOrderByCard()
		{
			FsPath sortFile = _resourcesDir.Join("tcg.sort.json");

			var json = sortFile.ReadAllText();
			var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(json);
			return result;
		}

		public Dictionary<string, string> GetTcgSetBySet()
		{
			FsPath setMapFile = _resourcesDir.Join("tcg.sets.map.txt");

			var tcgSetBySet = setMapFile.ReadAllLines()
				.Where(l => l != string.Empty)
				.Select(l => l.Split('\t'))
				.ToDictionary(p => p[0], p => p[1]);

			return tcgSetBySet;
		}

		public List<TcgSet> ParseSets()
		{
			FsPath setListFile = _resourcesDir.Join("tcg.sets.xml");

			var text = setListFile.ReadAllText();
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
			FsPath cardsFile = _resourcesDir.Join("tcg.json");
			var json = cardsFile.ReadAllText();
			var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, TcgCard>>>(json);
			return result;
		}

		public Dictionary<string, Dictionary<string, TcgCard>> ParseCards()
		{
			var result = new Dictionary<string, Dictionary<string, TcgCard>>();

			FsPath dir = _resourcesDir.Join("pages");

			var files = dir.EnumerateFiles("*.html");
			foreach (var file in files)
			{
				var content = file.ReadAllText();
				string setCode = file.Basename(extension: false);

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
				price = float.Parse(priceRaw.Substring(1), Str.Culture);
			return price;
		}
	}
}
