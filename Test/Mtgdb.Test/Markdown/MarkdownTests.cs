using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Mtgdb.Downloader;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class MarkdownTests
	{
		[Test]
		public void Render_local_help()
		{
			var helpFiles = getHelpFiles();

			var htmlTemplate = File.ReadAllText(AppDir.Root.AddPath("help\\template.html"));

			foreach (string helpFile in helpFiles)
			{
				var htmlFile = AppDir.Root.AddPath($"help\\{getPageName(helpFile)}.html");
				string htmlPage = getHtmlPage(helpFile, htmlTemplate, helpFiles);
				File.WriteAllText(htmlFile, htmlPage);
			}
		}

		private static string[] getHelpFiles()
		{
			var helpFiles = Directory.GetFiles(
				AppDir.Root.AddPath("..\\..\\Mtgdb.wiki"), "*.rest", SearchOption.TopDirectoryOnly);

			return helpFiles.OrderByDescending(f => Str.Equals("home.rest", Path.GetFileName(f)))
				.ToArray();
		}

		private static string getPageName(string helpFile)
		{
			return Path.GetFileNameWithoutExtension(helpFile);
		}

		private static string getHtmlPage(string mdFile, string htmlTemplate, IList<string> mdFiles)
		{
			string htmlContent = getContent(mdFile);

			foreach (string imgDirUrl in _imgDirUrls)
				htmlContent = htmlContent
					.Replace(imgDirUrl, string.Empty);

			htmlContent = htmlContent.Replace("id=\"user-content-", "id=\"");

			var hrefRegex = new Regex("(?<prefix>href=\"[^\"]*)(?<name>" + 
				string.Join("|", mdFiles.Select(f => Regex.Escape(HttpUtility.HtmlEncode(Path.GetFileNameWithoutExtension(f))))) 
				+ ")(?<postfix>[^\"]*\")",
				RegexOptions.Compiled | RegexOptions.IgnoreCase);

			htmlContent = hrefRegex.Replace(htmlContent, "${prefix}${name}.html${postfix}");
			
			string title = getPageTitle(mdFile);

			var navigationItems = getNavigationItems(mdFile, mdFiles);

			var htmlPage = htmlTemplate
				.Replace("<!--Header-->", $"Mtgdb.Gui help - {title}")
				.Replace("<!--Navigation-->", navigationItems)
				.Replace("<!--Title-->", title)
				.Replace("<!--Content-->", htmlContent);
			return htmlPage;
		}

		private static string getContent(string mdFile)
		{
			var client = new WebClientBase();
			string name = Path.GetFileNameWithoutExtension(mdFile);

			var pageContent = client.DownloadString("https://github.com/NikolayXHD/Mtgdb/wiki/" + name);

			var startIndex = pageContent.IndexOf("<div class=\"markdown-body\">", Str.Comparison);
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
				string pageName = getPageName(file);
				string pageTitle = getPageTitle(file);

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

		private static readonly string[] _imgDirUrls =
		{
			"wiki/",
			"../raw/master/out/help/"
		};
	}
}
