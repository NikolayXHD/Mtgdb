using System.Collections.Generic;
using System.Linq;
using Mtgdb.Controls.Statistics;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public static class Fields
	{
		private static readonly FieldBuilder<Card> _builder = new FieldBuilder<Card>();

		public static readonly Dictionary<string, IField<Card>> ByName = new IField<Card>[]
		{
			_builder.Get(nameof(Card.Name), c => c.Name),
			_builder.Get(nameof(Card.ManaCost), c => c.ManaCost),
			_builder.Get(nameof(Card.Type), c => c.Type),
			_builder.Get(nameof(Card.Cmc), c => c.Cmc),
			_builder.Get(nameof(Card.SetCode), c => c.SetCode),
			_builder.Get(nameof(Card.SetName), c => c.SetName),
			_builder.Get(nameof(Card.Artist), c => c.Artist),
			_builder.Get(nameof(Card.ReleaseDate), c => c.ReleaseDate),
			_builder.Get(nameof(Card.Rarity), c => c.Rarity),
			_builder.Get(nameof(Card.PricingLow), c => c.PricingLow, @"price_low"),
			_builder.Get(nameof(Card.PricingMid), c => c.PricingMid, @"price_mid"),
			_builder.Get(nameof(Card.PricingHigh), c => c.PricingHigh, @"price_high"),
			_builder.Get(nameof(Card.Loyalty), c => c.LoyaltyNum),
			_builder.Get(nameof(Card.Power), c => c.PowerNum),
			_builder.Get(nameof(Card.Toughness), c => c.ToughnessNum),

			_builder.Get(nameof(Card.IndexInFile), c => c.IndexInFile),

			_builder.Get(nameof(Card.NameEn), c => c.NameEn),
			_builder.Get(nameof(Card.TypeEn), c => c.TypeEn),

			_builder.Get(nameof(Card.Supertypes), c => c.Supertypes),
			_builder.Get(nameof(Card.Types), c => c.Types),
			_builder.Get(nameof(Card.Subtypes), c => c.Subtypes),


			_builder.Get(nameof(Card.CollectionCount), c => c.CollectionCount),
			_builder.Get(nameof(Card.DeckCount), c => c.DeckCount),
			_builder.Get(nameof(Card.PriceLow), c => c.PriceLow),
			_builder.Get(nameof(Card.PriceMid), c => c.PriceMid),
			_builder.Get(nameof(Card.PriceHigh), c => c.PriceHigh),
			_builder.Get(nameof(Card.DeckTotalLow), c => c.DeckTotalLow),
			_builder.Get(nameof(Card.DeckTotalMid), c => c.DeckTotalMid),
			_builder.Get(nameof(Card.DeckTotalHigh), c => c.DeckTotalHigh),
			_builder.Get(nameof(Card.CollectionTotalLow), c => c.CollectionTotalLow),
			_builder.Get(nameof(Card.CollectionTotalMid), c => c.CollectionTotalMid),
			_builder.Get(nameof(Card.CollectionTotalHigh), c => c.CollectionTotalHigh),

			_builder.Get(nameof(Card.IsSearchResult), c => c.IsSearchResult),
			_builder.Get(nameof(Card.HasImage), c => c.HasImage),

			_builder.Get(nameof(Card.ReleaseMonth), c => c.ReleaseMonth),
			_builder.Get(nameof(Card.ReleaseYear), c => c.ReleaseYear),
			_builder.Get(nameof(Card.Color), c => c.Color),

			_builder.Get(string.Empty, c => string.Empty, "")
		}.ToDictionary(_ => _.Name);

		public static readonly List<string> ChartFields = new List<string>
		{
			nameof(Card.ManaCost),
			nameof(Card.Cmc),
			nameof(Card.SetCode),
			nameof(Card.SetName),
			nameof(Card.Artist),
			nameof(Card.ReleaseDate),
			nameof(Card.ReleaseMonth),
			nameof(Card.ReleaseYear),
			nameof(Card.Rarity),
			nameof(Card.Loyalty),
			nameof(Card.Power),
			nameof(Card.Toughness),
			nameof(Card.NameEn),
			nameof(Card.TypeEn),
			nameof(Card.Supertypes),
			nameof(Card.Types),
			nameof(Card.Subtypes),

			nameof(Card.PriceLow),
			nameof(Card.PriceMid),
			nameof(Card.PriceHigh),
			nameof(Card.DeckCount),
			nameof(Card.DeckTotalLow),
			nameof(Card.DeckTotalMid),
			nameof(Card.DeckTotalHigh),
			nameof(Card.CollectionCount),
			nameof(Card.CollectionTotalLow),
			nameof(Card.CollectionTotalMid),
			nameof(Card.CollectionTotalHigh),

			nameof(Card.Color),
			nameof(Card.IsSearchResult),
			nameof(Card.HasImage)
		};
	}
}