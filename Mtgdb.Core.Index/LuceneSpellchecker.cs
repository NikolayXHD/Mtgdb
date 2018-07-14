using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lucene.Net.Contrib;
using Lucene.Net.Index;
using Lucene.Net.Store;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Index
{
	public abstract class LuceneSpellchecker<TId, TObj> : IDisposable
	{
		protected LuceneSpellchecker(IDocumentAdapter<TId, TObj> adapter)
		{
			_adapter = adapter;
			MaxCount = 20;

			_userFields = _adapter.GetUserFields()
				.Select(f => f + ":")
				.OrderBy(Str.Comparer)
				.ToReadOnlyList();

			_allTokensAreField = _userFields
				.Select(_ => TokenType.Field)
				.ToReadOnlyList();
		}

		public virtual void LoadIndex(DirectoryReader indexReader)
		{
			_reader = indexReader;

			Spellchecker = new Spellchecker(_adapter.IsAnyField);
			LoadSpellcheckerIndex();

			IsLoaded = true;
		}



		public IntellisenseSuggest Suggest(string language, TextInputState input)
		{
			string query = input.Text;
			int caret = input.Caret;

			var token = new MtgTolerantTokenizer(query).GetEditedToken(caret);

			if (token == null || token.Type.IsAny(TokenType.ModifierValue))
				return LuceneSpellcheckerConstants.EmptySuggest;

			string userField = token.ParentField ?? string.Empty;

			bool isFieldInvalid = !_adapter.IsAnyField(userField) && !_adapter.IsUserField(userField);

			if (!_adapter.IsSuggestAnalyzedIn(userField, language))
				token = token.PhraseStart ?? token;

			string valuePart =
				StringEscaper.Unescape(query.Substring(token.Position, caret - token.Position));

			if (token.Type.IsAny(TokenType.FieldValue | TokenType.Wildcard))
			{
				IReadOnlyList<string> valueSuggest;

				if (isFieldInvalid || string.IsNullOrEmpty(userField) && string.IsNullOrEmpty(valuePart))
					valueSuggest = ReadOnlyList.Empty<string>();
				else if (_adapter.IsAnyField(userField))
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
				return new IntellisenseSuggest(token, LuceneSpellcheckerConstants.BooleanOperators, LuceneSpellcheckerConstants.AllTokensAreBoolean);

			return LuceneSpellcheckerConstants.EmptySuggest;
		}

		public IntellisenseSuggest CycleValue(string language, TextInputState input, bool backward)
		{
			string query = input.Text;
			int caret = input.Caret;

			var token = new MtgTolerantTokenizer(query).GetEditedToken(caret);

			if (token == null || token.Type.IsAny(TokenType.ModifierValue))
				return null;

			string userField = token.ParentField ?? string.Empty;

			bool isFieldInvalid = !_adapter.IsAnyField(userField) && !_adapter.IsUserField(userField);

			if (isFieldInvalid)
				return null;

			string currentValue;

			if (!_adapter.IsSuggestAnalyzedIn(userField, language))
			{
				token = token.PhraseStart ?? token;
				currentValue = StringEscaper.Unescape(token.GetPhraseText(query));
			}
			else
			{
				currentValue = StringEscaper.Unescape(token.Value);
			}

			var allValues = GetValuesCache(userField, language);

			if (allValues.Count == 0)
				return null;

			var currentIndex =
				allValues.BinarySearchLastIndexOf(str => Str.Comparer.Compare(str, currentValue) <= 0);

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
				foreach (var userField in _adapter.GetUserFields())
				{
					bool matchesNumericType = _adapter.IsFloatField(userField) || _adapter.IsIntField(userField) && valueIsInt;

					if (!matchesNumericType)
						continue;

					var cache = GetValuesCache(userField, language);
					var similarNumbers = getNumericallySimilarValues(cache, value);
					numericValues.UnionWith(similarNumbers);
				}

			var enumerable = numericValues
				.OrderBy(Str.Comparer)
				.Take(MaxCount);

			if (IsLoaded)
			{
				var spellcheckerValues = Spellchecker.SuggestSimilar(null, value, MaxCount);

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
				var cache = GetValuesCache(userField, language);
				return new ListSegment<string>(cache, 0, MaxCount);
			}

			if (_adapter.IsNumericField(userField))
			{
				var cache = GetValuesCache(userField, language);
				return getNumericallySimilarValues(cache, value).Take(MaxCount).ToReadOnlyList();
			}

			if (!IsLoaded)
				return ReadOnlyList.Empty<string>();

			var spellcheckerField = _adapter.GetSpellcheckerFieldIn(userField, language);
			return Spellchecker.SuggestSimilar(spellcheckerField, value, MaxCount);
		}

		private static IEnumerable<string> getNumericallySimilarValues(
			IReadOnlyList<string> cache,
			string value) =>
			cache.Where(_ => _.IndexOf(value, Str.Comparison) >= 0);



		protected IReadOnlyList<string> GetValuesCache(string userField, string lang)
		{
			if (_abort)
				return ReadOnlyList.Empty<string>();

			var spellcheckerField = _adapter.GetSpellcheckerFieldIn(userField, lang);
			IReadOnlyList<string> values;

			lock (ValuesCache)
				if (ValuesCache.TryGetValue(spellcheckerField, out values))
					return values;

			values = _adapter.IsStoredInSpellchecker(userField, lang)
				? Spellchecker?.ReadAllValuesFrom(discriminant: spellcheckerField)
				: _reader?.Invoke(readAllValuesFrom, spellcheckerField);

			if (values == null)
				return ReadOnlyList.Empty<string>();

			lock (ValuesCache)
				ValuesCache[spellcheckerField] = values;

			return values;
		}

		private IReadOnlyList<string> readAllValuesFrom(DirectoryReader reader, string field)
		{
			var rawValues = reader.ReadRawValuesFrom(field);

			IEnumerable<string> enumerable;

			if (_adapter.IsFloatField(field))
				enumerable = rawValues.Select(term => term.TryParseFloat())
					.Where(val => val.HasValue)
					.Select(val => val.Value)
					.Distinct()
					.OrderBy(val => val)
					.Select(val => val.ToString(Str.Culture));

			else if (_adapter.IsIntField(field))
				enumerable = rawValues.Select(term => term.TryParseInt())
					.Where(val => val.HasValue)
					.Select(val => val.Value)
					.Distinct()
					.OrderBy(val => val)
					.Select(val => val.ToString(Str.Culture));

			else
				enumerable = rawValues.Select(term => term.Utf8ToString())
					.Distinct()
					.OrderBy(Str.Comparer);

			var result = enumerable.ToReadOnlyList();
			return result;
		}

		private IReadOnlyList<string> suggestFields(string fieldPart)
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

		protected virtual Directory LoadSpellcheckerIndex()
		{
			var index = new RAMDirectory();

			IsLoading = true;

			IReadOnlyList<(string UserField, string Language)> tasks =
				_adapter.GetUserFields()
					.Where(_adapter.IsIndexedInSpellchecker)
					.SelectMany(f => _adapter.GetFieldLanguages(f).Select(l => (f, l)))
					.ToReadOnlyList();

			TotalFields = tasks.Count;
			var indexedWordsByField = new Dictionary<string, HashSet<string>>(Str.Comparer);

			void getContentToIndex((string UserField, string Language) task)
			{
				var values = _adapter.IsStoredInSpellchecker(task.UserField, task.Language)
					? GetObjectsToIndex().SelectMany(c => 
						_adapter.GetSpellcheckerValues(c, task.UserField, task.Language))
					: GetValuesCache(task.UserField, task.Language);

				var spellcheckerField = _adapter.GetSpellcheckerFieldIn(task.UserField, task.Language);

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

			TotalFields = indexedWordsByField.Count;
			_indexedFields = 0;

			Spellchecker.BeginIndex(index);

			IndexUtils.ForEach(indexedWordsByField, IndexField, suppressParallelism: true);

			Spellchecker.EndIndex();

			IsLoading = false;
			IndexingProgress?.Invoke();

			return index;
		}

		protected abstract IEnumerable<TObj> GetObjectsToIndex();

		protected void IndexField(KeyValuePair<string, HashSet<string>> pair)
		{
			if (_abort)
				return;

			var words = pair.Value.OrderBy(Str.Comparer).ToReadOnlyList();

			foreach (string word in words)
			{
				if (_abort)
					return;

				Spellchecker.IndexWord(pair.Key, word);
			}

			Interlocked.Increment(ref _indexedFields);
			IndexingProgress?.Invoke();
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



		public event Action IndexingProgress;



		public int MaxCount
		{
			get => _allTokensAreValues.Count;
			set => _allTokensAreValues =
				Enumerable.Range(0, value).Select(_ => TokenType.FieldValue).ToReadOnlyList();
		}

		public int IndexedFields =>
			_indexedFields;

		public bool IsLoaded { get; protected set; }
		public bool IsLoading { get; private set; }
		public int TotalFields { get; private set; }

		protected Spellchecker Spellchecker;

		private bool _abort;

		private IReadOnlyList<TokenType> _allTokensAreValues;
		private DirectoryReader _reader;
		private int _indexedFields;

		private readonly IReadOnlyList<string> _userFields;
		private readonly IReadOnlyList<TokenType> _allTokensAreField;
		private readonly IDocumentAdapter<TId, TObj> _adapter;

		protected readonly Dictionary<string, IReadOnlyList<string>> ValuesCache =
			new Dictionary<string, IReadOnlyList<string>>();
	}
}