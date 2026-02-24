using System;
using System.Collections.Generic;

namespace Mtgdb.Data
{
	public class CardLocalization
	{
		public static string GetLang(string language)
		{
			switch (language)
			{
				case "French":
					return "fr";
				case "Chinese Simplified":
				case "Simplified Chinese":
					return "cn";
				case "Chinese Traditional":
				case "Traditional Chinese":
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
				case "Portuguese":
					return "pt";
				case "Russian":
					return "ru";
				case "Spanish":
					return "es";
				default:
					return null;
			}
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
