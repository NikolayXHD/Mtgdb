using System.Linq;
using Mtgdb.Dal;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class MtgaIntegrationTests
	{
		private StandardKernel _kernel;

		[OneTimeSetUp]
		public void SetUp()
		{
			_kernel = new StandardKernel();
			_kernel.Load<CoreModule>();
			_kernel.Load<DalModule>();
		}

		[Test]
		public void Import_returns_non_empty_collection()
		{
			var integration = _kernel.Get<MtgArenaIntegration>();
			var collection = integration.ImportCollection().ToList();

			Assert.That(collection, Is.Not.Null);
			Assert.That(collection, Is.Not.Empty);
		}

		// [Test]
		// public void Save_vanguard_data()
		// {
		// 	var repo = _kernel.Get<CardRepository>();
		//
		// 	const string van = "van";
		// 	repo.FilterSetCode = code => Str.Equals(code, van);
		// 	repo.LoadFile();
		// 	repo.Load();
		//
		// 	var patchByName = repo.SetsByCode[van].Cards.GroupBy(c=>c.NameEn)
		// 		.ToDictionary(
		// 			gr => gr.Key,
		// 			gr => new CardPatch { Hand = gr.First().Hand, Life = gr.First().Life },
		// 			Str.Comparer);
		//
		// 	var serialized = JsonConvert.SerializeObject(patchByName,
		// 		Formatting.Indented,
		// 		new JsonSerializerSettings
		// 		{
		// 			DefaultValueHandling = DefaultValueHandling.Ignore
		// 		});
		// }
	}
}