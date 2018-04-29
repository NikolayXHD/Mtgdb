using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	public class CardKeywords
	{
		public CardKeywords(Card card)
		{
			IndexInFile = card.IndexInFile;

			KeywordsByProperty = new Dictionary<string, HashSet<string>>(Str.Comparer);

			if (!string.IsNullOrEmpty(card.TextEn))
				for (int i = 0; i < KeywordDefinitions.Values.Count; i++)
					foreach (string value in parseValues(card, i))
						addKeyword(KeywordDefinitions.PropertyNames[i], value);
		}

		private static IEnumerable<string> parseValues(Card card, int propertyIndex)
		{
			string text = KeywordDefinitions.Getters[propertyIndex](card);
			var propertyValues = KeywordDefinitions.Values[propertyIndex];
			var patterns = KeywordDefinitions.Patterns[propertyIndex];

			if (string.IsNullOrEmpty(text))
				yield break;

			for (int j = 0; j < propertyValues.Count; j++)
			{
				var matches = patterns[j]?.Matches(text)
					.OfType<Match>()
					.ToList();

				if (matches?.Count > 0)
				{
					string displayText = KeywordRegexUtil.GetKeywordDisplayText(propertyValues[j]);
					yield return displayText;
				}
			}
		}

		private void addKeyword(string propertyName, string value)
		{
			if (!KeywordsByProperty.TryGetValue(propertyName, out var keywords))
			{
				keywords = new HashSet<string>(Str.Comparer);
				KeywordsByProperty.Add(propertyName, keywords);
			}

			keywords.Add(value);
		}



		public int IndexInFile { get; }

		public Dictionary<string, HashSet<string>> KeywordsByProperty { get; }
	}
}