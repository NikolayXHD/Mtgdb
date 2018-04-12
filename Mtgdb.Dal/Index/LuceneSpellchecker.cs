using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lucene.Net.Index;
using Lucene.Net.Contrib;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Dal.Index
{
	public class LuceneSpellchecker : IDisposable
	{
		public LuceneSpellchecker(CardRepository repo)
		{
			_stringSimilarity = new DamerauLevenshteinSimilarity();
			_repo = repo;

			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("suggest");
			MaxCount = 20;
		}

		public string IndexDirectoryParent
		{
			get => Version.Directory.Parent();

			// 0.32 not double-indexing not analyzed duplicates
			set => Version = new IndexVersion(value, "0.32");
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
			var spellChecker = new Spellchecker(_stringSimilarity);
			spellChecker.Load(Version.Directory);

			return spellChecker;
		}

		private Spellchecker createSpellchecker()
		{
			if (!_repo.IsLocalizationLoadingComplete)
				throw new InvalidOperationException($"{nameof(CardRepository)} must load localizations first");

			IsLoading = true;

			var spellchecker = new Spellchecker(_stringSimilarity);

			IReadOnlyList<(string UserField, string Language)> tasks =
				SpellcheckerDefinitions.GetIndexedUserFields()
					.SelectMany(f => f.GetFieldLanguages().Select(l => (f, l)))
					.ToReadOnlyList();

			TotalFields = tasks.Count;
			var indexedWordsByDiscriminant = new Dictionary<string, HashSet<string>>(Str.Comparer);

			Version.CreateDirectory();
			spellchecker.BeginIndex();

			void execute((string UserField, string Language) task)
			{
				if (_abort)
					return;

				var discriminant = task.UserField.GetDiscriminantIn(task.Language);

				var values = task.UserField.SpellchekFromOriginalIndexIn(task.Language)
					? getValuesCache(task.UserField, task.Language)
					: getValuesForSpellchecker(task.UserField, task.Language);

				foreach (string value in values)
				{
					bool isNewWord;
					lock (indexedWordsByDiscriminant)
					{
						if (!indexedWordsByDiscriminant.TryGetValue(discriminant, out var indexedWords))
						{
							indexedWords = new HashSet<string>(Str.Comparer);
							indexedWordsByDiscriminant.Add(discriminant, indexedWords);
						}

						isNewWord = indexedWords.Add(value);
					}

					if (isNewWord)
						spellchecker.IndexWord(discriminant, value);
				}

				Interlocked.Increment(ref _indexedFields);
				IndexingProgress?.Invoke();
			}

			var parallelOptions = IndexUtils.ParallelOptions;
			if (parallelOptions.MaxDegreeOfParallelism > 1)
				Parallel.ForEach(tasks, parallelOptions, execute);
			else
				foreach (var task in tasks)
					execute(task);

			if (_abort)
				return null;

			spellchecker.EndIndex(Version.Directory);

			IsLoading = false;
			Version.SetIsUpToDate();
			IndexingProgress?.Invoke();

			return spellchecker;
		}

		public IntellisenseSuggest Suggest(string language, string query, int caret)
		{
			var token = EditedTokenLocator.GetEditedToken(query, caret);

			if (token == null || token.Type.IsAny(TokenType.ModifierValue))
				return _emptySuggest;

			string userField = token.ParentField ?? string.Empty;

			bool isFieldInvalid =
				!string.IsNullOrEmpty(userField) &&
				!Str.Equals(MtgQueryParser.AnyField, userField) &&
				!DocumentFactory.UserFields.Contains(userField);

			if (!userField.IsAnalyzedIn(language))
				token = token.PhraseStart ?? token;

			string valuePart = StringEscaper.Unescape(query.Substring(token.Position, caret - token.Position));

			if (token.Type.IsAny(TokenType.FieldValue))
			{
				IReadOnlyList<string> valueSuggest;

				if (isFieldInvalid)
					valueSuggest = _emptySuggest.Values;
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
				return new IntellisenseSuggest(token, suggestAllFields(fieldPart: valuePart), _allTokensAreField);

			if (token.Type.IsAny(TokenType.Boolean))
				return new IntellisenseSuggest(token, _booleanOperators, _allTokensAreBoolean);

			return _emptySuggest;
		}

		private IReadOnlyList<string> suggestAllFieldValues(string value, string language)
		{
			var valuesSet = new HashSet<string>();
			var notCachedFields = new HashSet<string>(Str.Comparer);
			var discriminants = new HashSet<string>(Str.Comparer);

			bool valueIsNumeric = isValueNumeric(value);

			foreach (var userField in DocumentFactory.UserFields)
			{
				if (userField.IsNumericField() && !valueIsNumeric)
					continue;

				var cache = getValuesCache(userField, language, value);

				if (cache != null)
					valuesSet.UnionWith(cache);
				else
				{
					var spellcheckerField = userField.GetSpellcheckerFieldIn(language);
					notCachedFields.Add(spellcheckerField);
					discriminants.Add(userField.GetDiscriminantIn(language));
				}
			}

			if (notCachedFields.Count > 0 && IsLoaded)
			{
				var values = _spellchecker.SuggestSimilar(
					value,
					MaxCount,
					discriminants,
					notCachedFields,
					_reader);

				valuesSet.UnionWith(values);
			}

			return getTextuallySimilarValues(valuesSet.ToReadOnlyList(), value, MaxCount);
		}

		private IReadOnlyList<string> suggestValues(string userField, string language, string value)
		{
			var spellcheckerField = userField.GetSpellcheckerFieldIn(language);

			var cache = getValuesCache(userField, language, value);

			if (cache != null)
			{
				if (DocumentFactory.NumericFields.Contains(spellcheckerField))
					return getNumericallySimilarValues(cache, value);

				return getTextuallySimilarValues(cache, value, MaxCount);
			}

			if (!IsLoaded)
				return _emptySuggest.Values;

			var fields = new[] { spellcheckerField };
			var discriminants = new[] { userField.GetDiscriminantIn(language) };

			return _spellchecker.SuggestSimilar(value, MaxCount, discriminants, fields, _reader);
		}



		private IReadOnlyList<string> getValuesCache(string userField, string language, string value)
		{
			if (string.IsNullOrEmpty(value) || !userField.IsIndexedInSpellchecker())
				return getValuesCache(userField, language);

			return null;
		}

		private IReadOnlyList<string> getTextuallySimilarValues(IReadOnlyList<string> values, string value, int maxCount)
		{
			if (string.IsNullOrEmpty(value))
				return new ListSegment<string>(values, 0, maxCount);

			var similarities = values.Select(_ => _stringSimilarity.GetSimilarity(value, _))
				.ToArray();

			return Enumerable.Range(0, values.Count)
				.OrderByDescending(i => similarities[i])
				.ThenBy(i => values[i], Str.Comparer)
				.Select(i => values[i])
				.Take(maxCount)
				.ToReadOnlyList();
		}

		private IReadOnlyList<string> getNumericallySimilarValues(IReadOnlyList<string> cache, string value)
		{
			return cache.Where(_ => _.IndexOf(value, Str.Comparison) >= 0).Take(MaxCount).ToReadOnlyList();
		}

		private IReadOnlyList<string> getValuesCache(string userField, string language)
		{
			var spellcheckerField = userField.GetSpellcheckerFieldIn(language);
			IReadOnlyList<string> values;

			lock (_valuesCache)
				if (_valuesCache.TryGetValue(spellcheckerField, out values))
					return values;

			values = userField.SpellchekFromOriginalIndexIn(language)
				? _reader?.ReadAllValuesFrom(spellcheckerField)
				: _spellchecker?.ReadAllValuesFrom(discriminant: spellcheckerField);

			if (values == null)
				return _emptySuggest.Values;

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

		private static bool isValueNumeric(string queryText)
		{
			bool valueIsNumeric =
				int.TryParse(queryText, NumberStyles.Integer, Str.Culture, out _) ||
				float.TryParse(queryText, NumberStyles.Float, Str.Culture, out _);
			return valueIsNumeric;
		}



		private IReadOnlyList<string> suggestAllFields(string fieldPart)
		{
			if (string.IsNullOrEmpty(fieldPart))
				return _userFields;

			var result = getTextuallySimilarValues(_userFields, fieldPart, int.MaxValue);
			return result;
		}

		private IReadOnlyList<string> suggestFields(string fieldPart)
		{
			var fieldSuggest = _userFields
				.Where(_ => _.IndexOf(fieldPart, Str.Comparison) >= 0)
				.ToReadOnlyList();

			var ordered = getTextuallySimilarValues(fieldSuggest, fieldPart, int.MaxValue);
			return ordered;
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
			Enumerable.Empty<string>().ToReadOnlyList(),
			Enumerable.Empty<TokenType>().ToReadOnlyList());

		private bool _abort;
		private DirectoryReader _reader;
		private Spellchecker _spellchecker;

		private readonly DamerauLevenshteinSimilarity _stringSimilarity;
		private readonly Dictionary<string, IReadOnlyList<string>> _valuesCache = new Dictionary<string, IReadOnlyList<string>>();
		private readonly CardRepository _repo;
	}
}