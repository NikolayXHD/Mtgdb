using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mtgdb.Controls;
using Mtgdb.Data;

namespace Mtgdb.Gui
{
	public class QuickFilterFacade
	{
		public QuickFilterFacade(
			IList<IList<Regex>> patterns,
			IList<IList<string>> values,
			IList<string> fieldNames,
			IList<string> fieldNamesDisplay,
			KeywordSearcher searcher)
		{
			if (fieldNames.Count != values.Count || fieldNames.Count != fieldNamesDisplay.Count)
				throw new ArgumentException(@"invalid length");

			_values = values;
			_searcher = searcher;
			_patterns = patterns;

			_indexByFieldNameDisplay = Enumerable.Range(0, fieldNamesDisplay.Count)
				.GroupBy(i => fieldNamesDisplay[i])
				.ToDictionary(
					gr => gr.Key,
					gr => new HashSet<int>(gr));

			Required = Enumerable.Range(0, fieldNames.Count)
				.Select(i => new KeywordQueryTerm { FieldName = fieldNames[i] })
				.ToArray();

			Prohibited = Enumerable.Range(0, fieldNames.Count)
				.Select(i => new KeywordQueryTerm { FieldName = fieldNames[i] })
				.ToArray();

			RequiredSome = Enumerable.Range(0, fieldNames.Count)
				.Select(i => new KeywordQueryTerm { FieldName = fieldNames[i] })
				.ToArray();

			Ignored = Enumerable.Range(0, fieldNames.Count)
				.Select(i => new KeywordQueryTerm { FieldName = fieldNames[i] })
				.ToArray();
		}

		public void ApplyValueStates(IList<IList<FilterValueState>> states)
		{
			if (states.Count != _values.Count)
				throw new ArgumentException(@"states count");

			for (int c = 0; c < states.Count; c++)
			{
				var stateArr = states[c];
				var filterValues = _values[c];
				var patterns = _patterns[c];

				Required[c].Values = Enumerable.Range(0, stateArr.Count)
					.Where(i => stateArr[i] == FilterValueState.Required)
					.Select(i => filterValues[i])
					.ToArray();

				Required[c].Patterns = Enumerable.Range(0, stateArr.Count)
					.Where(i => stateArr[i] == FilterValueState.Required)
					.Select(i => patterns[i])
					.ToArray();

				RequiredSome[c].Values = Enumerable.Range(0, stateArr.Count)
					.Where(i => stateArr[i] == FilterValueState.RequiredSome)
					.Select(i => filterValues[i])
					.ToArray();

				RequiredSome[c].Patterns = Enumerable.Range(0, stateArr.Count)
					.Where(i => stateArr[i] == FilterValueState.RequiredSome)
					.Select(i => patterns[i])
					.ToArray();

				if (stateArr.All(_ => _ != FilterValueState.Prohibited))
					Ignored[c].Patterns = _emptyPatterns;
				else
					Ignored[c].Patterns = Enumerable.Range(0, stateArr.Count)
						.Where(i => stateArr[i] == FilterValueState.Ignored)
						.Select(i => patterns[i])
						.ToArray();

				Prohibited[c].Values = Enumerable.Range(0, stateArr.Count)
					.Where(i => stateArr[i] == FilterValueState.Prohibited)
					.Select(i => filterValues[i])
					.ToArray();
			}


			if (!_searcher.IsLoaded)
			{
				_matchingCardIds = null;
				return;
			}

			if (Required.Concat(RequiredSome).Concat(Prohibited).Sum(_=>_.Values.Count) == 0)
			{
				_matchingCardIds = null;
				return;
			}

			_matchingCardIds = _searcher
				.GetCardIds(Required, RequiredSome, Prohibited)
				.ToHashSet();
		}

		private KeywordQueryTerm[] Required { get; }
		private KeywordQueryTerm[] Prohibited { get; }
		private KeywordQueryTerm[] RequiredSome { get; }
		private KeywordQueryTerm[] Ignored { get; }

		public bool Evaluate(Card c)
		{
			if (_matchingCardIds == null)
				return true;

			return _matchingCardIds.Contains(c.IndexInFile);
		}

		public List<TextRange> GetMatches(string fieldValue, string fieldName)
		{
			if (!_indexByFieldNameDisplay.TryGetValue(fieldName, out var indexes))
				return null;

			var matches = new List<Match>();

			bool highlightIgnoredValues = fieldName != nameof(Card.Text);

			foreach (int i in indexes)
			{
				Required[i].AddMatches(fieldValue, matches);
				RequiredSome[i].AddMatches(fieldValue, matches);

				if (highlightIgnoredValues)
					Ignored[i].AddMatches(fieldValue, matches);
			}

			return matches
				.Select(TextRange.Copy)
				.ToList();
		}



		private HashSet<int> _matchingCardIds;

		private static readonly List<Regex> _emptyPatterns = new List<Regex>();

		private readonly IList<IList<string>> _values;
		private readonly KeywordSearcher _searcher;
		private readonly IList<IList<Regex>> _patterns;
		private readonly Dictionary<string, HashSet<int>> _indexByFieldNameDisplay;
	}
}