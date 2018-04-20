using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	public static class KeywordDefinitions
	{
		private static Func<string, Regex>[] PatternFactories { get; } =
		{
			KeywordRegexUtil.CreateContainsRegex,
			KeywordRegexUtil.CreateContainsRegex,
			KeywordRegexUtil.CreateEqualsRegex,
			KeywordRegexUtil.CreateContainsRegex,
			KeywordRegexUtil.CreateEqualsRegex,
			KeywordRegexUtil.CreateContainsRegex,
			KeywordRegexUtil.CreateContainsRegex
		};

		public static Func<Card, string>[] Getters { get; } =
		{
			c => c.ManaCost,
			c => c.TypeEn,
			c => c.Rarity,
			c => c.TextEn,
			c => c.Cmc.ToString(Str.Culture),
			c => c.TextEn,
			c => c.GeneratedMana
		};

		public static readonly IList<string> PropertyNamesDisplay = new[]
		{
			nameof(Card.ManaCost),
			nameof(Card.Type),
			nameof(Card.Rarity),
			nameof(Card.Text),
			nameof(Card.Cmc),
			nameof(Card.Text),
			nameof(Card.Text)
		};

		public static readonly string[] ManaCost =
		{
			"{W}",
			"{U}",
			"{B}",
			"{R}",
			"{G}",
			"{C}",
			"{W/P}",
			"{U/P}",
			"{B/P}",
			"{R/P}",
			"{G/P}",
			"{2/W}",
			"{2/U}",
			"{2/B}",
			"{2/R}",
			"{2/G}",
			"{W/U}",
			"{W/B}",
			"{R/W}",
			"{G/W}",
			"{U/B}",
			"{U/R}",
			"{G/U}",
			"{B/R}",
			"{B/G}",
			"{R/G}",
			"{X}",
			null
		};

		public static readonly string[] ManaAbility =
		{
			"{W}",
			"{U}",
			"{B}",
			"{R}",
			"{G}",
			"{C}",
			"{S}",
			"{E}",
			"{T}",
			"{Q}",
			"{X}",
			null
		};

		public static readonly string[] GeneratedMana =
		{
			"{W}",
			"{U}",
			"{B}",
			"{R}",
			"{G}",
			"{C}",
			"{any}",
			"{S}",
			"{E}",
			null
		};

		public static readonly string[] Cmc =
		{
			"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			null
		};

		public static readonly string[] Type =
		{
			"Creature",
			"Instant",
			"Sorcery",
			"Planeswalker",
			"Enchantment",
			"Artifact",
			"Land",
			null
		};

		public static readonly string[] Keywords =
		{
			"Annihilator",
			@"/\battacks? each (combat|turn) if able\b/ Attack each turn",
			"Awaken",
			"Can't be blocked",
			"Can't block",
			"Cohort",
			"Copy",
			"Counter target",
			"Deathtouch",
			"Defender",
			"Delirium",
			"Discard a card",
			"Doesn't untap",
			"Double Strike",
			"Draw a card",
			"Enchant",
			"Equip",
			"Exile",
			"First Strike",
			"Flash",
			"Flying",
			"Gain control",
			"Haste",
			"Hexproof",
			"Indestructible",
			"Ingest",
			"Intimidate",
			"Lifelink",
			"Madness",
			"Menace",
			"Prowess",
			"Rally",
			"Reach",
			"Regenerate",
			"Renown",
			"Scry",
			"Shroud",
			"Skulk",
			"Surge",
			"Trample",
			"Transform",
			"Undying",
			"Vigilance",
			null,
			"Absorb",
			"Affinity",
			"Afflict",
			"Aftermath",
			"Amplify",
			"Ascend",
			"Aura Swap",
			"Banding",
			"Battle Cry",
			"Bestow",
			"Bloodthirst",
			"Bushido",
			"Buyback",
			"Cascade",
			@"/\bchampion an?\b/ Champion",
			"Changeling",
			"Cipher",
			"Conspire",
			"Convoke",
			@"/\bcrew \d+\b/ Crew",
			"Cumulative Upkeep",
			"Cycling",
			"Dash",
			"Delve",
			"Dethrone",
			"Devoid",
			"Devour",
			"Dredge",
			"Echo",
			"Embalm",
			"Emerge",
			"Entwine",
			"Epic",
			"Escalate",
			"Eternalize",
			"Evoke",
			"Evolve",
			"Exalted",
			"Exploit",
			"Extort",
			"Fabricate",
			@"/\bfading \d+\b/ Fading",
			"Fear",
			"Flanking",
			"Flashback",
			"Forecast",
			"Fortify",
			"Frenzy",
			@"/\bfuse (?!counter)/ Fuse",
			@"/\bgraft \d+\b/ Graft",
			"Gravestorm",
			"Haunt",
			"Hidden Agenda",
			"Hideaway",
			"Horsemanship",
			"Improvise",
			"Infect",
			"Kicker",
			@"/\b(land|denim|plains|forest|swamp|mountain|island)walk\b/ Landwalk",
			"Plainswalk",
			"Forestwalk",
			"Swampwalk",
			"Mountainwalk",
			"Islandwalk",
			"Level Up",
			"Living Weapon",
			"Melee",
			"Miracle",
			"Modular",
			"Morph",
			"Myriad",
			"Ninjutsu",
			"Offering",
			"Outlast",
			"Overload",
			"Partner",
			"Persist",
			"Phasing",
			"Poisonous",
			"Protection",
			"Provoke",
			"Prowl",
			"Rampage",
			"Rebound",
			"Recover",
			"Reinforce",
			"Replicate",
			"Retrace",
			"Ripple",
			"Scavenge",
			"Shadow",
			"Soulbond",
			"Soulshift",
			"Splice",
			"Split Second",
			"Storm",
			"Sunburst",
			"Suspend",
			"Totem Armor",
			"Transfigure",
			"Transmute",
			"Tribute",
			"Undaunted",
			"Unearth",
			"Unleash",
			"Vanishing",
			"Wither",

			"Activate",
			"Attach",
			"Cast",
			"Create",
			"Destroy",
			"Exchange",
			"Fight",
			"Play",
			"Reveal",
			"Sacrifice",
			"Search",
			"Shuffle",
			"Tap",
			"Untap",
			"Bury",
			"Ante"
		};

		internal static readonly Dictionary<string, string> HarmfulAbilityExplanations =
			new Dictionary<string, string>
			{
				["can't be blocked, targeted, or dealt damage by"] = "can't be bl*cked, targeted, or dealt damage by",
				["can't be blocked, targeted, dealt damage, or enchanted by"] = "can't be bl*cked, targeted, dealt damage, or enchanted by",
				["can't be blocked except by"] = "can't be bl*cked except by",
				["can't be blocked as long as defending player controls"] = "can't be bl*cked as long as defending player controls",
				["can't be blocked by creatures with greater power"] = "can't be bl*cked by creatures with greater power",
				["this spell works on creatures that can't be blocked"] = "this spell works on creatures that can't be bl*cked",
				["except by creatures with flying or reach"] = "except by creatures with fl*ing or re*ch",
				["can block creatures with flying"] = "can block creatures with fl*ing",
				["deals combat damage before creatures without first strike"] = "deals combat damage before creatures without first str*ke"
			};

		public static readonly string[] Rarity =
		{
			"Common",
			"Uncommon",
			"Rare",
			"Mythic rare",
			"Basic land",
			null /*, "marketing", "double faced"*/
		};

		public static readonly IList<IList<string>> Values = new IList<string>[]
		{
			ManaCost,
			Type,
			Rarity,
			Keywords,
			Cmc,
			ManaAbility,
			GeneratedMana
		};

		public static IList<IList<Regex>> Patterns { get; } = Enumerable.Range(0, Values.Count)
			.Select(i => Values[i].Select(PatternFactories[i]).ToList())
			.Cast<IList<Regex>>()
			.ToList();

		public static IList<Dictionary<string, Regex>> PatternsByDisplayText { get; } =
			Enumerable.Range(0, Values.Count)
				.Select(i => Enumerable.Range(0, Patterns[i].Count)
					.Where(j => Values[i][j] != null)
					.ToDictionary(
						j => KeywordRegexUtil.GetKeywordDisplayText(Values[i][j]),
						j => Patterns[i][j],
						Str.Comparer))
				.ToList();

		public static readonly IList<string> PropertyNames = new[]
		{
			nameof(ManaCost),
			nameof(Type),
			nameof(Rarity),
			nameof(Keywords),
			nameof(Cmc),
			nameof(ManaAbility),
			nameof(GeneratedMana)
		};

		public static readonly int KeywordsIndex = PropertyNames.IndexOf(nameof(Keywords));

		public static Dictionary<string, Regex> KeywordPatternsByValue => PatternsByDisplayText[KeywordsIndex];
	}
}