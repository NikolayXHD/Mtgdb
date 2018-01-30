using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	public static class KeywordDefinitions
	{
		private static Func<string, Regex>[] Matchers { get; } = 
		{
			RegexUtil.CreateContainsRegex,
			RegexUtil.CreateContainsRegex,
			RegexUtil.CreateEqualsRegex,
			RegexUtil.CreateContainsRegex,
			RegexUtil.CreateEqualsRegex,
			RegexUtil.CreateContainsRegex,
			RegexUtil.CreateContainsRegex
		};
		
		public static Func<Card, string>[] Getters { get; } =
			new Func<Card, string>[]
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

		public static readonly string[] ManaGenerated =
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

		public static readonly string[] Ability =
		{
			"Annihilator",
			"Attacks each turn",
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
			null
		};

		internal static readonly Dictionary<string, string> AbilityHarmfulExplanations = new Dictionary<string, string>
		{
			{
				"can't be blocked except by creatures with flying or reach",
				"can't be bl*cked except by creatures with fl*ing or re*ch"
			},
			{
				"can block creatures with flying",
				"can block creatures with fl*ing"
			},
			{
				"deals combat damage before creatures without first strike",
				"deals combat damage before creatures without first str*ke"
			}
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
			Ability,
			Cmc,
			ManaAbility,
			ManaGenerated
		};

		public static IList<IList<Regex>> Patterns { get; } = Enumerable.Range(0, Values.Count)
			.Select(i => Values[i].Select(Matchers[i]).ToList())
			.Cast<IList<Regex>>()
			.ToList();

		public static readonly IList<string> PropertyNames = new[]
		{
			nameof(ManaCost),
			nameof(Type),
			nameof(Rarity),
			nameof(Ability),
			nameof(Cmc),
			nameof(ManaAbility),
			nameof(ManaGenerated)
		};
	}
}