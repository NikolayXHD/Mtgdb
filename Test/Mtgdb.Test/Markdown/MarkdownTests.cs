﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		public void Render_local_help()
		{
			var helpFiles = getHelpFiles();

			var htmlTemplate = File.ReadAllText(AppDir.Root.AddPath("help\\html\\template.html"));

			foreach (string helpFile in helpFiles)
			{
				string mdContent = File.ReadAllText(helpFile);

				var htmlFile = AppDir.Root.AddPath($"help\\html\\{getPageName(helpFile)}.html");
				string htmlPage = getHtmlPage(helpFile, mdContent, htmlTemplate, helpFiles);
				File.WriteAllText(htmlFile, htmlPage);

				var textFile = AppDir.Root.AddPath($"help\\{getTextFileName(helpFile)}.txt");
				var textContent = _imgRegex.Replace(mdContent, "${url}");

				File.WriteAllText(textFile, textContent);
			}
		}

		[Test]
		public void No_redundant_images()
		{
			string imgDir = AppDir.Root.AddPath("help\\img");
			var images = Directory.GetFiles(imgDir, "*.jpg", SearchOption.TopDirectoryOnly);

			var redundantImages = new HashSet<string>(images.Select(Path.GetFileNameWithoutExtension),
				Str.Comparer);

			var usedImages = new HashSet<string>(Str.Comparer);

			var helpFiles = getHelpFiles();

			var imgUrlRegex = new Regex(Regex.Escape(ImgDirUrl) + @"(?<name>[^@\/]+)\.jpg",
				RegexOptions.Compiled);

			foreach (string helpFile in helpFiles)
			{
				string mdContent = File.ReadAllText(helpFile);

				foreach (Match match in imgUrlRegex.Matches(mdContent))
					usedImages.Add(match.Groups["name"].Value);
			}

			redundantImages.ExceptWith(usedImages);
			CollectionAssert.IsNotEmpty(usedImages);
			CollectionAssert.IsEmpty(redundantImages);
		}


		private static string[] getHelpFiles()
		{
			var helpFiles = Directory.GetFiles(
				AppDir.Root.AddPath("..\\..\\Mtgdb.wiki"),
				"*.md",
				SearchOption.TopDirectoryOnly);
			return helpFiles;
		}

		private static string getTextFileName(string helpFile)
		{
			string name = Path.GetFileNameWithoutExtension(helpFile);
			return (name.Substring(0, 2) + name.Substring(3)).Replace('-', '_');

		}

		private static string getPageName(string helpFile)
		{
			return Path.GetFileNameWithoutExtension(helpFile).Substring(4).Replace("-", "_");
		}

		private static string getHtmlPage(string mdFile, string mdContent, string htmlTemplate, IList<string> mdFiles)
		{
			var readmeText = mdContent
				.Replace(".jpg?raw=true", ".jpg")
				.Replace(ImgDirUrl, "../img/");

			var pipeline = new MarkdownPipelineBuilder()
				.UseAdvancedExtensions()
				.Build();

			var htmlContent = Markdown.ToHtml(readmeText, pipeline);
			//htmlContent = _imgRegex.Replace(htmlContent, "<a href=${url}>${0}</a>");
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
			@"\[!\[[^\]]+\]\((?<url>[^\)]+)\)\]\(\1\)",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private const string ImgDirUrl = "https://github.com/NikolayXHD/Mtgdb/blob/master/output/help/img/";
	}
}
