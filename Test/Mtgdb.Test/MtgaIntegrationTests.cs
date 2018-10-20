using System.Linq;
using Mtgdb.Dal;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class MtgaIntegrationTests
	{
		[Test]
		public void Import_returns_non_empty_collection()
		{
			var kernel = new StandardKernel();
			kernel.Load<CoreModule>();
			kernel.Load<DalModule>();

			var integration = kernel.Get<MtgArenaIntegration>();
			var collection = integration.ImportCollection().ToList();

			Assert.That(collection, Is.Not.Null);
			Assert.That(collection, Is.Not.Empty);
		}
	}
}