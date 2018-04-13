using System;
using System.Collections.Generic;

namespace Mtgdb.Dal
{
	public class CardLocalization
	{
		private readonly Dictionary<string, Translation> _translations =
			new Dictionary<string, Translation>(Str.Comparer);

		private readonly Dictionary<string, ForeignName> _names =
			new Dictionary<string, ForeignName>(Str.Comparer);

		public void Add(Card card, ForeignName name, Translation translation)
		{
			var lang = getLang(name.Language);
			
			if (_names.ContainsKey(lang) || _translations.ContainsKey(lang))
				return;

			_translations.Add(lang, translation);

			if (!string.IsNullOrEmpty(name.Name) && !Str.Equals(name.Name, card.NameEn))
				_names.Add(lang, name);
		}

		private static string getLang(string language)
		{
			switch (language)
			{
				case "French":
					return "fr";
				case "Chinese Simplified":
					return "cn";
				case "Chinese Traditional":
					return "tw";
				case "German":
					return "de";
				case "Italian":
					return "it";
				case "Japanese":
					return "jp";
				case "Korean":
					return "kr";
				case "Portuguese (Brazil)":
					return "pt";
				case "Russian":
					return "ru";
				case "Spanish":
					return "es";
				default:
					throw new NotSupportedException();
			}
		}

		public string GetName(string language)
		{
			return _names.TryGet(language)?.Name;
		}

		public string GetType(string language)
		{
			return _translations.TryGet(language)?.Type;
		}

		public string GetAbility(string language)
		{
			return _translations.TryGet(language)?.Text;
		}

		public string GetFlavor(string language)
		{
			return _translations.TryGet(language)?.Flavor;
		}

		public const string DefaultLanguage = "en";

		public static IEnumerable<string> GetAllLanguages()
		{
			yield return DefaultLanguage;
			yield return "cn";
			yield return "tw";
			yield return "fr";
			yield return "de";
			yield return "it";
			yield return "jp";
			yield return "pt";
			yield return "ru";
			yield return "es";
			yield return "kr";
		}
	}
}