using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Markdig;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class MarkdownTests
	{
		[Test]
		public void Renders_html()
		{
			var helpFiles = Directory.GetFiles(
				AppDir.Root.AddPath("help"),
				"*.txt",
				SearchOption.TopDirectoryOnly);

			var htmlTemplate = File.ReadAllText(AppDir.Root.AddPath("help\\html\\template.html"));

			foreach (string helpFile in helpFiles)
			{
				var htmlFile = AppDir.Root.AddPath($"help\\html\\{getPageName(helpFile)}.html");
				string htmlPage = getHtmlPage(helpFile, htmlTemplate, helpFiles);
				File.WriteAllText(htmlFile, htmlPage);
			}
		}

		private static string getPageName(string helpFile)
		{
			return Path.GetFileNameWithoutExtension(helpFile).Substring(3);
		}

		private static string getHtmlPage(string mdFile, string htmlTemplate, IList<string> mdFiles)
		{
			var readmeText = File.ReadAllText(mdFile);
			readmeText = readmeText.Replace(
				"https://github.com/NikolayXHD/Mtgdb/blob/master/output/help/img/",
				"../img/");

			var pipeline = new MarkdownPipelineBuilder()
				.UseAdvancedExtensions()
				.Build();

			var htmlContent = Markdown.ToHtml(readmeText, pipeline);
			htmlContent = _imgRegex.Replace(htmlContent, "<a href=${url}>${0}</a>");
			string title = getPageTitle(mdFile);

			var navigationItems = getNavigationItems(mdFile, mdFiles);

			var htmlPage = htmlTemplate
				.Replace("<!--Header-->", $"Mtgdb.Gui help - {title}")
				.Replace("<!--Navigation-->", navigationItems)
				.Replace("<!--Title-->", title)
				.Replace("<!--Content-->", htmlContent);
			return htmlPage;
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
				.Replace('_', ' ');

			title = title.Substring(0, 1).ToUpperInvariant() + title.Substring(1);
			return title;
		}

		private static readonly Regex _imgRegex = new Regex(
			"<img src=\"(?<url>[^\"]+)\"(?: alt=\"[^\"]+\")? />",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);
	}
}
