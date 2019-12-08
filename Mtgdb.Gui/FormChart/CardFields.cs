using System.Collections.Generic;
using System.Linq;
using Mtgdb.Data;

namespace Mtgdb.Gui
{
	public class CardFields : Fields<Card>
	{
		public CardFields()
		{
			ByName = new IField<Card>[]
			{
				Builder.Get(nameof(Card.Name), c => c.Name(Ui)),
				Builder.Get(nameof(Card.ManaCost), c => c.ManaCost),
				Builder.Get(nameof(Card.Type), c => c.Type(Ui)),
				Builder.Get(nameof(Card.Cmc), c => c.Cmc),
				Builder.Get(nameof(Card.SetCode), c => c.SetCode),
				Builder.Get(nameof(Card.SetName), c => c.SetName),
				Builder.Get(nameof(Card.Artist), c => c.Artist),
				Builder.Get(nameof(Card.ReleaseDate), c => c.ReleaseDate),
				Builder.Get(nameof(Card.Rarity), c => c.Rarity),
				Builder.Get(nameof(Card.Price), c => c.Price),
				Builder.Get(nameof(Card.Loyalty), c => c.LoyaltyNum),
				Builder.Get(nameof(Card.Power), c => c.PowerNum),
				Builder.Get(nameof(Card.Toughness), c => c.ToughnessNum),

				Builder.Get(nameof(Card.IndexInFile), c => c.IndexInFile),

				Builder.Get(nameof(Card.NameEn), c => c.NameEn),
				Builder.Get(nameof(Card.TypeEn), c => c.TypeEn),

				Builder.Get(nameof(Card.Supertypes), c => c.Supertypes),
				Builder.Get(nameof(Card.Types), c => c.Types),
				Builder.Get(nameof(Card.Subtypes), c => c.Subtypes),


				Builder.Get(nameof(Card.CollectionCount), c => c.CollectionCount(Ui)),
				Builder.Get(nameof(Card.DeckCount), c => c.DeckCount(Ui)),
				Builder.Get(nameof(Card.DeckTotal), c => c.DeckTotal(Ui)),
				Builder.Get(nameof(Card.CollectionTotal), c => c.CollectionTotal(Ui)),

				Builder.Get(nameof(Card.IsSearchResult), c => c.IsSearchResult),
				Builder.Get(nameof(Card.HasImage), c => c.HasImage(Ui)),

				Builder.Get(nameof(Card.ReleaseMonth), c => c.ReleaseMonth),
				Builder.Get(nameof(Card.ReleaseYear), c => c.ReleaseYear),
				Builder.Get(nameof(Card.Color), c => c.Color),

				Builder.Get(string.Empty, c => string.Empty, "")
			}.ToDictionary(_ => _.Name);

			SplitFieldsByName = new Dictionary<string, IList<IField<Card>>>
			{
				[nameof(Card.Number)] = new List<IField<Card>>
				{
					Builder.Get(nameof(Card.Number) + nameof(Card.SortableNumber.Number), c => c.SortableNumber.Number),
					Builder.Get(nameof(Card.Number) + nameof(Card.SortableNumber.Letter), c => c.SortableNumber.Letter),
				}
			};
		}

		public UiModel Ui { get; set; }

		public List<string> ChartFields { get; } = new List<string>
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

			nameof(Card.Price),
			nameof(Card.DeckCount),
			nameof(Card.DeckTotal),
			nameof(Card.CollectionCount),
			nameof(Card.CollectionTotal),

			nameof(Card.Color),
			nameof(Card.IsSearchResult),
			nameof(Card.HasImage)
		};
	}
}
