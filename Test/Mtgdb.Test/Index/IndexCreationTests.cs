using System.Linq;
using System.Text.RegularExpressions;
using Mtgdb.Data;
using Mtgdb.Data.Index;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Test
{
	public class IndexCreationTests : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			_repo = new CardRepository(Kernel.Get<CardFormatter>(), () => null)
			{
				FilterSetCode = F.IsEqualTo("ISD", Str.Comparer)
			};

			_localizationRepo = new LocalizationRepository();
			_cardSearcher = new CardSearcher(_repo, new CardDocumentAdapter(_repo));
			_keywordSearcher = new KeywordSearcher(_repo);

			_repo.LoadFile();
			_repo.Load();

			_localizationRepo.LoadFile();
			_localizationRepo.Load();

			_repo.FillLocalizations(_localizationRepo);
		}

		[Test, Order(0)]
		public void KeywordSearcher_creates_index()
		{
			_keywordSearcher.IndexDirectoryParent = _keywordSearcher.IndexDirectoryParent.Concat("-test");
			_keywordSearcher.InvalidateIndex();

			Assert.That(_keywordSearcher.IsUpToDate, Is.Not.True);

			_keywordSearcher.Load();

			Assert.That(_keywordSearcher.IsUpToDate);
		}

		[Test, Order(0)]
		public void LuceneSearcher_creates_index()
		{
			_cardSearcher.IndexDirectoryParent = _cardSearcher.IndexDirectoryParent.Concat("-test");
			_cardSearcher.InvalidateIndex();

			Assert.That(_cardSearcher.IsUpToDate, Is.Not.True);

			var state = _cardSearcher.CreateState();
			_cardSearcher.LoadIndex(state);

			Assert.That(_cardSearcher.IsUpToDate);
		}

		[Test, Order(1)]
		public void LuceneSpellchecker_creates_index()
		{
			_cardSearcher.Spellchecker.IndexDirectoryParent =
				_cardSearcher.Spellchecker.IndexDirectoryParent.Concat("-test");
			_cardSearcher.Spellchecker.InvalidateIndex();

			Assert.That(_cardSearcher.Spellchecker.IsUpToDate, Is.Not.True);

			_cardSearcher.Spellchecker.LoadIndex(_cardSearcher.State);

			Assert.That(_cardSearcher.Spellchecker.IsUpToDate);
		}

		[Test, Order(1)]
		public void LuceneSearcher_searches()
		{
			var cards = _cardSearcher.SearchCards("nameen:vampire", "en")
				.ToArray();

			Assert.That(cards, Is.Not.Null.And.Not.Empty);

			foreach (var card in cards)
				Assert.That(card.NameEn, Does.Contain("vampire").IgnoreCase);
		}

		[Test, Order(1)]
		public void KeywordSearcher_searches()
		{
			var cardIds = _keywordSearcher.GetCardIds(
					Enumerable.Empty<KeywordQueryTerm>(),
					Sequence.From(new KeywordQueryTerm
					{
						FieldName = "Cmc",
						Patterns = Sequence.Array(new Regex("^0$")),
						Values = Sequence.Array("0")
					}),
					Enumerable.Empty<KeywordQueryTerm>())
				.ToArray();

			Assert.That(cardIds, Is.Not.Null.And.Not.Empty);

			foreach (var id in cardIds)
			{
				var card = _repo.Cards[id];
				Assert.That(card.Cmc, Is.EqualTo(0));
			}
		}

		[Test, Order(2)]
		public void LuceneSpellchecker_searches()
		{
			string query = "nameen:vampire";
			var suggest = _cardSearcher.Spellchecker.Suggest(new TextInputState(query, query.Length, selectionLength: 0), "en").Values;

			Assert.That(suggest, Is.Not.Null.And.Not.Empty);
			Assert.That(suggest, Has.Some.Contains("vampire").IgnoreCase);
		}

		private CardRepository _repo;
		private LocalizationRepository _localizationRepo;
		private CardSearcher _cardSearcher;
		private KeywordSearcher _keywordSearcher;
	}
}
