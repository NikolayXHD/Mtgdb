using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class CardRepository
	{
		public Func<string, bool> FilterSetCode { get; set; } =
			// str => Str.Equals(str, "ISD");
			_ => true;

		internal bool RememberOriginalPrices { get; set; }

		public CardRepository()
		{
			SetsFile = AppDir.Data.AddPath("AllPrintings.json");
			PricesFile = AppDir.Data.AddPath("AllPrices.json");
			CustomSetCodes = new string[0];
			PatchFile = AppDir.Data.AddPath("patch.v2.json");
			Cards = new List<Card>();
		}

		public void LoadFile()
		{
			_defaultSetsContent = File.ReadAllBytes(SetsFile);
			_priceContent = File.Exists(PricesFile)
				? File.ReadAllBytes(PricesFile)
				: null;

			_customSetContents = CustomSetCodes
				.Select(code => File.ReadAllBytes(AppDir.Data.AddPath("custom_sets").AddPath(code + ".json")))
				.ToArray();

			Patch = JsonConvert.DeserializeObject<Patch>(File.ReadAllText(PatchFile));
			Patch.IgnoreCase();

			IsFileLoadingComplete = true;
		}

		private IEnumerable<Set> deserializeSets()
		{
			return deserializeSets(_defaultSetsContent)
				.Concat(Enumerable.Range(0, CustomSetCodes.Length)
					.Where(i => FilterSetCode(CustomSetCodes[i]))
					.Select(i => deserializeSet(_customSetContents[i])));

			Set deserializeSet(byte[] content)
			{
				using var stream = new MemoryStream(content);
				using var stringReader = new StreamReader(stream);
				using var jsonReader = new JsonTextReader(stringReader);
				var set = new JsonSerializer().Deserialize<Set>(jsonReader);
				return set;
			}

			IEnumerable<Set> deserializeSets(byte[] content)
			{
				var serializer = new JsonSerializer();
				using var stream = new MemoryStream(content);
				using var stringReader = new StreamReader(stream);
				using var jsonReader = new JsonTextReader(stringReader);
				jsonReader.Read();

				while (true)
				{
					jsonReader.Read();

					if (jsonReader.TokenType == JsonToken.EndObject)
						// sets are over, all json was read
						break;

					string setCode = (string) jsonReader.Value;

					// skip set name
					jsonReader.Read();

					if (!FilterSetCode(setCode) || _customSetCodesSet.Contains(setCode))
					{
						jsonReader.Skip();
						continue;
					}

					var set = serializer.Deserialize<Set>(jsonReader);
					yield return set;
				}
			}
		}

		private Dictionary<string, PricePatch> deserializePrices()
		{
			if (_priceContent == null)
				return null;

			using Stream stream = new MemoryStream(_priceContent);
			using var stringReader = new StreamReader(stream);
			using var jsonReader = new JsonTextReader(stringReader);
			var result = new JsonSerializer().Deserialize<Dictionary<string, PricePatch>>(jsonReader);
			return result;
		}

		public void Load()
		{
			var prices = deserializePrices();

			foreach (var set in deserializeSets())
			{
				for (int i = 0; i < set.Cards.Count; i++)
				{
					var card = set.Cards[i];
					card.Set = set;
					card.Id = CardId.Generate(card);
					if (prices != null && prices.TryGetValue(card.MtgjsonId, out var pricePatch))
					{
						if (RememberOriginalPrices)
							card.OriginalPrices = card.Prices;
						card.Prices = pricePatch.Prices;
					}
					preProcessCard(card);
				}

				for (int i = set.Cards.Count - 1; i >= 0; i--)
					if (set.Cards[i].Remove)
						set.Cards.RemoveAt(i);

				// after preProcessCard, to have NameNormalized field set non empty
				set.CardsByName = set.Cards.GroupBy(_ => _.NameNormalized)
					.ToDictionary(
						gr => gr.Key,
						gr => gr.ToList(),
						Str.Comparer);

				foreach (var card in set.Cards)
					CardsById[card.Id] = card;

				for (int i = 0; i < set.Cards.Count; i++)
					postProcessCard(set.Cards[i]);

				ImageNameCalculator.CalculateImageNames(set, Patch);

				lock (SetsByCode)
					SetsByCode.Add(set.Code, set);

				lock (Cards)
					foreach (var card in set.Cards)
						Cards.Add(card);

				SetAdded?.Invoke();
			}

			CardsByName = Cards.GroupBy(_ => _.NameNormalized)
				.ToDictionary(
					gr => gr.Key,
					// card_by_name_sorting
					gr => gr.OrderByDescending(_ => _.ReleaseDate).ToList(),
					Str.Comparer);

			for (int i = 0; i < Cards.Count; i++)
			{
				var card = Cards[i];

				card.IndexInFile = i;

				card.Namesakes = CardsByName[card.NameNormalized]
					.Where(c => c != card)
					.OrderByDescending(c => c.ReleaseDate)
					.ToArray();
			}

			patchLegality();

			IsLoadingComplete = true;
			LoadingComplete?.Invoke();

			// release RAM
			_defaultSetsContent = null;
			_priceContent = null;
			Patch = null;
			Cards.Capacity = Cards.Count;

			foreach (var namesakeList in CardsByName.Values)
				namesakeList.Capacity = namesakeList.Count;
		}

		private void postProcessCard(Card card)
		{
			if (String.IsNullOrEmpty(card.Layout))
				card.Layout = "Normal";
			else if (Str.Equals(card.Layout, "Planar"))
			{
				if (card.TypesArr.Contains("Phenomenon"))
					card.Layout = CardLayouts.Phenomenon;
				else if (card.TypesArr.Contains("Plane"))
					card.Layout = CardLayouts.Plane;
			}

			const string timeshifted = "Timeshifted ";
			if (Card.BasicLandNames.Contains(card.NameEn))
				card.Rarity = "Basic land";
			else if (card.Rarity.StartsWith(timeshifted, Str.Comparison))
				card.Rarity = string.Intern(card.Rarity.Substring(timeshifted.Length));
		}

		private void preProcessCard(Card card)
		{
			if (card.LegalityByFormat != null)
				card.LegalityByFormat = card.LegalityByFormat.ToDictionary(
					pair => string.Intern(pair.Key),
					pair => string.Intern(pair.Value),
					Str.Comparer);

			if (Patch.Cards.TryGetValue(card.SetCode, out var patch))
				card.Patch(patch);

			if (Patch.Cards.TryGetValue(card.NameEn, out patch) && (string.IsNullOrEmpty(patch.Set) || Str.Equals(patch.Set, card.SetCode)))
				card.Patch(patch);

			card.NameNormalized = string.Intern(card.NameEn.RemoveDiacritics());
			card.Names = card.Names?.Select(_ => string.Intern(_.RemoveDiacritics())).ToList();

			if (card.SubtypesArr != null)
				card.Subtypes = string.Intern(string.Join(" ", card.SubtypesArr));
			else
				card.Subtypes = string.Empty;

			if (card.TypesArr != null)
				card.Types = string.Intern(string.Join(" ", card.TypesArr));
			else
				card.Types = string.Empty;

			if (card.SupertypesArr != null)
				card.Supertypes = string.Intern(string.Join(" ", card.SupertypesArr));
			else
				card.Supertypes = string.Empty;

			card.PowerNum = getPower(card.Power);
			card.ToughnessNum = getPower(card.Toughness);
			card.LoyaltyNum = getLoyalty(card.Loyalty);

			card.TextEn = card.TextEn?.Invoke1(LocalizationRepository.IncompleteChaosPattern.Replace, "{CHAOS}");
			card.FlavorEn = card.FlavorEn?.Invoke1(LocalizationRepository.IncompleteChaosPattern.Replace, "{CHAOS}");

			card.Color = card.ColorsArr != null && card.ColorsArr.Count > 0
				? string.Intern(string.Join(" ", card.ColorsArr))
				: "Colorless";

			if (!string.IsNullOrEmpty(card.OriginalText) && Str.Equals(card.OriginalText, card.TextEn))
				card.OriginalText = null;

			if (!string.IsNullOrEmpty(card.OriginalType) && Str.Equals(card.OriginalType, card.TypeEn))
				card.OriginalType = null;
		}

		private void patchLegality()
		{
			if (Patch.Legality == null)
				return;

			foreach (var pair in Patch.Legality)
			{
				string format = pair.Key;
				var patch = pair.Value;

				foreach (var card in Cards)
				{
					if (card.IsBannedIn(format) && !patch.Banned.Remove.Contains(card.NameEn) ||
						patch.Banned.Add.Contains(card.NameEn))
					{
						card.SetLegality(format, Legality.Banned);
					}
					else if (card.IsRestrictedIn(format) && !patch.Restricted.Remove.Contains(card.NameEn) ||
						patch.Restricted.Add.Contains(card.NameEn))
					{
						card.SetLegality(format, Legality.Restricted);
					}
					else if (card.IsLegalIn(format) && !card.Printings.Any(patch.Sets.Remove.Contains) ||
						card.Printings.Any(patch.Sets.Add.Contains))
					{
						card.SetLegality(format, Legality.Legal);
					}
					else
					{
						card.SetLegality(format, Legality.Illegal);
					}
				}
			}
		}

		private static float? getPower(string power)
		{
			if (string.IsNullOrEmpty(power))
				return null;

			var parts = power.Split('+');
			float sum = 0;

			foreach (string part in parts)
			{
				float partValue;

				if (part.EndsWith("½"))
				{
					if (part.Length == 1)
						sum += 0.5f;
					else if (float.TryParse(part.Substring(0, part.Length - 1), NumberStyles.Float, Str.Culture, out partValue))
						sum += partValue + 0.5f;
				}
				else
				{
					if (float.TryParse(part, NumberStyles.Float, Str.Culture, out partValue))
						sum += partValue;
				}
			}

			return sum;
		}

		private static int? getLoyalty(string loyalty)
		{
			if (string.IsNullOrEmpty(loyalty))
				return null;

			if (int.TryParse(loyalty, out int result))
				return result;

			return 0;
		}

		public string GetReleaseDateSimilarity(string cardSet, string setCode)
		{
			var cardReleaseDate = parseReleaseDate(SetsByCode.TryGet(cardSet)?.ReleaseDate);
			var setReleaseDate = parseReleaseDate(SetsByCode.TryGet(setCode)?.ReleaseDate);

			var n = (setReleaseDate - cardReleaseDate).TotalDays;

			if (n < 0)
				n = 1000000 + n;

			return ((int) n).ToString("D7", Str.Culture);
		}

		private static DateTime parseReleaseDate(string releaseDate)
		{
			if (!string.IsNullOrEmpty(releaseDate))
			{
				if (DateTime.TryParseExact(releaseDate, "yyyy-MM-dd", Str.Culture, DateTimeStyles.None, out var result))
					return result;
			}

			return DateTime.MinValue;
		}

		public void FillLocalizations(LocalizationRepository localizationRepository)
		{
			//var generatedManaMismatchCards = new List<Card>();

			for (int i = 0; i < Cards.Count; i++)
			{
				var card = Cards[i];
				card.Localization = localizationRepository.GetLocalization(card);

				//if (!string.IsNullOrEmpty(card.Localization?.GeneratedMana) && string.IsNullOrEmpty(card.GeneratedMana))
				//{
				//	generatedManaMismatchCards.Add(card);
				//}
			}

			IsLocalizationLoadingComplete = true;
			LocalizationLoadingComplete?.Invoke();
		}



		public event Action SetAdded;
		public event Action LoadingComplete;
		public event Action LocalizationLoadingComplete;

		public bool IsFileLoadingComplete { get; private set; }
		public bool IsLoadingComplete { get; private set; }
		public bool IsLocalizationLoadingComplete { get; private set; }

		internal string SetsFile { get; set; }
		private string PricesFile { get; set; }

		internal string[] CustomSetCodes
		{
			get => _customSetCodes;
			set
			{
				_customSetCodes = value;
				_customSetCodesSet = _customSetCodes.ToHashSet(Str.Comparer);
			}
		}

		private string PatchFile { get; }

		public List<Card> Cards { get; }
		public IDictionary<string, Set> SetsByCode { get; } = new Dictionary<string, Set>(Str.Comparer);
		public IDictionary<string, Card> CardsById { get; } = new Dictionary<string, Card>(Str.Comparer);

		public IDictionary<string, List<Card>> CardsByName { get; private set; }

		private byte[] _defaultSetsContent;
		private byte[][] _customSetContents;
		private string[] _customSetCodes;
		private HashSet<string> _customSetCodesSet;
		private byte[] _priceContent;

		private Patch Patch { get; set; }
	}
}
