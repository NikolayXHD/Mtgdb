using System.Threading.Tasks;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class HelpDownloaderUtil
	{
		[Test]
		public async Task UpdateLocalHelp()
		{
			await HelpDownloader.UpdateLocalHelp();
		}
	}
}
