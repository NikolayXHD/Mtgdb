using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Mtgdb.Data;
using Mtgdb.Downloader;

namespace Mtgdb.Util
{
	public class MagicspoilerClient : WebClientBase
	{
		public void DownloadSet(Set set, string targetDirectory)
		{
			var setDirectory = Path.Combine(targetDirectory, set.Code);
			Directory.CreateDirectory(setDirectory);

			if (!_siteSetCodes.TryGetValue(set.Code, out var siteSet))
				return;

			for (int i = 0; i < siteSet.Item2; i++)
			{
				string url = BaseUrl + siteSet.Item1;
				if (i > 0)
					url += "/page/" + (i + 1);

				string page = DownloadString(url);

				string beginCard = "<div class=\"spoiler-set-card\"";
				string endCards = "<div class=\"clear\"";

				int c = 0;

				while (true)
				{
					int start = page.IndexOf(beginCard, c, Str.Comparison);

					if (start < 0)
						break;

					c = page.IndexOf(beginCard, start + beginCard.Length, Str.Comparison);
					if (c < 0)
						c = page.IndexOf(endCards, start + beginCard.Length, Str.Comparison);

					if (c < 0)
						break;

					string cardElementText = page.Substring(start, c - start)
						.Replace("&laquo;", string.Empty)
						.Replace("&raquo;", string.Empty);

					var cardElem = XElement.Parse(cardElementText);

					var anchor = cardElem.Element("a");
					var title = anchor.Attribute("title").Value;
					var thumbSrc = anchor.Element("img").Attribute("src").Value;
					var nonCropSrc = _cropRegex.Replace(thumbSrc, string.Empty);
					var target = setDirectory.AddPath(nonCropSrc.Split('/').Last());

					if (File.Exists(target))
						continue;

					DownloadFile(nonCropSrc, target);
				}
			}
		}

		private static readonly Regex _cropRegex = new Regex(@"-\d+x\d+(?=\.(?:png|jpg)$)",
			 RegexOptions.IgnoreCase);

		private const string BaseUrl = "http://www.magicspoiler.com/mtg-set/";

		private static readonly Dictionary<string, Tuple<string, int>> _siteSetCodes = new Dictionary<string, Tuple<string, int>>
		{
			["XLN"] = new Tuple<string, int>("ixalan", 8),
			["C17"] = new Tuple<string, int>("commander-2017", 3),
			["IMA"] = new Tuple<string, int>("iconic-masters", 7),
			["DDT"] = new Tuple<string, int>("duel-decks-merfolk-vs-goblins", 7)
		};
	}
}
