using Mtgdb.Dal;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class DalTests
	{
		private readonly IKernel _kernel = new StandardKernel();

		[OneTimeSetUp]
		public void Setup()
		{
			_kernel.Load<CoreModule>();
			_kernel.Load<DalModule>();
		}

		[Test]
		public void Test_repository_is_not_empty()
		{
			var repo = _kernel.Get<CardRepository>();

			repo.LoadFile();
			repo.Load();

			Assert.That(repo.Cards, Is.Not.Null);
			Assert.That(repo.Cards, Is.Not.Empty);

			Assert.That(repo.SetsByCode, Is.Not.Null);
			Assert.That(repo.SetsByCode.Count, Is.GreaterThan(0));
		}
	}
}
