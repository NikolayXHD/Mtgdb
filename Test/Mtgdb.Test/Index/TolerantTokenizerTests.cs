using Lucene.Net.Contrib;
using NLog;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class TolerantTokenizerTests
	{
		[TestCase(@"PricingLow:{10 TO *]")]
		[TestCase(@"\""""angelic demon""")]
		[TestCase(@"""\""angelic demon""")]
		[TestCase(@"ManaCost:*E?")]
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

		private static TolerantTokenizer tokenize(string queryStr)
		{
			_log.Debug(queryStr);

			var parser = new TolerantTokenizer(queryStr);
			parser.Parse();

			if (parser.SyntaxErrors.Count > 0)
				_log.Debug("Errors:");

			foreach (string error in parser.SyntaxErrors)
				_log.Debug(error);

			_log.Debug("Tokens:");

			foreach (var token in parser.Tokens)
				_log.Debug(token);

			return parser;
		}

		[TearDown]
		public void Teardown()
		{
			LogManager.Flush();
		}

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}