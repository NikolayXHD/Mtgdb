using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lucene.Net.Contrib;
using Lucene.Net.Index;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Dal.Index
{
	public class LuceneSpellchecker : IDisposable
	{
		public LuceneSpellchecker(CardRepository repo)
		{
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("suggest");
			MaxCount = 20;

			_repo = repo;
		}

		public string IndexDirectoryParent
		{
			get => Version.Directory.Parent();

			// 0.38 add Dominaria Block
			set => Version = new IndexVersion(value, "0.38");
		}

		public void InvalidateIndex()
		{
			Version.Invalidate();
		}

		internal void LoadIndex(DirectoryReader indexReader)
		{
			_reader = indexReader;

			if (Version.IsUpToDate)
				_spellchecker = openSpellchecker();
			else
				_spellchecker = createSpellchecker();

			IsLoaded = true;
		}

		private Spellchecker openSpellchecker()
		{
			var spellChecker = new Spellchecker();
			spellChecker.Load(Version.Directory);

			return spellChecker;
		}

		private Spellchecker createSpellchecker()
		{
			if (!_repo.IsLocalizationLoadingComplete)
				throw new InvalidOperationException($"{nameof(CardRepository)} must load localizations first");

			IsLoading = true;

			var spellchecker = new Spellchecker();

			IReadOnlyList<(string UserField, string Language)> tasks =
				SpellcheckerDefinitions.GetIndexedUserFields()
					.SelectMany(f => f.GetFieldLanguages().Select(l => (f, l)))
					.ToReadOnlyList();

			TotalFields = tasks.Count;
			var indexedWordsByField = new Dictionary<string, HashSet<string>>(Str.Comparer);

			void getContentToIndex((string UserField, string Language) task)
			{
				if (_abort)
					return;

				var values = task.UserField.IsIndexedInSpellchecker(task.Language)
					? getValuesForSpellchecker(task.UserField, task.Language)
					: getValuesCache(task.UserField, task.Language);

				var spellcheckerField = task.UserField.GetSpellcheckerFieldIn(task.Language);

				HashSet<string> indexedWords;

				lock (indexedWordsByField)
				{
					if (!indexedWordsByField.TryGetValue(spellcheckerField, out indexedWords))
					{
						indexedWords = new HashSet<string>(Str.Comparer);
						indexedWordsByField.Add(spellcheckerField, indexedWords);
					}
				}

				lock (indexedWords)
					foreach (string value in values)
						indexedWords.Add(value);

				Interlocked.Increment(ref _indexedFields);
				IndexingProgress?.Invoke();
			}

			IndexUtils.ForEach(tasks, getContentToIndex);

			if (_abort)
				return null;

			TotalFields = indexedWordsByField.Count;
			_indexedFields = 0;

			Version.CreateDirectory();
			spellchecker.BeginIndex();

			void indexField(KeyValuePair<string, HashSet<string>> pair)
			{
				if (_abort)
					return;

				var discriminant = pair.Key;
				var words = pair.Value.OrderBy(Str.Comparer).ToReadOnlyList();

				foreach (string word in words)
				{
					if (_abort)
						return;

					spellchecker.IndexWord(discriminant, word);
				}

				Interlocked.Increment(ref _indexedFields);
				IndexingProgress?.Invoke();
			}

			IndexUtils.ForEach(indexedWordsByField, indexField, suppressParallelism: true);

			spellchecker.EndIndex(Version.Directory);

			IsLoading = false;
			Version.SetIsUpToDate();
			IndexingProgress?.Invoke();

			return spellchecker;
		}

		public IntellisenseSuggest Suggest(string language, TextInputState input)
		{
			string query = input.Text;
			int caret = input.Caret;

			var token = EditedTokenLocator.GetEditedToken(query, caret);

			if (token == null || token.Type.IsAny(TokenType.ModifierValue))
				return _emptySuggest;

			string userField = token.ParentField ?? string.Empty;

			bool isFieldInvalid =
				!string.IsNullOrEmpty(userField) &&
				!Str.Equals(MtgQueryParser.AnyField, userField) &&
				!DocumentFactory.UserFields.Contains(userField);

			if (!userField.IsSuggestAnalyzedIn(language))
				token = token.PhraseStart ?? token;

			string valuePart = StringEscaper.Unescape(query.Substring(token.Position, caret - token.Position));

			if (token.Type.IsAny(TokenType.FieldValue | TokenType.Wildcard))
			{
				IReadOnlyList<string> valueSuggest;

				if (isFieldInvalid || string.IsNullOrEmpty(userField) && string.IsNullOrEmpty(valuePart))
					valueSuggest = _emptyList;
				else if (string.IsNullOrEmpty(userField) || Str.Equals(userField, MtgQueryParser.AnyField))
					valueSuggest = suggestAllFieldValues(valuePart, language);
				else
					valueSuggest = suggestValues(userField, language, valuePart);

				if (!string.IsNullOrEmpty(userField))
					return new IntellisenseSuggest(token, valueSuggest, _allTokensAreValues);

				var fieldSuggest = suggestFields(fieldPart: valuePart);

				var values = fieldSuggest.Concat(valueSuggest).ToReadOnlyList();

				var types = fieldSuggest.Select(_ => TokenType.Field)
					.Concat(valueSuggest.Select(_ => TokenType.FieldValue))
					.ToReadOnlyList();

				return new IntellisenseSuggest(token, values, types);
			}

			if (token.Type.IsAny(TokenType.Field))
				return new IntellisenseSuggest(token, suggestFields(fieldPart: valuePart), _allTokensAreField);

			if (token.Type.IsAny(TokenType.Boolean))
				return new IntellisenseSuggest(token, _booleanOperators, _allTokensAreBoolean);

			return _emptySuggest;
		}

		public IntellisenseSuggest CycleValue(string language, TextInputState input, bool backward)
		{
			string query = input.Text;
			int caret = input.Caret;

			var token = EditedTokenLocator.GetEditedToken(query, caret);

			if (token == null || token.Type.IsAny(TokenType.ModifierValue))
				return null;

			string userField = token.ParentField ?? string.Empty;

			bool isFieldInvalid =
				!string.IsNullOrEmpty(userField) &&
				!Str.Equals(MtgQueryParser.AnyField, userField) &&
				!DocumentFactory.UserFields.Contains(userField);

			if (isFieldInvalid)
				return null;

			string currentValue;

			if (!userField.IsSuggestAnalyzedIn(language))
			{
				token = token.PhraseStart ?? token;
				currentValue = StringEscaper.Unescape(token.GetPhraseText(query));
			}
			else
			{
				currentValue = StringEscaper.Unescape(token.Value);
			}

			var allValues = getValuesCache(userField, language);

			if (allValues.Count == 0)
				return null;

			var currentIndex = allValues.BinarySearchLastIndexOf(str => Str.Comparer.Compare(str, currentValue) <= 0);

			int increment = backward ? -1 : 1;
			var nextIndex = currentIndex + increment;

			if (nextIndex == allValues.Count)
				nextIndex = 0;
			else if (nextIndex == -1)
				nextIndex = allValues.Count - 1;

			var nextValue = allValues[nextIndex];
			return new IntellisenseSuggest(token, ReadOnlyList.From(nextValue), _allTokensAreValues);
		}

		private IReadOnlyList<string> suggestAllFieldValues(string value, string language)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException($"empty {nameof(value)}", nameof(value));

			var numericValues = new HashSet<string>();

			bool valueIsInt = value.IsInt();
			bool valueIsFloat = value.IsFloat();

			if (valueIsInt || valueIsFloat)
				foreach (var userField in DocumentFactory.UserFields)
				{
					bool matchesNumericType = userField.IsFloatField() || userField.IsIntField() && valueIsInt;

					if (!matchesNumericType)
						continue;

					var cache = getValuesCache(userField, language);
					var similarNumbers = getNumericallySimilarValues(cache, value);
					numericValues.UnionWith(similarNumbers);
				}

			var enumerable = numericValues
				.OrderBy(Str.Comparer)
				.Take(MaxCount);

			if (IsLoaded)
			{
				var spellcheckerValues = _spellchecker.SuggestSimilar(null, value, MaxCount);

				enumerable = enumerable
					.Concat(spellcheckerValues.Where(v => !numericValues.Contains(v)))
					.ToReadOnlyList();
			}

			return enumerable.ToReadOnlyList();
		}

		private IReadOnlyList<string> suggestValues(string userField, string language, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				var cache = getValuesCache(userField, language);
				return new ListSegment<string>(cache, 0, MaxCount);
			}

			if (userField.IsNumericField())
			{
				var cache = getValuesCache(userField, language);
				return getNumericallySimilarValues(cache, value).Take(MaxCount).ToReadOnlyList();
			}

			if (!IsLoaded)
				return _emptyList;

			var spellcheckerField = userField.GetSpellcheckerFieldIn(language);
			return _spellchecker.SuggestSimilar(spellcheckerField, value, MaxCount);
		}



		private static IEnumerable<string> getNumericallySimilarValues(IReadOnlyList<string> cache, string value) =>
			cache.Where(_ => _.IndexOf(value, Str.Comparison) >= 0);

		private IReadOnlyList<string> getValuesCache(string userField, string lang)
		{
			var spellcheckerField = userField.GetSpellcheckerFieldIn(lang);
			IReadOnlyList<string> values;

			lock (_valuesCache)
				if (_valuesCache.TryGetValue(spellcheckerField, out values))
					return values;

			values = userField.IsIndexedInSpellchecker(lang)
				? _spellchecker?.ReadAllValuesFrom(discriminant: spellcheckerField)
				: _reader?.ReadAllValuesFrom(spellcheckerField);

			if (values == null)
				return _emptyList;

			lock (_valuesCache)
				_valuesCache[spellcheckerField] = values;

			return values;
		}



		private IEnumerable<string> getValuesForSpellchecker(string userField, string language)
		{
			if (!_repo.IsLocalizationLoadingComplete)
				throw new InvalidOperationException();

			return _repo.SetsByCode.Values.Where(FilterSet)
				.SelectMany(s => s.Cards
					.SelectMany(c => SpellcheckerDefinitions.SpellcheckerValueGetters[userField](c, language)));
		}

		private static IReadOnlyList<string> suggestFields(string fieldPart)
		{
			var fieldSuggest = _userFields
				.Where(_ => _.IndexOf(fieldPart, Str.Comparison) >= 0)
				.OrderByDescending(_ => _.StartsWith(fieldPart, Str.Comparison))
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



		public string IndexDirectory => Version.Directory;
		public bool IsUpToDate => Version.IsUpToDate;
		public bool IsLoaded { get; private set; }
		public bool IsLoading { get; private set; }

		public int IndexedFields => _indexedFields;
		private int _indexedFields;

		public int TotalFields { get; private set; }

		public int MaxCount
		{
			get => _allTokensAreValues.Count;
			set => _allTokensAreValues = Enumerable.Range(0, value).Select(_ => TokenType.FieldValue).ToReadOnlyList();
		}

		internal IndexVersion Version { get; set; }



		public event Action IndexingProgress;

		public Func<Set, bool> FilterSet { get; set; } = set => true;

		private static readonly IReadOnlyList<string> _userFields = DocumentFactory.UserFields
			.Select(f => f + ":")
			.OrderBy(Str.Comparer)
			.ToReadOnlyList();

		private static readonly IReadOnlyList<string> _booleanOperators =
			new List<string> { "AND", "OR", "NOT", "&&", "||", "!", "+", "-" }
				.AsReadOnlyList();

		private static readonly IReadOnlyList<TokenType> _allTokensAreField = _userFields
			.Select(_ => TokenType.Field)
			.ToReadOnlyList();

		private static readonly IReadOnlyList<TokenType> _allTokensAreBoolean = _booleanOperators
			.Select(_ => TokenType.Boolean)
			.ToReadOnlyList();

		private IReadOnlyList<TokenType> _allTokensAreValues;

		private readonly IntellisenseSuggest _emptySuggest = new IntellisenseSuggest(null,
			_emptyList,
			Enumerable.Empty<TokenType>().ToReadOnlyList());

		private bool _abort;
		private DirectoryReader _reader;
		private Spellchecker _spellchecker;

		private readonly Dictionary<string, IReadOnlyList<string>> _valuesCache =
			new Dictionary<string, IReadOnlyList<string>>();

		private readonly CardRepository _repo;

		private static readonly IReadOnlyList<string> _emptyList = Enumerable.Empty<string>().ToReadOnlyList();
	}
}