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
			string normalForm = pattern ?? name;
			string modifiedForm = customPattern ?? normalForm;

			var costKeywordPattern = or(
				before(normalForm, @" ?[\{—]"),
				after(KeywordIntroducers, modifiedForm),
				before(modifiedForm, KeywordOutroducers));

			return custom(name, costKeywordPattern);
		}

		private static string count(string name, string pattern = null, string customPattern = null)
		{
			string normalForm = pattern ?? name;
			string modifiedForm = customPattern ?? normalForm;

			var countKeywordPattern = or(
				before(normalForm, CountSuffix),
				after(KeywordIntroducers, modifiedForm),
				before(modifiedForm, KeywordOutroducers));

			return custom(name, countKeywordPattern);
		}

		private static string custom(string name, string pattern)
			=> $"/{pattern}/ {name}";

		private static string bound(
			string body,
			string notBefore = null,
			string before = null,
			string notAfter = null)
		{
			var result = new StringBuilder();

			if (notAfter != null)
				result.Append("(?<!\\b").Append(notAfter).Append(" )");

			result.Append("\\b").Append(body).Append("\\b");

			if (notBefore != null)
				result.Append("(?! ").Append(notBefore).Append("\\b)");

			if (before != null)
				result.Append("(?= ").Append(before).Append("\\b)");

			return result.ToString();
		}

		private static string cant(params string[] patterns) =>
			sequence(Sequence.From("can't").Concat(patterns), without: "can|unless");

		private static string or(params string[] patterns)
			=> "(" + string.Join("|", patterns) + ")";

		private static string optional(string pattern)
			=> $"({pattern})?";

		private static string sequence(params string[] patterns) =>
			sequence(patterns, null);

		private static string sequenceWithout(string without, params string[] patterns) =>
			sequence(patterns, without);

		private static string sequence(IEnumerable<string> patterns, string without)
		{
			string separator = @"[\s\p{P}-[\.]]+";
			string word = @"[^\s\p{P}]+";

			var patternsArr = patterns.ToArray();
			var notFillers = (IEnumerable<string>) patternsArr;

			if (without != null)
				notFillers = notFillers.Append(without);

			string notFiller = string.Join("|", notFillers);
			string fillers = $"(?(?={separator}({notFiller}))(?!)|{separator}{word})*";

			var builder = new StringBuilder();

			builder.Append(patternsArr[0]);

			for (int i = 1; i < patternsArr.Length; i++)
				builder.Append(fillers).Append(separator).Append(patternsArr[i]);

			var result = builder.ToString();
			return result;
		}

		private static string after(string condition, string pattern)
			=> $"(?<={condition}){pattern}";

		private static string notAfter(string condition, string pattern)
			=> $"(?<!{condition}){pattern}";

		private static string before(string pattern, string condition)
			=> $"{pattern}(?={condition})";

		private static string notBefore(string pattern, string condition)
			=> $"{pattern}(?!{condition})";


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
			custom("Attack if able",
				bound(sequenceWithout(@"block(s|ed)?\b",
					@"(?<!\bcan't )attacks?( or blocks?)?\b", "if able"))),
			count("Awaken"),
			custom("Block if able",
				bound(or(
					sequenceWithout(@"attacks?\b",
						@"(?<!\bcan't )(attacks? or )?block(s|ed)?\b", "if able"),
					sequence("able to\\b", "block\\b", "do (it|so)"),
					"must be blocked"))),
			custom("Can't attack", bound(or(cant("attack"), cant("be", "attacked")))),
			custom("Can't be blocked", bound(cant("be", "blocked"),
				notAfter: "this spell works on creatures that")),
			custom("Can't be countered", bound(cant("be", "countered"))),
			custom("Can't be regenerated", bound(cant("be", "regenerated"))),
			custom("Can't block", bound(cant("block"))),
			cost("Cohort"),
			custom("Copy", bound("cop(y|ies)")),
			custom("Counter", bound(or(
				notAfter(@"""|(\-|\+)\d ", or(
					sequenceWithout(
						@"(cop(y|ies)|activates?|casts?|plays?|search(es)?)\b",
						notBefore("counters?", " on"), "(spells?|abilit(y|ies))"),
					before("counters?", " (it|phantasmagorian|temporal extortion|brain gorgers)"))),
				notAfter("can't be ", "countered")))),
			custom("Deathtouch", bound("(un)?deathtouch")),
			custom("Defender", bound("defender",
				notBefore: "(en\\-vec|of the order)",
				notAfter: "(shu|sworn)")),
			"Delirium",
			custom("Discard", bound("discard(s|ed|ing)?", notBefore: @"it into exile")),
			custom("Doesn't untap", bound(or(
				"(doesn|can|don|won)'t untap",
				sequence("skips?", "untap", "steps?")))),
			"Double Strike",
			custom("Draw", bound("draws?", notBefore: "step", before: @"[^\.]*\bcards?")),
			"Enchant",
			cost("Equip"),
			custom("Exile", bound("exiles?", notBefore: "into darkness", notAfter: "tel\\-jilad")),
			custom("First Strike", bound("first strike",
				notAfter: "deals combat damage before creatures without")),
			custom("Flash", bound("flash",
				notBefore: "(conscription|foliage|of Insight)",
				notAfter: "aether")),
			custom("Flying", bound("Flying", notAfter: "(can block|except by) creatures with")),
			custom("Gain control", bound("gains? control")),
			"Haste",
			"Hexproof",
			custom("Indestructible", bound("indestructible", notBefore: "can't be destroyed by damage")),
			"Ingest",
			custom("Intimidate", bound("intimidate", notBefore: "can't be blocked except")),
			"Lifelink",
			cost("Madness"),
			"Menace",
			"Prowess",
			custom("Rally", bound("rally", before: "— ?")),
			custom("Reach", bound("reach",
				notBefore: "of Branches",
				notAfter: "(except by creatures with flying or|geier|myojin of night's)")),
			"Regenerate",
			count("Renown", customPattern: bound("renowned",
				notBefore: "weaver",
				notAfter: "if it isn't")),
			"Scry",
			"Shroud",
			custom("Skulk", bound("skulk", notAfter: "pit-")),
			cost("Surge"),
			"Trample",
			custom("Transform", bound("transform(s|ed)?")),
			custom("Undying", bound("undying", notBefore: "(beast|rage|flames|partisan)")),
			"Vigilance",

			null,

			count("Absorb"),
			custom("Affinity", bound("affinity", before: "for")),
			count("Afflict"),
			"Aftermath",
			count("Amplify"),
			"Ascend",
			cost("Aura Swap"),
			custom("Banding", bound("banding", notAfter: "any creatures with")),
			"Battle Cry",
			cost("Bestow"),
			custom("Bloodrush", bound("bloodrush", before: "—")),
			count("Bloodthirst"),
			count("Bushido"),
			"Buyback",
			custom("Cascade", bound("cascade", notAfter: "skyline")),
			custom("Champion", bound("champion", before: "an?")),
			"Changeling",
			"Cipher",
			"Conspire",
			"Convoke",
			count("crew", customPattern: "crews?"),
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
			custom("Epic", bound("epic", notAfter: "copy this spell except for its")),
			cost("Escalate"),
			cost("Eternalize"),
			cost("Evoke"),
			"Evolve",
			custom("Exalted", bound("exalted", notBefore: "(dragon|angel)", notAfter: "instances? of")),
			"Exploit",
			"Extort",
			count("Fabricate"),
			count("Fading"),
			custom("Fear", bound("fear", notAfter: "Shinen of")),
			custom("Flanking", bound("flanking",
				notBefore: "troops",
				notAfter: "whenever a creature without")),
			custom("Flashback", bound("flashback", notBefore: "cost")),
			cost("Forecast"),
			custom("Fortify", bound("fortify", notBefore: "only as a sorcery")),
			count("Frenzy"),
			custom("Fuse", bound("fuse", notBefore: "counters?")),
			custom("Goad", bound("goad(s|ed)?")),
			count("Graft"),
			"Gravestorm",
			"Haunt",
			"Hidden Agenda",
			"Hideaway",
			custom("Horsemanship", bound("horsemanship",
				notAfter: "can't be blocked except by creatures with")),
			"Improvise",
			custom("Infect", bound("infect",
				notBefore: "deal damage to creatures in the form",
				notAfter: "damage dealt by sources without")),
			cost("Kicker", pattern: "(multi)?kick(er|ed|s)"),
			custom("Landwalk", bound(
				optional("(snow(\\-covered)?|(non)?basic|legendary) ") +
				"(land|denim|desert|plains|forest|swamp|mountain|island)walk")),
			"Plainswalk",
			"Forestwalk",
			"Swampwalk",
			"Mountainwalk",
			"Islandwalk",
			custom("Level Up", bound("level (up|counter)", notBefore: "only as a sorcery")),
			"Living Weapon",
			custom("Melee", bound("melee", notAfter: "cast")),
			cost("Miracle"),
			count("Modular"),
			cost("Morph", pattern: "(mega)?morph"),
			custom("Myriad", bound("myriad", notBefore: "landscape")),
			cost("Ninjutsu"),
			custom("Offering", bound("offering", notAfter: "(volcanic|yawgmoth's vile|death pit|burnt)")),
			cost("Outlast"),
			cost("Overload"),
			custom("Partner", bound("partner", notAfter: "if both have")),
			"Persist",
			custom("Phasing", bound("phas(ing|es)( (in|out))?")),
			count("Poisonous"),
			custom("Protection", bound("protection", notAfter: "(circle of|teferi's)")),
			"Provoke",
			cost("Prowl"),
			count("Rampage"),
			"Rebound",
			cost("Recover"),
			count("Reinforce"),
			custom("Replicate", bound("replicate", notBefore: "cost")),
			"Retrace",
			count("Ripple"),
			custom("Scavenge", bound("scavenge", notBefore: "(only as a sorcery|cost)")),
			custom("Shadow", bound("shadow",
				notBefore: "guildmage",
				notAfter:
				"(can block or be blocked by only creatures with|nether|shifting|dragon|perilous|death's|elves of deep)")),
			"Soulbond",
			count("Soulshift"),
			cost("Splice onto Arcane", customPattern: "splice(d|s) onto"),
			"Split Second",
			custom("Storm", bound("storm",
				notBefore: "(seeker|crow|world|elemental|spirit|sculptor|entity|shaman|fleet|the vault)",
				notAfter:
				"(aether|cinder|comet|lava|hail|needle|wing|tropical|arrow|meteor|possibility|lightning|ion|captain lannery|primal|eye of the|yamabushi's)")),
			"Sunburst",
			custom("Suspend", bound("suspend(s|ed)?")),
			"Totem Armor",
			cost("Transfigure"),
			cost("Transmute"),
			count("Tribute"),
			"Undaunted",
			cost("Unearth", pattern: "unearth(s|ed)?"),
			"Unleash",
			custom("Vanishing", bound("vanishing", notBefore: "touch")),
			"Wither",
			custom("Activate", bound("activat(e(s|d)?|i(ng|on))")),
			custom("Attach", bound("attach(es|ed)?")),
			custom("Unattach", bound("unattach(es|ed)?")),
			custom("Cast", bound("cast(s|ing)?")),
			custom("Create", bound("create(s|d)?")),
			custom("Destroy", bound("destroy(s|ed)?")),
			custom("Exchange", bound("exchange(s|d)?")),
			custom("Fight", bound("fight(s|ed)?")),
			custom("Play", bound("play(s|ed)?")),
			custom("Reveal", bound("reveal(s|ed)?")),
			custom("Sacrifice", bound("sacrifice(s|d)?")),
			custom("Search", bound("search(es|ed)?")),
			custom("Shuffle", bound("shuffle(s|d)?")),
			custom("Tap", bound("tap(s|ped)?")),
			custom("Untap", bound("untap(s|ped)?")),
			custom("Bury", bound("bur(y|ies|ied)", notBefore: "ruin")),
			custom("Ante", bound("ante(s|d)?"))
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

		private const string KeywordIntroducers =
			@"\b(you|teammate|opponent|player|was|were|is|are|it's|they're|ha(s|d|ve)|gain(s|ed)?|can't|activate(s|d)?|with) ";

		private const string KeywordOutroducers =
			@" (abilit(y|ies)|costs?|(a|any|this|these|that|those)? (spells?|permanents?|vehicles?))\b";

		private const string CountSuffix = @"( (\d+|x)\b| ?— ?sunburst)";

		private static readonly int _keywordsIndex = PropertyNames.IndexOf(nameof(Keywords));

		public static Dictionary<string, Regex> KeywordPatternsByValue
			=> PatternsByDisplayText[_keywordsIndex];
	}
}