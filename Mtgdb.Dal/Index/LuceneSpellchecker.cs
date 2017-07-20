using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Contrib;
using SpellChecker.Net.Search.Spell;

namespace Mtgdb.Dal.Index
{
	public class LuceneSpellchecker : IDisposable
	{
		private static readonly IntellisenseSuggest EmptySuggest = new IntellisenseSuggest(null, new string[0]);
		
		private Spellchecker _spellchecker;
		private IndexReader _reader;
		private readonly DamerauLevenstineDistance _stringDistance;
		internal IndexVersion Version { get; }

		public bool IsLoaded { get; private set; }
		public bool IndexLoading { get; private set; }

		public event Action IndexingProgress;

		public int IndexedFields { get; private set; }
		public int TotalFields { get; private set; }

		public LuceneSpellchecker()
		{
			// 0.2 -> 0.3 new sets
			Version = new IndexVersion(AppDir.Data.AddPath("index").AddPath("suggest"), "0.3");
			_stringDistance = new DamerauLevenstineDistance();
		}

		public void LoadIndex(Analyzer analyzer, Directory index)
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
			IndexLoading = true;

			Version.CreateDirectory();

			var spellcheckerIndex = FSDirectory.Open(Version.Directory);
			var spellChecker = new Spellchecker(spellcheckerIndex, _stringDistance);

			TotalFields = DocumentFactory.TextFields.Count;
			foreach (string textField in DocumentFactory.TextFields)
			{
				spellChecker.IndexDictionary(new LuceneDictionary(indexReader, textField), analyzer);

				IndexedFields++;
				IndexLoading = IndexedFields < TotalFields;
				IndexingProgress?.Invoke();
			}

			IndexLoading = false;

			Version.SetIsUpToDate();
			return spellChecker;
		}

		public IntellisenseSuggest Suggest(string query, int caret, string language, int maxCount)
		{
			var token = EditedTokenLocator.GetEditedToken(query, caret);

			if (token == null || token.Type.Is(TokenType.ModifierValue))
				return EmptySuggest;

			string valuePart = token.Value.Substring(0, caret - token.Position);

			if (token.Type.Is(TokenType.FieldValue))
			{
				if (!IsLoaded)
					return EmptySuggest;

				var values = SuggestValues(valuePart.ToLowerInvariant(), token.ParentField, language, maxCount)
					.Select(StringEscaper.Escape)
					.ToArray();

				return new IntellisenseSuggest(token, values);
			}

			if (token.Type.Is(TokenType.Field))
				return new IntellisenseSuggest(token, suggestFields(valuePart));

			if (token.Type.Is(TokenType.Boolean))
				return new IntellisenseSuggest(
					token,
					new[] { "AND", "OR", "NOT", "&&", "||", "!", "+", "-" }.OrderByDescending(_ => _.Equals(token.Value)).ToArray());

			return EmptySuggest;
		}



		public string[] SuggestValues(string value, string field, string language, int maxCount)
		{
			if (!IsLoaded)
				throw new InvalidOperationException("Index must be loaded first");

			field = DocumentFactory.Normalize(field, language);

			if (field.IsNumericField())
				return getNumericSuggest(value, field, language, maxCount);

			if (string.IsNullOrEmpty(value))
				return getValues(field, language, maxCount).ToArray();

			if (DocumentFactory.LimitedValuesFields.Contains(field))
				return fullScan(field, value, language, maxCount);

			return _spellchecker.SuggestSimilar(value, maxCount, _reader, field);
		}

		private string[] fullScan(string field, string value, string language, int maxCount)
		{
			var values = getValues(field, language, _reader.MaxDoc)
				.OrderByDescending(_ => _stringDistance.GetDistance(value, _))
				.Take(maxCount)
				.ToArray();

			return values;
		}

		private string[] getNumericSuggest(string value, string field, string language, int maxCount)
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
			field = DocumentFactory.Normalize(field, language);

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
				.OrderByDescending(_ => _stringDistance.GetDistance(field, _))
				.ToArray();
		}

		public void Dispose()
		{
			_reader.Dispose();
			_spellchecker.Dispose();
		}
	}
}