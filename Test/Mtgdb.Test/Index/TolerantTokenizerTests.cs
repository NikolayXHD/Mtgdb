using Lucene.Net.Contrib;
using Mtgdb.Data;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class TolerantTokenizerTests : TestsBase
	{
		[TestCase(@"PricingLow:{10 TO ?]")]
		[TestCase(@"\""""angelic demon""")]
		[TestCase(@"""\""angelic demon""")]
		[TestCase(@"ManaCost:*E?")]
		// ReSharper disable once StringLiteralTypo
		[TestCase(@"NameEn:(NOT(angel OR demon NOT ""angelic demon"") OR [Animal TO Clan]) AND nofield")]
		[TestCase(@"NameEn:/.{1,2}nge.*/")]
		public void When_query_is_correct_detected_errors_count_is_0(string queryStr)
		{
			var tokenizer = tokenize(queryStr);
			Assert.That(tokenizer.SyntaxErrors.Count, Is.EqualTo(0));
		}

		[TestCase(@"NameEn:(NOT(angel OR demon NOT")]
		[TestCase(@"))NOT(angel O")]
		[TestCase(@"type:/unfinished regex")]
		public void When_query_is_NOT_correct_tokenize_does_not_throw(string queryStr)
		{
			var tokenizer = tokenize(queryStr);
			Assert.That(tokenizer.Tokens, Is.Not.Null);
			Assert.That(tokenizer.Tokens, Is.Not.Empty);
		}

		private TolerantTokenizer tokenize(string queryStr)
		{
			Log.Debug(queryStr);

			var parser = new MtgTolerantTokenizer(queryStr);
			parser.Parse();

			if (parser.SyntaxErrors.Count > 0)
				Log.Debug("Errors:");

			foreach (string error in parser.SyntaxErrors)
				Log.Debug(error);

			Log.Debug("Tokens:");

			foreach (var token in parser.Tokens)
				Log.Debug(token);

			return parser;
		}
	}
}
