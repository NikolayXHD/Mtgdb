using System;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Dal.Index
{
	public static class SpellcheckerDefinitions
	{
		public static bool IsAnalyzedIn(this string userField, string lang)
		{
			var spellcheckerField = userField.GetSpellcheckerFieldIn(lang);
			return DocumentFactory.NotAnalyzedFields.Contains(spellcheckerField);
		}

		public static string GetDiscriminantIn(this string userField, string lang)
		{
			var transformed = transform(userField, lang);

			if (SpellcheckerValueGetters.ContainsKey(transformed))
				return userField.GetSpellcheckerFieldIn(lang);

			var result = _fieldDiscriminants[transformed];
			return result;
		}

		public static bool SpellchekFromOriginalIndexIn(this string userField, string lang)
		{
			if (DocumentFactory.NumericFields.Contains(userField))
				return true;

			var transformed = transform(userField, lang);
			return _fieldDiscriminants.ContainsKey(transformed);
		}

		public static bool IsDiscriminantExclusiveToField(string discriminant)
		{
			return !discriminant.All(char.IsDigit);
		}



		public static string GetSpellcheckerFieldIn(this string userField, string lang)
		{
			var transformed = transform(userField, lang);
			string localized = transformed.GetFieldLocalizedIn(lang);

			return localized;
		}

		private static string transform(string userField, string lang)
		{
			if (Str.Equals(userField, MtgQueryParser.Like))
				return nameof(Card.NameEn);

			if (DocumentFactory.LocalizedFields.Contains(userField) && Str.Equals(lang, CardLocalization.DefaultLanguage))
				return userField.GetFieldLocalizedIn(lang);
			
			return userField;
		}

		public static IEnumerable<string> GetIndexedUserFields()
		{
			return DocumentFactory.UserFields.Where(f => !Str.Equals(f, MtgQueryParser.Like) && IsIndexedInSpellchecker(f));
		}

		public static bool IsIndexedInSpellchecker(this string userField)
		{
			return !DocumentFactory.NumericFields.Contains(userField);
		}

		public static IEnumerable<string> GetFieldLanguages(this string userField)
		{
			if (DocumentFactory.LocalizedFields.Contains(userField))
				return DocumentFactory.Languages;

			return Unit.Sequence((string) null);
		}

		public static Dictionary<string, Func<Card, string, IEnumerable<string>>> SpellcheckerValueGetters { get; } =
			new Dictionary<string, Func<Card, string, IEnumerable<string>>>(Str.Comparer)
			{
				[nameof(Card.Name)] = (c, lang) => Unit.Sequence(c.GetName(lang)),
				[nameof(Card.NameEn)] = (c, lang) => Unit.Sequence(c.NameEn),
				[nameof(Card.Artist)] = (c, lang) => Unit.Sequence(c.Artist),
				[nameof(Card.SetName)] = (c, lang) => Unit.Sequence(c.SetName),
				[nameof(Card.LegalIn)] = (c, lang) => c.LegalFormats,
				[nameof(Card.RestrictedIn)] = (c, lang) => c.RestrictedFormats,
				[nameof(Card.BannedIn)] = (c, lang) => c.BannedFormats
			};

		private static readonly Dictionary<string, string> _fieldDiscriminants = new Dictionary<string, string>(Str.Comparer)
		{
			[nameof(Card.Text)] = "9",
			[nameof(Card.Flavor)] = "9",
			[nameof(Card.Type)] = "9",

			[nameof(Card.TextEn)] = "7",
			[nameof(Card.FlavorEn)] = "7",
			[nameof(Card.OriginalText)] = "7",

			[nameof(Card.TypeEn)] = "3",
			[nameof(Card.OriginalType)] = "3",
			[nameof(Card.Subtypes)] = "3",
			[nameof(Card.ReleaseDate)] = "3",

			[nameof(KeywordDefinitions.Keywords)] = "2",
			[nameof(Card.SetCode)] = "2",

			[nameof(Card.Color)] = "1",
			[nameof(Card.Supertypes)] = "1",
			[nameof(Card.Types)] = "1",
			[nameof(Card.Power)] = "1",
			[nameof(Card.Toughness)] = "1",
			[nameof(Card.Loyalty)] = "1",
			[nameof(Card.Layout)] = "1",
			[nameof(Card.Rarity)] = "1",
			[nameof(Card.GeneratedMana)] = "1",
			[nameof(Card.ManaCost)] = "1",
			[nameof(Card.Rarity)] = "1"
		};
	}
}