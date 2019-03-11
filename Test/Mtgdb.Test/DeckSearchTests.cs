using System.IO;
using System.Linq;
using Mtgdb.Data.Index;
using Mtgdb.Data.Model;
using Mtgdb.Ui;
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

		[Test]
		public void Deck_can_be_found_by_name()
		{
			var names = new[]
			{
				"name1",
				"name2"
			};

			foreach (string name in names)
			{
				var deck = Deck.Create();
				deck.Name = name;
				_model.Add(deck);
			}

			_model.Save();

			_searcher.LoadIndexes();

			foreach (string name in names)
			{
				var searchResult = _searcher.Search("name: " + name);

				Assert.That(searchResult, Is.Not.Null);
				Assert.That(searchResult.RelevanceById, Is.Not.Null);
				Assert.That(searchResult.RelevanceById.Count, Is.EqualTo(1));

				var foundDeck = _model.GetModels().FirstOrDefault(m => searchResult.RelevanceById.ContainsKey(m.Id));
				Assert.That(foundDeck, Is.Not.Null);
				Assert.That(foundDeck.Name, Is.EqualTo(name));
			}
		}

		private DeckSearcher _searcher;
		private DeckListModel _model;
	}
}