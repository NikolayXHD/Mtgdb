using System.Drawing;
using Mtgdb.Gui.Resx;

namespace Mtgdb.Gui
{
	public static class QuickFilterImages
	{
		public static void SetImages(FormMain formMain)
		{
			formMain.FilterAbility.PropertyImages = _ability;
			formMain.FilterCmc.PropertyImages = _cmc;
			formMain.FilterGeneratedMana.PropertyImages = _manaGenerated;
			formMain.FilterManaAbility.PropertyImages = _manaAbility;
			formMain.FilterManaCost.PropertyImages = _manaCost;
			formMain.FilterManager.PropertyImages = _filterManager;
			formMain.FilterRarity.PropertyImages = _rarity;
			formMain.FilterType.PropertyImages = _type;
		}

		private static readonly Bitmap[] _filterManager =
		{
			ResourcesFilter.quick_filters_20,
			ResourcesFilter.search_20,
			ResourcesFilter.legality_20,
			ResourcesFilter.box_20,
			ResourcesFilter.deck
		};

		private static readonly Bitmap[] _manaCost =
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
			ResourcesCost.na
		};

		private static readonly Bitmap[] _manaAbility =
		{
			ResourcesCost.w,
			ResourcesCost.u,
			ResourcesCost.b,
			ResourcesCost.r,
			ResourcesCost.g,
			ResourcesCost.c,
			ResourcesCost.s,
			ResourcesCost.e,
			ResourcesCost.t,
			ResourcesCost.q,
			ResourcesCost.x,
			ResourcesCost.na
		};

		private static readonly Bitmap[] _manaGenerated =
		{
			ResourcesCost.w,
			ResourcesCost.u,
			ResourcesCost.b,
			ResourcesCost.r,
			ResourcesCost.g,
			ResourcesCost.c,
			ResourcesCost.any,
			ResourcesCost.s,
			ResourcesCost.e,
			ResourcesCost.na
		};

		private static readonly Bitmap[] _type =
		{
			ResourcesType.creature,
			ResourcesType.instant,
			ResourcesType.sorcery,
			ResourcesType.planeswalker,
			ResourcesType.enchantment,
			ResourcesType.artifact,
			ResourcesType.land,
			ResourcesCost.na
		};

		private static readonly Bitmap[] _ability =
		{
			ResourcesAbilities.Aftermath,
			ResourcesAbilities.Annihilator,
			ResourcesAbilities.Ascend,
			ResourcesAbilities.Attacks_each_turn,
			ResourcesAbilities.Awake,
			ResourcesAbilities.Bestow,
			ResourcesAbilities.Block_if_able,
			ResourcesAbilities.Cant_attack,
			ResourcesAbilities.Cant_be_blocked,
			ResourcesAbilities.Cant_be_countered,
			ResourcesAbilities.Cant_block,
			ResourcesAbilities.Cascade,
			ResourcesAbilities.Cipher,
			ResourcesAbilities.Cohort,
			ResourcesAbilities.Copy,
			ResourcesAbilities.Counter,
			ResourcesAbilities.Create,
			ResourcesAbilities.Crew,
			ResourcesAbilities.Cycling,
			ResourcesAbilities.Deal_damage,
			ResourcesAbilities.Deathtouch,
			ResourcesAbilities.Defender,
			ResourcesAbilities.Delirium,
			ResourcesAbilities.Destroy,
			ResourcesAbilities.Discard,
			ResourcesAbilities.Doesnt_untap,
			ResourcesAbilities.Double_Strike,
			ResourcesAbilities.Draw_a_card,
			ResourcesAbilities.Enchant,
			ResourcesAbilities.Equip,
			ResourcesAbilities.Exalted,
			ResourcesAbilities.Exile,
			ResourcesAbilities.Extra_turn,
			ResourcesAbilities.Fear,
			ResourcesAbilities.Fight,
			ResourcesAbilities.First_strike,
			ResourcesAbilities.Flash,
			ResourcesAbilities.Flashback,
			ResourcesAbilities.Flying,
			ResourcesAbilities.Fuse,
			ResourcesAbilities.Gain_control,
			ResourcesAbilities.Haste,
			ResourcesAbilities.Hexproof,
			ResourcesAbilities.Indestructible,
			ResourcesAbilities.Infect,
			ResourcesAbilities.Intimidate,
			ResourcesAbilities.Landwalk,
			ResourcesAbilities.Lifelink,
			ResourcesAbilities.Madness,
			ResourcesAbilities.Menace,
			ResourcesAbilities.Morph,
			ResourcesAbilities.Persist,
			ResourcesAbilities.Protection,
			ResourcesAbilities.Prowess,
			ResourcesAbilities.Rally,
			ResourcesAbilities.Reach,
			ResourcesAbilities.Rebound,
			ResourcesAbilities.Regenerate,
			ResourcesAbilities.Renown,
			ResourcesAbilities.Sacrifice,
			ResourcesAbilities.Scry,
			ResourcesAbilities.Search,
			ResourcesAbilities.Shadow,
			ResourcesAbilities.Shroud,
			ResourcesAbilities.Skulk,
			ResourcesAbilities.Soulbound,
			ResourcesAbilities.Split_second,
			ResourcesAbilities.Suspend,
			ResourcesAbilities.Trample,
			ResourcesAbilities.Transform,
			ResourcesAbilities.Undying,
			ResourcesAbilities.Unearth,
			ResourcesAbilities.Vigilance,
			ResourcesAbilities.Wither
		};

		private static readonly Bitmap[] _cmc =
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

		private static readonly Bitmap[] _rarity =
		{
			ResourcesRarity.common2,
			ResourcesRarity.uncommon2,
			ResourcesRarity.rare2,
			ResourcesRarity.mythic2,
			ResourcesType.land,
			ResourcesCost.na
		};
	}
}