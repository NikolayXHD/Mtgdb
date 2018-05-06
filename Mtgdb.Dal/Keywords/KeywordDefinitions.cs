using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	public static class KeywordDefinitions
	{
		private static string hasCost(string name, string pattern = null, string customPattern = null)
		{
			var costKeywordPattern =
				$"({pattern ?? name}{positiveLookahead(@" ?[\{—]")}|{positiveLookbehind(KeywordIntroducers)}{customPattern ?? pattern ?? name}|{customPattern ?? pattern ?? name}{positiveLookahead(KeywordOutroducers)})";
			return define(name, costKeywordPattern);
		}

		private static string hasCount(string name, string customPattern = null)
		{
			customPattern = customPattern ?? name;

			var countKeywordDefinition = $"({name}{positiveLookahead(ThenCount)}|{positiveLookbehind(KeywordIntroducers)}{customPattern ?? name})";
			return define(name, countKeywordDefinition);
		}

		private static string define(
			string name,
			string custom = null,
			string unlessBefore = null,
			string unlessBeforeRaw = null,
			string then = null,
			string unlessAfter = null)
		{
			custom = custom ?? name;

			string result = $"/{pattern(body: custom, unlessBefore: unlessBefore, unlessBeforeRaw: unlessBeforeRaw, then: then, unlessAfter: unlessAfter)}/ {name}";

			return result;
		}

		private static string pattern(
			string body = null,
			string unlessBefore = null,
			string unlessBeforeRaw = null,
			string then = null,
			string unlessAfter = null)
		{
			var result = new StringBuilder();

			result.Append("\\b");
			if (unlessAfter != null)
				result.Append(negativeLookbehind("\\b" + unlessAfter + " "));

			result
				.Append(body)
				.Append(@"\b");

			if (unlessBefore != null)
				result.Append(negativeLookahead(' ' + unlessBefore + "\\b"));

			if (unlessBeforeRaw != null)
				result.Append(negativeLookahead(unlessBeforeRaw));

			if (then != null)
				result.Append(positiveLookahead(' ' + then + "\\b"));

			return result.ToString();
		}

		private static string interval(string open, string close, params string[] interrupt)
		{
			string separator = @"[\s\p{P}]+";
			string word = @"[^\s\p{P}]+";

			return $@"{open}(?(?={separator}({string.Join("|", interrupt.Append(open).Append(close))}))(?!)|{separator}{word})*{separator}{close}";
		}

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
			define("Attack if able", interval(@"(?<!\bcan't )attacks?( or blocks?)?\b", "if able", @"block(s|ed)?\b")),
			hasCount("Awaken"),
			define("Block if able",
				"(" +
				"must be blocked" +
				"|" +
				interval(@"able to block\b", "do (it|so)") +
				"|" +
				interval(@"(?<!\bcan't )(attacks? or )?block(s|ed)?\b", "if able", @"attacks?\b") +
				")"),
			define("Can't be blocked", unlessAfter: "this spell works on creatures that"),
			"Can't be countered",
			"Can't be regenerated",
			"Can't block",
			hasCost("Cohort"),
			define("Copy", "cop(y|ies)"),
			define("Counter",
				custom: @"(?<!\bcan't be )countered|counters?(?= (it|target|phantasmagorian|temporal extortion|brain gorgers|[^\.]*\b(spells?|abilit(y|ies)))\b)"),
			"Deathtouch",
			define("Defender", unlessBefore: "(en\\-vec|of the order)", unlessAfter: "(shu|sworn)"),
			"Delirium",
			define("Discard",
				custom: "discards?",
				unlessBefore: @"(this card\, discard )?it into exile\. when you do\, cast it for its madness"),
			define("Doesn't untap", "(doesn|can|don)'t untap"),
			"Double Strike",
			define("Draw", "draws?", unlessBefore: "step", then: @"[^\.]*\bcards?"),
			"Enchant",
			hasCost("Equip"),
			define("Exile", "exiles?", unlessBefore: "into darkness", unlessAfter: "tel\\-jilad"),
			define("First Strike", unlessAfter: "deals combat damage before creatures without"),
			define("Flash", unlessBefore: "(conscription|foliage|of Insight)", unlessAfter: "aether"),
			define("Flying", unlessAfter: "(can block|except by) creatures with"),
			define("Gain control", "gains? control"),
			"Haste",
			"Hexproof",
			define("Indestructible", unlessBefore: "can't be destroyed by damage"),
			"Ingest",
			define("Intimidate", unlessBefore: "can't be blocked except"),
			"Lifelink",
			hasCost("Madness"),
			"Menace",
			"Prowess",
			define("Rally", then: "— ?"),
			define("Reach", unlessBefore: "of Branches", unlessAfter: "(except by creatures with flying or|geier|myojin of night's)"),
			"Regenerate",
			hasCount("Renown", customPattern: pattern(body: "renowned", unlessBefore: "weaver", unlessAfter: "if it isn't")),
			"Scry",
			"Shroud",
			define("Skulk", unlessBeforeRaw: @"\bpit-"),
			hasCost("Surge"),
			"Trample",
			define("Transform", "transform(s|ed)?"),
			define("Undying", unlessBefore: "(beast|rage|flames|partisan)"),
			"Vigilance",
			null,
			hasCount("Absorb"),
			define("Affinity", then: "for"),
			hasCount("Afflict"),
			"Aftermath",
			hasCount("Amplify"),
			"Ascend",
			hasCost("Aura Swap"),
			define("Banding", unlessAfter: "any creatures with"),
			"Battle Cry",
			hasCost("Bestow"),
			define("Bloodrush", then: "—"),
			hasCount("Bloodthirst"),
			hasCount("Bushido"),
			"Buyback",
			define("Cascade", unlessAfter: "skyline"),
			define("Champion", then: "an?"),
			"Changeling",
			"Cipher",
			"Conspire",
			"Convoke",
			define("Crew", $@"\b(crews?(?={ThenCount}|( a)? vehicles?)\b)\b"),
			hasCost("Cumulative Upkeep"),
			hasCost("Cycling", pattern: @"(\bbasic )?\w*cycling"),
			hasCost("Dash"),
			"Delve",
			"Dethrone",
			"Devoid",
			hasCount("Devour", customPattern: "devoured"),
			hasCount("Dredge"),
			hasCost("Echo"),
			hasCost("Embalm"),
			hasCost("Emerge"),
			hasCost("Entwine"),
			define("Epic", unlessAfter: "copy this spell except for its"),
			hasCost("Escalate"),
			hasCost("Eternalize"),
			hasCost("Evoke"),
			"Evolve",
			define("Exalted", unlessBefore: "(dragon|angel)", unlessAfter: "instances? of"),
			"Exploit",
			"Extort",
			hasCount("Fabricate"),
			hasCount("Fading"),
			define("Fear", unlessAfter: "Shinen of"),
			define("Flanking", unlessBefore: "troops", unlessAfter: "whenever a creature without"),
			define("Flashback", unlessBefore: "cost"),
			hasCost("Forecast"),
			define("Fortify", unlessBefore: "only as a sorcery"),
			hasCount("Frenzy"),
			define("Fuse", unlessBefore: "counters?"),
			define("Goad", "goad(s|ed)?"),
			hasCount("Graft"),
			"Gravestorm",
			"Haunt",
			"Hidden Agenda",
			"Hideaway",
			define("Horsemanship", unlessAfter: "can't be blocked except by creatures with"),
			"Improvise",
			define("Infect", unlessBefore: "deal damage to creatures in the form", unlessAfter: "damage dealt by sources without"),
			hasCost("Kicker", pattern: "(multi)?kick(er|ed|s)"),
			define("Landwalk", "((snow(\\-covered)?|(non)?basic|legendary) )?(land|denim|desert|plains|forest|swamp|mountain|island)walk"),
			"Plainswalk",
			"Forestwalk",
			"Swampwalk",
			"Mountainwalk",
			"Islandwalk",
			define("Level Up", "level (up|counter)", unlessBefore: "only as a sorcery"),
			"Living Weapon",
			define("Melee", unlessAfter: "cast"),
			hasCost("Miracle"),
			hasCount("Modular"),
			hasCost("Morph", pattern: "(mega)?morph"),
			define("Myriad", unlessBefore: "landscape"),
			hasCost("Ninjutsu"),
			define("Offering", unlessAfter: "(volcanic|yawgmoth's vile|death pit|burnt)"),
			hasCost("Outlast"),
			hasCost("Overload"),
			define("Partner", unlessAfter: "if both have"),
			"Persist",
			"Phasing",
			hasCount("Poisonous"),
			define("Protection", unlessAfter: "(circle of|teferi's)"),
			"Provoke",
			hasCost("Prowl"),
			hasCount("Rampage"),
			"Rebound",
			hasCost("Recover"),
			hasCount("Reinforce"),
			define("Replicate", unlessBefore: "cost"),
			"Retrace",
			hasCount("Ripple"),
			define("Scavenge", unlessBefore: "(only as a sorcery|cost)"),
			define("Shadow",
				unlessBefore: "guildmage",
				unlessAfter: "(can block or be blocked by only creatures with|nether|shifting|dragon|perilous|death's|elves of deep)"),
			"Soulbond",
			hasCount("Soulshift"),
			hasCost("Splice onto Arcane", customPattern: "splice(d|s) onto"),
			"Split Second",
			define("Storm",
				unlessBefore: "(seeker|crow|world|elemental|spirit|sculptor|entity|shaman|fleet|the vault)",
				unlessAfter: "(aether|cinder|comet|lava|hail|needle|wing|tropical|arrow|meteor|possibility|lightning|ion|captain lannery|primal|eye of the|yamabushi's)"),
			"Sunburst",
			define("Suspend", "suspend(s|ed)?"),
			"Totem Armor",
			hasCost("Transfigure"),
			hasCost("Transmute"),
			hasCount("Tribute"),
			"Undaunted",
			hasCost("Unearth", pattern: "unearth(s|ed)?"),
			"Unleash",
			define("Vanishing", unlessBefore: "touch"),
			"Wither",

			define("Activate", "activat(e(s|d)?|i(ng|on))"),
			define("Attach", "attach(es|ed)?"),
			define("Unattach", "unattach(es|ed)?"),
			define("Cast", "cast(s|ing)?"),
			define("Create", "create(s|d)?"),
			define("Destroy", "destroy(s|ed)?"),
			define("Exchange", "exchange(s|d)?"),
			define("Fight", "fight(s|ed)?"),
			define("Play", "play(s|ed)?"),
			define("Reveal", "reveal(s|ed)?"),
			define("Sacrifice", "sacrifice(s|d)?"),
			define("Search", "search(es|ed)?"),
			define("Shuffle", "shuffle(s|d)?"),
			define("Tap", "tap(s|ped)?"),
			define("Untap", "untap(s|ped)?"),
			define("Bury", "bur(y|ies|ied)", unlessBefore: "ruin"),
			define("Ante", "ante(s|d)?")
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

		private const string KeywordIntroducers = @"\b(player|was|were|is|are|it's|they're|ha(s|d|ve)|gain(s|ed)?|activate(s|d)?|with) ";
		private const string KeywordOutroducers = @" (abilit(y|ies)|costs?|(a|any|this|these|that|those)?(spells?|permanents?))\b";
		private const string ThenCount = @"( (\d+|x)| ?— ?sunburst)\b";

		private static readonly int _keywordsIndex = PropertyNames.IndexOf(nameof(Keywords));
		public static Dictionary<string, Regex> KeywordPatternsByValue => PatternsByDisplayText[_keywordsIndex];
	}
}