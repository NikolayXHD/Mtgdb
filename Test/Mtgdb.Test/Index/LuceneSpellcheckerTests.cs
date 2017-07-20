using System;
using System.Diagnostics;
using Mtgdb.Dal.Index;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class LuceneSpellcheckerTests
	{
		private LuceneSpellchecker _spellchecker;
		private LuceneSearcher _searcher;

		[OneTimeSetUp]
		public void Setup()
		{
			TestLoadingUtil.LoadModules();
			TestLoadingUtil.LoadSearcher();
			_searcher = TestLoadingUtil.Searcher;
			_spellchecker = _searcher.Spellchecker;
		}

		[OneTimeTearDown]
		public void Teardown()
		{
			_spellchecker.Dispose();
			_searcher.Dispose();
		}


		[TestCase("*", @"d", "en")]
		[TestCase("NameEn", @"neveiral", null)]
		[TestCase("Name", @"гел", "ru")]
		[TestCase("*", @"арха", "ru")]
		[TestCase("*", @"ange", "en")]
		[TestCase("TextEn", @"disc", null)]
		public void Suggest_text_values(string field, string value, string language)
		{
			suggest(field, value, language);
		}

		[TestCase("PricingMid", @"34", null)]
		[TestCase("Loyalty", @"3", null)]
		public void Suggest_numeric_values(string field, string value, string language)
		{
			suggest(field, value, language);
		}

		[TestCase("en", "text:Gel*de", 11)]
		[TestCase("en", "PricingMid:[", 10)]
		[TestCase("en", "PricingMid:[", 11)]
		[TestCase("en", "PricingMid:[", 12)]
		[TestCase("en", "PricingMid:[0 TO 1] OR", 18)]
		[TestCase("en", "PricingMid:[0 TO 1] OR", 19)]
		[TestCase("en", "PricingMid:[0 TO 1] OR", 20)]
		[TestCase("en", "PricingMid:[0 TO 1] OR", 21)]
		[TestCase("en", "PricingMid:[0 TO 1] OR", 22)]
		public void Suggest_by_user_input(string langauge, string queryStr, int caret)
		{
			suggestByInput(queryStr, caret, langauge);
		}

		private void suggest(string field, string value, string language)
		{
			var sw = new Stopwatch();
			sw.Start();

			var list = _spellchecker.SuggestValues(value, field, language, 20);

			sw.Stop();
			Console.WriteLine($"Suggest retrieved in {sw.ElapsedMilliseconds} ms");

			Assert.That(list, Is.Not.Null);
			Assert.That(list, Is.Not.Empty);

			foreach (string variant in list)
				Console.WriteLine(variant);
		}

		private void suggestByInput(string query, int caret, string language)
		{
			var sw = new Stopwatch();
			sw.Start();

			var suggest = _spellchecker.Suggest(query, caret, language, 50);

			sw.Stop();
			Console.WriteLine($"Suggest retrieved in {sw.ElapsedMilliseconds} ms");

			Assert.That(suggest, Is.Not.Null);

			var list = suggest.Values;

			Assert.That(list, Is.Not.Null);
			Assert.That(list, Is.Not.Empty);

			Console.WriteLine("Token: " + suggest.Token);
			Console.WriteLine("Suggest:");
			foreach (string variant in list)
				Console.WriteLine(variant);
		}
	}
}