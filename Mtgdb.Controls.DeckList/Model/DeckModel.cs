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
			int countCollected(Card c, int countInDeck) => Math.Min(countInDeck, c.CollectionCount(Ui));
			int countCollectedSide(Card c, int countInDeck) => (c.CollectionCount(Ui) - countInMain(c)).WithinRange(0, countInDeck);

			float priceTotal(Card c, int countInDeck) => countInDeck * (c.PriceMid ?? 0f);
			float priceCollected(Card c, int countInDeck) => countCollected(c, countInDeck) * (c.PriceMid ?? 0f);
			float priceCollectedSide(Card c, int countInDeck) => countCollectedSide(c, countInDeck) * (c.PriceMid ?? 0f);

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

			_priceCollectedCache = new DeckAggregateCache<float, float, float>(
				() => Ui,
				() => Deck,
				() => 0f,
				(a, b) => a + b,
				priceCollected,
				a => a);

			_countCollectedCache = new DeckAggregateCache<int, int, int>(
				() => Ui,
				() => Deck,
				() => 0,
				(a, b) => a + b,
				countCollected,
				a => a);

			_priceCollectedSideCache = new DeckAggregateCache<float, float, float>(
				() => Ui,
				() => Deck,
				() => 0f,
				(a, b) => a + b,
				priceCollectedSide,
				a => a);

			_countCollectedSideCache = new DeckAggregateCache<int, int, int>(
				() => Ui,
				() => Deck,
				() => 0,
				(a, b) => a + b,
				countCollectedSide,
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
			Ui = ui;
		}

		public void Invalidate()
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
			MainCollectedPrice / Price(Zone.Main);

		public float SideCollectedPricePercent =>
			SideCollectedPrice / Price(Zone.Side);

		public float MainCollectedCountPercent =>
			(float) MainCollectedCount / Count(Zone.Main);

		public float SideCollectedCountPercent =>
			(float) SideCollectedCount / Count(Zone.Side);

		public float MainCollectedUnknownPricePercent =>
			(float) MainCollectedUnknownPriceCount / UnknownPriceCount(Zone.Main);

		public float SideCollectedUnknownPricePercent =>
			(float) SideCollectedUnknownPriceCount / UnknownPriceCount(Zone.Side);



		public IReadOnlyList<string> Legal
		{
			get
			{
				if (_legalFormatsCache != null)
					return _legalFormatsCache;

				if (!Ui.CardRepo.IsLoadingComplete)
					return ReadOnlyList.Empty<string>();

				bool isAllowedIn(string format, string id, int count)
				{
					var c = Ui.CardRepo.CardsById[id];
					return c.IsLegalIn(format) || c.IsRestrictedIn(format) && count <= 1;
				}

				_legalFormatsCache = Legality.Formats
					.Where(format => Deck.MainDeck.Count
						.All(_ => isAllowedIn(format, id: _.Key, count: _.Value)))
					.ToReadOnlyList();

				return _legalFormatsCache;
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



		public UiModel Ui
		{
			get => _ui;
			set
			{
				_ui = value;
				Invalidate();
			}
		}

		public Deck Deck
		{
			get => _deck;
			set
			{
				_deck = value;
				Invalidate();
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
		private readonly DeckAggregateCache<float, float, float> _priceCollectedCache;
		private readonly DeckAggregateCache<int, int, int> _countCollectedCache;
		private readonly DeckAggregateCache<float, float, float> _priceCollectedSideCache;
		private readonly DeckAggregateCache<int, int, int> _countCollectedSideCache;
		private readonly DeckAggregateCache<IList<string>, Dictionary<string, int>, string> _generatedManaCache;

		private IReadOnlyList<string> _legalFormatsCache;

		private UiModel _ui;
		private Deck _deck;
	}
}