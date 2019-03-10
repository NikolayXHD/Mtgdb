using System.IO;
using System.Linq;
using Mtgdb.Data.Index;
using Mtgdb.Data.Model;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class DeckSearchTests: TestsBase
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			LoadModules();

			_searcher = Kernel.Get<DeckSearcher>();
			_searcher.IndexDirectoryParent += "-test";

			_model = Kernel.Get<DeckListModel>();
			_model.FileName = AppDir.History.AddPath("decks.test.json");
		}

		[SetUp]
		public void Setup() =>
			clearDeckList();

		private void clearDeckList()
		{
			if (File.Exists(_model.FileName))
				File.Delete(_model.FileName);

			_model.Load();

			_searcher.InvalidateIndex();
		}

		[Test]
		public void Deck_list_Is_initially_empty()
		{
			var count = _model.GetModels().Count();
			Assert.That(count, Is.EqualTo(0));
		}

		private DeckSearcher _searcher;
		private DeckListModel _model;
	}
}