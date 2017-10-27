using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class CardKeywords
	{
		public const string NoneKeyword = "_none_";
		
		[JsonProperty]
		public int IndexInFile { get; set; }

		[JsonProperty]
		public Dictionary<string, HashSet<string>> KeywordsByProperty { get; set; }

		public void LoadKeywordsFrom(Card card)
		{
			KeywordsByProperty = new Dictionary<string, HashSet<string>>(Str.Comparer);

			for (int i = 0; i < KeywordDefinitions.Values.Count; i++)
			{
				string text = KeywordDefinitions.Getters[i](card);

				string propertyName = KeywordDefinitions.PropertyNames[i];

				if (!string.IsNullOrEmpty(text))
				{
					if (propertyName == nameof(KeywordDefinitions.Ability))
						foreach (var pair in KeywordDefinitions.AbilityHarmfulExplanations)
							text = text.Replace(pair.Key, pair.Value);

					var propertyValues = KeywordDefinitions.Values[i];
					var propertyValuePatterns = KeywordDefinitions.Patterns[i];

					for (int j = 0; j < propertyValues.Count; j++)
					{
						var matches = propertyValuePatterns[j]?.Matches(text)
							.OfType<Match>()
							.ToList();

						if (matches?.Count > 0)
							addKeyword(propertyName, propertyValues[j]);
					}
				}

				if (!KeywordsByProperty.ContainsKey(propertyName))
					KeywordsByProperty.Add(propertyName, new HashSet<string> { NoneKeyword });
			}
		}

		private void addKeyword(string propertyName, string value)
		{
			HashSet<string> keywords;
			if (!KeywordsByProperty.TryGetValue(propertyName, out keywords))
			{
				keywords = new HashSet<string>(Str.Comparer);
				KeywordsByProperty.Add(propertyName, keywords);
			}

			keywords.Add(value);
		}
	}
}