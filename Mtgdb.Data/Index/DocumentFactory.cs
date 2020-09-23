using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;

namespace Mtgdb.Data.Index
{
	public static class DocumentFactory
	{
		static DocumentFactory()
		{
			addTextField(nameof(KeywordDefinitions.Keywords), nameof(Card.Text));

			addTextField(nameof(Card.Number));

			addTextField(nameof(Card.NameEn), nameof(Card.Name));
			addTextField(nameof(Card.TypeEn), nameof(Card.Type));
			addTextField(nameof(Card.TextEn), nameof(Card.Text));
			addTextField(nameof(Card.FlavorEn), nameof(Card.Flavor));

			addTextField(nameof(Card.Color));

			addTextField(nameof(Card.SetName));
			addTextField(nameof(Card.SetCode));
			addTextField(nameof(Card.Artist));

			addTextField(nameof(Card.OriginalText), nameof(Card.Text));
			addTextField(nameof(Card.OriginalType), nameof(Card.Type));

			addTextField(nameof(Card.Supertypes), nameof(Card.Type));
			addTextField(nameof(Card.Types), nameof(Card.Type));
			addTextField(nameof(Card.Subtypes), nameof(Card.Type));

			addTextField(nameof(Card.LegalIn), nameof(Card.Rulings));
			addTextField(nameof(Card.RestrictedIn), nameof(Card.Rulings));
			addTextField(nameof(Card.BannedIn), nameof(Card.Rulings));

			addTextField(nameof(Card.Power));
			addTextField(nameof(Card.Toughness));
			addTextField(nameof(Card.Loyalty));
			addTextField(nameof(Card.ReleaseDate));
			addTextField(nameof(Card.Layout));

			addTextField(nameof(Card.GeneratedMana), nameof(Card.Text));
			addTextField(nameof(Card.ManaCost));
			addTextField(nameof(Card.Rarity));

			addFloatField(nameof(Card.PowerNum), nameof(Card.Power));
			addFloatField(nameof(Card.ToughnessNum), nameof(Card.Toughness));
			addIntField(nameof(Card.LoyaltyNum), nameof(Card.Loyalty));

			addIntField(nameof(Card.Hand));
			addIntField(nameof(Card.Life));

			addFloatField(nameof(Card.Cmc));
			addFloatField(nameof(Card.Price));

			foreach (var lang in Languages)
			{
				addSpecificTextField(nameof(Card.Name), lang);
				addSpecificTextField(nameof(Card.Type), lang);
				addSpecificTextField(nameof(Card.Text), lang);
				addSpecificTextField(nameof(Card.Flavor), lang);
			}

			UserFields.Add(CardQueryParser.Like);
		}

		private static void addTextField(string userField, string displayField = null)
		{
			_textFields.Add(userField);
			UserFields.Add(userField);

			if (displayField != null)
				DisplayFieldByIndexField.Add(userField, displayField);
		}

		private static void addSpecificTextField(string userField, string language)
		{
			if (language == null)
				throw new ArgumentNullException(nameof(language));

			UserFields.Add(userField);
			LocalizedFields.Add(userField);

			userField = userField.ToLowerInvariant();

			var localized = GetLocalizedField(userField, language);
			_textFields.Add(localized);

			DisplayFieldByIndexField.Add(localized, userField);
		}

		private static void addFloatField(string userField, string displayFieldName = null)
		{
			UserFields.Add(userField);
			FloatFields.Add(userField);

			if (displayFieldName != null)
				DisplayFieldByIndexField.Add(userField, displayFieldName);
		}

		private static void addIntField(string userField, string displayFieldName = null)
		{
			UserFields.Add(userField);
			IntFields.Add(userField);

			if (displayFieldName != null)
				DisplayFieldByIndexField.Add(userField, displayFieldName);
		}



		public static Document ToDocument(this CardKeywords cardKeywords)
		{
			var doc = new Document();
			doc.addIdField(nameof(CardKeywords.IndexInFile), cardKeywords.IndexInFile);

			foreach (var pair in cardKeywords.GetAllValues())
			{
				string fieldName = pair.PropertyName.ToLowerInvariant();
				foreach (string value in pair.Values)
					doc.Add(new Field(fieldName, value, IndexUtils.StringFieldType));
			}

			return doc;
		}

		public static Document ToDocument(this Card card)
		{
			var doc = new Document();

			if (!string.IsNullOrEmpty(card.Number))
				doc.addTextField(nameof(Card.Number), card.Number);

			// Tested
			doc.addIdField(nameof(card.IndexInFile), card.IndexInFile);

			doc.addTextField(nameof(card.Color), card.Color);

			// Tested
			doc.addTextField(nameof(card.NameEn), card.NameEn);

			// Tested
			doc.addTextField(nameof(card.SetName), card.SetName);

			//Tested
			doc.addTextField(nameof(card.SetCode), card.SetCode);

			//Tested
			if (!string.IsNullOrEmpty(card.OriginalText))
				doc.addTextField(nameof(Card.OriginalText), card.OriginalText);

			//Tested
			if (!string.IsNullOrEmpty(card.OriginalType))
				doc.addTextField(nameof(Card.OriginalType), card.OriginalType);

			//Tested
			if (!string.IsNullOrEmpty(card.Artist))
				doc.addTextField(nameof(card.Artist), card.Artist);


			if (!string.IsNullOrEmpty(card.TextEn))
			{
				// Tested
				doc.addTextField(nameof(card.TextEn), card.TextEn);
			}

			foreach (string keyword in card.GetKeywords())
				doc.addTextField(nameof(KeywordDefinitions.Keywords), keyword);

			foreach (string keyword in card.GetCastKeywords())
				doc.addTextField(nameof(KeywordDefinitions.Keywords), keyword);

			// Tested
			if (!string.IsNullOrEmpty(card.FlavorEn))
				doc.addTextField(nameof(card.FlavorEn), card.FlavorEn);

			// Tested
			if (!string.IsNullOrEmpty(card.TypeEn))
				doc.addTextField(nameof(card.TypeEn), card.TypeEn);

			// Tested
			if (!string.IsNullOrEmpty(card.Supertypes))
				doc.addTextField(nameof(card.Supertypes), card.Supertypes);

			// Tested
			if (!string.IsNullOrEmpty(card.Types))
				doc.addTextField(nameof(card.Types), card.Types);

			// Tested
			if (!string.IsNullOrEmpty(card.Subtypes))
				doc.addTextField(nameof(card.Subtypes), card.Subtypes);

			foreach (var pair in card.LegalityByFormat)
			{
				// Tested
				var legality = pair.Value;
				var format = pair.Key;

				if (Str.Equals(legality, Legality.Legal))
					doc.addTextField(nameof(card.LegalIn), format);
				else if (Str.Equals(legality, Legality.Restricted))
					doc.addTextField(nameof(card.RestrictedIn), format);
				else if (Str.Equals(legality, Legality.Banned))
					doc.addTextField(nameof(card.BannedIn), format);
				else if (Str.Equals(legality, Legality.Future))
					doc.addTextField(nameof(card.FutureIn), format);
				else
					throw new NotSupportedException($"Unknown legality {legality}");
			}

			// Tested
			if (!string.IsNullOrEmpty(card.Power))
				doc.addTextField(nameof(card.Power), card.Power);

			// Tested
			if (!string.IsNullOrEmpty(card.Toughness))
				doc.addTextField(nameof(card.Toughness), card.Toughness);

			// Tested
			if (!string.IsNullOrEmpty(card.Loyalty))
				doc.addTextField(nameof(card.Loyalty), card.Loyalty);

			// Tested
			if (card.PowerNum.HasValue)
				doc.addNumericField(nameof(card.PowerNum), card.PowerNum.Value);

			// Tested
			if (card.ToughnessNum.HasValue)
				doc.addNumericField(nameof(card.ToughnessNum), card.ToughnessNum.Value);

			// Tested
			if (card.LoyaltyNum.HasValue)
				doc.addNumericField(nameof(card.LoyaltyNum), card.LoyaltyNum.Value);

			// Tested
			if (card.Life.HasValue)
				doc.addNumericField(nameof(card.Life), card.Life.Value);

			// Tested
			if (card.Hand.HasValue)
				doc.addNumericField(nameof(card.Hand), card.Hand.Value);

			// Tested
			doc.addNumericField(nameof(card.Cmc), card.Cmc);

			// Tested
			foreach (var mana in card.GeneratedManaArrExpanded)
				doc.addTextField(nameof(card.GeneratedMana), mana);

			// Tested
			if (!string.IsNullOrEmpty(card.ManaCost))
				doc.addTextField(nameof(card.ManaCost), card.ManaCost);

			// Tested
			if (!string.IsNullOrEmpty(card.Rarity))
				doc.addTextField(nameof(card.Rarity), card.Rarity);

			// Tested
			if (!string.IsNullOrEmpty(card.ReleaseDate))
				doc.addTextField(nameof(card.ReleaseDate), card.ReleaseDate);

			// Tested
			if (card.Price.HasValue)
				doc.addNumericField(nameof(card.Price), card.Price.Value);

			// Tested
			if (!string.IsNullOrEmpty(card.Layout))
				doc.addTextField(nameof(Card.Layout), card.Layout);

			foreach (var lang in Languages)
			{
				// Tested
				string name = card.Localization?.GetName(lang);
				if (!string.IsNullOrEmpty(name))
					doc.addTextField(nameof(Card.Name), lang, name);

				// Tested
				string type = card.Localization?.GetType(lang);
				if (!string.IsNullOrEmpty(type))
				{
					var typeParts = type.Split(Sequence.Array(' ', '—'), StringSplitOptions.RemoveEmptyEntries);

					foreach (string typePart in typeParts)
						doc.addTextField(nameof(Card.Type), lang, typePart);
				}

				// Tested
				string text = card.Localization?.GetAbility(lang);
				if (!string.IsNullOrEmpty(text))
					doc.addTextField(nameof(Card.Text), lang, text);

				// Tested
				string flavor = card.Localization?.GetFlavor(lang);
				if (!string.IsNullOrEmpty(flavor))
					doc.addTextField(nameof(Card.Flavor), lang, flavor);
			}

			return doc;
		}



		private static void addIdField(this Document doc, string fieldName, int fieldValue)
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



		private static void addTextField(this Document doc, string userField, string fieldValue)
		{
			userField = userField.ToLower(Str.Culture);
			addSpecificTextField(doc, userField, userField, fieldValue);
		}

		private static void addTextField(this Document doc, string userField, string language, string fieldValue)
		{
			if (language == null)
				throw new ArgumentNullException(nameof(language));

			userField = userField.ToLower(Str.Culture);

			var localized = GetLocalizedField(userField, language);
			addSpecificTextField(doc, userField, localized, fieldValue);
		}

		private static void addSpecificTextField(Document doc, string userField, string localized, string fieldValue)
		{
			localized = localized.ToLower(Str.Culture);

			if (!_textFields.Contains(localized))
				throw new InvalidOperationException($"Text field {localized} not initialized");

			if (NotAnalyzedFields.Contains(userField))
				doc.Add(new Field(localized, fieldValue, IndexUtils.StringFieldType));
			else
				doc.Add(new TextField(localized, fieldValue, Field.Store.NO));
		}



		private static void addNumericField(this Document doc, string userField, float fieldValue)
		{
			userField = userField.ToLower(Str.Culture);

			if (!FloatFields.Contains(userField))
				throw new ArgumentException($"Numeric float field {userField} not initialized");

			var field = new SingleField(userField, fieldValue, Field.Store.NO);
			doc.Add(field);
		}

		private static void addNumericField(this Document doc, string userField, int fieldValue)
		{
			userField = userField.ToLower(Str.Culture);

			if (!IntFields.Contains(userField))
				throw new ArgumentException($"Numeric int field {userField} not initialized");

			var field = new Int32Field(userField, fieldValue, Field.Store.NO);
			doc.Add(field);
		}



		public static string GetLocalizedField(string userField, string language)
		{
			if (language == null)
				throw new InvalidOperationException($"Language must be specified for localized field {userField}");

			if (LocalizedFields.Contains(userField))
				return string.Intern(userField + language);

			return userField;
		}



		public static readonly IReadOnlyList<string> Languages =
			CardLocalization.GetAllLanguages()
				.Where(lang => !Str.Equals(lang, CardLocalization.DefaultLanguage))
				.ToList();

		public static readonly HashSet<string> IntFields = new HashSet<string>(Str.Comparer);
		public static readonly HashSet<string> FloatFields = new HashSet<string>(Str.Comparer);

		private static readonly HashSet<string> _textFields = new HashSet<string>(Str.Comparer);

		public static readonly HashSet<string> LocalizedFields = new HashSet<string>(Str.Comparer);
		public static readonly HashSet<string> UserFields = new HashSet<string>(Str.Comparer);

		public static HashSet<string> NotAnalyzedFields { get; } =
			new HashSet<string>(Str.Comparer)
			{
				nameof(KeywordDefinitions.Keywords),
				nameof(Card.SetCode),
				nameof(Card.Power),
				nameof(Card.Toughness),
				nameof(Card.Loyalty),
				nameof(Card.ReleaseDate),
				nameof(Card.Rarity),
				nameof(Card.Layout),
				nameof(Card.Number)
			};

		public static readonly Dictionary<string, string> DisplayFieldByIndexField =
			new Dictionary<string, string>(Str.Comparer);
	}
}
