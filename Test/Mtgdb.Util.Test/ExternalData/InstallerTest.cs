using Mtgdb.Downloader;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class InstallerTest
	{
		[Test]
		public void Download_from_gdrive()
		{
			var client = new GdriveWebClient();
			client.DownloadFile("https://drive.google.com/uc?id=0B_zQYOTucmnUOVE1eDU0STJZeE0&export=download", "D:\\temp");
		}
	}
}