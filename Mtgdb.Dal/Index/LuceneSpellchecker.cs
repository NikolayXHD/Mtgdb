using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Lucene.Net.Index;
using Lucene.Net.Contrib;
using Lucene.Net.Store;
using ReadOnlyCollectionsExtensions;
using Token = Lucene.Net.Contrib.Token;

namespace Mtgdb.Dal.Index
{
	public class LuceneSpellchecker : IDisposable
	{
		public LuceneSpellchecker()
		{
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("suggest");
			_stringDistance = new DamerauLevenstineDistance();
		}

		public string IndexDirectory => Version.Directory;

		public string IndexDirectoryParent
		{
			get => Version.Directory.Parent();

			// 0.27 after refactoring, to be on the safe side
			set => Version = new IndexVersion(value, "0.27");
		}

		public bool IsUpToDate => Version.IsUpToDate;

		public void InvalidateIndex()
		{
			Version.Invalidate();
		}

		internal void LoadIndex(MtgdbAnalyzer analyzer, CardRepository repository, DirectoryReader indexReader)
		{
			_cardRepository = repository;
			_reader = indexReader;

			if (Version.IsUpToDate)
				_spellchecker = openSpellchecker();
			else
				_spellchecker = createSpellchecker(repository, analyzer);

			IsLoaded = true;
		}

		private Spellchecker openSpellchecker()
		{
			var spellcheckerIndex = FSDirectory.Open(Version.Directory);
			var spellChecker = new Spellchecker(spellcheckerIndex, _stringDistance);

			return spellChecker;
		}

		private Spellchecker createSpellchecker(CardRepository repository, MtgdbAnalyzer analyzer)
		{
			if (!repository.IsLocalizationLoadingComplete)
				throw new InvalidOperationException($"{nameof(CardRepository)} must load localiztions first");

			IsLoading = true;

			Version.CreateDirectory();

			var spellcheckerIndex = FSDirectory.Open(Version.Directory);
			var spellchecker = new Spellchecker(spellcheckerIndex, _stringDistance);

			var fields = new HashSet<string>();
			fields.UnionWith(DocumentFactory.TextFields);
			fields.ExceptWith(DocumentFactory.LimitedValueGetters.Keys);
			fields.ExceptWith(DocumentFactory.CombinatoricValueGetters.Keys);
			fields.ExceptWith(DocumentFactory.CombinatoricValueFields);

			spellchecker.BeginIndex();

			var indexedWords = new HashSet<string>(Str.Comparer);
			var indexedValues = new HashSet<string>(Str.Comparer);

			TotalSets = repository.SetsByCode.Count;

			foreach (var set in repository.SetsByCode.Values)
			{
				if (_abort)
					break;

				if (!FilterSet(set))
					continue;

				foreach (var card in set.Cards)
				{
					if (_abort)
						break;

					var doc = card.Document;

					foreach (string fieldName in fields)
					{
						if (_abort)
							break;

						var docField = doc.GetField(fieldName.ToLowerInvariant());

						if (docField == null)
							continue;

						bool isAnalyzed = DocumentFactory.NotAnalyzedFields.Contains(fieldName);

						string value = docField.GetStringValue()?.ToLowerInvariant();

						if (string.IsNullOrEmpty(value))
							continue;

						if (isAnalyzed && !indexedValues.Add(value) || !isAnalyzed && indexedWords.Contains(value))
							continue;

						foreach (var token in analyzer.GetTokens(fieldName, value))
							if (indexedWords.Add(token.Term))
								spellchecker.IndexWord(token.Term);
					}
				}

				IndexedSets++;
				IsLoading = IndexedSets < TotalSets;
				IndexingProgress?.Invoke();
			}

			spellchecker.EndIndex();

			if (_abort)
				return null;

			IsLoading = false;

			Version.SetIsUpToDate();
			return spellchecker;
		}

		public IntellisenseSuggest Suggest(string query, int caret, string language)
		{
			var token = EditedTokenLocator.GetEditedToken(query, caret);

			if (token == null || token.Type.IsAny(TokenType.ModifierValue))
				return _emptySuggest;

			string valuePart = token.Value.Substring(0, caret - token.Position);

			if (token.Type.IsAny(TokenType.FieldValue))
			{
				var valueSuggest = suggestValues(language, token, StringEscaper.Unescape(valuePart));

				if (!string.IsNullOrEmpty(token.ParentField))
					return new IntellisenseSuggest(token, valueSuggest, _allTokensAreValues);
				
				var fieldSuggest = suggestFields(valuePart);

				var values = fieldSuggest.Concat(valueSuggest).ToReadOnlyList();

				var types = fieldSuggest.Select(_ => TokenType.Field)
					.Concat(valueSuggest.Select(_ => TokenType.FieldValue))
					.ToReadOnlyList();

				return new IntellisenseSuggest(token, values, types);
			}

			if (token.Type.IsAny(TokenType.Field))
				return new IntellisenseSuggest(token, suggestAllFields(valuePart), _allTokensAreField);

			if (token.Type.IsAny(TokenType.Boolean))
				return new IntellisenseSuggest(token, _booleanOperators, _allTokensAreBoolean);

			return _emptySuggest;
		}

		private IReadOnlyList<string> suggestValues(string language, Token token, string valuePart)
		{
			if (!IsLoaded)
				return _emptySuggest.Values;

			return SuggestValues(valuePart.ToLowerInvariant(), token.ParentField, language);
		}



		internal IReadOnlyList<string> SuggestValues(string value, string field, string language)
		{
			if (!IsLoaded)
				throw new InvalidOperationException("Index must be loaded first");

			bool isFieldInvalid =
				!string.IsNullOrEmpty(field) &&
				!Str.Equals(field, NumericAwareQueryParser.AnyField) &&
				!DocumentFactory.UserFields.Contains(field);

			if (isFieldInvalid)
				return _emptySuggest.Values;

			var valueIsNumeric = isValueNumeric(value);

			if (string.IsNullOrEmpty(field) || field == NumericAwareQueryParser.AnyField)
			{
				var valuesSet = new HashSet<string>();

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (userField.IsNumericField() && !valueIsNumeric)
						continue;

					var values = suggestFieldValues(value, userField, language, MaxCount / 4);
					valuesSet.UnionWith(values);
				}

				return getMostSimilarValues(valuesSet.ToReadOnlyList(), value, MaxCount);
			}

			return suggestFieldValues(value, field, language, MaxCount);
		}

		private IReadOnlyList<string> suggestFieldValues(string value, string field, string language, int maxCount)
		{
			if (DocumentFactory.NumericFields.Contains(field))
				return getNumericSuggest(value, field, language, maxCount);

			if (DocumentFactory.LimitedValueGetters.ContainsKey(field))
				return getLimitedValuesSuggest(field, value, maxCount);

			if (DocumentFactory.CombinatoricValueGetters.ContainsKey(field))
				return getCombinatoricValuesSuggest(field, value, maxCount);

			if (_legalityFields.Contains(field))
				return getMostSimilarValues(_legalityValues, value, _legalityValues.Count);

			if (string.IsNullOrEmpty(value) || DocumentFactory.CombinatoricValueFields.Contains(field))
			{
				var values = getAllFieldValues(field, language, _reader.MaxDoc).Distinct().ToReadOnlyList();
				return getMostSimilarValues(values, value, maxCount);
			}

			return _spellchecker.SuggestSimilar(value, maxCount, _reader, DocumentFactory.Localize(field, language));
		}

		private IReadOnlyList<string> getLimitedValuesSuggest(string field, string value, int maxCount)
		{
			if (!_cardRepository.IsLoadingComplete)
				return _emptySuggest.Values;

			var values = getAllLimitedValues(field);

			return getMostSimilarValues(values, value, maxCount);
		}

		private IReadOnlyList<string> getCombinatoricValuesSuggest(string field, string value, int maxCount)
		{
			if (!_cardRepository.IsLoadingComplete)
				return _emptySuggest.Values;

			var values = getAllCombinatoricValues(field);

			return getMostSimilarValues(values, value, maxCount);
		}



		private IReadOnlyList<string> getMostSimilarValues(IReadOnlyList<string> values, string value, int maxCount)
		{
			if (string.IsNullOrEmpty(value))
				return values.OrderBy(Str.Comparer).Take(maxCount).ToReadOnlyList();

			var similarities = values.Select(_ => _stringDistance.GetDistance(value, _))
				.ToArray();

			return Enumerable.Range(0, values.Count)
				.OrderByDescending(i => similarities[i])
				.Select(i => values[i])
				.Take(maxCount)
				.ToReadOnlyList();
		}

		private IReadOnlyList<string> getAllLimitedValues(string field)
		{
			if (_limitedFieldValues.TryGetValue(field, out var values))
				return values;

			values = _cardRepository.Cards
				.Select(DocumentFactory.LimitedValueGetters[field])
				.Where(_ => _ != null)
				.Select(_ => _.Trim().ToLowerInvariant())
				.Distinct()
				.ToReadOnlyList();

			_limitedFieldValues[field] = values;

			return values;
		}

		private IReadOnlyList<string> getAllCombinatoricValues(string field)
		{
			if (_limitedFieldValues.TryGetValue(field, out var values))
				return values;

			values = _cardRepository.Cards
				.SelectMany(DocumentFactory.CombinatoricValueGetters[field])
				.Where(_ => _ != null)
				.Select(_ => _.Trim().ToLowerInvariant())
				.Distinct()
				.ToReadOnlyList();

			_limitedFieldValues[field] = values;

			return values;
		}

		private IReadOnlyList<string> getNumericSuggest(string value, string field, string language, int maxCount)
		{
			var values = getAllFieldValues(field, language, _reader.MaxDoc)
				.Where(_ => _.IndexOf(value, Str.Comparison) >= 0);

			if (field.IsFloatField())
				values = values.OrderBy(float.Parse);
			else if (field.IsIntField())
				values = values.OrderBy(int.Parse);

			return values.Take(maxCount).ToReadOnlyList();
		}

		private IEnumerable<string> getAllFieldValues(string field, string language, int maxCount)
		{
			field = DocumentFactory.Localize(field, language);
			int count = 0;

			var terms = MultiFields.GetTerms(_reader, field);
			var iterator = terms.GetIterator(reuse: null);

			if (field.IsFloatField())
			{
				while (iterator.Next() != null && count <= maxCount)
				{
					var value = iterator.Term.TryParseFloat();

					if (value.HasValue)
					{
						string result = value.ToString();
						yield return result;
						count++;
					}
				}
			}
			else if (field.IsIntField())
			{
				while (iterator.Next() != null && count <= maxCount)
				{
					var value = iterator.Term.TryParseInt();

					if (value.HasValue)
					{
						string result = value.ToString();
						yield return result;
						count++;
					}
				}
			}
			else
			{
				while (iterator.Next() != null && count <= maxCount)
				{
					if (iterator.Term == null)
						continue;

					var value = iterator.Term.Utf8ToString();
					yield return value;
					count++;
				}
			}
		}

		private IReadOnlyList<string> suggestAllFields(string field)
		{
			if (string.IsNullOrEmpty(field))
				return _userFields;

			var similarities = _userFields.Select(_ => _stringDistance.GetDistance(field, _))
				.ToArray();

			var userFields = Enumerable.Range(0, _userFields.Count)
				.OrderByDescending(i => similarities[i])
				.Select(i => _userFields[i])
				.ToReadOnlyList();

			return userFields;
		}

		private static IReadOnlyList<string> suggestFields(string valuePart)
		{
			var fieldSuggest = _userFields
				.Where(_ => _.IndexOf(valuePart, Str.Comparison) >= 0)
				.OrderBy(Str.Comparer)
				.ToReadOnlyList();

			return fieldSuggest;
		}

		public void Dispose()
		{
			abortLoading();

			IsLoaded = false;
			_reader?.Dispose();
		}

		private void abortLoading()
		{
			if (!IsLoading)
				return;

			_abort = true;

			while (IsLoading)
				Thread.Sleep(100);

			_abort = false;
		}

		private static bool isValueNumeric(string queryText)
		{
			bool valueIsNumeric =
				int.TryParse(queryText, NumberStyles.Integer, Str.Culture, out _) ||
				float.TryParse(queryText, NumberStyles.Float, Str.Culture, out _);
			return valueIsNumeric;
		}

		public Func<Set, bool> FilterSet { get; set; } = set => true;

		public bool IsLoaded { get; private set; }
		public bool IsLoading { get; private set; }

		public event Action IndexingProgress;

		public int IndexedSets { get; private set; }
		public int TotalSets { get; private set; }

		private Spellchecker _spellchecker;
		internal IndexVersion Version { get; set; }

		public int MaxCount
		{
			get => _allTokensAreValues.Count;
			set => _allTokensAreValues = Enumerable.Range(0, value).Select(_ => TokenType.FieldValue).ToReadOnlyList();
		}

		private bool _abort;
		private DirectoryReader _reader;

		private readonly DamerauLevenstineDistance _stringDistance;

		private readonly Dictionary<string, IReadOnlyList<string>> _limitedFieldValues = new Dictionary<string, IReadOnlyList<string>>();
		private CardRepository _cardRepository;

		private static readonly IReadOnlyList<string> _userFields = DocumentFactory.UserFields
			.Select(f => f + ":")
			.OrderBy(Str.Comparer)
			.ToReadOnlyList();

		private static readonly IReadOnlyList<string> _booleanOperators =
			new List<string> { "AND", "OR", "NOT", "&&", "||", "!", "+", "-" }
				.AsReadOnlyList();

		private static readonly HashSet<string> _legalityFields = new HashSet<string>(Str.Comparer)
		{
			nameof(Card.LegalIn),
			nameof(Card.RestrictedIn),
			nameof(Card.BannedIn)
		};

		private static readonly IReadOnlyList<string> _legalityValues = Legality.Formats;

		private static readonly IReadOnlyList<TokenType> _allTokensAreField = _userFields
			.Select(_ => TokenType.Field)
			.ToReadOnlyList();

		private static readonly IReadOnlyList<TokenType> _allTokensAreBoolean = _booleanOperators
			.Select(_ => TokenType.Boolean)
			.ToReadOnlyList();

		private IReadOnlyList<TokenType> _allTokensAreValues = Enumerable.Range(0, 20)
			.Select(_ => TokenType.FieldValue)
			.ToReadOnlyList();

		private readonly IntellisenseSuggest _emptySuggest = new IntellisenseSuggest(null,
			Enumerable.Empty<string>().ToReadOnlyList(),
			Enumerable.Empty<TokenType>().ToReadOnlyList());
	}
}