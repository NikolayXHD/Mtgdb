using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	public static class KeywordDefinitions
	{
		private static string cost(string name, string pattern = null, string customPattern = null)
		{
			var costKeywordPattern =
				$"({pattern ?? name}{positiveLookahead(@" ?[\{—]")}|{positiveLookbehind(KeywordIntroducers)}{customPattern ?? pattern ?? name}|{customPattern ?? pattern ?? name}{positiveLookahead(KeywordOutroducers)})";
			return custom(name, costKeywordPattern);
		}

		private static string count(string name, string customPattern = null)
		{
			customPattern = customPattern ?? name;

			var countKeywordDefinition = $"({name}{positiveLookahead(ThenCount)}|{positiveLookbehind(KeywordIntroducers)}{customPattern ?? name})";
			return custom(name, countKeywordDefinition);
		}

		private static string custom(
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

		private static string cant(params string[] patterns) =>
			sequence(Sequence.From("can't").Concat(patterns), without: Array.From("can", "unless"));

		private static string or(params string[] patterns)
		{
			return "(" + string.Join("|", patterns) + ")";
		}

		private static string sequence(params string[] patterns) =>
			sequence(patterns, Array.Empty<string>());

		private static string sequence(IEnumerable<string> patterns, string[] without)
		{
			string separator = @"[\s\p{P}-[\.]]+";
			string word = @"[^\s\p{P}]+";

			var patternsArr = patterns.ToArray();

			string notFiller = string.Join("|", patternsArr.Concat(without));
			string fillers = $"(?(?={separator}({notFiller}))(?!)|{separator}{word})*";

			var builder = new StringBuilder();

			builder.Append(patternsArr[0]);

			for (int i = 1; i < patternsArr.Length; i++)
				builder.Append(fillers).Append(separator).Append(patternsArr[i]);

			var result = builder.ToString();
			return result;
		}

		private static string positiveLookbehind(string pattern)
		{
			return "(?<=" + pattern + ")";
		}

		private static string negativeLookbehind(string pattern)
		{
			return "(?<!" + pattern + ")";
		}

		private static string positiveLookahead(string pattern)
		{
			return "(?=" + pattern + ")";
		}

		private static string negativeLookahead(string pattern)
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
			count("Annihilator"),

			custom("Attack if able", sequence(
				Sequence.From(@"(?<!\bcan't )attacks?( or blocks?)?\b", "if able"),
				without: Array.From(@"block(s|ed)?\b"))),

			count("Awaken"),

			custom("Block if able",
				"(" +
				"must be blocked" +
				"|" +
				sequence("able to\\b", "block\\b", "do (it|so)") +
				"|" +
				sequence(
					Sequence.From(@"(?<!\bcan't )(attacks? or )?block(s|ed)?\b", "if able"),
					without: Array.From(@"attacks?\b")) +
				")"),

			custom("Can't attack", or(cant("attack"), cant("be", "attacked"))),
			custom("Can't be blocked", cant("be", "blocked"), unlessAfter: "this spell works on creatures that"),
			custom("Can't be countered", cant("be", "countered")),
			custom("Can't be regenerated", cant("be", "regenerated")),
			custom("Can't block", cant("block")),

			cost("Cohort"),

			custom("Copy", "cop(y|ies)"),

			custom("Counter", custom: 
				@"(?<!\bcan't be )countered|counters?(?! (from|on|put|aren't)\b)(?= (it|target|phantasmagorian|temporal extortion|brain gorgers|[^\.]*\b(spells?|abilit(y|ies)))\b)"),

			custom("Deathtouch", "(un)?deathtouch"),

			custom("Defender", unlessBefore: "(en\\-vec|of the order)", unlessAfter: "(shu|sworn)"),
			
			"Delirium",
			
			custom("Discard",
				custom: "discard(s|ed|ing)?",
				unlessBefore: @"it into exile"),
			
			custom("Doesn't untap", "(doesn|can|don)'t untap"),
			
			"Double Strike",
			
			custom("Draw", "draws?", unlessBefore: "step", then: @"[^\.]*\bcards?"),
			
			"Enchant",
			
			cost("Equip"),
			custom("Exile", "exiles?", unlessBefore: "into darkness", unlessAfter: "tel\\-jilad"),
			custom("First Strike", unlessAfter: "deals combat damage before creatures without"),
			custom("Flash", unlessBefore: "(conscription|foliage|of Insight)", unlessAfter: "aether"),
			custom("Flying", unlessAfter: "(can block|except by) creatures with"),
			custom("Gain control", "gains? control"),
			"Haste",
			"Hexproof",
			custom("Indestructible", unlessBefore: "can't be destroyed by damage"),
			"Ingest",
			custom("Intimidate", unlessBefore: "can't be blocked except"),
			"Lifelink",
			cost("Madness"),
			"Menace",
			"Prowess",
			custom("Rally", then: "— ?"),
			custom("Reach", unlessBefore: "of Branches", unlessAfter: "(except by creatures with flying or|geier|myojin of night's)"),
			"Regenerate",
			count("Renown", customPattern: pattern(body: "renowned", unlessBefore: "weaver", unlessAfter: "if it isn't")),
			"Scry",
			"Shroud",
			custom("Skulk", unlessBeforeRaw: @"\bpit-"),
			cost("Surge"),
			"Trample",
			custom("Transform", "transform(s|ed)?"),
			custom("Undying", unlessBefore: "(beast|rage|flames|partisan)"),
			"Vigilance",
			null,
			count("Absorb"),
			custom("Affinity", then: "for"),
			count("Afflict"),
			"Aftermath",
			count("Amplify"),
			"Ascend",
			cost("Aura Swap"),
			custom("Banding", unlessAfter: "any creatures with"),
			"Battle Cry",
			cost("Bestow"),
			custom("Bloodrush", then: "—"),
			count("Bloodthirst"),
			count("Bushido"),
			"Buyback",
			custom("Cascade", unlessAfter: "skyline"),
			custom("Champion", then: "an?"),
			"Changeling",
			"Cipher",
			"Conspire",
			"Convoke",
			custom("Crew", $@"\b(crews?(?={ThenCount}|( a)? vehicles?)\b)\b"),
			cost("Cumulative Upkeep"),
			cost("Cycling", pattern: @"(basic )?\w*cycling", customPattern: "\\w*cycl(ing|e(s|d)?)"),
			cost("Dash"),
			"Delve",
			"Dethrone",
			"Devoid",
			count("Devour", customPattern: "devoured"),
			count("Dredge"),
			cost("Echo"),
			cost("Embalm"),
			cost("Emerge"),
			cost("Entwine"),
			custom("Epic", unlessAfter: "copy this spell except for its"),
			cost("Escalate"),
			cost("Eternalize"),
			cost("Evoke"),
			"Evolve",
			custom("Exalted", unlessBefore: "(dragon|angel)", unlessAfter: "instances? of"),
			"Exploit",
			"Extort",
			count("Fabricate"),
			count("Fading"),
			custom("Fear", unlessAfter: "Shinen of"),
			custom("Flanking", unlessBefore: "troops", unlessAfter: "whenever a creature without"),
			custom("Flashback", unlessBefore: "cost"),
			cost("Forecast"),
			custom("Fortify", unlessBefore: "only as a sorcery"),
			count("Frenzy"),
			custom("Fuse", unlessBefore: "counters?"),
			custom("Goad", "goad(s|ed)?"),
			count("Graft"),
			"Gravestorm",
			"Haunt",
			"Hidden Agenda",
			"Hideaway",
			custom("Horsemanship", unlessAfter: "can't be blocked except by creatures with"),
			"Improvise",
			custom("Infect", unlessBefore: "deal damage to creatures in the form", unlessAfter: "damage dealt by sources without"),
			cost("Kicker", pattern: "(multi)?kick(er|ed|s)"),
			custom("Landwalk", "((snow(\\-covered)?|(non)?basic|legendary) )?(land|denim|desert|plains|forest|swamp|mountain|island)walk"),
			"Plainswalk",
			"Forestwalk",
			"Swampwalk",
			"Mountainwalk",
			"Islandwalk",
			custom("Level Up", "level (up|counter)", unlessBefore: "only as a sorcery"),
			"Living Weapon",
			custom("Melee", unlessAfter: "cast"),
			cost("Miracle"),
			count("Modular"),
			cost("Morph", pattern: "(mega)?morph"),
			custom("Myriad", unlessBefore: "landscape"),
			cost("Ninjutsu"),
			custom("Offering", unlessAfter: "(volcanic|yawgmoth's vile|death pit|burnt)"),
			cost("Outlast"),
			cost("Overload"),
			custom("Partner", unlessAfter: "if both have"),
			"Persist",
			"Phasing",
			count("Poisonous"),
			custom("Protection", unlessAfter: "(circle of|teferi's)"),
			"Provoke",
			cost("Prowl"),
			count("Rampage"),
			"Rebound",
			cost("Recover"),
			count("Reinforce"),
			custom("Replicate", unlessBefore: "cost"),
			"Retrace",
			count("Ripple"),
			custom("Scavenge", unlessBefore: "(only as a sorcery|cost)"),
			custom("Shadow",
				unlessBefore: "guildmage",
				unlessAfter: "(can block or be blocked by only creatures with|nether|shifting|dragon|perilous|death's|elves of deep)"),
			"Soulbond",
			count("Soulshift"),
			cost("Splice onto Arcane", customPattern: "splice(d|s) onto"),
			"Split Second",
			custom("Storm",
				unlessBefore: "(seeker|crow|world|elemental|spirit|sculptor|entity|shaman|fleet|the vault)",
				unlessAfter: "(aether|cinder|comet|lava|hail|needle|wing|tropical|arrow|meteor|possibility|lightning|ion|captain lannery|primal|eye of the|yamabushi's)"),
			"Sunburst",
			custom("Suspend", "suspend(s|ed)?"),
			"Totem Armor",
			cost("Transfigure"),
			cost("Transmute"),
			count("Tribute"),
			"Undaunted",
			cost("Unearth", pattern: "unearth(s|ed)?"),
			"Unleash",
			custom("Vanishing", unlessBefore: "touch"),
			"Wither",

			custom("Activate", "activat(e(s|d)?|i(ng|on))"),
			custom("Attach", "attach(es|ed)?"),
			custom("Unattach", "unattach(es|ed)?"),
			custom("Cast", "cast(s|ing)?"),
			custom("Create", "create(s|d)?"),
			custom("Destroy", "destroy(s|ed)?"),
			custom("Exchange", "exchange(s|d)?"),
			custom("Fight", "fight(s|ed)?"),
			custom("Play", "play(s|ed)?"),
			custom("Reveal", "reveal(s|ed)?"),
			custom("Sacrifice", "sacrifice(s|d)?"),
			custom("Search", "search(es|ed)?"),
			custom("Shuffle", "shuffle(s|d)?"),
			custom("Tap", "tap(s|ped)?"),
			custom("Untap", "untap(s|ped)?"),
			custom("Bury", "bur(y|ies|ied)", unlessBefore: "ruin"),
			custom("Ante", "ante(s|d)?")
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

		private const string KeywordIntroducers = @"\b(you|teammate|opponent|player|was|were|is|are|it's|they're|ha(s|d|ve)|gain(s|ed)?|can't|activate(s|d)?|with) ";
		private const string KeywordOutroducers = @" (abilit(y|ies)|costs?|(a|any|this|these|that|those)?(spells?|permanents?))\b";
		private const string ThenCount = @"( (\d+|x)| ?— ?sunburst)\b";

		private static readonly int _keywordsIndex = PropertyNames.IndexOf(nameof(Keywords));
		public static Dictionary<string, Regex> KeywordPatternsByValue => PatternsByDisplayText[_keywordsIndex];
	}
}