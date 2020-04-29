using System.Threading;
using System.Threading.Tasks;
using Mtgdb.Dev;
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

		[Test]
		public async Task Update_translations()
		{
			ParsedDir.CreateDirectory();
			ResultDir.CreateDirectory();

			var client = new GathererClient(Repo);
			await client.DownloadTranslations(DownloadedDir, CancellationToken.None);
			client.ParseTranslations(DownloadedDir, ParsedDir);
			client.MergeTranslations(ParsedDir, ResultDir);
			client.SaveNonEnglishTranslations(ResultDir);
		}

		private static readonly FsPath RootDir = DevPaths.DataDrive.Join("temp", "gatherer");
		private static readonly FsPath DownloadedDir = RootDir.Join("downloaded");
		private static readonly FsPath ParsedDir = RootDir.Join("parsed");
		private static readonly FsPath ResultDir = RootDir.Join("result");
	}
}
