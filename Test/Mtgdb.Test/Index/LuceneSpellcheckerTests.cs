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
		[TestCase("NameEn", "viniral", null, "nevinyrral")]
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
				Assert.That(list, Has.Some.Contain(expectedSuggest));
		}

		[TestCase("PricingMid", @"34", null)]
		[TestCase("LoyaltyNum", @"3", null)]
		public void Suggest_numeric_values(string field, string value, string language)
		{
			var list = suggest(field, value, language);

			Assert.That(list.All(v => float.TryParse(v, out _)));
			Assert.That(list.Any(v => v.Contains(value)));
		}

	
		
		[TestCaseSource(typeof(IntellisenseCases), nameof(IntellisenseCases.Cases))]
		public void Suggest_by_user_input(string[] languages, string[] expectedSuggests, string queryWithCaret)
		{
			foreach (string language in languages)
			{
				int caret = queryWithCaret.IndexOf(IntellisenseCases.CaretIndicator);
				string queryStr = queryWithCaret.Substring(0, caret) + queryWithCaret.Substring(caret + 1);

				var list = suggestByInput(queryStr, caret, language);

				foreach (string expectedSuggest in expectedSuggests)
				{
					if (expectedSuggest == IntellisenseCases.Float)
						Assert.That(list.All(v => float.TryParse(v, out _)));
					else if (!string.IsNullOrEmpty(expectedSuggest))
						Assert.That(list, Does.Contain(expectedSuggest));
					else
						Assert.That(list, Is.Empty);
				}
			}
		}

		private IReadOnlyList<string> suggest(string field, string value, string language)
		{
			var sw = new Stopwatch();
			sw.Start();

			string query = $"{field}:{value}";

			var list = Spellchecker.Suggest(language, query, caret: query.Length).Values;

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

			var suggest = Spellchecker.Suggest(language, query, caret);

			sw.Stop();
			Log.Debug($"Suggest retrieved in {sw.ElapsedMilliseconds} ms");

			Assert.That(suggest, Is.Not.Null);

			var list = suggest.Values;

			Assert.That(list, Is.Not.Null);
			
			Log.Debug("Token: " + suggest.Token);
			Log.Debug("Suggest:");
			
			foreach (string variant in list)
				Log.Debug(variant);

			return list;
		}
	}
}