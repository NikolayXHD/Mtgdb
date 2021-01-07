using System;
using FluentAssertions;
using Mtgdb.Data;
using Mtgdb.Data.Model;
using Mtgdb.Ui;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	[Parallelizable(ParallelScope.Self)]
	public class DeckModelTests : TestsBase
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			LoadModules();

			_repo = new CardRepository(Kernel.Get<CardFormatter>(), () => null)
			{
				FilterSetCode = F.IsWithin(Sequence.From("ME2", "ICE"), Str.Comparer),
			};

			_repo.LoadFile();
			_repo.Load();
			_priceRepo = new PriceRepository(() => null);
			// do not call _priceRepo.Load on purpose
			_priceRepo.FillPrice(_repo);

			_transformation = new CollectedCardsDeckTransformation(_repo, _priceRepo);
		}

		[SetUp]
		public void Setup()
		{
			_deckEditor = new DeckEditorModel();
			_collectionEditor = new CollectionEditorModel();
		}

		[Test]
		public void Total_count(
			[Values(0, 1, 2, 4)] int deckCount,
			[Values(0, 1, 2, 4)] int collectionCount)
		{
			var card = _repo.Cards[0];
			if (deckCount > 0)
				_deckEditor.Add(card, deckCount);
			if (collectionCount > 0)
				_collectionEditor.Add(card, collectionCount);

			var model = createModel();
			model.MainCount.Should().Be(deckCount);
			model.MainCollectedCount.Should().Be(Math.Min(deckCount, collectionCount));
		}

		private DeckModel createModel()
		{
			var originalDeck = _deckEditor.Snapshot();
			var collection = _collectionEditor.Snapshot();
			return new DeckModel(originalDeck, _repo, _priceRepo, collection, _transformation);
		}

		private CardRepository _repo;
		private DeckEditorModel _deckEditor;
		private CollectionEditorModel _collectionEditor;
		private PriceRepository _priceRepo;
		private CollectedCardsDeckTransformation _transformation;
	}
}
