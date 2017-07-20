using System;
using Lucene.Net.Contrib;
using Mtgdb.Dal.Index;
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
		public void Tokenize_correct_query(string queryStr)
		{
			var tokenizer = tokenize(queryStr);
			Assert.That(tokenizer.SyntaxErrors.Count, Is.EqualTo(0));
		}

		[TestCase(@"NameEn:(NOT(angel OR demon NOT", 0)]
		[TestCase(@"))NOT(angel O", 2)]
		public void Tokenize_incorrect_query(string queryStr, int expectedErrors)
		{
			var tokenizer = tokenize(queryStr);
			Assert.That(tokenizer.SyntaxErrors.Count, Is.EqualTo(expectedErrors));
		}

		private static TolerantTokenizer tokenize(string queryStr)
		{
			Console.WriteLine(queryStr);
			Console.WriteLine();

			var parser = new TolerantTokenizer(queryStr);
			parser.Parse();

			if (parser.SyntaxErrors.Count > 0)
				Console.WriteLine("Errors:");

			foreach (string error in parser.SyntaxErrors)
				Console.WriteLine(error);

			Console.WriteLine("Tokens:");

			foreach (var token in parser.Tokens)
				Console.WriteLine(token);

			return parser;
		}
	}
}