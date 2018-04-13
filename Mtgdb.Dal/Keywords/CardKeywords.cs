using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class CardKeywords
	{
		[JsonProperty]
		public int IndexInFile { get; set; }

		[JsonProperty]
		public Dictionary<string, HashSet<string>> KeywordsByProperty { get; set; }

		public void Parse(Card card)
		{
			KeywordsByProperty = new Dictionary<string, HashSet<string>>(Str.Comparer);

			for (int i = 0; i < KeywordDefinitions.Values.Count; i++)
				foreach (string value in ParseValues(card, i))
					addKeyword(KeywordDefinitions.PropertyNames[i], value);
		}

		public static IEnumerable<string> ParseValues(Card card, int propertyIndex)
		{
			string text = KeywordDefinitions.Getters[propertyIndex](card);
			var propertyValues = KeywordDefinitions.Values[propertyIndex];
			var patterns = KeywordDefinitions.Patterns[propertyIndex];

			if (string.IsNullOrEmpty(text))
				yield break;

			if (propertyIndex == KeywordDefinitions.KeywordsIndex)
				foreach (var pair in KeywordDefinitions.HarmfulAbilityExplanations)
					text = text.Replace(pair.Key, pair.Value);

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
	}
}