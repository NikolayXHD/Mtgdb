using System;
using System.Collections.Generic;
using Mtgdb.Dal;
using System.Linq;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Controls
{
	public class DeckModel
	{
		public DeckModel(Deck deck, CardRepository repo, ICardCollection collection, IDeckTransformation transformation)
		{
			int countInMain(Card c) => Deck.MainDeck.Count.TryGet(c.Id);
			int countTotal(Card c, int countInDeck) => countInDeck;
			int countCollected(Card c, int countInDeck) => Math.Min(countInDeck, _collection.GetCount(c));
			int countCollectedSide(Card c, int countInDeck) => (_collection.GetCount(c) - countInMain(c)).WithinRange(0, countInDeck);

			float priceTotal(Card c, int countInDeck) => countInDeck * (c.PriceMid ?? 0f);
			float priceCollected(Card c, int countInDeck) => countCollected(c, countInDeck) * (c.PriceMid ?? 0f);
			float priceCollectedSide(Card c, int countInDeck) => countCollectedSide(c, countInDeck) * (c.PriceMid ?? 0f);

			IList<string> generatedMana(Card c, int countInDeck) => c.GeneratedManaArr;

			_priceTotalCache = new DeckAggregateCache<float, float, float>(
				repo,
				() => Deck,
				() => 0f,
				(a, b) => a + b,
				priceTotal,
				a => a);

			_countTotalCache = new DeckAggregateCache<int, int, int>(
				repo,
				() => Deck,
				() => 0,
				(a, b) => a + b,
				countTotal,
				a => a);

			_priceCollectedCache = new DeckAggregateCache<float, float, float>(
				repo,
				() => Deck,
				() => 0f,
				(a, b) => a + b,
				priceCollected,
				a => a);

			_countCollectedCache = new DeckAggregateCache<int, int, int>(
				repo,
				() => Deck,
				() => 0,
				(a, b) => a + b,
				countCollected,
				a => a);

			_priceCollectedSideCache = new DeckAggregateCache<float, float, float>(
				repo,
				() => Deck,
				() => 0f,
				(a, b) => a + b,
				priceCollectedSide,
				a => a);

			_countCollectedSideCache = new DeckAggregateCache<int, int, int>(
				repo,
				() => Deck,
				() => 0,
				(a, b) => a + b,
				countCollectedSide,
				a => a);

			_generatedManaCache = new DeckAggregateCache<IList<string>, Dictionary<string, int>, string>(
				repo,
				() => OriginalDeck,
				() => new Dictionary<string, int>(Str.Comparer),
				(a, b) =>
				{
					foreach (string s in b)
					{
						a.TryGetValue(s, out int count);
						count++;
						a[s] = count;
					}

					return a;
				},
				generatedMana,
				a => string.Concat(a.Keys.OrderBy(s => KeywordDefinitions.GeneratedMana.IndexOf(s, Str.Comparer))));

			_filterNone = c => true;
			_filterPriceIsUnknown = c => !c.PriceMid.HasValue;

			_filterIsCreature = c => c.TypesArr.IndexOf("creature", Str.Comparer) >= 0;
			_filterIsLand = c => c.TypesArr.IndexOf("land", Str.Comparer) >= 0;
			_filterIsOtherSpell = c => !_filterIsCreature(c) && !_filterIsLand(c);

			_filterIsLandAndPriceIsUnknown = c => _filterIsLand(c) && _filterPriceIsUnknown(c);
			_filterIsCreatureAndPriceIsUnknown = c => _filterIsCreature(c) && _filterPriceIsUnknown(c);
			_filterIsOtherSpellAndPriceIsUnknown = c => _filterIsOtherSpell(c) && _filterPriceIsUnknown(c);

			_collection = collection;
			_repo = repo;
			_transformation = transformation;
			OriginalDeck = deck;
		}

		private void clearCaches()
		{
			_priceCollectedCache?.Clear();
			_countCollectedCache?.Clear();

			_priceCollectedSideCache?.Clear();
			_countCollectedSideCache?.Clear();

			_priceTotalCache?.Clear();
			_countTotalCache?.Clear();

			_generatedManaCache?.Clear();
			_legalFormatsCache = null;
		}

		public bool IsCurrent { get; set; }

		public string Name
		{
			get => OriginalDeck.Name;
			set => OriginalDeck.Name = value;
		}

		public string Mana =>
			_generatedManaCache.GetAggregate(Zone.Main, _filterNone);



		public int LandCount =>
			_countTotalCache.GetAggregate(Zone.Main, _filterIsLand);

		public float LandPrice =>
			_priceTotalCache.GetAggregate(Zone.Main, _filterIsLand);

		public int LandUnknownPriceCount =>
			_countTotalCache.GetAggregate(Zone.Main, _filterIsLandAndPriceIsUnknown);



		public int CreatureCount =>
			_countTotalCache.GetAggregate(Zone.Main, _filterIsCreature);

		public float CreaturePrice =>
			_priceTotalCache.GetAggregate(Zone.Main, _filterIsCreature);

		public int CreatureUnknownPriceCount =>
			_countTotalCache.GetAggregate(Zone.Main, _filterIsCreatureAndPriceIsUnknown);



		public int OtherSpellCount =>
			_countTotalCache.GetAggregate(Zone.Main, _filterIsOtherSpell);

		public float OtherSpellPrice =>
			_priceTotalCache.GetAggregate(Zone.Main, _filterIsOtherSpell);

		public int OtherSpellUnknownPriceCount =>
			_countTotalCache.GetAggregate(Zone.Main, _filterIsOtherSpellAndPriceIsUnknown);



		public int MainCount =>
			_countTotalCache.GetAggregate(Zone.Main, _filterNone);

		public int SideCount =>
			_countTotalCache.GetAggregate(Zone.Side, _filterNone);

		public float MainPrice =>
			_priceTotalCache.GetAggregate(Zone.Main, _filterNone);

		public float SidePrice =>
			_priceTotalCache.GetAggregate(Zone.Side, _filterNone);

		public int MainUnknownPriceCount =>
			_countTotalCache.GetAggregate(Zone.Main, _filterPriceIsUnknown);

		public int SideUnknownPriceCount =>
			_countTotalCache.GetAggregate(Zone.Side, _filterPriceIsUnknown);



		public int MainCollectedCount =>
			_countCollectedCache.GetAggregate(Zone.Main, _filterNone);

		public int SideCollectedCount =>
			_countCollectedSideCache.GetAggregate(Zone.Side, _filterNone);

		public float MainCollectedPrice =>
			_priceCollectedCache.GetAggregate(Zone.Main, _filterNone);

		public float SideCollectedPrice =>
			_priceCollectedSideCache.GetAggregate(Zone.Side, _filterNone);

		public int MainCollectedUnknownPriceCount =>
			_countCollectedCache.GetAggregate(Zone.Main, _filterPriceIsUnknown);

		public int SideCollectedUnknownPriceCount =>
			_countCollectedSideCache.GetAggregate(Zone.Side, _filterPriceIsUnknown);



		public float MainCollectedPricePercent =>
			MainCollectedPrice / MainPrice;

		public float SideCollectedPricePercent =>
			SideCollectedPrice / SidePrice;

		public float MainCollectedCountPercent =>
			(float) MainCollectedCount / MainCount;

		public float SideCollectedCountPercent =>
			(float) SideCollectedCount / SideCount;

		public float MainCollectedUnknownPricePercent =>
			(float) MainCollectedUnknownPriceCount / MainUnknownPriceCount;

		public float SideCollectedUnknownPricePercent =>
			(float) SideCollectedUnknownPriceCount / SideUnknownPriceCount;



		public IReadOnlyList<string> Legal
		{
			get
			{
				if (_legalFormatsCache != null)
					return _legalFormatsCache;

				if (!_repo.IsLoadingComplete)
					return ReadOnlyList.Empty<string>();

				bool isAllowedIn(string format, string id, int count)
				{
					var c = _repo.CardsById[id];
					return c.IsLegalIn(format) || c.IsRestrictedIn(format) && count <= 1;
				}

				_legalFormatsCache = Legality.Formats
					.Where(format => OriginalDeck.MainDeck.Count
						.All(_ => isAllowedIn(format, id: _.Key, count: _.Value)))
					.ToReadOnlyList();

				return _legalFormatsCache;
			}
		}

		public long Id
		{
			get => OriginalDeck.Id;
			set => OriginalDeck.Id = value;
		}

		public DateTime? Saved
		{
			get => OriginalDeck.Saved;
			set => OriginalDeck.Saved = value;
		}


		public ICardCollection Collection
		{
			get => _collection;
			set
			{
				_collection = value;
				_isDeckUpToDate = false;
			}
		}

		public Deck Deck
		{
			get
			{
				UpdateTransformedDeck();
				return _deck;
			}
		}

		public void ResetTransformedDeck()
		{
			_isDeckUpToDate = false;
			clearCaches();
		}

		public void UpdateTransformedDeck()
		{
			lock (_sync)
			{
				if (_isDeckUpToDate)
					return;

				var prevValue = _deck;
				var newValue = _transformation?.Transform(OriginalDeck, _collection) ?? OriginalDeck;

				if (prevValue == null || !prevValue.IsEquivalentTo(newValue))
				{
					_deck = _transformation?.Transform(OriginalDeck, _collection) ?? OriginalDeck;
					clearCaches();
				}

				_isDeckUpToDate = true;
			}
		}

		public bool MayContainCardNames(HashSet<string> cardNameEns)
		{
			if (_cardNames == null)
				return true;

			var result = _cardNames.Overlaps(cardNameEns);
			return result;
		}

		public bool IsEquivalentTo(Deck other) =>
			OriginalDeck.IsEquivalentTo(other);

		private readonly Func<Card, bool> _filterNone;
		private readonly Func<Card, bool> _filterPriceIsUnknown;
		private readonly Func<Card, bool> _filterIsCreature;
		private readonly Func<Card, bool> _filterIsLandAndPriceIsUnknown;
		private readonly Func<Card, bool> _filterIsCreatureAndPriceIsUnknown;
		private readonly Func<Card, bool> _filterIsOtherSpellAndPriceIsUnknown;
		private readonly Func<Card, bool> _filterIsLand;
		private readonly Func<Card, bool> _filterIsOtherSpell;

		private readonly DeckAggregateCache<float, float, float> _priceTotalCache;
		private readonly DeckAggregateCache<int, int, int> _countTotalCache;
		private readonly DeckAggregateCache<float, float, float> _priceCollectedCache;
		private readonly DeckAggregateCache<int, int, int> _countCollectedCache;
		private readonly DeckAggregateCache<float, float, float> _priceCollectedSideCache;
		private readonly DeckAggregateCache<int, int, int> _countCollectedSideCache;
		private readonly DeckAggregateCache<IList<string>, Dictionary<string, int>, string> _generatedManaCache;

		private IReadOnlyList<string> _legalFormatsCache;

		private ICardCollection _collection;

		public Deck OriginalDeck
		{
			get => _originalDeck;
			set
			{
				_originalDeck = value;

				ResetTransformedDeck();

				bool isRepoLoaded = _repo.IsLoadingComplete;
				if (_cardNames == null && isRepoLoaded)
					_cardNames = new HashSet<string>(Str.Comparer);

				if (!isRepoLoaded)
					return;

				_cardNames.Clear();
				_cardNames.UnionWith(value.MainDeck.Order.Select(_ => _repo.CardsById[_].NameEn));
				_cardNames.UnionWith(value.Sideboard.Order.Select(_ => _repo.CardsById[_].NameEn));
			}
		}

		private readonly CardRepository _repo;
		private readonly IDeckTransformation _transformation;

		private bool _isDeckUpToDate;
		private Deck _deck;
		private HashSet<string> _cardNames;
		private readonly object _sync = new object();
		private Deck _originalDeck;
	}
}