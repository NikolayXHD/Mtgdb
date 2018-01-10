using System.IO;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class TranslationDownloaderUtils : TestsBase
	{
		private const string DownloadedDir = @"D:\temp\gatherer-downloaded";
		private const string ParsedDir = @"D:\temp\gatherer-parsed";
		private const string ResultDir = @"D:\temp\gatherer-result";

		[Test]
		public void Update_translations()
		{
			LoadModules();
			LoadCards();

			Directory.CreateDirectory(ParsedDir);
			Directory.CreateDirectory(ResultDir);

			var client = new GathererClient(Repo);
			client.DownloadTranslations(DownloadedDir);
			client.ParseTranslations(DownloadedDir, ParsedDir);
			client.MergeTranslations(ParsedDir, ResultDir);
			client.SaveNonEnglishTranslations(ResultDir);
		}
	}
}