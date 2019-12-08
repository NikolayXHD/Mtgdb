using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers.Classic;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Data.Index
{
	public class CardDocumentAdapter : IDocumentAdapter<int, Card>
	{
		public CardDocumentAdapter(CardRepository repo)
		{
			_repo = repo;
		}

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

		public IReadOnlyDictionary<string, string> FieldByAlias { get; } =
			new Dictionary<string, string>(Str.Comparer)
			{
				["e"] = nameof(Card.SetCode),
				["ee"] = nameof(Card.SetName),
				["n"] = nameof(Card.NameEn),
				["c"] = nameof(Card.Color),
				["t"] = nameof(Card.TypeEn),
				["o"] = nameof(Card.TextEn),
				["m"] = nameof(Card.ManaCost),
				["pow"] = nameof(Card.PowerNum),
				["tou"] = nameof(Card.ToughnessNum),
				["loy"] = nameof(Card.LoyaltyNum),
				["f"] = nameof(Card.LegalIn),
				["a"] = nameof(Card.Artist),
				["ft"] = nameof(Card.FlavorEn)
			}.AsReadOnlyDictionary();

		public IEnumerable<string> GetFieldLanguages(string userField)
		{
			if (DocumentFactory.LocalizedFields.Contains(userField))
				return DocumentFactory.Languages;

			return Sequence.From((string) null);
		}

		public Analyzer CreateAnalyzer() =>
			new MtgAnalyzer(this);

		public QueryParser CreateQueryParser(string language, Analyzer analyzer) =>
			new CardQueryParser((MtgAnalyzer) analyzer, _repo, this, language);

		private readonly Dictionary<string, Func<Card, string, IEnumerable<string>>> _spellcheckerValues
			= new Dictionary<string, Func<Card, string, IEnumerable<string>>>(Str.Comparer)
		{
			[nameof(Card.Name)] = (c, lang) =>
				c.Localization?.GetName(lang)?.Invoke0(Sequence.From) ?? Enumerable.Empty<string>(),

			[nameof(Card.NameEn)] = (c, lang) => Sequence.From(c.NameEn),
			[nameof(Card.Artist)] = (c, lang) => Sequence.From(c.Artist),
			[nameof(Card.SetName)] = (c, lang) => Sequence.From(c.SetName),

			[nameof(Card.LegalIn)] = (c, lang) => c.LegalFormats,
			[nameof(Card.RestrictedIn)] = (c, lang) => c.RestrictedFormats,
			[nameof(Card.BannedIn)] = (c, lang) => c.BannedFormats,
			[nameof(Card.FutureIn)] = (c, lang) => c.FutureFormats
		};

		private const string AnyField = "*";
		private readonly CardRepository _repo;
	}
}
