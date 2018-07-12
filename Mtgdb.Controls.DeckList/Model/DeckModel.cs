using System;
using System.Collections.Generic;
using Mtgdb.Dal;
using System.Linq;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Controls
{
	public class DeckModel
	{
		public DeckModel(Deck deck, UiModel ui)
		{
			int countInMain(Card c) => Deck.MainDeck.Count.TryGet(c.Id);
			int countTotal(Card c, int countInDeck) => countInDeck;
			int countOwned(Card c, int countInDeck) => Math.Min(countInDeck, c.CollectionCount(Ui));
			int countOwnedSide(Card c, int countInDeck) => (c.CollectionCount(Ui) - countInMain(c)).WithinRange(0, countInDeck);

			float priceTotal(Card c, int countInDeck) => countInDeck * (c.PriceMid ?? 0f);
			float priceOwned(Card c, int countInDeck) => countOwned(c, countInDeck) * (c.PriceMid ?? 0f);
			float priceOwnedSide(Card c, int countInDeck) => countOwnedSide(c, countInDeck) * (c.PriceMid ?? 0f);

			IList<string> generatedMana(Card c, int countInDeck) => c.GeneratedManaArr;

			_priceTotalCache = new DeckAggregateCache<float, float, float>(
				() => Ui,
				() => Deck,
				() => 0f,
				(a, b) => a + b,
				priceTotal,
				a => a);

			_countTotalCache = new DeckAggregateCache<int, int, int>(
				() => Ui,
				() => Deck,
				() => 0,
				(a, b) => a + b,
				countTotal,
				a => a);

			_priceOwnedCache = new DeckAggregateCache<float, float, float>(
				() => Ui,
				() => Deck,
				() => 0f,
				(a, b) => a + b,
				priceOwned,
				a => a);

			_countOwnedCache = new DeckAggregateCache<int, int, int>(
				() => Ui,
				() => Deck,
				() => 0,
				(a, b) => a + b,
				countOwned,
				a => a);

			_priceOwnedSideCache = new DeckAggregateCache<float, float, float>(
				() => Ui,
				() => Deck,
				() => 0f,
				(a, b) => a + b,
				priceOwnedSide,
				a => a);

			_countOwnedSideCache = new DeckAggregateCache<int, int, int>(
				() => Ui,
				() => Deck,
				() => 0,
				(a, b) => a + b,
				countOwnedSide,
				a => a);

			_generatedManaCache = new DeckAggregateCache<IList<string>, Dictionary<string, int>, string>(
				() => Ui,
				() => Deck,
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

			_deck = deck;
			_ui = ui;
		}

		public void CollectionChanged()
		{
			_priceOwnedCache?.Clear();
			_countOwnedCache?.Clear();
			_priceOwnedSideCache?.Clear();
			_countOwnedSideCache?.Clear();
		}

		public void DeckChanged()
		{
			_priceOwnedCache?.Clear();
			_countOwnedCache?.Clear();
			
			_priceOwnedSideCache?.Clear();
			_countOwnedSideCache?.Clear();
			
			_priceTotalCache?.Clear();
			_countTotalCache?.Clear();

			_generatedManaCache?.Clear();
			_legalFormatsCache = null;
		}

		public bool IsCurrent { get; set; }

		public string Name => Deck.Name;

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



		public int Count(Zone zone) =>
			_countTotalCache.GetAggregate(zone, _filterNone);

		public float Price(Zone zone) =>
			_priceTotalCache.GetAggregate(zone, _filterNone);

		public int UnknownPriceCount(Zone zone) =>
			_countTotalCache.GetAggregate(zone, _filterPriceIsUnknown);



		public int MainOwnedCount =>
			_countOwnedCache.GetAggregate(Zone.Main, _filterNone);

		public int SideOwnedCount =>
			_countOwnedSideCache.GetAggregate(Zone.Side, _filterNone);

		public float MainOwnedPrice =>
			_priceOwnedCache.GetAggregate(Zone.Main, _filterNone);

		public float SideOwnedPrice =>
			_priceOwnedSideCache.GetAggregate(Zone.Side, _filterNone);

		public int MainOwnedUnknownPriceCount =>
			_countOwnedCache.GetAggregate(Zone.Main, _filterPriceIsUnknown);

		public int SideOwnedUnknownPriceCount =>
			_countOwnedSideCache.GetAggregate(Zone.Side, _filterPriceIsUnknown);



		public float MainOwnedPricePercent =>
			MainOwnedPrice / Price(Zone.Main);

		public float SideOwnedPricePercent =>
			SideOwnedPrice / Price(Zone.Side);

		public float MainOwnedCountPercent =>
			(float) MainOwnedCount / Count(Zone.Main);

		public float SideOwnedCountPercent =>
			(float) SideOwnedCount / Count(Zone.Side);

		public float MainOwnedUnknownPricePercent =>
			(float) MainOwnedUnknownPriceCount / UnknownPriceCount(Zone.Main);

		public float SideOwnedUnknownPricePercent =>
			(float) SideOwnedUnknownPriceCount / UnknownPriceCount(Zone.Side);



		public IReadOnlyList<string> Legal
		{
			get
			{
				if (_legalFormatsCache != null)
					return _legalFormatsCache;

				if (!_ui.CardRepo.IsLoadingComplete)
					return ReadOnlyList.Empty<string>();

				bool isAllowedIn(string format, string id, int count)
				{
					var c = _ui.CardRepo.CardsById[id];
					return c.IsLegalIn(format) || c.IsRestrictedIn(format) && count <= 1;
				}

				_legalFormatsCache = Legality.Formats
					.Where(format => Deck.MainDeck.Count
						.All(_ => isAllowedIn(format, id: _.Key, count: _.Value)))
					.ToReadOnlyList();

				return _legalFormatsCache;
			}
		}



		public UiModel Ui
		{
			get => _ui;
			set
			{
				CollectionChanged();
				_ui = value;
			}
		}

		public int Id
		{
			get => _deck.Id;
			set => _deck.Id = value;
		}

		public DateTime? Saved
		{
			get => _deck.Saved;
			set => _deck.Saved = value;
		}

		public Deck Deck
		{
			get => _deck;
			set
			{
				_deck = value;
				DeckChanged();
			}
		}



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
		private readonly DeckAggregateCache<float, float, float> _priceOwnedCache;
		private readonly DeckAggregateCache<int, int, int> _countOwnedCache;
		private readonly DeckAggregateCache<float, float, float> _priceOwnedSideCache;
		private readonly DeckAggregateCache<int, int, int> _countOwnedSideCache;
		private readonly DeckAggregateCache<IList<string>, Dictionary<string, int>, string> _generatedManaCache;

		private IReadOnlyList<string> _legalFormatsCache;

		private UiModel _ui;
		private Deck _deck;
	}
}