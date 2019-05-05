using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

			var htmlTemplate = File.ReadAllText(Path.Combine(AppDir.Root, "help", "template.html"));

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
				AppDir.Root.AddPath("..\\..\\mtgdb.wiki"), "*.rest", SearchOption.TopDirectoryOnly);

			return helpFiles.OrderByDescending(f => Str.Equals("home", Path.GetFileNameWithoutExtension(f)))
				.Where(f => !_obsoletePages.Contains(getPageTitle(f)))
				.ToArray();
		}

		private static string getPageName(string helpFile)
		{
			return Path.GetFileNameWithoutExtension(helpFile);
		}

		private static string getHtmlPage(string helpFileName, string htmlTemplate, IList<string> helpFileNames)
		{
			string completePageContent = getHelpPage(helpFileName);
			string helpContent = getMainSectionContent(completePageContent);

			helpContent = trimSeoSectionFrom(helpContent);

			foreach (string imgDirUrl in _imgDirUrls)
				helpContent = helpContent
					.Replace(imgDirUrl, string.Empty);

			helpContent = helpContent
				.Replace("id=\"user-content-", "id=\"");

			var hrefRegex = new Regex(
				"(?<prefix>href=\"[^\"]*)(?<name>" +
				string.Join("|", helpFileNames.Select(f => Regex.Escape(HttpUtility.HtmlEncode(Path.GetFileNameWithoutExtension(f))))) +
				")(?<postfix>[^\"]*\")",
				RegexOptions.IgnoreCase);

			helpContent = hrefRegex.Replace(helpContent, "${prefix}${name}.html${postfix}");

			var navigationItems = getNavigationItems(helpFileName, completePageContent, out string selectedItemName);
			string title = selectedItemName ?? getPageTitle(helpFileName);

			var htmlPage = htmlTemplate
				.Replace("<!--Header-->", $"Mtgdb.Gui help - {title}")
				.Replace("<!--Navigation-->", navigationItems)
				.Replace("<!--Title-->", title)
				.Replace("<!--Content-->", helpContent);

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

		private static string getHelpPage(string helpFileName)
		{
			var client = new WebClientBase();
			string name = Path.GetFileNameWithoutExtension(helpFileName);
			var pageContent = client.DownloadString("https://github.com/NikolayXHD/Mtgdb/wiki/" + name);
			return pageContent;
		}

		private static string getMainSectionContent(string pageContent)
		{
			var startIndex = pageContent.IndexOf(HelpContentOpenElement, Str.Comparison);
			return getTagContent(pageContent, startIndex);
		}

		private static string getNavigationItems(string mdFile, string pageContent, out string selectedItemName)
		{
			int sidebarPosition = findSidebarDiv(pageContent);
			string sidebarDiv = getTagContent(pageContent, sidebarPosition);
			string sidebarContent = getNestedContent(sidebarDiv);

			string pageName = Path.GetFileNameWithoutExtension(mdFile);
			string selectedAnchorName = null;

			var anchorRegex = new Regex("(?:<a href=\"(?!https?://))(?:wiki/)?(?<url>[^\"]+)\"");
			var preProcessedAnchors = anchorRegex.Replace(sidebarContent, match =>
			{
				string fileName = HttpUtility.HtmlDecode(match.Groups["url"].Value);
				if (fileName.Equals(pageName, Str.Comparison))
				{
					string anchor = getTagContent(sidebarContent, match.Index, "a");
					selectedAnchorName = getNestedContent(anchor, "a");
					return $"<a class=\"selected\" href=\"{match.Groups["url"]}.html\"";
				}

				return $"<a href=\"{match.Groups["url"]}.html\"";
			});

			selectedItemName = selectedAnchorName;
			return preProcessedAnchors;
		}

		private static string getTagContent(string pageContent, int startIndex, string tagName = "div")
		{
			var contentPrefix = pageContent.Substring(startIndex);

			var openers = getOpenTagRegex(tagName).Matches(contentPrefix);
			var closers = getCloseTagRegex(tagName).Matches(contentPrefix);

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

		private static string getNestedContent(string tagContent, string tagName = "div")
		{
			var openers = getOpenTagRegex(tagName).Matches(tagContent);
			var closers = getCloseTagRegex(tagName).Matches(tagContent);

			if (openers.Count == 0)
				throw new FormatException();

			if (closers.Count == 0)
				throw new FormatException();

			int start = openers[0].Length;
			int end = closers[closers.Count - 1].Index;

			string result = tagContent.Substring(start, end - start);
			return result;
		}

		private static Regex getOpenTagRegex(string tagName) =>
			new Regex($"<{tagName}(?:\\s[^>]*)?>", RegexOptions.IgnoreCase);

		private static Regex getCloseTagRegex(string tagName) =>
			new Regex($"</{tagName}>", RegexOptions.IgnoreCase);

		private static int findSidebarDiv(string pageContent)
		{
			int classIndex = pageContent.IndexOf("wiki-custom-sidebar", Str.Comparison);

			if (classIndex < 0)
				throw new FormatException();

			string prefix = pageContent.Substring(0, classIndex);

			int containerIndex = prefix.LastIndexOf("<div ", Str.Comparison);

			if (containerIndex < 0)
				throw new FormatException();

			return containerIndex;
		}

		private static string getPageTitle(string helpFile)
		{
			string title = getPageName(helpFile)
				.Replace('-', ' ');

			title = title.Substring(0, 1).ToUpperInvariant() + title.Substring(1);
			return title;
		}

		private static readonly Regex _htmlHeaderTagRegex = new Regex("<h[1-6]>",
			RegexOptions.IgnoreCase);

		private const string HelpContentOpenElement = "<div class=\"markdown-body\">";
		private const string HelpContentCloseElement = "</div>";

		private static readonly string[] _imgDirUrls =
		{
			"wiki/",
			"../raw/master/out/help/"
		};

		private static readonly HashSet<string> _obsoletePages = new HashSet<string>(Str.Comparer)
		{
			"_Sidebar",
			"2.1 Drag n drop or paste from Clipboard to import decks from websites",
			"4. Search text"
		};
	}
}
