using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Mtgdb.Test
{
	public class IntellisenseCases
	{
		public static IEnumerable Cases => completeCases(
			otherCases(),
			artistCases(),
			legalityCases(),
			numericCases()
		);

		private static IEnumerable<Case> otherCases()
		{
			return new[]
			{
				add("■",
					_allFields),

				add("PricingMid■:[",
					"PricingMid:"),

				add("PricingMid:■[",
					Float),

				add("PricingMid:[■",
					Float),

				add("PricingMid:[0 TO 1■] OR",
					Float),

				add("PricingMid:[0 TO 1]■ OR",
					_allFields),

				add("PricingMid:[0 TO 1] ■OR",
					"OR"),

				add("PricingMid:[0 TO 1] O■R",
					"OR"),

				add("PricingMid:[0 TO 1] OR■",
					"OR")
			};
		}

		private static IEnumerable<Case> artistCases()
		{
			return new[]
			{
				add("Artist:■",
					"aaron boyd",
					"al davidson"),

				add("Artist:a■",
					"alan pollack"),

				add("Artist:b■",
					"bastien l. deharme",
					"bob petillo"),

				add("Artist:wood■",
					"ash wood",
					"sam wood",
					"todd lockwood"),

				add("Artist:zzzz■") // empty suggest
			};
		}

		private static IEnumerable<Case> legalityCases()
		{
			return new[]
			{
				add("BannedIn:■",
					"commander",
					"vintage"),

				add("BannedIn:c■",
					"commander",
					"legacy",
					"urza block"),

				add("BannedIn:zzzz■"), // empty suggest

				add("LegalIn:■",
					"amonkhet block",
					"kaladesh block"),

				add("LegalIn:a■",
					"amonkhet block",
					"ixalan block"),

				add("LegalIn:r■",
					"ravnica block",
					"mirrodin block"),

				add("LegalIn:zzzz■"), // empty suggest

				add("RestrictedIn:■",
					"un-sets",
					"vintage"),

				add("RestrictedIn:u■",
					"un-sets"),

				add("RestrictedIn:v■",
					"vintage"),

				add("RestrictedIn:zzzz■") // empty suggest
			};
		}

		private static IEnumerable<Case> numericCases()
		{
			return new[]
			{
				add("Cmc:■",
					"0",
					"0.5",
					"10"),

				add("Cmc:10■",
					"10",
					"1000000"),

				add("Cmc:z■"), // empty



				add("Hand:■",
					"-4",
					"3"),

				add("Hand:1■",
					"-1",
					"1"),

				add("Hand:z■"), // empty



				add("Life:■",
					"-8",
					"12"),

				add("Life:3■",
					"-3",
					"3",
					"30"),

				add("Life:z■"), // empty



				add("LoyaltyNum:■",
					"0",
					"7"),

				add("LoyaltyNum:2■",
					"2"),

				add("LoyaltyNum:z■"), // empty



				add("PowerNum:■",
					"-1",
					"0.5",
					"1.5",
					"2.5",
					"3.5",
					"9",
					"11"),

				add("PowerNum:9■",
					"9",
					"99"),

				add("PowerNum:z■"), // empty



				add("PricingHigh:■",
					"0.29",
					"0.35"),

				add("PricingHigh:1■",
					"0.51",
					"1"),

				add("PricingHigh:z■"), // empty



				add("PricingLow:■",
					"0.01",
					"0.15"),

				add("PricingLow:1■",
					"0.01",
					"0.1",
					"1"),

				add("PricingLow:z■"), // empty



				add("PricingMid:■",
					"0.14",
					"0.19"),

				add("PricingMid:2■",
					"0.2",
					"0.42"),

				add("PricingMid:z■"), // empty



				add("ToughnessNum:■",
					"-1",
					"0.5",
					"1.5",
					"2.5",
					"3.5",
					"9"),

				add("ToughnessNum:3■",
					"3",
					"3.5",
					"13"),

				add("ToughnessNum:z■") // empty
			};
		}


		private static Case add(string queryWithCaret, params string[] expectedValues)
		{
			return new Case(queryWithCaret, expectedValues);
		}

		private static IEnumerable<TestCaseData> completeCases(params IEnumerable<Case>[] partialCases)
		{
			return partialCases.SelectMany(gr => gr.Select(c => new TestCaseData(_languages, c.ExpectedValues, c.QueryWithCaret)));
		}

		private class Case
		{
			public string QueryWithCaret { get; }
			public IEnumerable<string> ExpectedValues { get; }

			public Case(string queryWithCaret, params string[] expectedValues)
			{
				QueryWithCaret = queryWithCaret;
				ExpectedValues = expectedValues;
			}
		}



		public const string Float = "{float}";
		public const char CaretIndicator = '■';

		private static readonly string[] _languages =
		{
			"en",
			"ru"
		};

		private static readonly string[] _allFields =
		{
			"Artist:",
			"Types:"
		};
	}
}