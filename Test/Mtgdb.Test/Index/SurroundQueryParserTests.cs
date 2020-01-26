using FluentAssertions;
using Lucene.Net.QueryParsers.Surround.Parser;
using Lucene.Net.QueryParsers.Surround.Query;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class SurroundQueryParserTests
	{
		private QueryParser _parser;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_parser = new QueryParser();
		}

		[TestCase("abc", "abc")]
		[TestCase("1", "1")]
		public void When_no_special_characters_Then_simple_term_query(string queryStr, string term)
		{
			_parser.Parse2(queryStr).Should().BeAssignableTo<SrndTermQuery>()
				.Which.TermText.Should().Be(term);
		}

		[TestCase("abc*", "abc", '*')]
		public void When_ends_with_star_Then_prefix_query(string queryStr, string prefix, char suffixOperator)
		{
			var query = _parser.Parse2(queryStr).Should().BeAssignableTo<SrndPrefixQuery>().Which;
			query.Prefix.Should().Be(prefix);
			query.SuffixOperator.Should().Be(suffixOperator);
		}

		[TestCase("pref?", "pref?")]
		public void When_ends_with_question_Then_trunc_query(string queryStr, string truncated)
		{
			_parser.Parse2(queryStr).Should().BeAssignableTo<SrndTruncQuery>()
				.Which.Truncated.Should().Be(truncated);
		}
	}
}
