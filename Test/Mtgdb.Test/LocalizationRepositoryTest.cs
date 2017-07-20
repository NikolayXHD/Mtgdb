using System.IO;
using Mtgdb.Dal;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class LocalizationRepositoryTest
	{
		[OneTimeSetUp]
		public void SetUp()
		{
			string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
			string directoryName = Path.GetDirectoryName(location);
			Directory.SetCurrentDirectory(directoryName);
		}

		[Test]
		public void Test()
		{
			var repo = new LocalizationRepository();
			repo.Load();
		}
	}
}
