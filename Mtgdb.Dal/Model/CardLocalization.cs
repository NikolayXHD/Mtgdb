using System;
using System.Collections.Generic;

namespace Mtgdb.Dal
{
	public class CardLocalization
	{
		private readonly CardLocalizationRaw _raw;

		internal CardLocalization(CardLocalizationRaw raw)
		{
			_raw = raw;
		}

		public string GetName(string language)
		{
			switch (language)
			{
				case "en":
					return _raw.Name;
				case "cn":
					return _raw.NameCn;
				case "tw":
					return _raw.NameTw;
				case "fr":
					return _raw.NameFr;
				case "de":
					return _raw.NameDe;
				case "it":
					return _raw.NameIt;
				case "jp":
					return _raw.NameJp;
				case "pt":
					return _raw.NamePt;
				case "ru":
					return _raw.NameRu;
				case "es":
					return _raw.NameEs;
				case "kr":
					return _raw.NameKo;
				default:
					throw new InvalidOperationException($"language {language} not supported");
			}
		}

		public string GetType(string language)
		{
			switch (language)
			{
				case "en":
					return _raw.Type;
				case "cn":
					return _raw.TypeCn;
				case "tw":
					return _raw.TypeTw;
				case "fr":
					return _raw.TypeFr;
				case "de":
					return _raw.TypeDe;
				case "it":
					return _raw.TypeIt;
				case "jp":
					return _raw.TypeJp;
				case "pt":
					return _raw.TypePt;
				case "ru":
					return _raw.TypeRu;
				case "es":
					return _raw.TypeEs;
				case "kr":
					return _raw.TypeKo;
				default:
					throw new InvalidOperationException($"language {language} not supported");
			}
		}

		public string GetAbility(string language)
		{
			switch (language)
			{
				case "en":
					return _raw.Ability;
				case "cn":
					return _raw.AbilityCn;
				case "tw":
					return _raw.AbilityTw;
				case "fr":
					return _raw.AbilityFr;
				case "de":
					return _raw.AbilityDe;
				case "it":
					return _raw.AbilityIt;
				case "jp":
					return _raw.AbilityJp;
				case "pt":
					return _raw.AbilityPt;
				case "ru":
					return _raw.AbilityRu;
				case "es":
					return _raw.AbilityEs;
				case "kr":
					return _raw.AbilityKo;
				default:
					throw new InvalidOperationException($"language {language} not supported");
			}
		}

		public string GetFlavor(string language)
		{
			switch (language)
			{
				case "en":
					return _raw.Flavor;
				case "cn":
					return _raw.FlavorCn;
				case "tw":
					return _raw.FlavorTw;
				case "fr":
					return _raw.FlavorFr;
				case "de":
					return _raw.FlavorDe;
				case "it":
					return _raw.FlavorIt;
				case "jp":
					return _raw.FlavorJp;
				case "pt":
					return _raw.FlavorPt;
				case "ru":
					return _raw.FlavorRu;
				case "es":
					return _raw.FlavorEs;
				case "kr":
					return _raw.FlavorKo;
				default:
					throw new InvalidOperationException($"language {language} not supported");
			}
		}

		public static IEnumerable<string> GetAllLanguages()
		{
			yield return "en";
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

		public float? PricingLow => _raw.PricingLow;
		public float? PricingMid => _raw.PricingMid;
		public float? PricingHigh => _raw.PricingHigh;
		public string GeneratedMana => _raw.GeneratedMana;
	}
}