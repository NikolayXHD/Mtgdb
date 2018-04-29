using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	public static class KeywordDefinitions
	{
		private static string positiveLookbehind([JetBrains.Annotations.RegexPattern] string pattern)
		{
			return "(?<=" + pattern + ")";
		}

		private static string negativeLookbehind([JetBrains.Annotations.RegexPattern] string pattern)
		{
			return "(?<!" + pattern + ")";
		}

		private static string positiveLookahead([JetBrains.Annotations.RegexPattern] string pattern)
		{
			return "(?=" + pattern + ")";
		}

		private static string negativeLookahead([JetBrains.Annotations.RegexPattern] string pattern)
		{
			return "(?!" + pattern + ")";
		}

		private static string pattern(
			string name,
			string pattern = null,
			string then = null,
			string unlessBefore = null,
			string unlessBeforeRaw = null,
			string unlessAfter = null)
		{
			pattern = pattern ?? name;

			var result = new StringBuilder();

			result.Append(@"/\b");

			if (unlessAfter != null)
				result.Append(negativeLookbehind("\\b" + unlessAfter + " "));

			result
				.Append(pattern)
				.Append(@"\b");

			if (unlessBefore != null)
				result.Append(negativeLookahead(' ' + unlessBefore + "\\b"));

			if (unlessBeforeRaw != null)
				result.Append(negativeLookahead(unlessBeforeRaw));

			if (then != null)
				result.Append(positiveLookahead(' ' + then + "\\b"));

			result.Append("/ ")
				.Append(name);

			return result.ToString();
		}

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
			hasCount("Annihilator"),
			pattern("Attack each turn", "attacks? each (combat|turn)", "if able"),
			hasCount("Awaken"),
			pattern("Can't be blocked", unlessAfter: "this spell works on creatures that"),
			"Can't be countered",
			"Can't block",
			hasCost("Cohort"),
			pattern("Copy", "cop(y|ies)"),
			pattern("Counter",
				@"(?<!\bcan't be )countered|counters?(?= (it|target|phantasmagorian|temporal extortion|brain gorgers|[^\.]*\b(spells?|abilit(y|ies)))\b)"),
			"Deathtouch",
			"Defender",
			"Delirium",
			pattern("Discard",
				"discards?",
				unlessBefore: @"(this card\, discard )?it into exile\. when you do\, cast it for its madness"),
			pattern("Doesn't untap", "(doesn|can|don)'t untap"),
			"Double Strike",
			pattern("Draw", "draws?", @"[\P{P}]*\bcards?", unlessAfter: "when you", unlessBefore: "step"),
			"Enchant",
			hasCost("Equip"),
			pattern("Exile", "exiles?"),
			pattern("First Strike", unlessAfter: "deals combat damage before creatures without"),
			"Flash",
			pattern("Flying", unlessAfter: "(can block|except by) creatures with"),
			pattern("Gain control", "gains? control"),
			"Haste",
			"Hexproof",
			pattern("Indestructible", unlessBefore: "can't be destroyed by damage"),
			"Ingest",
			pattern("Intimidate", unlessBefore: "can't be blocked except"),
			"Lifelink",
			hasCost("Madness"),
			"Menace",
			"Prowess",
			pattern("Rally", then: "—"),
			pattern("Reach", unlessAfter: "except by creatures with flying or"),
			"Regenerate",
			hasCount("Renown"),
			hasCount("Scry"),
			"Shroud",
			pattern("Skulk", unlessBeforeRaw: @"\bpit-"),
			hasCost("Surge"),
			"Trample",
			"Transform",
			pattern("Undying", unlessBefore: "(beast|rage|flames|partisan)"),
			"Vigilance",
			null,
			hasCount("Absorb"),
			pattern("Affinity", then: "for"),
			hasCount("Afflict"),
			"Aftermath",
			hasCount("Amplify"),
			"Ascend",
			hasCost("Aura Swap"),
			pattern("Banding", unlessAfter: "any creatures with"),
			"Battle Cry",
			hasCost("Bestow"),
			pattern("Bloodrush", then: "—"),
			hasCount("Bloodthirst"),
			hasCount("Bushido"),
			hasCost("Buyback"),
			pattern("Cascade", unlessAfter: "skyline"),
			pattern("Champion", then: "an?"),
			"Changeling",
			"Cipher",
			"Conspire",
			"Convoke",
			hasCount("Crew"),
			hasCost("Cumulative Upkeep"),
			hasCost("Cycling", @"\w*cycling"),
			hasCost("Dash"),
			"Delve",
			"Dethrone",
			"Devoid",
			hasCount("Devour"),
			hasCount("Dredge"),
			hasCost("Echo"),
			hasCost("Embalm"),
			hasCost("Emerge"),
			hasCost("Entwine"),
			pattern("Epic", unlessAfter: "copy this spell except for its"),
			hasCost("Escalate"),
			hasCost("Eternalize"),
			hasCost("Evoke"),
			"Evolve",
			pattern("Exalted", unlessAfter: "instances? of", unlessBefore: "dragon"),
			"Exploit",
			"Extort",
			hasCount("Fabricate"),
			hasCount("Fading"),
			"Fear",
			pattern("Flanking", unlessBefore: "troops", unlessAfter: "whenever a creature without"),
			pattern("Flashback", unlessBefore: "cost"),
			hasCost("Forecast"),
			pattern("Fortify", unlessBefore: "only as a sorcery"),
			hasCount("Frenzy"),
			pattern("Fuse", unlessBefore: "counters?"),
			hasCount("Graft"),
			"Gravestorm",
			"Haunt",
			"Hidden Agenda",
			"Hideaway",
			pattern("Horsemanship", unlessAfter: "can't be blocked except by creatures with"),
			"Improvise",
			pattern("Infect", unlessAfter: "damage dealt by sources without", unlessBefore: "deal damage to creatures in the form"),
			hasCost("Kicker"),
			pattern("Landwalk", "(land|denim|desert|plains|forest|swamp|mountain|island)walk"),
			"Plainswalk",
			"Forestwalk",
			"Swampwalk",
			"Mountainwalk",
			"Islandwalk",
			pattern("Level Up", unlessBefore: "only as a sorcery"),
			"Living Weapon",
			pattern("Melee", unlessAfter: "cast"),
			hasCost("Miracle"),
			hasCount("Modular"),
			hasCost("Morph"),
			pattern("Myriad", unlessBefore: "landscape"),
			hasCost("Ninjutsu"),
			pattern("Offering", unlessAfter: "(volcanic|yawgmoth's vile|death pit)"),
			hasCost("Outlast"),
			hasCost("Overload"),
			pattern("Partner", unlessAfter: "if both have"),
			"Persist",
			"Phasing",
			hasCount("Poisonous"),
			pattern("Protection", unlessAfter: "Circle of"),
			"Provoke",
			hasCost("Prowl"),
			hasCount("Rampage"),
			"Rebound",
			hasCost("Recover"),
			hasCount("Reinforce"),
			pattern("Replicate", unlessBefore: "cost"),
			"Retrace",
			hasCount("Ripple"),
			pattern("Scavenge", unlessBefore: "(only as a sorcery|cost)"),
			pattern("Shadow",
				unlessAfter: "(can block or be blocked by only creatures with|nether|shifting|dragon|perilous|death's|elves of deep)",
				unlessBefore: "guildmage"),
			"Soulbond",
			hasCount("Soulshift"),
			hasCost("Splice onto Arcane"),
			"Split Second",
			pattern("Storm",
				unlessAfter: "(aether|cinder|comet|lava|hail|needle|wing|tropical|arrow|meteor|possibility|lightning|ion|captain lannery|primal|eye of the|yamabushi's)",
				unlessBefore: "(seeker|crow|world|elemental|spirit|sculptor|entity|shaman|fleet|the vault)"),
			"Sunburst",
			"Suspend",
			"Totem Armor",
			hasCost("Transfigure"),
			hasCost("Transmute"),
			hasCount("Tribute"),
			"Undaunted",
			hasCost("Unearth"),
			"Unleash",
			pattern("Vanishing", unlessBefore: "touch"),
			"Wither",

			pattern("Activate", "activates?"),
			pattern("Attach", "attach(es)?"),
			pattern("Cast", "casts?"),
			pattern("Create", "creates?"),
			pattern("Destroy", "destroys?"),
			pattern("Exchange", "exchanges?"),
			pattern("Fight", "fights?"),
			pattern("Play", "plays?"),
			pattern("Reveal", "reveals?"),
			pattern("Sacrifice", "sacrifices?"),
			pattern("Search", "search(es)?"),
			pattern("Shuffle", "shuffles?"),
			pattern("Tap", "taps?"),
			pattern("Untap", "untaps?"),
			pattern("Bury", "bur(y|ies)"),
			pattern("Ante", "antes?")
		};

		private static string hasCost(string name, string customPattern = null)
		{
			customPattern = customPattern ?? name;
			var costKeywordPattern = $"({customPattern}{positiveLookahead(@" ?[\{—]")}|{positiveLookbehind("(has|have|gains?|activate|with) ")}{customPattern})";
			return pattern(name, costKeywordPattern);
		}

		private static string hasCount(string name)
		{
			var countKeywordDefinition = $"({name}{positiveLookahead(@" (\d+|x)\b")}|{positiveLookbehind("(has|have|gains?|activate|with) ")}{name})";
			return pattern(name, countKeywordDefinition);
		}

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