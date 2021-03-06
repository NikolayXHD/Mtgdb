using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mtgdb.Data
{
	public class KeywordQueryTerm
	{
		public string FieldName { get; set; }
		public IList<string> Values { get; set; }
		public IList<Regex> Patterns { get; set; }

		public void AddMatches(string fieldValue, List<Match> matches)
		{
			if (Patterns == null)
				return;

			foreach (Regex pattern in Patterns)
				if (pattern != null)
					matches.AddRange(pattern.Matches(fieldValue).Cast<Match>());
		}
	}
}