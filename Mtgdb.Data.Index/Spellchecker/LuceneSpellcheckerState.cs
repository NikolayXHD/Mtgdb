using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lucene.Net.Index;
using Lucene.Net.Store;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Data
{
	public abstract class LuceneSpellcheckerState<TId, TDoc> : IDisposable
	{
		protected LuceneSpellcheckerState(
			Spellchecker spellchecker,
			LuceneSearcherState<TId, TDoc> searcherState,
			IDocumentAdapter<TId, TDoc> adapter,
			Func<int> maxCount,
			bool loaded)
		{
			_spellchecker = spellchecker;
			_reader = searcherState.Reader;
			_adapter = adapter;
			_maxCount = maxCount;

			IsLoaded = loaded;
		}

		protected abstract IEnumerable<TDoc> GetObjectsToIndex();

		public IReadOnlyList<string> SuggestAllFieldValues(string value, string language)
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
				.Take(_maxCount());

			if (IsLoaded)
			{
				var spellcheckerValues = _spellchecker.SuggestSimilar(null, value, _maxCount());

				enumerable = enumerable
					.Concat(spellcheckerValues.Where(v => !numericValues.Contains(v)))
					.ToReadOnlyList();
			}

			return enumerable.ToReadOnlyList();
		}

		public IReadOnlyList<string> SuggestValues(string userField, string language, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				var cache = GetValuesCache(userField, language);
				return new ListSegment<string>(cache, 0, _maxCount());
			}

			if (_adapter.IsNumericField(userField))
			{
				var cache = GetValuesCache(userField, language);
				return getNumericallySimilarValues(cache, value).Take(_maxCount()).ToReadOnlyList();
			}

			if (!IsLoaded)
				return ReadOnlyList.Empty<string>();

			var spellcheckerField = _adapter.GetSpellcheckerFieldIn(userField, language);
			return _spellchecker.SuggestSimilar(spellcheckerField, value, _maxCount());
		}

		public IReadOnlyList<string> GetValuesCache(string userField,  string lang)
		{
			if (_abort)
				return ReadOnlyList.Empty<string>();

			var spellcheckerField = _adapter.GetSpellcheckerFieldIn(userField, lang);
			IReadOnlyList<string> values;

			lock (_valuesCache)
				if (_valuesCache.TryGetValue(spellcheckerField, out values))
					return values;

			values = _adapter.IsStoredInSpellchecker(userField, lang)
				? _spellchecker?.ReadAllValuesFrom(discriminant: spellcheckerField)
				: _reader?.Invoke1(readAllValuesFrom, spellcheckerField);

			if (values == null)
				return ReadOnlyList.Empty<string>();

			lock (_valuesCache)
				_valuesCache[spellcheckerField] = values;

			return values;
		}

		public Directory CreateIndex()
		{
			if (IsLoaded)
				throw new InvalidOperationException();

			IsLoading = true;

			var index = new RAMDirectory();

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
			}

			IndexUtils.ForEach(tasks, getContentToIndex);

			TotalFields = indexedWordsByField.Count;
			_indexedFields = 0;

			_spellchecker.BeginIndex(index);

			IndexUtils.ForEach(
				indexedWordsByField,
				indexField,
				suppressParallelism: true);

			if (_abort)
				return null;

			_spellchecker.EndIndex();

			IsLoading = false;
			IndexingProgress?.Invoke();

			IsLoaded = true;

			return index;
		}

		public void Dispose()
		{
			_abort = true;

			_spellchecker?.Dispose();
			_reader?.Dispose();

			IsLoaded = false;
		}



		private void indexField(KeyValuePair<string, HashSet<string>> pair)
		{
			if (_abort)
				return;

			var words = pair.Value.OrderBy(Str.Comparer).ToReadOnlyList();

			foreach (string word in words)
			{
				if (_abort)
					return;

				_spellchecker.IndexWord(pair.Key, word);
			}

			Interlocked.Increment(ref _indexedFields);
			IndexingProgress?.Invoke();
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

		private static IEnumerable<string> getNumericallySimilarValues(IReadOnlyList<string> cache, string value) =>
			cache.Where(_ => _.IndexOf(value, Str.Comparison) >= 0);



		public bool IsLoaded { get; private set; }
		public bool IsLoading { get; private set; }
		public int TotalFields { get; private set; }
		public int IndexedFields => _indexedFields;

		private int _indexedFields;
		private bool _abort;

		private readonly Func<int> _maxCount;
		private readonly Spellchecker _spellchecker;
		private readonly DirectoryReader _reader;
		private readonly IDocumentAdapter<TId, TDoc> _adapter;

		private readonly Dictionary<string, IReadOnlyList<string>> _valuesCache =
			new Dictionary<string, IReadOnlyList<string>>();

		public event Action IndexingProgress;
	}
}