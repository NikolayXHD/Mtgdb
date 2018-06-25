using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lucene.Net.Documents;
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
			userField.EndsWith("count", Str.Comparison);

		public bool IsFloatField(string userField) => 
			userField.EndsWith("price", Str.Comparison) || userField.EndsWith("percent", Str.Comparison);

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
			false;

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

			return doc;
		}

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
			nameof(DeckModel.Mana)
		};

		private readonly Dictionary<string, Func<DeckModel, IEnumerable<string>>> _spellcheckerValues
			= new Dictionary<string, Func<DeckModel, IEnumerable<string>>>(Str.Comparer)
			{
				[nameof(DeckModel.Name)] = d => Sequence.From(d.Name),
			};

		private const string AnyField = "*";
	}
}