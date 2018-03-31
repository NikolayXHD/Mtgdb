using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace Mtgdb.Dal.Index
{
	public static class DocumentFactory
	{
		static DocumentFactory()
		{
			_langs = CardLocalization.GetAllLanguages().ToList();

			addTextField(nameof(Card.Color), analyze: false);

			addTextField(nameof(Card.SetName));
			addTextField(nameof(Card.SetCode), analyze: false);
			addTextField(nameof(Card.Artist));

			addTextField(nameof(Card.OriginalText), nameof(Card.Text));
			addTextField(nameof(Card.OriginalType), nameof(Card.Type));

			addTextField(nameof(Card.NameEn),
				nameof(Card.Name));

			addTextField(nameof(Card.TextEn),
				nameof(Card.Text));

			addTextField(nameof(Card.FlavorEn),
				nameof(Card.Flavor));

			addTextField(nameof(Card.TypeEn),
				nameof(Card.Type));

			addTextField(nameof(Card.Supertypes),
				nameof(Card.Type),
				analyze: false);

			addTextField(nameof(Card.Types),
				nameof(Card.Type),
				analyze: false);

			addTextField(nameof(Card.Subtypes),
				nameof(Card.Type),
				analyze: false);

			addTextField(nameof(Card.LegalIn),
				nameof(Card.Rulings));

			addTextField(nameof(Card.RestrictedIn),
				nameof(Card.Rulings));

			addTextField(nameof(Card.BannedIn),
				nameof(Card.Rulings));

			addTextField(nameof(Card.GeneratedMana),
				nameof(Card.Text),
				analyze: false);

			addTextField(nameof(Card.Power), analyze: false);
			addTextField(nameof(Card.Toughness), analyze: false);
			addTextField(nameof(Card.Loyalty), analyze: false);
			addTextField(nameof(Card.ReleaseDate), analyze: false);
			addTextField(nameof(Card.Layout), analyze: false);
			addTextField(nameof(Card.ManaCost));
			addTextField(nameof(Card.Rarity));

			addFloatField(nameof(Card.PowerNum),
				nameof(Card.Power));
			addFloatField(nameof(Card.ToughnessNum),
				nameof(Card.Toughness));
			addIntField(nameof(Card.LoyaltyNum),
				nameof(Card.Loyalty));

			addIntField(nameof(Card.Hand));
			addIntField(nameof(Card.Life));

			addFloatField(nameof(Card.Cmc));
			addFloatField(nameof(Card.PricingLow));
			addFloatField(nameof(Card.PricingMid));
			addFloatField(nameof(Card.PricingHigh));

			foreach (var lang in _langs)
			{
				if (Str.Equals(lang, CardLocalization.DefaultLanguage))
					continue;

				addSpecificTextField(nameof(Card.Name), lang);
				addSpecificTextField(nameof(Card.Type), lang);
				addSpecificTextField(nameof(Card.Text), lang);
				addSpecificTextField(nameof(Card.Flavor), lang);
			}
		}

		public static Document ToDocument(this CardKeywords cardKeywords)
		{
			var doc = new Document();
			doc.addIdField(nameof(CardKeywords.IndexInFile), cardKeywords.IndexInFile);

			foreach (var pair in cardKeywords.KeywordsByProperty)
				foreach (string value in pair.Value)
				{
					doc.Add(new StringField(
						pair.Key.ToLowerInvariant(),
						value.ToLowerInvariant(),
						Field.Store.NO));
				}

			return doc;
		}

		public static Document ToDocument(this Card card)
		{
			var doc = new Document();

			// Tested
			doc.addIdField(nameof(card.IndexInFile), card.IndexInFile);

			// tested
			if (card.ColorsArr?.Count > 0)
				foreach (var color in card.ColorsArr)
					doc.addTextField(nameof(card.Color), color);
			else if (!string.IsNullOrEmpty(card.Color))
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

			// Tested
			if (!string.IsNullOrEmpty(card.TextEn))
				doc.addTextField(nameof(card.TextEn), card.TextEn);

			// Tested
			if (!string.IsNullOrEmpty(card.FlavorEn))
				doc.addTextField(nameof(card.FlavorEn), card.FlavorEn);

			// Tested
			if (!string.IsNullOrEmpty(card.TypeEn))
				doc.addTextField(nameof(card.TypeEn), card.TypeEn);

			// Tested
			if (card.SupertypesArr != null)
				foreach (string type in card.SupertypesArr)
					doc.addTextField(nameof(card.Supertypes), type);

			// Tested
			if (card.TypesArr != null)
				foreach (string type in card.TypesArr)
					doc.addTextField(nameof(card.Types), type);

			// Tested
			if (card.SubtypesArr != null)
				foreach (string type in card.SubtypesArr)
					doc.addTextField(nameof(card.Subtypes), type);

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
			foreach (var mana in card.GeneratedManaArr)
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
			if (card.PricingHigh.HasValue)
				doc.addNumericField(nameof(card.PricingHigh), card.PricingHigh.Value);

			// Tested
			if (card.PricingMid.HasValue)
				doc.addNumericField(nameof(card.PricingMid), card.PricingMid.Value);

			// Tested
			if (card.PricingLow.HasValue)
				doc.addNumericField(nameof(card.PricingLow), card.PricingLow.Value);

			// Tested
			if (!string.IsNullOrEmpty(card.Layout))
				doc.addTextField(nameof(Card.Layout), card.Layout);

			foreach (var lang in _langs)
			{
				// Tested
				string name = card.Localization?.GetName(lang);
				if (!string.IsNullOrEmpty(name))
					doc.addTextField(nameof(Card.Name), name, lang);

				// Tested
				string type = card.Localization?.GetType(lang);
				if (!string.IsNullOrEmpty(type))
				{
					var typeParts = type.Split(new[] { ' ', '—' }, StringSplitOptions.RemoveEmptyEntries);

					foreach (string typePart in typeParts)
						doc.addTextField(nameof(Card.Type), typePart, lang);
				}

				// Tested
				string text = card.Localization?.GetAbility(lang);
				if (!string.IsNullOrEmpty(text))
					doc.addTextField(nameof(Card.Text), text, lang);

				// Tested
				string flavor = card.Localization?.GetFlavor(lang);
				if (!string.IsNullOrEmpty(flavor))
					doc.addTextField(nameof(Card.Flavor), flavor, lang);
			}

			return doc;
		}

		private static void addIdField(this Document doc, string fieldName, int fieldValue)
		{
			fieldName = fieldName.ToLowerInvariant();
			var field = new Int32Field(fieldName, fieldValue, Field.Store.YES);

			doc.Add(field);
		}

		private static void addTextField(this Document doc, string fieldName, string fieldValue)
		{
			fieldName = fieldName.ToLowerInvariant();
			addSpecificTextField(doc, fieldName, fieldValue);
		}

		private static void addTextField(this Document doc, string fieldName, string fieldValue, string language)
		{
			if (language == null)
				throw new ArgumentNullException(nameof(language));

			fieldName = fieldName.ToLowerInvariant();

			var localizedFieldName = getLocalizedField(fieldName, language);
			addSpecificTextField(doc, localizedFieldName, fieldValue);
		}

		private static void addSpecificTextField(Document doc, string fieldName, string fieldValue)
		{
			fieldName = fieldName.ToLowerInvariant();

			if (!TextFields.Contains(fieldName))
				throw new InvalidOperationException($"Text field {fieldName} not intialized");

			TextField field;
			if (NotAnalyzedFields.Contains(fieldName))
				field = new TextField(fieldName, new SingleTokenTokenStream(new Token(fieldValue.ToLowerInvariant(), 0, fieldValue.Length)));
			else
				field = new TextField(fieldName, fieldValue, Field.Store.NO);

			doc.Add(field);
		}

		private static void addNumericField(this Document doc, string fieldName, float fieldValue)
		{
			fieldName = fieldName.ToLowerInvariant();

			if (!fieldName.IsFloatField())
				throw new ArgumentException($"Numeric float field {fieldName} not intialized");

			var field = new SingleField(fieldName, fieldValue, Field.Store.NO);
			doc.Add(field);
		}

		private static void addNumericField(this Document doc, string fieldName, int fieldValue)
		{
			fieldName = fieldName.ToLowerInvariant();

			if (!fieldName.IsIntField())
				throw new ArgumentException($"Numeric int field {fieldName} not intialized");

			var field = new Int32Field(fieldName, fieldValue, Field.Store.NO);
			doc.Add(field);
		}



		private static void addTextField(string fieldName, string displayField = null, bool analyze = true)
		{
			TextFields.Add(fieldName);
			UserFields.Add(fieldName);

			if (displayField != null)
				DisplayFieldByIndexField.Add(fieldName, displayField);

			if (!analyze)
				NotAnalyzedFields.Add(fieldName);
		}

		private static void addSpecificTextField(string fieldName, string language)
		{
			if (language == null)
				throw new ArgumentNullException(nameof(language));

			UserFields.Add(fieldName);

			fieldName = fieldName.ToLowerInvariant();

			_localizedFields.Add(fieldName);

			var localizedFieldName = getLocalizedField(fieldName, language);
			TextFields.Add(localizedFieldName);

			DisplayFieldByIndexField.Add(localizedFieldName, fieldName);
		}

		private static void addFloatField(string fieldName, string displayFieldName = null)
		{
			UserFields.Add(fieldName);
			_floatFields.Add(fieldName);

			if (displayFieldName != null)
				DisplayFieldByIndexField.Add(fieldName, displayFieldName);
		}

		private static void addIntField(string fieldName, string displayFieldName = null)
		{
			UserFields.Add(fieldName);
			_intFields.Add(fieldName);

			if (displayFieldName != null)
				DisplayFieldByIndexField.Add(fieldName, displayFieldName);
		}

		private static string getLocalizedField(string fieldName, string language)
		{
			if (language == null)
				throw new InvalidOperationException($"Language must be specified for localized field {fieldName}");

			if (Str.Equals(language, CardLocalization.DefaultLanguage))
				return fieldName + CardLocalization.DefaultLanguage;

			return fieldName + "_" + language;
		}

		public static string Localize(string field, string language)
		{
			field = field.ToLowerInvariant();

			if (_localizedFields.Contains(field))
				return getLocalizedField(field, language);

			return field;
		}



		public static int GetId(this ScoreDoc scoreDoc, IndexSearcher indexSearcher)
		{
			var doc = indexSearcher.Doc(scoreDoc.Doc);
			string value = doc.Get(nameof(Card.IndexInFile).ToLowerInvariant());
			return int.Parse(value);
		}

		public static float? TryParseFloat(this BytesRef val)
		{
			if (val == null)
				return null;

			if (val.Length < 6)
				return null;

			int intVal = NumericUtils.PrefixCodedToInt32(val);
			float result = NumericUtils.SortableInt32ToSingle(intVal);
			return result;
		}

		public static int? TryParseInt(this BytesRef val)
		{
			if (val == null)
				return null;

			if (val.Length < 6)
				return null;

			int intVal = NumericUtils.PrefixCodedToInt32(val);
			return intVal;
		}

		public static bool IsNumericField(this string field)
		{
			return field.IsFloatField() || field.IsIntField();
		}

		public static bool IsIntField(this string field)
		{
			return _intFields.Contains(field);
		}

		public static bool IsFloatField(this string field)
		{
			return _floatFields.Contains(field);
		}

		private static readonly List<string> _langs;

		private static readonly HashSet<string> _intFields = new HashSet<string>(Str.Comparer);
		private static readonly HashSet<string> _floatFields = new HashSet<string>(Str.Comparer);
		public static readonly HashSet<string> TextFields = new HashSet<string>(Str.Comparer);

		private static readonly HashSet<string> _localizedFields = new HashSet<string>(Str.Comparer);
		public static readonly HashSet<string> UserFields = new HashSet<string>(Str.Comparer);
		public static readonly HashSet<string> NotAnalyzedFields = new HashSet<string>(Str.Comparer);

		public static readonly Dictionary<string, Func<Card, string>> LimitedValueGetters =
			new Dictionary<string, Func<Card, string>>(Str.Comparer)
			{
				[nameof(Card.SetName)] = c => c.SetName,
				[nameof(Card.SetCode)] = c => c.SetCode,
				[nameof(Card.Artist)] = c => c.Artist,
				[nameof(Card.Supertypes)] = c => c.Supertypes,
				[nameof(Card.Types)] = c => c.Types,
				[nameof(Card.Power)] = c => c.Power,
				[nameof(Card.Toughness)] = c => c.Toughness,
				[nameof(Card.Loyalty)] = c => c.Loyalty,
				[nameof(Card.Rarity)] = c => c.Rarity,
				[nameof(Card.ReleaseDate)] = c => c.ReleaseDate,
				[nameof(Card.Layout)] = c => c.Layout
			};

		public static readonly Dictionary<string, Func<Card, string, string>> LimitedLocalizedValueGetters =
			new Dictionary<string, Func<Card, string, string>>(Str.Comparer);

		public static readonly HashSet<string> CombinatoricValueFields = new HashSet<string>
		{
			nameof(Card.OriginalType),
			nameof(Card.TypeEn),
			nameof(Card.Type),
			nameof(Card.Subtypes),
			nameof(Card.LegalIn),
			nameof(Card.RestrictedIn),
			nameof(Card.BannedIn),
			nameof(Card.Color),
			nameof(Card.GeneratedMana),
			nameof(Card.ManaCost)
		};

		public static readonly Dictionary<string, string> DisplayFieldByIndexField = new Dictionary<string, string>(Str.Comparer);
	}
}