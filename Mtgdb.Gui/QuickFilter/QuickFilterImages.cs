using System.Drawing;
using Mtgdb.Gui.Resx;

namespace Mtgdb.Gui
{
	public static class QuickFilterImages
	{
		public static void SetImages(FormMain formMain)
		{
			formMain.FilterAbility.PropertyImages = _keywords;
			formMain.FilterCastKeyword.PropertyImages = _castKeywords;
			formMain.FilterCmc.PropertyImages = _cmc;
			formMain.FilterGeneratedMana.PropertyImages = _manaGenerated;
			formMain.FilterManaAbility.PropertyImages = _manaAbility;
			formMain.FilterManaCost.PropertyImages = _manaCost;
			formMain.FilterManager.PropertyImages = _filterManager;
			formMain.FilterRarity.PropertyImages = _rarity;
			formMain.FilterType.PropertyImages = _type;
			formMain.FilterLayout.PropertyImages = _layout;
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

		private static readonly Bitmap[] _castKeywords =
		{
			ResourcesAbilities.Aftermath,
			ResourcesAbilities.Affinity,
			ResourcesAbilities.Awake,
			ResourcesAbilities.Bestow,
			ResourcesAbilities.Buyback,
			ResourcesAbilities.Cant_be_countered,
			ResourcesAbilities.Cascade,
			ResourcesAbilities.Champion,
			ResourcesAbilities.Cipher,
			ResourcesAbilities.Conspire,
			ResourcesAbilities.Convoke,
			ResourcesAbilities.Cycling,
			ResourcesAbilities.Dash,
			ResourcesAbilities.Delve,
			ResourcesAbilities.Dredge,
			ResourcesAbilities.Embalm,
			ResourcesAbilities.Emerge,
			ResourcesAbilities.Eternalize,
			ResourcesAbilities.Flash,
			ResourcesAbilities.Flashback,
			ResourcesAbilities.Fuse,
			ResourcesAbilities.Improvise,
			ResourcesAbilities.Madness,
			ResourcesAbilities.Miracle,
			ResourcesAbilities.Morph,
			ResourcesAbilities.Rebound,
			ResourcesAbilities.Retrace,
			ResourcesAbilities.Scavenge,
			ResourcesAbilities.Soulbound,
			ResourcesAbilities.Splice_onto_arcane,
			ResourcesAbilities.Split_second,
			ResourcesAbilities.Surge,
			ResourcesAbilities.Suspend,
			ResourcesAbilities.Transmute,
			ResourcesAbilities.Unearth
		};

		private static readonly Bitmap[] _keywords =
		{
			ResourcesAbilities.Annihilator,
			ResourcesAbilities.Ascend,
			ResourcesAbilities.Attacks_each_turn,
			ResourcesAbilities.Block_if_able,
			ResourcesAbilities.Cant_attack,
			ResourcesAbilities.Cant_be_blocked,
			ResourcesAbilities.Cant_be_regenerated,
			ResourcesAbilities.Cant_block,
			ResourcesAbilities.Changeling,
			ResourcesAbilities.Cohort,
			ResourcesAbilities.Copy,
			ResourcesAbilities.Counter,
			ResourcesAbilities.Create_token,
			ResourcesAbilities.Crew,
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
			ResourcesAbilities.Extort,
			ResourcesAbilities.Extra_turn,
			ResourcesAbilities.Fear,
			ResourcesAbilities.Fight,
			ResourcesAbilities.First_strike,
			ResourcesAbilities.Flying,
			ResourcesAbilities.Gain_control,
			ResourcesAbilities.Haste,
			ResourcesAbilities.Hexproof,
			ResourcesAbilities.Indestructible,
			ResourcesAbilities.Infect,
			ResourcesAbilities.Ingest,
			ResourcesAbilities.Intimidate,
			ResourcesAbilities.Landwalk,
			ResourcesAbilities.Lifelink,
			ResourcesAbilities.Menace,
			ResourcesAbilities.Modular,
			ResourcesAbilities.Persist,
			ResourcesAbilities.Phasing,
			ResourcesAbilities.Protection,
			ResourcesAbilities.Prowess,
			ResourcesAbilities.Rally,
			ResourcesAbilities.Reach,
			ResourcesAbilities.Regenerate,
			ResourcesAbilities.Renown,
			ResourcesAbilities.Sacrifice,
			ResourcesAbilities.Scry,
			ResourcesAbilities.Search,
			ResourcesAbilities.Shadow,
			ResourcesAbilities.Shroud,
			ResourcesAbilities.Skulk,
			ResourcesAbilities.Soulshift,
			ResourcesAbilities.Totem_armor,
			ResourcesAbilities.Trample,
			ResourcesAbilities.Undying,
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

		private static readonly Bitmap[] _layout =
		{
			ResourcesAbilities.Normal,
			ResourcesAbilities.Aftermath,
			ResourcesAbilities.Fuse,
			ResourcesAbilities.Meld,
			ResourcesAbilities.Renown,
			ResourcesAbilities.Transform,
			ResourcesAbilities.Cycling,
			ResourcesAbilities.Phenomenon,
			ResourcesAbilities.Plane,
			ResourcesAbilities.Scheme,
			ResourcesAbilities.Vanguard,
			ResourcesAbilities.Create_token,
			ResourcesCost.na
		};
	}
}