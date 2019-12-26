using System.Threading;
using System.Threading.Tasks;
using Mtgdb.Downloader;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class InstallerTest
	{
		[Test]
		public async Task Download_from_gdrive()
		{
			var client = new GdriveWebClient();
			await client.DownloadFile(
				"https://drive.google.com/uc?id=0B_zQYOTucmnUOVE1eDU0STJZeE0&export=download",
				"D:\\temp\\file.temp",
					CancellationToken.None
				);
		}
	}
}
