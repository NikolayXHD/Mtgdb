using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable StringLiteralTypo

namespace Mtgdb.Data
{
	public static class KeywordDefinitions
	{
		static KeywordDefinitions()
		{
			Patterns = Enumerable.Range(0, Values.Count)
				.Select(i => Values[i].Select(PatternFactories[i]).ToList())
				.Cast<IList<Regex>>()
				.ToList();

			PatternsByDisplayText =
				Enumerable.Range(0, Values.Count)
					.Select(i => Enumerable.Range(0, Patterns[i].Count)
						.Where(j => Values[i][j] != null)
						.ToDictionary(
							j => KeywordRegexUtil.GetKeywordDisplayText(Values[i][j]),
							j => Patterns[i][j],
							Str.Comparer))
					.ToList();

			KeywordsIndex = PropertyNames.IndexOf(nameof(Keywords));
			CastKeywordsIndex = PropertyNames.IndexOf(nameof(CastKeywords));
		}

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



		public static readonly IList<string> ManaCost = new[]
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

		public static readonly IList<string> ManaAbility = new[]
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

		public static readonly IList<string> GeneratedMana = new[]
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

		public static readonly IList<string> Cmc = new[]
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

		public static readonly IList<string> Type = new[]
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

		public static readonly IList<string> CastKeywords = new[]
		{
			AftermathKeyword,
			custom("Affinity", bound("affinity", before: "for")),
			count("Awaken"),
			cost("Bestow", bound("bestow", notBefore: "cost")),
			"Buyback",
			custom("Can't be countered", bound(cant("be", "countered"))),
			custom("Cascade", bound("cascade", notAfter: "skyline")),
			custom("Champion", bound("champion", before: "an?")),
			"Cipher",
			"Conspire",
			"Convoke",

			custom("Counter", bound(or(
				notAfter(@"""|(\-|\+)\d ", or(
					sequenceWithout(
						@"(cop(y|ies)|activates?|casts?|plays?|search(es)?)\b",
						notBefore("counters?", " on"), "(spells?|abilit(y|ies))"),
					before("counters?", " (it|phantasmagorian|temporal extortion|brain gorgers)"))),
				notAfter("can't be ", "countered")))),

			cost("Cycling", pattern: @"(basic )?\w*cycling", customPattern: "\\w*cycl(ing|e(s|d)?)"),
			cost("Dash", bound("dash", notAfter: "you may cast this spell for its")),
			"Delve",
			custom("Discard", bound("discard(s|ed|ing)?", notBefore: "it into exile")),
			custom("Draw", bound("draws?", notBefore: "step", before: @"[^\.]*\bcards?")),
			count("Dredge"),
			cost("Embalm"),
			cost("Emerge"),
			cost("Eternalize"),
			"Extra turn",
			custom("Flash", bound("flash",
				notBefore: "(conscription|foliage|of Insight)",
				notAfter: "aether")),
			custom("Flashback", bound("flashback", notBefore: "cost")),
			custom("Fuse", bound("fuse", notBefore: "counters?")),
			"Improvise",
			cost("Madness"),
			cost("Miracle"),
			cost("Morph", bound("(mega)?morph", notBefore: "cost")),
			cost("Overload"),
			"Rebound",
			"Retrace",
			custom("Scavenge", bound("scavenge", notBefore: "(only as a sorcery|cost)")),
			"Scry",
			custom("Search", bound("search(es|ed)?")),
			"Soulbond",
			count("Soulshift"),
			cost("Splice onto arcane", customPattern: "splice(d|s) onto"),
			"Split Second",
			cost("Surge", bound("surge", notAfter: "cast this spell for its")),
			custom("Suspend", bound("suspend(s|ed)?")),
			cost("Transmute"),
			cost("Unearth", pattern: "unearth(s|ed)?"),

			// non quick-filter values follow

			"Addendum",
			cost("Spectacle")
		};

		public static readonly IList<string> Keywords = new[]
		{
			count("Annihilator"),
			"Ascend",
			custom("Attack if able",
				bound(sequenceWithout(@"block(s|ed)?\b",
					@"(?<!\bcan't )attacks?( or blocks?)?\b", "if able"))),
			custom("Block if able",
				bound(or(
					sequenceWithout(@"attacks?\b",
						@"(?<!\bcan't )(attacks? or )?block(s|ed)?\b", "if able"),
					sequence("able to\\b", "block\\b", "do (it|so)"),
					"must be blocked"))),
			custom("Can't attack", bound(or(cant("attack"), cant("be", "attacked")))),
			custom("Can't be blocked", bound(cant("be", "blocked"),
				notAfter: "this spell works on creatures that")),
			custom("Can't be regenerated", bound(cant("be", "regenerated"))),
			custom("Can't block", bound(cant("block"))),
			"Changeling",
			cost("Cohort"),
			custom("Copy", bound("cop(y|ies)")),
			custom("Create token", bound("create(s|d)?")),
			count("Crew", customPattern: "crews?"),
			custom("Deal damage", bound(@"deals?( \d+)? damage")),
			custom("Deathtouch", bound("(un)?deathtouch")),
			custom("Defender", bound("defender",
				notBefore: "(en\\-vec|of the order)",
				notAfter: "(shu|sworn)")),
			"Delirium",
			custom("Destroy", bound("destroy(s|ed)?")),
			count("Devour", customPattern: "devoured"),
			custom("Doesn't untap", bound(or(
				"(doesn|can|don|won)'t untap",
				sequence("skips?", "untap", "steps?")))),
			"Double Strike",
			"Enchant",
			cost("Equip"),
			custom("Exalted", bound("exalted", notBefore: "(dragon|angel)", notAfter: "instances? of")),
			"Exert",
			custom("Exile", bound("exiles?", notBefore: "into darkness", notAfter: "tel\\-jilad")),
			custom("Fear", bound("fear", notAfter: "Shinen of")),
			custom("Fight", bound("fight(s|ed)?")),
			custom("First Strike", bound("first strike",
				notAfter: "deals combat damage before creatures without")),
			custom("Flying", bound("Flying", notAfter: "(can block|except by) creatures with")),
			custom("Gain control", bound("gains? control")),
			"Haste",
			"Hexproof",
			custom("Indestructible", bound("indestructible", notBefore: "can't be destroyed by damage")),
			custom("Infect", bound("infect",
				notBefore: "deal damage to creatures in the form",
				notAfter: "damage dealt by sources without")),
			"Ingest",
			custom("Ingest processor", bound(sequence("from", "exile", "into", "graveyard"))),
			custom("Intimidate", bound("intimidate", notBefore: "can't be blocked except")),
			custom("Landwalk", bound(
				optional("(snow(\\-covered)?|(non)?basic|legendary) ") +
				"(land|denim|desert|plains|forest|swamp|mountain|island)walk")),
			"Lifelink",
			"Menace",
			count("Modular"),
			cost("Ninjutsu"),
			"Persist",
			custom("Phasing", bound(or("phasing", "phases? (in|out)"))),
			custom("Protection", bound("protection", notAfter: "(circle of|teferi's)")),
			"Prowess",
			custom("Rally", bound("rally", before: "— ?")),
			custom("Reach", bound("reach",
				notBefore: "of Branches",
				notAfter: "(except by creatures with flying or|geier|myojin of night's)")),
			"Regenerate",
			count("Renown", customPattern: bound("renowned",
				notBefore: "weaver",
				notAfter: "if it isn't")),
			custom("Sacrifice", bound("sacrifice(s|d)?")),
			custom("Shadow", bound("shadow",
				notBefore: "(guildmage|-watcher)",
				notAfter:
				"(can block or be blocked by only creatures with|nether|feral|shifting|dragon|perilous|death's|elves of deep)")),
			"Shroud",
			custom("Skulk", bound("skulk", notAfter: "pit-")),
			"Totem Armor",
			"Trample",
			custom("Undying", bound("undying", notBefore: "(beast|rage|flames|partisan)")),
			"Vigilance",
			"Wither",

			// non quick-filter values follow

			count("Absorb"),
			count("Adapt"),
			count("Afflict"),
			count("Afterlife"),
			count("Amplify"),
			cost("Aura Swap"),
			"Assist",
			custom("Banding", bound("banding", notAfter: "any creatures with")),
			"Battle Cry",
			custom("Bloodrush", bound("bloodrush", before: "—")),
			count("Bloodthirst"),
			count("Bushido"),
			cost("Cumulative Upkeep"),
			"Dethrone",
			"Devoid",
			cost("Echo"),
			cost("Entwine"),
			custom("Epic", bound("epic", notAfter: "copy this spell except for its")),
			cost("Escalate"),
			cost("Evoke"),
			"Evolve",
			"Exploit",
			custom("Explore", bound("explores")),
			"Extort",
			count("Fabricate"),
			count("Fading"),
			custom("Flanking", bound("flanking",
				notBefore: "troops",
				notAfter: "whenever a creature without")),
			cost("Forecast"),
			"Forestwalk",
			custom("Historic", bound("historic", notAfter: "sagas are")),
			"Islandwalk",
			"Jump-start",
			"Mountainwalk",
			custom("Mentor", bound("mentor", notAfter: "(wizard|stromkirk|proud|stern|harsh)")),
			"Plainswalk",
			"Swampwalk",
			custom("Fortify", bound("fortify", notBefore: "only as a sorcery")),
			count("Frenzy"),
			custom("Goad", bound("goad(s|ed)?")),
			count("Graft"),
			"Gravestorm",
			"Haunt",
			"Hidden Agenda",
			"Hideaway",
			custom("Horsemanship", bound("horsemanship",
				notAfter: "can't be blocked except by creatures with")),
			cost("Kicker", pattern: "(multi)?kick(er|ed|s)"),
			custom("Level Up", bound("level (up|counter)", notBefore: "only as a sorcery")),
			"Living Weapon",
			custom("Manifest", bound("manifest", notAfter: "to")),
			custom("Melee", bound("melee", notAfter: "cast")),
			custom("Myriad", bound("myriad", notBefore: "landscape")),
			custom("Offering", bound("offering", notAfter: "(volcanic|yawgmoth's vile|death pit|burnt)")),
			cost("Outlast"),
			custom("Partner", bound("partner", notAfter: "if both have")),
			count("Poisonous"),
			"Provoke",
			cost("Prowl"),
			count("Rampage"),
			cost("Recover"),
			count("Reinforce"),
			custom("Replicate", bound("replicate", notBefore: "cost")),
			custom("Riot", bound("riot", notBefore:"(piker|ringleader)")),
			count("Ripple"),
			custom("Storm", bound(@"(grave)?storm",
				notBefore: "(seeker|crow|world|elemental|spirit|sculptor|entity|shaman|fleet|the vault)",
				notAfter:
				"(aether|cinder|comet|lava|hail|needle|wing|tropical|arrow|meteor|possibility|lightning|ion|captain lannery|primal|eye of the|yamabushi's)")),
			"Sunburst",
			count("Support"),
			count("Surveil", bound("surveil", notAfter: "to")),
			cost("Transfigure"),
			custom("Transform", bound("transform(s|ed)?")),
			count("Tribute", bound("tribute")),
			"Undaunted",
			custom("Undergrowth", bound("undergrowth", notBefore: "(champion|scavenger)")),
			"Unleash",
			custom("Vanishing", bound("vanishing", notBefore: "touch")),

			custom("Activate", bound("activat(e(s|d)?|i(ng|on))")),
			custom("Attach", bound("attach(es|ed)?")),
			custom("Unattach", bound("unattach(es|ed)?")),
			custom("Cast", bound("cast(s|ing)?")),
			custom("Exchange", bound("exchange(s|d)?")),
			custom("Play", bound("play(s|ed)?")),
			custom("Reveal", bound("reveal(s|ed)?")),
			custom("Shuffle", bound("shuffle(s|d)?")),
			custom("Tap", bound("tap(s|ped)?")),
			custom("Untap", bound("untap(s|ped)?", notAfter: "skips? (their|the|your)( next)?")),
			custom("Bury", bound("bur(y|ies|ied)", notBefore: "ruin")),
			custom("Ante", bound("ante(s|d)?"))
		};

		public static readonly IList<string> Rarity = new[]
		{
			"Common",
			"Uncommon",
			"Rare",
			"Mythic",
			"Basic land",
			null /*, "marketing", "double faced"*/
		};

		public static readonly IList<string> Layout = new[]
		{
			CardLayouts.Normal,
			CardLayouts.Saga,
			CardLayouts.Aftermath,
			CardLayouts.Split,
			CardLayouts.Meld,
			CardLayouts.Leveler,
			CardLayouts.Transform,
			CardLayouts.Flip,
			CardLayouts.Phenomenon,
			CardLayouts.Plane,
			CardLayouts.Scheme,
			CardLayouts.Vanguard,
			null
		};



		private static Func<string, Regex>[] PatternFactories { get; } =
		{
			KeywordRegexUtil.CreateContainsRegex,
			KeywordRegexUtil.CreateContainsRegex,
			KeywordRegexUtil.CreateEqualsRegex,
			KeywordRegexUtil.CreateContainsRegex,
			KeywordRegexUtil.CreateEqualsRegex,
			KeywordRegexUtil.CreateContainsRegex,
			KeywordRegexUtil.CreateContainsRegex,
			KeywordRegexUtil.CreateEqualsRegex,
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
			c => c.GeneratedMana,
			c => c.Layout,
			c => c.TextEn
		};

		public static readonly IList<string> PropertyNamesDisplay = new[]
		{
			nameof(Card.ManaCost),
			nameof(Card.Type),
			nameof(Card.Rarity),
			nameof(Card.Text),
			nameof(Card.Cmc),
			nameof(Card.Text),
			nameof(Card.Text),
			nameof(Card.Layout),
			nameof(Card.Text)
		};

		public static readonly IList<IList<string>> Values = new[]
		{
			ManaCost,
			Type,
			Rarity,
			Keywords,
			Cmc,
			ManaAbility,
			GeneratedMana,
			Layout,
			CastKeywords
		};

		public static readonly IList<string> PropertyNames = new[]
		{
			nameof(ManaCost),
			nameof(Type),
			nameof(Rarity),
			nameof(Keywords),
			nameof(Cmc),
			nameof(ManaAbility),
			nameof(GeneratedMana),
			nameof(Layout),
			nameof(CastKeywords)
		};


		public static IList<IList<Regex>> Patterns { get; }

		public static IList<Dictionary<string, Regex>> PatternsByDisplayText { get; }

		public static readonly int KeywordsIndex;
		public static readonly int CastKeywordsIndex;

		private const string KeywordIntroducers =
			@"\b(you|teammate|opponent|player|was|were|is|are|it's|they're|ha(s|d|ve)|gain(s|ed)?|can't|activate(s|d)?|with) ";

		private const string KeywordOutroducers =
			@" ((a|any|this|these|that|those|target) )?(it|abilit(y|ies)|costs?|spells?|permanents?|vehicles?)\b";

		private const string CountSuffix = @"( (\d+|x)\b| ?— ?sunburst)";

		public const string AftermathKeyword = "Aftermath";
	}
}