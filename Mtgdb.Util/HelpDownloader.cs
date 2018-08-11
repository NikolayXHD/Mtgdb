using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Mtgdb.Downloader;

namespace Mtgdb.Util
{
	public static class HelpDownloader
	{
		public static void UpdateLocalHelp()
		{
			var helpFileNames = getHelpFileNames();

			var htmlTemplate = File.ReadAllText(AppDir.Root.AddPath("help\\template.html"));

			foreach (string helpFileName in helpFileNames)
			{
				var htmlFile = AppDir.Root.AddPath($"help\\{getPageName(helpFileName)}.html");
				string htmlPage = getHtmlPage(helpFileName, htmlTemplate, helpFileNames);
				File.WriteAllText(htmlFile, htmlPage);
			}
		}

		private static string[] getHelpFileNames()
		{
			var helpFiles = Directory.GetFiles(
				AppDir.Root.AddPath("..\\..\\Mtgdb.wiki"), "*.rest", SearchOption.TopDirectoryOnly);

			return helpFiles.OrderByDescending(f => Str.Equals("home", Path.GetFileNameWithoutExtension(f)))
				.ToArray();
		}

		private static string getPageName(string helpFile)
		{
			return Path.GetFileNameWithoutExtension(helpFile);
		}

		private static string getHtmlPage(string helpFileName, string htmlTemplate, IList<string> helpFileNames)
		{
			string htmlContent = getOnlineHelpContent(helpFileName);
			htmlContent = trimSeoSectionFrom(htmlContent);

			foreach (string imgDirUrl in _imgDirUrls)
				htmlContent = htmlContent
					.Replace(imgDirUrl, string.Empty);

			htmlContent = htmlContent.Replace("id=\"user-content-", "id=\"");

			var hrefRegex = new Regex(
				"(?<prefix>href=\"[^\"]*)(?<name>" +
				string.Join("|", helpFileNames.Select(f => Regex.Escape(HttpUtility.HtmlEncode(Path.GetFileNameWithoutExtension(f))))) +
				")(?<postfix>[^\"]*\")",
				RegexOptions.Compiled | RegexOptions.IgnoreCase);

			htmlContent = hrefRegex.Replace(htmlContent, "${prefix}${name}.html${postfix}");
			
			string title = getPageTitle(helpFileName);

			var navigationItems = getNavigationItems(helpFileName, helpFileNames);

			var htmlPage = htmlTemplate
				.Replace("<!--Header-->", $"Mtgdb.Gui help - {title}")
				.Replace("<!--Navigation-->", navigationItems)
				.Replace("<!--Title-->", title)
				.Replace("<!--Content-->", htmlContent);
			return htmlPage;
		}

		private static string trimSeoSectionFrom(string htmlContent)
		{
			if (!htmlContent.StartsWith(HelpContentOpenElement, Str.Comparison))
				throw new FormatException("Unexpected html content start");

			if (!htmlContent.EndsWith(HelpContentCloseElement, Str.Comparison))
				throw new FormatException("Unexpected html content end");

			var seoStartIndex = htmlContent.IndexOf("seo phrases", Str.Comparison);

			if (seoStartIndex < 0)
				return htmlContent;

			var headerMatches = _htmlHeaderTagRegex.Matches(htmlContent);
			var lastHeaderMatch = headerMatches.OfType<Match>().LastOrDefault(_ => _.Index < seoStartIndex);

			if (lastHeaderMatch == null)
				return htmlContent;

			string result = htmlContent.Substring(0, lastHeaderMatch.Index) + HelpContentCloseElement;
			return result;
		}

		private static string getOnlineHelpContent(string helpFileName)
		{
			var client = new WebClientBase();
			string name = Path.GetFileNameWithoutExtension(helpFileName);

			var pageContent = client.DownloadString("https://github.com/NikolayXHD/Mtgdb/wiki/" + name);

			var startIndex = pageContent.IndexOf(HelpContentOpenElement, Str.Comparison);
			var contentPrefix = pageContent.Substring(startIndex);

			var openers = new Regex("<div").Matches(contentPrefix);
			var closers = new Regex("</div>").Matches(contentPrefix);

			var tags = openers.Cast<Match>()
				.Select(m => new { index = m.Index, length = m.Length, open = true })
				.Concat(closers.Cast<Match>().Select(m => new { index = m.Index, length = m.Length, open = false }))
				.OrderBy(_ => _.index)
				.ToArray();

			int c = 0;
			for (int i = 0; i < tags.Length; i++)
			{
				if (tags[i].open)
					c++;
				else
					c--;

				if (c == 0)
					return contentPrefix.Substring(0, tags[i].index + tags[i].length);
			}

			throw new FormatException();
		}

		private static string getNavigationItems(string mdFile, IList<string> mdFiles)
		{
			var menuBuilder = new StringBuilder();
			foreach (string file in mdFiles)
			{
				string pageTitle = getPageTitle(file);
				if (ObsoletePages.Contains(pageTitle))
					continue;

				string pageName = getPageName(file);

				string aClass = Str.Equals(mdFile, file) ? "selected" : "";
				menuBuilder.AppendFormat("<li class=\"{0}\"><a href=\"{1}.html\">{2}</a></li>",
					aClass,
					pageName,
					pageTitle);
			}

			return menuBuilder.ToString();
		}

		private static string getPageTitle(string helpFile)
		{
			string title = getPageName(helpFile)
				.Replace('-', ' ');

			title = title.Substring(0, 1).ToUpperInvariant() + title.Substring(1);
			return title;
		}

		private static readonly Regex _htmlHeaderTagRegex = new Regex("<h[1-6]>",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private const string HelpContentOpenElement = "<div class=\"markdown-body\">";
		private const string HelpContentCloseElement = "</div>";

		private static readonly string[] _imgDirUrls =
		{
			"wiki/",
			"../raw/master/out/help/"
		};

		private static readonly HashSet<string> ObsoletePages = new HashSet<string>(Str.Comparer)
		{
			"_Sidebar",
			"2.1 Drag n drop or paste from Clipboard to import decks from websites",
			"4. Search text"
		};
	}
}
