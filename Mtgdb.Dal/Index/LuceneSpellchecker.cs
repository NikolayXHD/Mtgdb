using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Index;
using Lucene.Net.Contrib;
using Lucene.Net.Store;

namespace Mtgdb.Dal.Index
{
	public class LuceneSpellchecker : IDisposable
	{
		public LuceneSpellchecker()
		{
			// 0.21 RIX
			Version = new IndexVersion(AppDir.Data.AddPath("index").AddPath("suggest"), "0.21");
			_stringDistance = new DamerauLevenstineDistance();
		}

		public bool IsUpToDate => Version.IsUpToDate;

		internal void LoadIndex(Analyzer analyzer, CardRepository repository, DirectoryReader indexReader)
		{
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

		private Spellchecker createSpellchecker(CardRepository repository, Analyzer analyzer)
		{
			IsLoading = true;

			Version.CreateDirectory();

			var spellcheckerIndex = FSDirectory.Open(Version.Directory);
			var spellchecker = new Spellchecker(spellcheckerIndex, _stringDistance);

			var fields = new HashSet<string>();
			fields.UnionWith(DocumentFactory.TextFields);
			fields.ExceptWith(DocumentFactory.LimitedValuesFields);
			
			spellchecker.BeginIndex();

			var indexedWords = new HashSet<string>(Str.Comparer);
			var indexedValues = new HashSet<string>(Str.Comparer);

			var keywordAnalyzer = new KeywordAnalyzer();

			TotalSets = repository.SetsByCode.Count;

			foreach (var set in repository.SetsByCode.Values)
			{
				if (_abort)
					break;

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

						var fieldAnalyzer = isAnalyzed
							? keywordAnalyzer
							: analyzer;

						string value = docField.GetStringValue()?.ToLowerInvariant();

						if (string.IsNullOrEmpty(value))
							continue;

						if (isAnalyzed && !indexedValues.Add(value) || !isAnalyzed && indexedWords.Contains(value))
							continue;

						var tokenStream = fieldAnalyzer.GetTokenStream(fieldName, value);

						tokenStream.Reset();

						using (tokenStream)
							while (tokenStream.IncrementToken())
							{
								var term = tokenStream.GetAttribute<ICharTermAttribute>().ToString();
								if (indexedWords.Add(term))
									spellchecker.IndexWord(term);
							}
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

		public IntellisenseSuggest Suggest(string query, int caret, string language, int maxCount)
		{
			var token = EditedTokenLocator.GetEditedToken(query, caret);

			if (token == null || token.Type.Is(TokenType.ModifierValue))
				return _emptySuggest;

			string valuePart = token.Value.Substring(0, caret - token.Position);

			if (token.Type.Is(TokenType.FieldValue))
			{
				if (!IsLoaded)
					return _emptySuggest;

				var values = SuggestValues(valuePart.ToLowerInvariant(), token.ParentField, language, maxCount);
				return new IntellisenseSuggest(token, values);
			}

			if (token.Type.Is(TokenType.Field))
				return new IntellisenseSuggest(token, suggestFields(valuePart));

			if (token.Type.Is(TokenType.Boolean))
				return new IntellisenseSuggest(
					token,
					new[] { "AND", "OR", "NOT", "&&", "||", "!", "+", "-" }.OrderByDescending(_ => _.Equals(token.Value)).ToArray());

			return _emptySuggest;
		}



		public IList<string> SuggestValues(string value, string field, string language, int maxCount)
		{
			if (!IsLoaded)
				throw new InvalidOperationException("Index must be loaded first");

			if (!string.IsNullOrEmpty(field) && !DocumentFactory.UserFields.Contains(field))
				// incomplete or incorrect field name
				return _emptySuggest.Values;

			var valueIsNumeric = isValueNumeric(value);

			if (string.IsNullOrEmpty(field) || field == NumericAwareQueryParser.AnyField)
			{
				var valuesSet = new HashSet<string>();

				foreach (var userField in DocumentFactory.UserFields)
				{
					if (!valueIsNumeric && userField.IsNumericField())
						continue;

					var specificValues = SuggestValues(value, userField, language, maxCount / 4);
					valuesSet.UnionWith(specificValues);
				}

				var values = valuesSet.ToList();

				if (string.IsNullOrEmpty(value))
					values.Sort(Str.Comparer);
				else
				{
					var similarities = values.Select(_ => _stringDistance.GetDistance(value, _))
						.ToArray();

					values = Enumerable.Range(0, values.Count)
						.OrderByDescending(i => similarities[i])
						.Select(i => values[i])
						.ToList();
				}

				if (values.Count > maxCount)
					values.RemoveRange(maxCount, values.Count - maxCount);

				return values;
			}

			field = DocumentFactory.Localize(field, language);

			if (field.IsNumericField())
				return getNumericSuggest(value, field, language, maxCount);

			if (DocumentFactory.LimitedValuesFields.Contains(field))
				return fullScan(field, value, language, maxCount);

			if (string.IsNullOrEmpty(value))
			{
				var result = getValues(field, language, maxCount).ToArray();
				return result;
			}
			else
			{
				var values = _spellchecker.SuggestSimilar(value, maxCount, _reader, field);
				return values;
			}
		}

		private IList<string> fullScan(string field, string value, string language, int maxCount)
		{
			var values = getValues(field, language, _reader.MaxDoc)
				.Distinct()
				.ToList();

			if (!string.IsNullOrEmpty(value))
			{
				var similarities = values.Select(_ => _stringDistance.GetDistance(value, _))
					.ToArray();

				values = Enumerable.Range(0, values.Count)
					.OrderByDescending(i => similarities[i])
					.Select(i => values[i])
					.ToList();
			}
			else
				values.Sort(Str.Comparer);

			return values.Take(maxCount).ToArray();
		}

		private IList<string> getNumericSuggest(string value, string field, string language, int maxCount)
		{
			var values = getValues(field, language, _reader.MaxDoc)
				.Where(_ => _.IndexOf(value, Str.Comparison) >= 0);

			if (field.IsFloatField())
				values = values.OrderBy(float.Parse);
			else if (field.IsIntField())
				values = values.OrderBy(int.Parse);

			return values.Take(maxCount).ToArray();
		}

		private IEnumerable<string> getValues(string field, string language, int maxCount)
		{
			if (string.IsNullOrEmpty(field) || field == NumericAwareQueryParser.AnyField)
				foreach (var userField in DocumentFactory.UserFields)
					foreach (string specificResult in getValues(userField, language, maxCount))
						yield return specificResult;

			field = DocumentFactory.Localize(field, language);
			int count = 0;

			var terms = MultiFields.GetTerms(_reader, field);
			var iterator = terms.GetIterator(null);

			if (field.IsFloatField())
			{
				while (iterator.Next() != null && count <= maxCount)
				{
					var value = iterator.Term.TryParseFloat();

					if (value.HasValue)
					{
						yield return value.ToString();
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
						yield return value.ToString();
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

		private IList<string> suggestFields(string field)
		{
			var userFields = DocumentFactory.UserFields.ToList();

			if (!string.IsNullOrEmpty(field))
			{
				var similarities = userFields.Select(_ => _stringDistance.GetDistance(field, _))
					.ToArray();

				userFields = Enumerable.Range(0, userFields.Count)
					.OrderByDescending(i => similarities[i])
					.Select(i => userFields[i])
					.ToList();
			}
			else
			{
				userFields.Sort(Str.Comparer);
			}

			return userFields;
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
			int intVal;
			float floatVal;

			bool valueIsNumeric =
				int.TryParse(queryText, NumberStyles.Integer, CultureInfo.InvariantCulture, out intVal) ||
				float.TryParse(queryText, NumberStyles.Float, CultureInfo.InvariantCulture, out floatVal);
			return valueIsNumeric;
		}

		public bool IsLoaded { get; private set; }
		public bool IsLoading { get; private set; }

		public event Action IndexingProgress;

		public int IndexedSets { get; private set; }
		public int TotalSets { get; private set; }

		private static readonly IntellisenseSuggest _emptySuggest = new IntellisenseSuggest(null, new string[0]);

		private Spellchecker _spellchecker;
		private readonly DamerauLevenstineDistance _stringDistance;
		internal IndexVersion Version { get; }
		private bool _abort;
		private DirectoryReader _reader;
	}
}