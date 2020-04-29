using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using Mtgdb.Dev;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class WizardImageHtmlPageUtil
	{
		[TestCase("Throne of Eldraine Variants _ MAGIC_ THE GATHERING.html", "celd.png")]
		public void RenameWizardsWebpageImages(string htmlFile, string targetSubdir)
		{
			FsPath htmlPath = HtmlDir.Join(htmlFile);
			FsPath targetDir = DevPaths.GathererOriginalDir.Join(targetSubdir);

			string htmlFileName = htmlPath.Basename(extension: false);
			FsPath directoryName = htmlPath.Parent();
			if (!directoryName.HasValue())
				throw new ArgumentException(htmlPath.Value, nameof(htmlPath));

			FsPath filesDirectory = directoryName.Join(htmlFileName + "_files");

			string content = htmlPath.ReadAllText();
			var matches = _imgTagPattern.Matches(content);

			targetDir.CreateDirectory();
			foreach (Match match in matches)
			{
				string originalFileName = match.Groups["file"].Value;
				string ext = Path.GetExtension(originalFileName);

				FsPath filePath = filesDirectory.Join(originalFileName);

				string name = HttpUtility.HtmlDecode(match.Groups["name"].Value)
					.Replace(" // ", "");

				FsPath defaultTargetPath = targetDir.Join(name + ext);

				bool defaultTargetExists = defaultTargetPath.IsFile();

				if (defaultTargetExists || getTargetPath(1).IsFile())
				{
					if (defaultTargetExists)
						defaultTargetPath.MoveFileTo(getTargetPath(1));

					for (int i = 2; i < 12; i++)
					{
						FsPath targetPath = getTargetPath(i);
						if (!targetPath.IsFile())
						{
							filePath.CopyFileTo(targetPath, overwrite: false);
							break;
						}
					}
				}
				else
				{
					filePath.CopyFileTo(defaultTargetPath, overwrite: false);
				}

				FsPath getTargetPath(int num) =>
					targetDir.Join(name + num + ext);
			}
		}

		private static readonly FsPath HtmlDir = DevPaths.DataDrive.Join("temp", "html");
		private static readonly Regex _imgTagPattern = new Regex(
			@"<img alt=""(?<name>[^""]+)"" src=""[^""]+\/(?<file>[^""]+)""");
	}
}
