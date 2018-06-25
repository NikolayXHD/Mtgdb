using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Mtgdb.Index;

namespace Mtgdb.Dal.Index
{
	public class CardDocumentAdapter : IDocumentAdapter<int, Card>
	{
		public bool IsSuggestAnalyzedIn(string userField, string lang)
		{
			var spellcheckerField = GetSpellcheckerFieldIn(userField, lang);

			return
				!IsNotAnalyzed(spellcheckerField) && 
				!_spellcheckerValues.ContainsKey(spellcheckerField);
		}

		public bool IsIndexedInSpellchecker(string f) =>
			!Str.Equals(f, CardQueryParser.Like) && !this.IsNumericField(f);

		public bool IsStoredInSpellchecker(string userField, string lang)
		{
			var transformed = transform(userField, lang);
			return _spellcheckerValues.ContainsKey(transformed);
		}

		public IEnumerable<string> GetSpellcheckerValues(Card obj, string userField, string language) => 
			_spellcheckerValues[userField](obj, language);

		public Document ToDocument(Card card) =>
			card.Document;

		private string transform(string userField, string lang)
		{
			if (Str.Equals(userField, CardQueryParser.Like))
				return nameof(Card.NameEn);

			if (DocumentFactory.LocalizedFields.Contains(userField) && Str.Equals(lang, CardLocalization.DefaultLanguage))
				return GetFieldLocalizedIn(userField, lang);

			return userField;
		}

		public string GetSpellcheckerFieldIn(string userField, string lang)
		{
			var transformed = transform(userField, lang);
			string localized = GetFieldLocalizedIn(transformed, lang);

			return localized;
		}

		public string GetFieldLocalizedIn(string userField, string lang)
		{
			userField = userField.ToLower(Str.Culture);

			if (DocumentFactory.LocalizedFields.Contains(userField))
				return DocumentFactory.GetLocalizedField(userField, lang);

			return userField;
		}

		public bool IsUserField(string userField) =>
			DocumentFactory.UserFields.Contains(userField);

		public bool IsAnyField(string field) =>
			string.IsNullOrEmpty(field) || field == AnyField;

		public bool IsIntField(string userField) =>
			DocumentFactory.IntFields.Contains(userField);

		public bool IsFloatField(string userField) =>
			DocumentFactory.FloatFields.Contains(userField);

		public bool IsNotAnalyzed(string userField) => 
			DocumentFactory.NotAnalyzedFields.Contains(userField);

		public int GetId(Document doc) =>
			int.Parse(doc.Get(nameof(Card.IndexInFile).ToLower(Str.Culture)));

		public IEnumerable<string> GetUserFields() =>
			DocumentFactory.UserFields;

		public IEnumerable<string> GetFieldLanguages(string userField)
		{
			if (DocumentFactory.LocalizedFields.Contains(userField))
				return DocumentFactory.Languages;

			return Sequence.From((string) null);
		}



		private readonly Dictionary<string, Func<Card, string, IEnumerable<string>>> _spellcheckerValues
			= new Dictionary<string, Func<Card, string, IEnumerable<string>>>(Str.Comparer)
		{
			[nameof(Card.Name)] = (c, lang) => 
				c.Localization?.GetName(lang)?.Invoke(Sequence.From) ?? Enumerable.Empty<string>(),

			[nameof(Card.NameEn)] = (c, lang) => Sequence.From(c.NameEn),
			[nameof(Card.Artist)] = (c, lang) => Sequence.From(c.Artist),
			[nameof(Card.SetName)] = (c, lang) => Sequence.From(c.SetName)
		};

		private const string AnyField = "*";
	}
}