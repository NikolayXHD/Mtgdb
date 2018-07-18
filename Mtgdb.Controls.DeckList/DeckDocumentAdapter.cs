using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers.Classic;
using Mtgdb.Index;

namespace Mtgdb.Controls
{
	public class DeckDocumentAdapter : IDocumentAdapter<int, DeckModel>
	{
		[UsedImplicitly]
		public DeckDocumentAdapter()
		{
		}

		public IEnumerable<string> GetUserFields() =>
			_userFields;

		public bool IsUserField(string userField) => 
			_userFields.Contains(userField);

		public IEnumerable<string> GetFieldLanguages(string userField) => 
			Sequence.From((string) null);

		public bool IsIntField(string userField) => 
			userField != null && userField.EndsWith("count", Str.Comparison);

		public bool IsFloatField(string userField) =>
			userField != null && (userField.EndsWith("price", Str.Comparison) || userField.EndsWith("percent", Str.Comparison));

		public bool IsIndexedInSpellchecker(string userField) =>
			!this.IsNumericField(userField);

		public bool IsStoredInSpellchecker(string userField, string lang) => 
			_spellcheckerValues.ContainsKey(userField);

		public string GetSpellcheckerFieldIn(string userField, string lang) => 
			userField.ToLower(Str.Culture);

		public string GetFieldLocalizedIn(string userField, string lang) =>
			userField.ToLower(Str.Culture);

		public bool IsAnyField(string field) =>
			string.IsNullOrEmpty(field) || field == AnyField;

		public bool IsSuggestAnalyzedIn(string userField, string lang) => 
			!IsNotAnalyzed(userField) && 
			!_spellcheckerValues.ContainsKey(userField);

		public bool IsNotAnalyzed(string userField) =>
			Str.Equals(userField, nameof(DeckModel.Saved));

		public IEnumerable<string> GetSpellcheckerValues(DeckModel obj, string userField, string language) =>
			_spellcheckerValues[userField](obj);

		public int GetId(Document doc) => 
			int.Parse(doc.Get(nameof(DeckModel.Id).ToLower(Str.Culture)));

		public Document ToDocument(DeckModel deck)
		{
			var doc = new Document();
			addIdField(doc, nameof(DeckModel.Id), deck.Id);

			if (!string.IsNullOrEmpty(deck.Name))
				addTextField(doc, nameof(DeckModel.Name), deck.Name);

			if (!string.IsNullOrEmpty(deck.Mana))
				addTextField(doc, nameof(DeckModel.Mana), deck.Mana);

			if (deck.Saved.HasValue)
				addTextField(doc, nameof(DeckModel.Saved), Format(deck.Saved.Value));

			foreach (string format in deck.Legal)
				addTextField(doc, nameof(DeckModel.Legal), format);

			addNumericField(doc, nameof(DeckModel.LandCount), deck.LandCount);
			addNumericField(doc, nameof(DeckModel.CreatureCount), deck.CreatureCount);
			addNumericField(doc, nameof(DeckModel.OtherSpellCount), deck.OtherSpellCount);

			addNumericField(doc, nameof(DeckModel.MainCount), deck.MainCount);
			addNumericField(doc, nameof(DeckModel.MainCollectedCount), deck.MainCollectedCount);
			addNumericField(doc, nameof(DeckModel.MainCollectedCountPercent), deck.MainCollectedCountPercent);

			addNumericField(doc, nameof(DeckModel.SideCount), deck.SideCount);
			addNumericField(doc, nameof(DeckModel.SideCollectedCount), deck.SideCollectedCount);
			addNumericField(doc, nameof(DeckModel.SideCollectedCountPercent), deck.SideCollectedCountPercent);

			addNumericField(doc, nameof(DeckModel.LandPrice), deck.LandPrice);
			addNumericField(doc, nameof(DeckModel.CreaturePrice), deck.CreaturePrice);
			addNumericField(doc, nameof(DeckModel.OtherSpellPrice), deck.OtherSpellPrice);

			addNumericField(doc, nameof(DeckModel.MainPrice), deck.MainPrice);
			addNumericField(doc, nameof(DeckModel.MainCollectedPrice), deck.MainCollectedPrice);
			addNumericField(doc, nameof(DeckModel.MainCollectedPricePercent), deck.MainCollectedPricePercent);

			addNumericField(doc, nameof(DeckModel.SidePrice), deck.SidePrice);
			addNumericField(doc, nameof(DeckModel.SideCollectedPrice), deck.SideCollectedPrice);
			addNumericField(doc, nameof(DeckModel.SideCollectedPricePercent), deck.SideCollectedPricePercent);

			addNumericField(doc, nameof(DeckModel.LandUnknownPriceCount), deck.LandUnknownPriceCount);
			addNumericField(doc, nameof(DeckModel.CreatureUnknownPriceCount), deck.CreatureUnknownPriceCount);
			addNumericField(doc, nameof(DeckModel.OtherSpellUnknownPriceCount), deck.OtherSpellUnknownPriceCount);

			addNumericField(doc, nameof(DeckModel.MainUnknownPriceCount), deck.MainUnknownPriceCount);
			addNumericField(doc, nameof(DeckModel.MainCollectedUnknownPriceCount), deck.MainCollectedUnknownPriceCount);
			addNumericField(doc, nameof(DeckModel.MainCollectedUnknownPricePercent), deck.MainCollectedUnknownPricePercent);

			addNumericField(doc, nameof(DeckModel.SideUnknownPriceCount), deck.SideUnknownPriceCount);
			addNumericField(doc, nameof(DeckModel.SideCollectedUnknownPriceCount), deck.SideCollectedUnknownPriceCount);
			addNumericField(doc, nameof(DeckModel.SideCollectedUnknownPricePercent), deck.SideCollectedUnknownPricePercent);

			return doc;
		}

		private void addNumericField(Document doc, string userField, float fieldValue)
		{
			if (float.IsNaN(fieldValue))
				return;

			userField = userField.ToLower(Str.Culture);

			if (!IsFloatField(userField))
				throw new ArgumentException($"{userField} is not float");

			var field = new SingleField(userField, fieldValue, Field.Store.NO);
			doc.Add(field);
		}

		private void addNumericField(Document doc, string userField, int fieldValue)
		{
			userField = userField.ToLower(Str.Culture);

			if (!IsIntField(userField))
				throw new ArgumentException($"{userField} is not int");

			var field = new Int32Field(userField, fieldValue, Field.Store.NO);
			doc.Add(field);
		}

		public static string Format(DateTime savedValue) =>
			savedValue.ToString("yyyy-MM-dd HH:mm:ss");

		public Analyzer CreateAnalyzer() =>
			new MtgAnalyzer(this);

		public QueryParser CreateQueryParser(string language, Analyzer analyzer) =>
			new DeckQueryParser((MtgAnalyzer) analyzer, this);

		private static void addIdField(Document doc, string fieldName, int fieldValue)
		{
			fieldName = fieldName.ToLower(Str.Culture);

			var field = new Int32Field(fieldName,
				fieldValue,
				new FieldType(Int32Field.TYPE_STORED)
				{
					IsIndexed = false
				});

			doc.Add(field);
		}

		private void addTextField(Document doc, string userField, string fieldValue)
		{
			userField = userField.ToLower(Str.Culture);

			if (IsNotAnalyzed(userField))
				doc.Add(new Field(userField, fieldValue, IndexUtils.StringFieldType));
			else
				doc.Add(new TextField(userField, fieldValue, Field.Store.NO));
		}

		private static readonly HashSet<string> _userFields = new HashSet<string>(Str.Comparer)
		{
			nameof(DeckModel.Name),
			nameof(DeckModel.Mana),
			nameof(DeckModel.Saved),
			nameof(DeckModel.Legal),

			nameof(DeckModel.LandCount),
			nameof(DeckModel.CreatureCount),
			nameof(DeckModel.OtherSpellCount),

			nameof(DeckModel.MainCount),
			nameof(DeckModel.MainCollectedCount),
			nameof(DeckModel.MainCollectedCountPercent),

			nameof(DeckModel.SideCount),
			nameof(DeckModel.SideCollectedCount),
			nameof(DeckModel.SideCollectedCountPercent),

			nameof(DeckModel.LandPrice),
			nameof(DeckModel.CreaturePrice),
			nameof(DeckModel.OtherSpellPrice),

			nameof(DeckModel.MainPrice),
			nameof(DeckModel.MainCollectedPrice),
			nameof(DeckModel.MainCollectedPricePercent),

			nameof(DeckModel.SidePrice),
			nameof(DeckModel.SideCollectedPrice),
			nameof(DeckModel.SideCollectedPricePercent),

			nameof(DeckModel.LandUnknownPriceCount),
			nameof(DeckModel.CreatureUnknownPriceCount),
			nameof(DeckModel.OtherSpellUnknownPriceCount),

			nameof(DeckModel.MainUnknownPriceCount),
			nameof(DeckModel.MainCollectedUnknownPriceCount),
			nameof(DeckModel.MainCollectedUnknownPricePercent),

			nameof(DeckModel.MainUnknownPriceCount),
			nameof(DeckModel.SideCollectedUnknownPriceCount),
			nameof(DeckModel.SideCollectedUnknownPricePercent)
		};

		private readonly Dictionary<string, Func<DeckModel, IEnumerable<string>>> _spellcheckerValues
			= new Dictionary<string, Func<DeckModel, IEnumerable<string>>>(Str.Comparer)
			{
				[nameof(DeckModel.Name)] = d => Sequence.From(d.Name),
				[nameof(DeckModel.Legal)] = d => d.Legal
			};

		private const string AnyField = "*";
	}
}