using System.Linq;
using FluentAssertions;
using Lucene.Net.QueryParsers.Classic;
using Mtgdb.Data;
using Mtgdb.Data.Index;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class QueryParserTests
	{
		[TestCase(@"o: ""\+ ? \+""")]
		public void Complex_phrase_query_is_parsed(string query)
		{
			var adapter = new CardDocumentAdapter(null);
			var parser = new CardQueryParser(new MtgAnalyzer(adapter), null, adapter, null)
			{
				DefaultOperator = Operator.AND
			};

			var q = parser.Parse(query);
			q.Should().BeAssignableTo<ComplexPhraseQueryParserPatched.ComplexPhraseQuery>();
		}

		[Test]
		public void Power_toughness_buff_text_is_treated_as_one_word()
		{
			var analyzer = new MtgAnalyzer(new CardDocumentAdapter(null));
			var tokens = analyzer.GetTokens("TextEn", "Add +1/+1 counter");
			tokens.Select(_ => _.Term).Should().BeEquivalentTo("add", "+1/+1", "counter");
		}
	}
}
