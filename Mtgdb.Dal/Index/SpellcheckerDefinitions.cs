using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Dal.Index
{
	public static class SpellcheckerDefinitions
	{
		public static bool IsAnalyzedIn(this string userField, string lang)
		{
			var spellcheckerField = userField.GetSpellcheckerFieldIn(lang);
			return DocumentFactory.NotAnalyzedFields.Contains(spellcheckerField);
		}

		public static bool IsIndexedInSpellchecker(this string userField, string lang)
		{
			var transformed = transform(userField, lang);
			return SpellcheckerValueGetters.ContainsKey(transformed);
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
			return DocumentFactory.UserFields.Where(f => !Str.Equals(f, MtgQueryParser.Like) && !DocumentFactory.NumericFields.Contains(f));
		}

		public static IEnumerable<string> GetFieldLanguages(this string userField)
		{
			if (DocumentFactory.LocalizedFields.Contains(userField))
				return DocumentFactory.Languages;

			return Unit.Sequence((string) null);
		}

		public static IReadOnlyList<string> ReadAllValuesFrom(this DirectoryReader reader, string field)
		{
			var rawValues = reader.ReadRawValuesFrom(field);

			IEnumerable<string> enumerable;

			if (field.IsFloatField())
				enumerable = rawValues.Select(term => term.TryParseFloat())
					.Where(val => val.HasValue)
					.Select(val => val.Value)
					.Distinct()
					.OrderBy(val => val)
					.Select(val => val.ToString(Str.Culture));

			else if (field.IsIntField())
				enumerable = rawValues.Select(term => term.TryParseInt())
					.Where(val => val.HasValue)
					.Select(val => val.Value)
					.Distinct()
					.OrderBy(val => val)
					.Select(val => val.ToString(Str.Culture));
			
			else
				enumerable = rawValues.Select(term => term.Utf8ToString())
					.Distinct()
					.OrderBy(Str.Comparer);

			var result = enumerable.ToReadOnlyList();
			return result;
		}

		public static Dictionary<string, Func<Card, string, IEnumerable<string>>> SpellcheckerValueGetters { get; } =
			new Dictionary<string, Func<Card, string, IEnumerable<string>>>(Str.Comparer)
			{
				[nameof(Card.Name)] = (c, lang) => c.Localization?.GetName(lang)?.Invoke(Unit.Sequence) ?? Enumerable.Empty<string>(),
				[nameof(Card.NameEn)] = (c, lang) => Unit.Sequence(c.NameEn),
				[nameof(Card.Artist)] = (c, lang) => Unit.Sequence(c.Artist),
				[nameof(Card.SetName)] = (c, lang) => Unit.Sequence(c.SetName)
			};
	}
}