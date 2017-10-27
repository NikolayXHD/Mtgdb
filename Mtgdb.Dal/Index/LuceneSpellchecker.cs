using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Contrib;
using SpellChecker.Net.Search.Spell;

namespace Mtgdb.Dal.Index
{
	public class LuceneSpellchecker : IDisposable
	{
		public LuceneSpellchecker()
		{
			// 0.9 Not storing * in separate field
			Version = new IndexVersion(AppDir.Data.AddPath("index").AddPath("suggest"), "0.9");
			_stringDistance = new DamerauLevenstineDistance();
		}

		public bool IsUpToDate => Version.IsUpToDate;

		internal void LoadIndex(Analyzer analyzer, Directory index)
		{
			_reader = IndexReader.Open(index, readOnly: true);

			if (Version.IsUpToDate)
				_spellchecker = openSpellchecker();
			else
				_spellchecker = createSpellchecker(analyzer, _reader);
			
			IsLoaded = true;
		}

		private Spellchecker openSpellchecker()
		{
			var spellcheckerIndex = FSDirectory.Open(Version.Directory);
			var spellChecker = new Spellchecker(spellcheckerIndex, _stringDistance);
			return spellChecker;
		}

		private Spellchecker createSpellchecker(Analyzer analyzer, IndexReader indexReader)
		{
			IsLoading = true;

			Version.CreateDirectory();

			var spellcheckerIndex = FSDirectory.Open(Version.Directory);
			var spellChecker = new Spellchecker(spellcheckerIndex, _stringDistance);

			TotalFields = DocumentFactory.TextFields.Count;
			foreach (string textField in DocumentFactory.TextFields)
			{
				if (_abort)
					return null;

				spellChecker.IndexDictionary(new LuceneDictionary(indexReader, textField), analyzer, () => _abort);

				IndexedFields++;
				IsLoading = IndexedFields < TotalFields;
				IndexingProgress?.Invoke();
			}

			IsLoading = false;

			Version.SetIsUpToDate();
			return spellChecker;
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

			if (string.IsNullOrEmpty(field) || field == NumericAwareQueryParser.AnyField)
			{
				var result = new List<string>();

				foreach (var userField in DocumentFactory.UserFields)
				{
					var specificValues = SuggestValues(value, userField, language, maxCount);
					result.AddRange(specificValues);
				}

				result.Sort(Str.Comparer);

				if (result.Count > maxCount)
					result.RemoveRange(maxCount, result.Count - maxCount);

				return result;
			}

			field = DocumentFactory.Localize(field, language);

			if (field.IsNumericField())
				return getNumericSuggest(value, field, language, maxCount);

			if (string.IsNullOrEmpty(value))
				return getValues(field, language, maxCount).ToArray();

			if (DocumentFactory.LimitedValuesFields.Contains(field))
				return fullScan(field, value, language, maxCount);

			return _spellchecker.SuggestSimilar(value, maxCount, _reader, field);
		}

		private IList<string> fullScan(string field, string value, string language, int maxCount)
		{
			var values = getValues(field, language, _reader.MaxDoc)
				.OrderByDescending(_ => _stringDistance.GetSimilarity(value, _))
				.Take(maxCount)
				.ToArray();

			return values;
		}

		private IList<string> getNumericSuggest(string value, string field, string language, int maxCount)
		{
			var strVals = getValues(field, language, _reader.MaxDoc)
				.Where(_ => _.IndexOf(value, Str.Comparison) >= 0);

			if (field.IsFloatField())
				strVals = strVals.OrderBy(float.Parse);
			else if (field.IsIntField())
				strVals = strVals.OrderBy(int.Parse);

			var result = strVals.Take(maxCount).ToArray();
			return result;
		}

		private IEnumerable<string> getValues(string field, string language, int maxCount)
		{
			if (string.IsNullOrEmpty(field) || field == NumericAwareQueryParser.AnyField)
				foreach (var userField in DocumentFactory.UserFields)
					foreach (string specificResult in getValues(userField, language, maxCount))
						yield return specificResult;

			field = DocumentFactory.Localize(field, language);

			bool isFloat = field.IsFloatField();
			bool isInt = field.IsIntField();

			var terms = _reader.Terms(new Term(field));

			int count = 0;
			while (terms.Next())
			{
				if (count >= maxCount || !Str.Equals(terms.Term.Field, field))
					yield break;

				string text = terms.Term.Text;

				if (isFloat)
				{
					var value = text.TryParseFloat();
					if (value.HasValue)
					{
						yield return value.ToString();
						count++;
					}
				}
				else if (isInt)
				{
					var value = text.TryParseInt();
					if (value.HasValue)
					{
						yield return value.ToString();
						count++;
					}
				}
				else
				{
					yield return text;
					count++;
				}
			}
		}

		private string[] suggestFields(string field)
		{
			if (string.IsNullOrEmpty(field))
				return DocumentFactory.UserFields
					.OrderBy(_ => _)
					.ToArray();

			return DocumentFactory.UserFields
				.OrderByDescending(_ => _stringDistance.GetSimilarity(field, _))
				.ToArray();
		}

		public void Dispose()
		{
			abortLoading();

			IsLoaded = false;
			_reader?.Dispose();
			_spellchecker?.Dispose();
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

		public bool IsLoaded { get; private set; }
		public bool IsLoading { get; private set; }

		public event Action IndexingProgress;

		public int IndexedFields { get; private set; }
		public int TotalFields { get; private set; }



		private static readonly IntellisenseSuggest _emptySuggest = new IntellisenseSuggest(null, new string[0]);

		private Spellchecker _spellchecker;
		private IndexReader _reader;
		private readonly DamerauLevenstineDistance _stringDistance;
		internal IndexVersion Version { get; }
		private bool _abort;
	}
}