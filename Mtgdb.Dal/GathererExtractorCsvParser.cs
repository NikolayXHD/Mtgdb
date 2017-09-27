using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	internal class GathererExtractorCsvParser
	{
		public int Count => _rows.Count;

		public GathererExtractorCsvParser(string text)
		{
			const string nameSeparator = " // ";
			var nameSeparators = new[] { nameSeparator };

			var lines = text.Split(SeparatorRow);

			var fieldsArray = lines[1].Split(new [] {SeparatorField}, StringSplitOptions.None);

			_fields = Enumerable.Range(0, fieldsArray.Length)
				.ToDictionary(i => fieldsArray[i], Str.Comparer);

			_rows = new List<List<string>>();

			foreach (string line in lines.Skip(3))
			{
				var fields = line.Split(new[] { SeparatorField }, StringSplitOptions.None).ToList();

				if (fields.Count <= 1 && (fields.Count == 0 || fields[0] == string.Empty))
					continue;
				
				if (fields[0].Contains(nameSeparator))
				{
					var fields1 = new List<string>(fields.Count);
					var fields2 = new List<string>(fields.Count);

					for (int i = 0; i < fields.Count; i++)
					{
						var valueParts = fields[i].Split(nameSeparators, StringSplitOptions.None);
						if (valueParts.Length == 2)
						{
							fields1.Add(valueParts[0]);
							fields2.Add(valueParts[1]);
						}
						else
						{
							fields1.Add(fields[i]);
							fields2.Add(fields[i]);
						}
					}
					
					_rows.Add(fields1);
					_rows.Add(fields2);
				}
				else
				{
					_rows.Add(fields);
				}
			}
		}

		public CardLocalizationRaw Read(int i)
		{
			var row = _rows[i];

			var result = new CardLocalizationRaw
			{
				Set = intern(row[_fields[FieldSetCode]]),
				
				Name = intern(row[_fields[FieldName]]),
				NameCn = intern(nullify(row[_fields[FieldName + LangCn]])),
				NameTw = intern(nullify(row[_fields[FieldName + LangTw]])),
				NameFr = intern(nullify(row[_fields[FieldName + LangFr]])),
				NameDe = intern(nullify(row[_fields[FieldName + LangDe]])),
				NameIt = intern(nullify(row[_fields[FieldName + LangIt]])),
				NameJp = intern(nullify(row[_fields[FieldName + LangJp]])),
				NamePt = intern(nullify(row[_fields[FieldName + LangPt]])),
				NameRu = intern(nullify(row[_fields[FieldName + LangRu]])),
				NameEs = intern(nullify(row[_fields[FieldName + LangEs]])),
				NameKo = intern(nullify(row[_fields[FieldName + LangKo]])),

				Type = intern(row[_fields[FieldType]]),
				TypeCn = intern(nullify(row[_fields[FieldType + LangCn]])),
				TypeTw = intern(nullify(row[_fields[FieldType + LangTw]])),
				TypeFr = intern(nullify(row[_fields[FieldType + LangFr]])),
				TypeDe = intern(nullify(row[_fields[FieldType + LangDe]])),
				TypeIt = intern(nullify(row[_fields[FieldType + LangIt]])),
				TypeJp = intern(nullify(row[_fields[FieldType + LangJp]])),
				TypePt = intern(nullify(row[_fields[FieldType + LangPt]])),
				TypeRu = intern(nullify(row[_fields[FieldType + LangRu]])),
				TypeEs = intern(nullify(row[_fields[FieldType + LangEs]])),
				TypeKo = intern(nullify(row[_fields[FieldType + LangKo]])),

				Ability = intern(normalize(row[_fields[FieldAbility]])),
				AbilityCn = intern(normalize(row[_fields[FieldAbility + LangCn]])),
				AbilityTw = intern(normalize(row[_fields[FieldAbility + LangTw]])),
				AbilityFr = intern(normalize(row[_fields[FieldAbility + LangFr]])),
				AbilityDe = intern(normalize(row[_fields[FieldAbility + LangDe]])),
				AbilityIt = intern(normalize(row[_fields[FieldAbility + LangIt]])),
				AbilityJp = intern(normalize(row[_fields[FieldAbility + LangJp]])),
				AbilityPt = intern(normalize(row[_fields[FieldAbility + LangPt]])),
				AbilityRu = intern(normalize(row[_fields[FieldAbility + LangRu]])),
				AbilityEs = intern(normalize(row[_fields[FieldAbility + LangEs]])),
				AbilityKo = intern(normalize(row[_fields[FieldAbility + LangKo]])),

				Flavor = intern(normalize(row[_fields[FieldFlavor]])),
				FlavorCn = intern(normalize(row[_fields[FieldFlavor + LangCn]])),
				FlavorTw = intern(normalize(row[_fields[FieldFlavor + LangTw]])),
				FlavorFr = intern(normalize(row[_fields[FieldFlavor + LangFr]])),
				FlavorDe = intern(normalize(row[_fields[FieldFlavor + LangDe]])),
				FlavorIt = intern(normalize(row[_fields[FieldFlavor + LangIt]])),
				FlavorJp = intern(normalize(row[_fields[FieldFlavor + LangJp]])),
				FlavorPt = intern(normalize(row[_fields[FieldFlavor + LangPt]])),
				FlavorRu = intern(normalize(row[_fields[FieldFlavor + LangRu]])),
				FlavorEs = intern(normalize(row[_fields[FieldFlavor + LangEs]])),
				FlavorKo = intern(normalize(row[_fields[FieldFlavor + LangKo]]))
			};

			return result;
		}

		private static string intern(string value)
		{
			if (value == null)
				return null;

			return string.Intern(value);
		}

		private static string normalize(string str)
		{
			if (str == string.Empty)
				return null;

			str = _normalizePattern.Replace(str, match => match.Value == SeparatorLine ? "\n" : string.Empty);
			str = IncompleteChaosPattern.Replace(str, "{CHAOS}");

			return str;
		}

		private static string nullify(string str)
		{
			if (str == string.Empty)
				return null;

			return str;
		}

		private readonly Dictionary<string, int> _fields;
		private readonly List<List<string>> _rows;

		private const string SeparatorLine = "£";

		private static readonly Regex _normalizePattern = new Regex("£|#", RegexOptions.Compiled);
		public static readonly Regex IncompleteChaosPattern = new Regex("(?<!{)CHAOS(?!})", RegexOptions.Compiled);

		private const char SeparatorRow = '\r';
		private const string SeparatorField = "||";

		private const string FieldName = "name";
		private const string FieldSetCode = "set_code";
		private const string FieldType = "type";

		private const string FieldFlavor = "flavor";

		private const string FieldAbility = "ability";

		private const string LangCn = "_CN";
		private const string LangTw = "_TW";
		private const string LangFr = "_FR";
		private const string LangDe = "_DE";
		private const string LangIt = "_IT";
		private const string LangJp = "_JP";
		private const string LangPt = "_PT";
		private const string LangRu = "_RU";
		private const string LangEs = "_ES";
		private const string LangKo = "_KO";

		/*
		private static float? parseFloat(string val)
		{
			if (val == string.Empty || val == "N/A")
				return null;

			return float.Parse(val);
		}

		private const string FieldPricingLow = "pricing_low";
		private const string FieldPricingMid = "pricing_mid";
		private const string FieldPricingHigh = "pricing_high";
		private const string FieldRarity = "rarity";
		private const string FieldRuling = "ruling";
		private const string FieldVariation = "variation";
		private const string FieldColor = "color";
		private const string FieldGeneratedMana = "generated_mana";
		private const string FieldManaCost = "manacost";
		private const string FieldConvertedManaCost = "converted_manacost";
		private const string FieldArtist = "artist";
		private const string FieldPower = "power";
		private const string FieldToughness = "toughness";
		private const string FieldLoyalty = "loyalty";
		private const string FieldId = "id";
		private const string FieldSet = "set";

		private const string SeparatorFlip = "———";
		private const string TagItalicBegin = "#_";
		private const string TagItalicEnd = "_#";
		*/
	}
}
