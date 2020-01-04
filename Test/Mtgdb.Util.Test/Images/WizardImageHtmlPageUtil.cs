using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class WizardImageHtmlPageUtil
	{
		[TestCase(HtmlDir + @"\Throne of Eldraine Variants _ MAGIC_ THE GATHERING.html", OriginalDir + @"\celd.png")]
		public void RenameWizardsWebpageImages(string htmlPath, string targetDir)
		{
			string htmlFileName = Path.GetFileNameWithoutExtension(htmlPath);
			string directoryName = Path.GetDirectoryName(htmlPath) ??
				throw new ArgumentException(htmlPath, nameof(htmlPath));
			string filesDirectory = Path.Combine(directoryName, htmlFileName + "_files");

			string content = File.ReadAllText(htmlPath);
			var matches = _imgTagPattern.Matches(content);

			Directory.CreateDirectory(targetDir);
			foreach (Match match in matches)
			{
				string originalFileName = match.Groups["file"].Value;
				string ext = Path.GetExtension(originalFileName);

				string filePath = Path.Combine(filesDirectory, originalFileName);

				string name = HttpUtility.HtmlDecode(match.Groups["name"].Value)
					.Replace(" // ", "");

				string defaultTargetPath = Path.Combine(targetDir, name + ext);

				bool defaultTargetExists = File.Exists(defaultTargetPath);

				if (defaultTargetExists || File.Exists(getTargetPath(1)))
				{
					if (defaultTargetExists)
						File.Move(defaultTargetPath, getTargetPath(1));

					for (int i = 2; i < 12; i++)
					{
						string targetPath = getTargetPath(i);
						if (!File.Exists(targetPath))
						{
							File.Copy(filePath, targetPath, overwrite: false);
							break;
						}
					}
				}
				else
				{
					File.Copy(filePath, defaultTargetPath, overwrite: false);
				}

				string getTargetPath(int num) =>
					Path.Combine(targetDir, name + num + ext);
			}
		}

		private const string OriginalDir = @"D:\Distrib\games\mtg\Gatherer.Original";
		private const string HtmlDir = @"D:\temp\html";
		private static readonly Regex _imgTagPattern = new Regex(
			@"<img alt=""(?<name>[^""]+)"" src=""[^""]+\/(?<file>[^""]+)""");
	}
}