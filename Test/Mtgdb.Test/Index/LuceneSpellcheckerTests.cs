using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class LuceneSpellcheckerTests : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadIndexes();
			Spellchecker.MaxCount = 50;
		}

		[TestCase("*", "d", "en", null)]
		[TestCase("NameEn", "neveinral", null, "nevinyrral")]
		[TestCase("NameEn", "vinira", null, "nevinyrral")]
		[TestCase("TextEn", "disk", null, "disk")]
		[TestCase("Name", "гел", "ru", "ангел")]
		[TestCase("*", "арха", "ru", "архангел")]
		[TestCase("*", "ange", "en", "angel")]
		[TestCase("layout", "aft", null, "aftermath")]
		[TestCase("generatedmana", "w", null, "{w}")]
		public void Suggest_text_values(string field, string value, string language, string expectedSuggest)
		{
			var list = suggest(field, value, language);

			if (expectedSuggest != null)
				Assert.That(list, Does.Contain(expectedSuggest));
		}

		[TestCase("PricingMid", @"34", null)]
		[TestCase("Loyalty", @"3", null)]
		public void Suggest_numeric_values(string field, string value, string language)
		{
			var list = suggest(field, value, language);

			Assert.That(list.All(v=>float.TryParse(v, out _)));
			Assert.That(list.Any(v => v.Contains(value)));
		}

		[TestCase("en",
			"text:Gel*demo",
			"-------------^", "demon")]
		[TestCase("en",
			"PricingMid:[",
			"----------^--", "PricingMid:")]
		[TestCase("en",
			"PricingMid:[",
			"-----------^-", "{float}")]
		[TestCase("en",
			"PricingMid:[",
			"------------^", "{float}")]
		[TestCase("en",
			"PricingMid:[0 TO 1] OR",
			"------------------^----", "{float}")]
		[TestCase("en",
			"PricingMid:[0 TO 1] OR",
			"-------------------^---", "PricingMid:" /*complete field list*/)]
		[TestCase("en",
			"PricingMid:[0 TO 1] OR",
			"--------------------^--", "OR")]
		[TestCase("en",
			"PricingMid:[0 TO 1] OR",
			"---------------------^-", "OR")]
		[TestCase("en",
			"PricingMid:[0 TO 1] OR",
			"----------------------^", "OR")]
		public void Suggest_by_user_input(string language, string queryStr, string caretIndicator, string expectedSuggest)
		{
			int caret = caretIndicator.IndexOf("^", Str.Comparison);

			var list = suggestByInput(queryStr, caret, language);

			if (expectedSuggest == "{float}")
			{
				Assert.That(list.All(v => float.TryParse(v, out _)));
			}
			else if (expectedSuggest != null)
			{
				Assert.That(list, Does.Contain(expectedSuggest));
			}
		}

		private IReadOnlyList<string> suggest(string field, string value, string language)
		{
			var sw = new Stopwatch();
			sw.Start();

			var list = Spellchecker.SuggestValues(value, field, language);

			sw.Stop();
			Log.Debug($"Suggest retrieved in {sw.ElapsedMilliseconds} ms");

			Assert.That(list, Is.Not.Null);
			Assert.That(list, Is.Not.Empty);

			foreach (string variant in list)
				Log.Debug(variant);

			return list;
		}

		private IReadOnlyList<string> suggestByInput(string query, int caret, string language)
		{
			var sw = new Stopwatch();
			sw.Start();

			var suggest = Spellchecker.Suggest(query, caret, language);

			sw.Stop();
			Log.Debug($"Suggest retrieved in {sw.ElapsedMilliseconds} ms");

			Assert.That(suggest, Is.Not.Null);

			var list = suggest.Values;

			Assert.That(list, Is.Not.Null);
			Assert.That(list, Is.Not.Empty);

			Log.Debug("Token: " + suggest.Token);
			Log.Debug("Suggest:");
			foreach (string variant in list)
				Log.Debug(variant);

			return list;
		}
	}
}