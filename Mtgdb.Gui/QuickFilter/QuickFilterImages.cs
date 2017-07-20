using System.Drawing;
using Mtgdb.Gui.Resx;

namespace Mtgdb.Gui
{
	public static class QuickFilterImages
	{
		public static void SetImages(FormMain formMain)
		{
			formMain.FilterAbility.PropertyImages = Ability;
			formMain.FilterCmc.PropertyImages = Cmc;
			formMain.FilterGeneratedMana.PropertyImages = ManaAbility;
			formMain.FilterManaAbility.PropertyImages = ManaAbility;
			formMain.FilterManaCost.PropertyImages = ManaCost;
			formMain.FilterManager.PropertyImages = FilterManager;
			formMain.FilterRarity.PropertyImages = Rarity;
			formMain.FilterType.PropertyImages = Type;
		}

		private static readonly Image[] FilterManager =
		{
			ResourcesFilter.Quick_filters,
			ResourcesFilter.Search_icon,
			Resources.Legality20,
			Resources.Box20,
			ResourcesAbilities.draw_a_card
		};

		private static readonly Image[] ManaCost =
		{
			ResourcesCost.w,
			ResourcesCost.u,
			ResourcesCost.b,
			ResourcesCost.r,
			ResourcesCost.g,
			ResourcesCost.c,
			ResourcesCost.wp,
			ResourcesCost.up,
			ResourcesCost.bp,
			ResourcesCost.rp,
			ResourcesCost.gp,
			ResourcesCost._2w,
			ResourcesCost._2u,
			ResourcesCost._2b,
			ResourcesCost._2r,
			ResourcesCost._2g,
			ResourcesCost.wu,
			ResourcesCost.wb,
			ResourcesCost.rw,
			ResourcesCost.gw,
			ResourcesCost.ub,
			ResourcesCost.ur,
			ResourcesCost.gu,
			ResourcesCost.br,
			ResourcesCost.bg,
			ResourcesCost.rg,
			ResourcesCost.x,
			Resources.na
		};

		private static readonly Image[] ManaAbility =
		{
			ResourcesCost.w,
			ResourcesCost.u,
			ResourcesCost.b,
			ResourcesCost.r,
			ResourcesCost.g,
			ResourcesCost.c,
			ResourcesCost.e,
			ResourcesCost.t,
			ResourcesCost.q,
			ResourcesCost.x,
			Resources.na
		};

		private static readonly Image[] Type =
		{
			ResourcesType.creature,
			ResourcesType.instant,
			ResourcesType.sorcery,
			ResourcesType.planeswalker,
			ResourcesType.enchantment,
			ResourcesType.artifact,
			ResourcesType.land,
			Resources.na
		};

		private static readonly Image[] Ability =
		{
			ResourcesAbilities.Annihilator,
			ResourcesAbilities.Attacks_each_turn,
			ResourcesAbilities.awake,
			ResourcesAbilities.Cant_be_blocked,
			ResourcesAbilities.Cant_block,
			ResourcesAbilities.Cohort,
			ResourcesAbilities.Copy,
			ResourcesAbilities.Counter,
			ResourcesAbilities.Deathtouch,
			ResourcesAbilities.Defender,
			ResourcesAbilities.Delirium,
			ResourcesAbilities.discard,
			ResourcesAbilities.Doesnt_untap,
			ResourcesAbilities.Double_Strike,
			ResourcesAbilities.draw_a_card,
			ResourcesAbilities.Enchant,
			ResourcesAbilities.Equip,
			ResourcesAbilities.Exile,
			ResourcesAbilities.First_strike,
			ResourcesAbilities.Flash,
			ResourcesAbilities.Flying,
			ResourcesAbilities.Gain_control,
			ResourcesAbilities.Haste,
			ResourcesAbilities.Hexproof,
			ResourcesAbilities.Indestructible,
			ResourcesAbilities.Ingest,
			ResourcesAbilities.Intimidate,
			ResourcesAbilities.Lifelink,
			ResourcesAbilities.madness,
			ResourcesAbilities.Menace,
			ResourcesAbilities.Prowess,
			ResourcesAbilities.Rally,
			ResourcesAbilities.Reach,
			ResourcesAbilities.Regenerate,
			ResourcesAbilities.Renown,
			ResourcesAbilities.Scry,
			ResourcesAbilities.Shroud,
			ResourcesAbilities.Skulk,
			ResourcesAbilities.surge,
			ResourcesAbilities.Trample,
			ResourcesAbilities.Transform,
			ResourcesAbilities.Undying,
			ResourcesAbilities.Vigilance
		};

		private static readonly Image[] Cmc =
		{
			ResourcesCost.cmc0,
			ResourcesCost.cmc1,
			ResourcesCost.cmc2,
			ResourcesCost.cmc3,
			ResourcesCost.cmc4,
			ResourcesCost.cmc5,
			ResourcesCost.cmc6,
			ResourcesCost.cmc7,
		};

		private static readonly Image[] Rarity =
		{
			ResourcesRarity.common2,
			ResourcesRarity.uncommon2,
			ResourcesRarity.rare2,
			ResourcesRarity.mythic2,
			ResourcesType.land,
			Resources.na
		};
	}
}