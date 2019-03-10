using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class DeckSearchTests: TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadPrices();
		}
	}
}