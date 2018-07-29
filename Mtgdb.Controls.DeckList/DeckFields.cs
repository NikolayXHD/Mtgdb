using System.Linq;
using Mtgdb.Controls.Statistics;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public class DeckFields : Fields<DeckModel>
	{
		public DeckFields()
		{
			ByName = new IField<DeckModel>[]
			{
				Builder.Get(nameof(DeckModel.Id), d => d.Id),
				Builder.Get(nameof(DeckModel.Name), d => d.Name),
				Builder.Get(nameof(DeckModel.Mana), d => d.Mana),

				Builder.Get(nameof(DeckModel.Legal), d => d.Legal),
				Builder.Get(nameof(DeckModel.Saved), d => d.Saved),

				Builder.Get(nameof(DeckModel.LandCount), d => d.LandCount),
				Builder.Get(nameof(DeckModel.CreatureCount), d => d.CreatureCount),
				Builder.Get(nameof(DeckModel.OtherSpellCount), d => d.OtherSpellCount),

				Builder.Get(nameof(DeckModel.MainCount), d => d.MainCount),
				Builder.Get(nameof(DeckModel.MainCollectedCount), d => d.MainCollectedCount),
				Builder.Get(nameof(DeckModel.MainCollectedCountPercent), d => d.MainCollectedCountPercent),

				Builder.Get(nameof(DeckModel.SideCount), d => d.SideCount),
				Builder.Get(nameof(DeckModel.SideCollectedCount), d => d.SideCollectedCount),
				Builder.Get(nameof(DeckModel.SideCollectedCountPercent), d => d.SideCollectedCountPercent),

				Builder.Get(nameof(DeckModel.LandPrice), d => d.LandPrice),
				Builder.Get(nameof(DeckModel.CreaturePrice), d => d.CreaturePrice),
				Builder.Get(nameof(DeckModel.OtherSpellPrice), d => d.OtherSpellPrice),

				Builder.Get(nameof(DeckModel.MainPrice), d => d.MainPrice),
				Builder.Get(nameof(DeckModel.MainCollectedPrice), d => d.MainCollectedPrice),
				Builder.Get(nameof(DeckModel.MainCollectedPricePercent), d => d.MainCollectedPricePercent),

				Builder.Get(nameof(DeckModel.SidePrice), d => d.SidePrice),
				Builder.Get(nameof(DeckModel.SideCollectedPrice), d => d.SideCollectedPrice),
				Builder.Get(nameof(DeckModel.SideCollectedPricePercent), d => d.SideCollectedPricePercent),

				Builder.Get(nameof(DeckModel.LandUnknownPriceCount), d => d.LandUnknownPriceCount),
				Builder.Get(nameof(DeckModel.CreatureUnknownPriceCount), d => d.CreatureUnknownPriceCount),
				Builder.Get(nameof(DeckModel.OtherSpellUnknownPriceCount), d => d.OtherSpellUnknownPriceCount),

				Builder.Get(nameof(DeckModel.MainUnknownPriceCount), d => d.MainUnknownPriceCount),
				Builder.Get(nameof(DeckModel.MainCollectedUnknownPriceCount), d => d.MainCollectedUnknownPriceCount),
				Builder.Get(nameof(DeckModel.MainCollectedUnknownPricePercent), d => d.MainCollectedUnknownPricePercent),

				Builder.Get(nameof(DeckModel.SideUnknownPriceCount), d => d.SideUnknownPriceCount),
				Builder.Get(nameof(DeckModel.SideCollectedUnknownPriceCount), d => d.SideCollectedUnknownPriceCount),
				Builder.Get(nameof(DeckModel.SideCollectedUnknownPricePercent), d => d.SideCollectedUnknownPricePercent)
			}.ToDictionary(_ => _.Name);
		}
	}
}