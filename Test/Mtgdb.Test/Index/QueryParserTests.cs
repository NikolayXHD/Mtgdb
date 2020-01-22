using System.Linq;
using FluentAssertions;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Mtgdb.Data;
using Mtgdb.Data.Index;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class QueryParserTests
	{
		[TestCase(@"texten: ""\+ ? \+""", true)]
		[TestCase(@"nameen: ""a bc""~3", true)]
		[TestCase(@"nameen: ""some /regex/""~", true)]
		[TestCase(@"nameen: ""another (/regex/ OR prefix*)""~.", false)]
		[TestCase(@"nameen: ""another (/regex/ OR prefix*)""~0.", false)]
		[TestCase(@"nameen: ""another (/regex/ OR prefix*)""~1.", false)]
		[TestCase(@"nameen: ""another (/regex/ OR prefix*)""~1.5", false)]
		[Ignore("Surround query")]
		public void Complex_phrase_query_is_parsed(string query, bool isInOrder)
		{
			// o: "under n (your OR its n owner n ' n s) n control"
			// works correctly, while complex phrase query parser not.
			// regex inside query is not supported
			// n or w operator is required between phrase terms
			// phrase terms are not analyzed so "owner's" will not work, must be "owner n ' n s"
			var parser = getParser();
			parser.Parse(query);
		}

		[Test]
		public void Complex_phrase_srnd()
		{
			var parser = getParser();
			var query = parser.Parse("o: \"under (your OR its owner's) control\"");
			query.Should().NotBeNull();
		}

		[TestCase(nameof(Card.ManaCost), @"{w/u}{b}", "{w/u}", "{b}")]
		[TestCase(nameof(Card.TextEn), @"{3}{r/2}{g}: destroy", "{3}", "{r/2}", "{g}", ":", "destroy")]
		public void Mana_symbols_are_treated_as_words(string field, string value, params string[] expectedWords)
		{
			var analyzer = new MtgAnalyzer(new CardDocumentAdapter(null));
			var tokens = analyzer.GetTokens(field, value).Select(_ => _.Term);
			tokens.Should().BeEquivalentTo(expectedWords);
		}

		[Test]
		public void Power_toughness_buff_text_is_treated_as_one_word()
		{
			var analyzer = new MtgAnalyzer(new CardDocumentAdapter(null));
			var tokens = analyzer.GetTokens("TextEn", "Add +1/+1 counter");
			tokens.Select(_ => _.Term).Should().BeEquivalentTo("add", "+1/+1", "counter");
		}

		[Test]
		public void Range_query_is_parsed()
		{
			string queryStr = "Hand:[3 TO ?]";
			var parser = getParser();
			var query = parser.Parse(queryStr);
			query.Should().BeAssignableTo<NumericRangeQuery<int>>();
			var rangeQ = (NumericRangeQuery<int>) query;
			rangeQ.Min.Should().Be(3);
			rangeQ.IncludesMin.Should().BeTrue();
			rangeQ.Max.Should().BeNull();
			rangeQ.IncludesMax.Should().BeTrue();
		}

		private static CardQueryParser getParser()
		{
			var adapter = new CardDocumentAdapter(null);
			var parser = new CardQueryParser(new MtgAnalyzer(adapter), null, adapter, null)
			{
				DefaultOperator = Operator.AND
			};
			return parser;
		}
	}
}
