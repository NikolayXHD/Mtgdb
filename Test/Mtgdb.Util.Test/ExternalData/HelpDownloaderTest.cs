using NUnit.Framework;

namespace Mtgdb.Util
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
