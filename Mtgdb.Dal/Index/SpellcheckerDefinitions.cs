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

		public static string GetDiscriminatorIn(this string userField, string lang)
		{
			var transformed = transform(userField, lang);

			if (_fieldDiscriminators.TryGetValue(transformed, out var discriminator))
				return discriminator;

			throw new ArgumentException($"discriminator not found for {transformed}", nameof(userField));
		}

		public static string GetSpellcheckerFieldIn(this string userField, string lang)
		{
			var transformed = transform(userField, lang);
			string localized = transformed.GetFieldLocalizedIn(lang);

			if (NotAnalyzedDuplicates.Contains(userField))
				return localized + NotAnalyzedSuffix;

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

			return Enumerable.Repeat((string) null, 1);
		}

		public const string NotAnalyzedSuffix = "na";

		public static readonly HashSet<string> NotAnalyzedDuplicates = new HashSet<string>(Str.Comparer)
		{
			nameof(Card.Name),
			nameof(Card.NameEn),
			nameof(Card.SetName),
			nameof(Card.Artist),

			nameof(Card.LegalIn),
			nameof(Card.RestrictedIn),
			nameof(Card.BannedIn)
		};

		private static readonly Dictionary<string, string> _fieldDiscriminators = new Dictionary<string, string>(Str.Comparer)
		{
			[nameof(Card.Text)] = "9",
			[nameof(Card.Flavor)] = "9",
			[nameof(Card.Name)] = "9",
			[nameof(Card.Type)] = "9",

			[nameof(Card.NameEn)] = "7",
			[nameof(Card.TextEn)] = "7",
			[nameof(Card.FlavorEn)] = "7",
			[nameof(Card.OriginalText)] = "7",



			[nameof(Card.TypeEn)] = "3",
			[nameof(Card.OriginalType)] = "3",
			[nameof(Card.Subtypes)] = "3",
			[nameof(Card.Artist)] = "3",

			[nameof(KeywordDefinitions.Keywords)] = "2",
			[nameof(Card.SetName)] = "2",
			[nameof(Card.SetCode)] = "2",
			[nameof(Card.ReleaseDate)] = "2",


			[nameof(Card.Color)] = "1",
			[nameof(Card.Supertypes)] = "1",
			[nameof(Card.Types)] = "1",
			[nameof(Card.LegalIn)] = "1",
			[nameof(Card.RestrictedIn)] = "1",
			[nameof(Card.BannedIn)] = "1",
			[nameof(Card.Power)] = "1",
			[nameof(Card.Toughness)] = "1",
			[nameof(Card.Loyalty)] = "1",
			[nameof(Card.Layout)] = "1",
			[nameof(Card.Rarity)] = "1",
			[nameof(Card.GeneratedMana)] = "1",
			[nameof(Card.ManaCost)] = "1",
			[nameof(Card.Rarity)] = "1",
		};
	}
}