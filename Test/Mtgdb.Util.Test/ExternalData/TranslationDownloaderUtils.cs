using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Mtgdb.Test;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class TranslationDownloaderUtils : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadCards();
		}

		private const string DownloadedDir = @"D:\temp\gatherer\downloaded";
		private const string ParsedDir = @"D:\temp\gatherer\parsed";
		private const string ResultDir = @"D:\temp\gatherer\result";

		[Test]
		public async Task Update_translations()
		{
			Directory.CreateDirectory(ParsedDir);
			Directory.CreateDirectory(ResultDir);

			var client = new GathererClient(Repo);
			await client.DownloadTranslations(DownloadedDir, CancellationToken.None);
			client.ParseTranslations(DownloadedDir, ParsedDir);
			client.MergeTranslations(ParsedDir, ResultDir);
			client.SaveNonEnglishTranslations(ResultDir);
		}
	}
}
