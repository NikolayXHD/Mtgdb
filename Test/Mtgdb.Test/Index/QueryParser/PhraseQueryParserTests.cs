using FluentAssertions;
using Lucene.Net.Index;
using Lucene.Net.Search.Spans;
using Mtgdb.Data;
using Mtgdb.Data.Index;
using NUnit.Framework;

namespace Mtgdb.Test
{
	public class PhraseQueryParserTests
	{
		private const string F = "f";

		[Test]
		public void Multi_token_word_is_treated_as_phrase()
		{
			var parser = getParser();
			var query = parser.Parse("your OR its owner's");
			query.Should().BeEquivalentTo(
				new SpanOrQuery(
					new SpanTermQuery(new Term(F, "your")),
					new SpanNearQuery(
						new SpanQuery[]
						{
							new SpanTermQuery(new Term(F, "its")),
							new SpanNearQuery(
								new SpanQuery[]
								{
									new SpanTermQuery(new Term(F, "owner")),
									new SpanTermQuery(new Term(F, "'")),
									new SpanTermQuery(new Term(F, "s")),
								},
								0, true)
						}, 0, true)));
		}

		[Test]
		public void Empty_phrase_is_expanded()
		{
			var parser = getParser();
			var query = parser.Parse("counter (target OR >) spell");
			query.Should().BeAssignableTo<SpanOrQuery>().Which.Should().BeEquivalentTo(
				new SpanOrQuery(
					new SpanNearQuery(
						new SpanQuery[]
						{
							new SpanTermQuery(new Term(F, "counter")),
							new SpanTermQuery(new Term(F, "target")),
							new SpanTermQuery(new Term(F, "spell")),
						}, 0, true),
					new SpanNearQuery(
						new SpanQuery[]
						{
							new SpanTermQuery(new Term(F, "counter")),
							new SpanTermQuery(new Term(F, "spell")),
						}, 0, true)
				));
		}

		[Test]
		public void When_empty_phrase_is_nested_in_boolean_tree_Then_it_is_still_expanded()
		{
			var parser = getParser();
			var query = parser.Parse("(counter OR (target OR >)) spell");
			query.Should().BeAssignableTo<SpanOrQuery>().Which.Should().BeEquivalentTo(
				new SpanOrQuery(
					new SpanNearQuery(
						new SpanQuery[]
						{
							new SpanOrQuery(
								new SpanTermQuery(new Term(F, "counter")),
								new SpanTermQuery(new Term(F, "target"))),
							new SpanTermQuery(new Term(F, "spell")),
						}, 0, true),
					new SpanTermQuery(new Term(F, "spell"))
				));
		}


		[Test]
		public void Two_empty_phrases_are_expanded_as_cartesian_product()
		{
			var parser = getParser();
			var query = parser.Parse("(> OR counter) (target OR >) spell");
			query.Should().BeAssignableTo<SpanOrQuery>().Which.Should().BeEquivalentTo(
				new SpanOrQuery(
					new SpanNearQuery(
						new SpanQuery[]
						{
							new SpanTermQuery(new Term(F, "counter")),
							new SpanTermQuery(new Term(F, "target")),
							new SpanTermQuery(new Term(F, "spell")),
						}, 0, true),
					new SpanNearQuery(
						new SpanQuery[]
						{
							new SpanTermQuery(new Term(F, "target")),
							new SpanTermQuery(new Term(F, "spell")),
						}, 0, true),
					new SpanNearQuery(
						new SpanQuery[]
						{
							new SpanTermQuery(new Term(F, "counter")),
							new SpanTermQuery(new Term(F, "spell")),
						}, 0, true),
					new SpanTermQuery(new Term(F, "spell"))
				));
		}

		private static PhraseQueryParser getParser() =>
			new PhraseQueryParser("*", new MtgAnalyzer(new CardDocumentAdapter(null)))
			{
				PhraseField = F,
				InOrder = true,
				Slop = 0
			};
	}
}
