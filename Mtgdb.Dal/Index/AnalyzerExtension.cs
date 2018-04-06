using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Contrib;

namespace Mtgdb.Dal.Index
{
	public static class AnalyzerExtension
	{
		public static IEnumerable<(string Term, int Offset)> GetTokens(this Analyzer analyzer, string field, string value)
		{
			using (var tokenStream = analyzer.GetTokenStream(field ?? string.Empty, value))
			{
				tokenStream.Reset();

				while (tokenStream.IncrementToken())
				{
					string term = tokenStream.GetAttribute<ICharTermAttribute>().ToString();
					int offset = tokenStream.GetAttribute<IOffsetAttribute>().StartOffset;

					yield return (term, offset);
				}
			}
		}

		public static string GetValueExpression(this Analyzer analyzer, string field, string value)
		{
			var builder = new StringBuilder();

			var valueTokens = GetTokens(analyzer, field, value).ToList();

			if (valueTokens.Count == 0)
				return null;

			if (valueTokens.Count > 1)
				builder.Append('"');

			for (int i = 0; i < valueTokens.Count; i++)
			{
				var token = valueTokens[i];

				if (i > 0)
				{
					var prevToken = valueTokens[i - 1];

					if (prevToken.Offset + prevToken.Term.Length < token.Offset)
						builder.Append(' ');
				}

				builder.Append(StringEscaper.Escape(token.Term));
			}

			if (valueTokens.Count > 1)
				builder.Append('"');

			return builder.ToString();
		}
	}
}