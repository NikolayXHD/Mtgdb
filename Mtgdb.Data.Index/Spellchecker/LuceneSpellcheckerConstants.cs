using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Contrib;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Data
{
	public static class LuceneSpellcheckerConstants
	{
		public static readonly IReadOnlyList<string> BooleanOperators =
			new List<string> { "AND", "OR", "NOT", "&&", "||", "!", "+", "-" }
				.AsReadOnlyList();

		public static readonly IReadOnlyList<TokenType> AllTokensAreBoolean = BooleanOperators
			.Select(_ => TokenType.Boolean)
			.ToReadOnlyList();

		public static readonly IntellisenseSuggest EmptySuggest =
			new IntellisenseSuggest(null, ReadOnlyList.Empty<string>(), Enumerable.Empty<TokenType>().ToReadOnlyList());
	}
}