using System;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Data
{
	public class CardKeywords
	{
		public CardKeywords(Card card)
		{
			_card = card;
			_keywordsByProperty = new Dictionary<string, HashSet<string>>(Str.Comparer);
		}

		internal HashSet<string> GetPropertyValues(string propertyName)
		{
			var index = KeywordDefinitions.PropertyNames.IndexOf(propertyName, Str.Comparer);
			
			if (index < 0)
				throw new ArgumentException();

			string name = KeywordDefinitions.PropertyNames[index];

			lock (_sync)
			{
				if (_keywordsByProperty.TryGetValue(name, out var values))
					return values;

				values = parseValues(_card, index).ToHashSet(Str.Comparer);

				_keywordsByProperty.Add(name, values);
				return values;
			}
		}

		internal IEnumerable<(string PropertyName, HashSet<string> Values)> GetAllValues()
		{
			return KeywordDefinitions.PropertyNames
				.Select(propertyName => (propertyName, GetPropertyValues(propertyName)));
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
				if (patterns[j] == null)
					continue;

				if (patterns[j].IsMatch(text))
				{
					string displayText = KeywordRegexUtil.GetKeywordDisplayText(propertyValues[j]);
					yield return displayText;
				}
			}
		}



		public int IndexInFile =>
			_card.IndexInFile;

		internal ICollection<string> OtherKeywords => 
			GetPropertyValues(nameof(KeywordDefinitions.Keywords));

		internal ICollection<string> CastKeywords =>
			GetPropertyValues(nameof(KeywordDefinitions.CastKeywords));

		private readonly Card _card;
		private readonly Dictionary<string, HashSet<string>> _keywordsByProperty;
		private readonly object _sync = new object();
	}
}