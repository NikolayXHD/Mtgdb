using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace Mtgdb.Dal.Index
{
	public static class DocumentFactory
	{
		private static readonly List<string> Langs;
		private const string AnyField = "*";

		private static readonly HashSet<string> IntFields = new HashSet<string>(Str.Comparer);
		private static readonly HashSet<string> FloatFields = new HashSet<string>(Str.Comparer);
		public static readonly HashSet<string> TextFields = new HashSet<string>(Str.Comparer);

		private static readonly HashSet<string> LocalizedFields = new HashSet<string>(Str.Comparer);
		public static readonly HashSet<string> UserFields = new HashSet<string>(Str.Comparer);
		public static readonly HashSet<string> LimitedValuesFields = new HashSet<string>(Str.Comparer);

		public static readonly Dictionary<string, string> DisplayFieldByIndexField = new Dictionary<string, string>(Str.Comparer);
		

		static DocumentFactory()
		{
			Langs = CardLocalization.GetAllLanguages().ToList();

			addTextField(nameof(Card.SetName), isLimitedValues: true);
			addTextField(nameof(Card.SetCode), isLimitedValues: true);
			addTextField(nameof(Card.Artist), isLimitedValues: true);

			addTextField(nameof(Card.NameEn),
				nameof(Card.Name));
			addTextField(nameof(Card.TextEn),
				nameof(Card.Text));
			addTextField(nameof(Card.FlavorEn),
				nameof(Card.Flavor));
			addTextField(nameof(Card.TypeEn),
				nameof(Card.Type), isLimitedValues: true);
			addTextField(nameof(Card.SupertypesArr),
				nameof(Card.Type), isLimitedValues: true);
			addTextField(nameof(Card.TypesArr),
				nameof(Card.Type), isLimitedValues: true);
			addTextField(nameof(Card.SubtypesArr),
				nameof(Card.Type), isLimitedValues: true);
			addTextField(nameof(Card.LegalIn),
				nameof(Card.Rulings), isLimitedValues: true);
			addTextField(nameof(Card.RestrictedIn),
				nameof(Card.Rulings), isLimitedValues: true);
			addTextField(nameof(Card.BannedIn),
				nameof(Card.Rulings), isLimitedValues: true);
			addTextField(nameof(Card.GeneratedMana),
				nameof(Card.Text), isLimitedValues: true);

			addTextField(nameof(Card.Power), isLimitedValues: true);
			addTextField(nameof(Card.Toughness), isLimitedValues: true);
			addTextField(nameof(Card.Loyalty), isLimitedValues: true);
			addTextField(nameof(Card.ManaCost), isLimitedValues: true);
			addTextField(nameof(Card.Rarity), isLimitedValues: true);
			addTextField(nameof(Card.ReleaseDate), isLimitedValues: true);

			addFloatField(nameof(Card.PowerNum),
				nameof(Card.Power));
			addFloatField(nameof(Card.ToughnessNum),
				nameof(Card.Toughness));
			addIntField(nameof(Card.LoyaltyNum),
				nameof(Card.Loyalty));

			addFloatField(nameof(Card.Cmc));
			addFloatField(nameof(Card.PricingLow));
			addFloatField(nameof(Card.PricingMid));
			addFloatField(nameof(Card.PricingHigh));

			foreach (var lang in Langs)
			{
				addSpecificTextField(nameof(Card.Name), lang);
				addSpecificTextField(nameof(Card.Type), lang, isLimitedValues: true);
				addSpecificTextField(nameof(Card.Text), lang);
				addSpecificTextField(nameof(Card.Flavor), lang);
				addSpecificTextField(AnyField, lang);
			}
		}

		public static Document ToDocument(this CardKeywords cardKeywords)
		{
			var doc = new Document();
			doc.addIdField(nameof(cardKeywords.Id), cardKeywords.Id);

			foreach (var pair in cardKeywords.KeywordsByProperty)
				foreach (string value in pair.Value)
					doc.Add(new Field(pair.Key.ToLowerInvariant(), value.ToLowerInvariant(), Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS));

			return doc;
		}

		public static Document ToDocument(this Card card)
		{
			var doc = new Document();

			// Tested
			doc.addIdField(nameof(card.Id), card.Id);

			// Tested
			doc.addTextField(nameof(card.NameEn), card.NameEn);

			// Tested
			doc.addTextField(nameof(card.SetName), card.SetName);

			//Tested
			doc.addTextField(nameof(card.SetCode), card.SetCode);

			if (!string.IsNullOrEmpty(card.Artist))
				doc.addTextField(nameof(card.Artist), card.Artist);

			// Tested
			if (!string.IsNullOrEmpty(card.TextEn))
				doc.addTextField(nameof(card.TextEn), card.TextEn);

			if (!string.IsNullOrEmpty(card.FlavorEn))
				doc.addTextField(nameof(card.FlavorEn), card.FlavorEn);

			// Tested
			if (!string.IsNullOrEmpty(card.TypeEn))
				doc.addTextField(nameof(card.TypeEn), card.TypeEn);

			// Tested
			if (card.SupertypesArr != null)
				foreach (string type in card.SupertypesArr)
					doc.addTextField(nameof(card.SupertypesArr), type);

			// Tested
			if (card.TypesArr != null)
				foreach (string type in card.TypesArr)
					doc.addTextField(nameof(card.TypesArr), type);

			// Tested
			if (card.SubtypesArr != null)
				foreach (string type in card.SubtypesArr)
					doc.addTextField(nameof(card.SubtypesArr), type);

			foreach (var note in card.LegalityByFormat.Values)
			{
				// Tested
				if (Str.Equals(note.Legality, Legality.Legal))
					doc.addTextField(nameof(card.LegalIn), note.Format);

				// Tested
				else if (Str.Equals(note.Legality, Legality.Restricted))
					doc.addTextField(nameof(card.RestrictedIn), note.Format);

				// Tested
				else if (Str.Equals(note.Legality, Legality.Banned))
					doc.addTextField(nameof(card.BannedIn), note.Format);

				else
					throw new NotSupportedException($"Unknown legality {note.Legality}");
			}

			// Tested
			if (!string.IsNullOrEmpty(card.Power))
				doc.addTextField(nameof(card.Power), card.Power);

			// Tested
			if (!string.IsNullOrEmpty(card.Toughness))
				doc.addTextField(nameof(card.Toughness), card.Toughness);

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
			doc.addNumericField(nameof(card.Cmc), card.Cmc);

			if (!string.IsNullOrEmpty(card.GeneratedMana))
				doc.addTextField(nameof(card.GeneratedMana), card.GeneratedMana);

			if (!string.IsNullOrEmpty(card.ManaCost))
				doc.addTextField(nameof(card.ManaCost), card.ManaCost);

			// Tested
			if (!string.IsNullOrEmpty(card.Rarity))
				doc.addTextField(nameof(card.Rarity), card.Rarity);

			// Tested
			if (!string.IsNullOrEmpty(card.ReleaseDate))
				doc.addTextField(nameof(card.ReleaseDate), card.ReleaseDate);

			// Tested
			if (card.PricingHigh.HasValue)
				doc.addNumericField(nameof(card.PricingHigh), card.PricingHigh.Value);

			// Tested
			if (card.PricingMid.HasValue)
				doc.addNumericField(nameof(card.PricingMid), card.PricingMid.Value);

			// Tested
			if (card.PricingLow.HasValue)
				doc.addNumericField(nameof(card.PricingLow), card.PricingLow.Value);

			foreach (var lang in Langs)
			{
				// Tested
				string name = card.GetName(lang);

				if (!string.IsNullOrEmpty(name))
					doc.addTextField(nameof(card.Name), name, lang);

				// Tested
				string type = card.GetType(lang);
				if (!string.IsNullOrEmpty(type))
				{
					var typeParts = type.Split(new[] { ' ', '—' }, StringSplitOptions.RemoveEmptyEntries);

					foreach (string typePart in typeParts)
						doc.addTextField(nameof(card.Type), typePart, lang);
				}

				// Tested
				string text = card.GetText(lang);
				if (!string.IsNullOrEmpty(text))
					doc.addTextField(nameof(card.Text), text, lang);

				// Tested
				string flavor = card.GetFlavor(lang);
				if (!string.IsNullOrEmpty(flavor))
					doc.addTextField(nameof(card.Flavor), flavor, lang);
			}

			return doc;
		}

		private static IEnumerable<string> parseManaSymbols(string manaCost)
		{
			if (string.IsNullOrEmpty(manaCost))
				yield break;

			int open = -1;

			for (int i = 0; i < manaCost.Length; i++)
			{
				char c = manaCost[i];

				if (c == '{')
					open = i;
				else if (c == '}')
				{
					int length = i - open - 1;

					if (length > 0)
					{
						string symbol = manaCost.Substring(open + 1, length);
						yield return symbol;
					}

					open = -1;
				}
			}
		}

		private static void addIdField(this Document doc, string fieldName, string fieldValue)
		{
			fieldName = fieldName.ToLowerInvariant();

			doc.Add(new Field(
				fieldName,
				fieldValue,
				Field.Store.YES,
				Field.Index.NOT_ANALYZED_NO_NORMS));
		}

		private static void addTextField(this Document doc, string fieldName, string fieldValue)
		{
			fieldName = fieldName.ToLowerInvariant();
			addSpecificTextField(doc, fieldName, fieldValue);

			// поиск "по любому полю" и определённым языком требует,
			// чтобы значения нейтральных к языку полей попали
			// во все индексы "по любому полю" *_ru, *_en и т.д.
			foreach (var lang in Langs)
				addAnyTextField(doc, fieldValue, lang);
		}

		private static void addTextField(this Document doc, string fieldName, string fieldValue, string language)
		{
			fieldName = fieldName.ToLowerInvariant();

			if (language == null)
				throw new ArgumentNullException(nameof(language));

			var localizedFieldName = getLocalizedField(fieldName, language);
			addSpecificTextField(doc, localizedFieldName, fieldValue);

			addAnyTextField(doc, fieldValue, language);
		}

		private static void addSpecificTextField(Document doc, string fieldName, string fieldValue)
		{
			fieldName = fieldName.ToLowerInvariant();

			if (!TextFields.Contains(fieldName))
				throw new InvalidOperationException($"Text field {fieldName} not intialized");

			doc.Add(new Field(fieldName, fieldValue, Field.Store.NO, Field.Index.ANALYZED_NO_NORMS));
		}

		private static void addAnyTextField(Document doc, string fieldValue, string language)
		{
			var anyFieldName = getLocalizedField(AnyField, language);

			if (!TextFields.Contains(anyFieldName))
				throw new InvalidOperationException($"Text field {anyFieldName} not intialized");

			doc.Add(new Field(anyFieldName, fieldValue, Field.Store.NO, Field.Index.ANALYZED_NO_NORMS));
		}

		private static void addNumericField(this Document doc, string fieldName, float fieldValue)
		{
			fieldName = fieldName.ToLowerInvariant();

			if (!fieldName.IsFloatField())
				throw new ArgumentException($"Numeric float field {fieldName} not intialized");

			var field = new NumericField(fieldName);
			field.SetFloatValue(fieldValue);
			doc.Add(field);
			doc.Add(new Field(AnyField, fieldValue.ToString(CultureInfo.InvariantCulture), Field.Store.NO, Field.Index.ANALYZED_NO_NORMS));
		}

		private static void addNumericField(this Document doc, string fieldName, int fieldValue)
		{
			fieldName = fieldName.ToLowerInvariant();

			if (!fieldName.IsIntField())
				throw new ArgumentException($"Numeric int field {fieldName} not intialized");

			var field = new NumericField(fieldName);
			field.SetIntValue(fieldValue);
			doc.Add(field);
			doc.Add(new Field(AnyField, fieldValue.ToString(), Field.Store.NO, Field.Index.ANALYZED_NO_NORMS));
		}



		private static void addTextField(string fieldName, string displayField = null, bool isLimitedValues = false)
		{
			TextFields.Add(fieldName);
			UserFields.Add(fieldName);

			if (displayField != null)
				DisplayFieldByIndexField.Add(fieldName, displayField);

			if (isLimitedValues)
				LimitedValuesFields.Add(fieldName);
		}

		private static void addSpecificTextField(string fieldName, string language, bool isLimitedValues = false)
		{
			if (language == null)
				throw new ArgumentNullException(nameof(language));

			if (fieldName != AnyField)
				UserFields.Add(fieldName);

			LocalizedFields.Add(fieldName);
			var localizedFieldName = getLocalizedField(fieldName, language);
			TextFields.Add(localizedFieldName);

			DisplayFieldByIndexField.Add(localizedFieldName, fieldName);

			if (isLimitedValues)
				LimitedValuesFields.Add(localizedFieldName);
		}

		private static void addFloatField(string fieldName, string displayFieldName = null)
		{
			UserFields.Add(fieldName);
			FloatFields.Add(fieldName);

			if (displayFieldName != null)
				DisplayFieldByIndexField.Add(fieldName, displayFieldName);
		}

		private static void addIntField(string fieldName, string displayFieldName = null)
		{
			UserFields.Add(fieldName);
			IntFields.Add(fieldName);

			if (displayFieldName != null)
				DisplayFieldByIndexField.Add(fieldName, displayFieldName);
		}

		private static string getLocalizedField(string fieldName, string language)
		{
			if (language == null)
				throw new InvalidOperationException($"Language must be specified for localized field {fieldName}");

			return fieldName + "_" + language;
		}

		public static string Normalize(string field, string language)
		{
			if (string.IsNullOrEmpty(field))
				field = AnyField;

			field = field.ToLowerInvariant();

			if (LocalizedFields.Contains(field))
				return getLocalizedField(field, language);

			return field;
		}



		public static string GetId(this ScoreDoc scoreDoc, IndexSearcher indexSearcher)
		{
			return indexSearcher.Doc(scoreDoc.Doc).Get(nameof(Card.Id).ToLowerInvariant());
		}



		public static float? TryParseFloat(this string val)
		{
			int shift = val[0] - NumericUtils.SHIFT_START_INT;
			if (shift > 0 && shift <= 31)
				return null;

			return NumericUtils.SortableIntToFloat(NumericUtils.PrefixCodedToInt(val));
		}

		public static int? TryParseInt(this string val)
		{
			int shift = val[0] - NumericUtils.SHIFT_START_INT;
			if (shift > 0 && shift <= 31)
				return null;

			return NumericUtils.PrefixCodedToInt(val);
		}

		public static bool IsNumericField(this string field)
		{
			return field.IsFloatField() || field.IsIntField();
		}

		public static bool IsIntField(this string field)
		{
			return IntFields.Contains(field);
		}

		public static bool IsFloatField(this string field)
		{
			return FloatFields.Contains(field);
		}
	}
}