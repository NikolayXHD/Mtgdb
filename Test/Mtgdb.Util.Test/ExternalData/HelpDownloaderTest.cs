using NUnit.Framework;

namespace Mtgdb.Util.ExternalData
{
	[TestFixture]
	public class HelpDownloaderUtil
	{
		[Test]
		public void UpdateLocalHelp()
		{
			HelpDownloader.UpdateLocalHelp();
		}
	}
}
