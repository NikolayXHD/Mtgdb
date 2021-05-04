﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mtgdb.Data
{
	public class CardRepository
	{
		public Func<string, bool> FilterSetCode { get; set; } =
			// str => Str.Equals(str, "ISD");
			_ => true;

		public CardRepository(CardFormatter formatter, Func<IDataDownloader> downloaderFactory)
		{
			_formatter = formatter;
			_downloaderFactory = downloaderFactory;
			SetsFile = AppDir.Data.Join("AllPrintings.json");
			CustomSetCodes = new string[0];
			PatchFile = AppDir.Data.Join("patch.v2.json");
			Cards = new List<Card>();
		}


		public async Task DownloadFile(CancellationToken token)
		{
			if (!SetsFile.IsFile())
				await Downloader.DownloadMtgjson(token);

			IsDownloadComplete.Signal();
		}

		public void LoadFile()
		{
			_defaultSetsContent = SetsFile.ReadAllBytes();

			_customSetContents = CustomSetCodes
				.Select(code => AppDir.Data.Join("custom_sets", code + ".json").ReadAllBytes())
				.ToArray();

			Patch = JsonConvert.DeserializeObject<Patch>(PatchFile.ReadAllText());
			Patch.IgnoreCase();

			IsFileLoadingComplete.Signal();
		}

		private IEnumerable<Set> deserializeAllSets()
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
				jsonReader.Read(); // {

				while (true)
				{
					jsonReader.Read(); //   "data":
					var rootField = (string)jsonReader.Value;
					if (Str.Equals(rootField, "data"))
						break;

					//   "meta":
					jsonReader.Read(); //   {
					serializer.Deserialize<JObject>(jsonReader);
				}

				jsonReader.Read(); //   {

				while (true)
				{
					jsonReader.Read(); // "10E":

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



		public void Load()
		{
			if (!IsFileLoadingComplete.Signaled)
				throw new InvalidOperationException();

			foreach (var set in deserializeAllSets())
			{
				preProcessSet(set);

				for (int i = set.ActualCards.Count - 1; i >= 0; i--)
				{
					var card = set.ActualCards[i];
					card.Set = set;

					if (!string.IsNullOrEmpty(card.FaceName))
						card.NameEn = card.FaceName;

					card.Id = string.Intern(CardId.Generate(card));

					card.Formatter = _formatter;

					preProcessCard(card);
					preProcessCardOrToken(card);

					if (set.ActualCards[i].Remove)
						set.ActualCards.RemoveAt(i);
				}

				var tokenLegalityByFormat = set.ActualCards.Aggregate(
					new HashSet<string>(Legality.Formats, Str.Comparer),
					(formats, card) =>
					{
						formats.IntersectWith(card.LegalityByFormat.Keys);
						return formats;
					}).ToDictionary(_ => _, _ => Legality.Legal, Str.Comparer);

				for (int i = 0; i < set.Tokens.Count; i++)
				{
					var token = set.Tokens[i];
					token.IsToken = true;
					token.Set = set;

					if (!string.IsNullOrEmpty(token.FaceName))
						token.NameEn = token.FaceName;

					token.Id = string.Intern(CardId.Generate(token));
					token.LegalityByFormat = tokenLegalityByFormat;
					token.Formatter = _formatter;
					preProcessToken(token);
					preProcessCardOrToken(token);
				}

				var cards = new List<Card>(set.ActualCards.Count + set.Tokens.Count);
				cards.AddRange(set.ActualCards);
				cards.AddRange(set.Tokens);
				set.Cards = cards;

				// after preProcessCard, to have NameNormalized field set non empty
				set.ActualCardsByName = set.ActualCards.GroupBy(_ => _.NameNormalized)
					.ToDictionary(
						gr => gr.Key,
						gr => gr.ToList(),
						Str.Comparer);

				set.TokensByName = set.Tokens.GroupBy(_ => _.NameNormalized)
					.ToDictionary(
						gr => gr.Key,
						gr => gr.ToList(),
						Str.Comparer);

				set.ActualCardsById = set.ActualCards
					.ToDictionary(_ => _.MtgjsonId);

				set.TokensById = set.Tokens
					.ToDictionary(_ => _.MtgjsonId);

				foreach (var card in set.Cards)
					CardsById[card.Id] = card;

				ImageNameCalculator.CalculateCardImageNames(set, Patch);

				lock (SetsByCode)
					SetsByCode.Add(set.Code, set);

				lock (Cards)
					foreach (var card in set.Cards)
						Cards.Add(card);

				SetAdded?.Invoke();
			}

			CardsByName = toNamesakesMap(Cards.Where(c=>!c.IsToken));
			TokensByName = toNamesakesMap(Cards.Where(c => c.IsToken));

			CardIdsByName = CardsByName.ToDictionary(_ => _.Key, _ => _.Value.ToHashSet(c => c.Id), Str.Comparer);
			TokenIdsByName = TokensByName.ToDictionary(_ => _.Key, _ => _.Value.ToHashSet(c => c.Id), Str.Comparer);

			CardPrintingsByName = CardsByName.ToDictionary(_ => _.Key, toPrintings, Str.Comparer);
			TokenPrintingsByName = TokensByName.ToDictionary(_ => _.Key, toPrintings, Str.Comparer);

			CardsAndTokensByTcgPlayerProductId = toTcgPlayerProductIdMap(Cards);

			for (int i = 0; i < Cards.Count; i++)
			{
				var card = Cards[i];
				card.IndexInFile = i;
				card.Namesakes = MapByName(card.IsToken)[card.NameNormalized];
				card.NamesakeIds = MapIdByName(card.IsToken)[card.NameNormalized];
				card.Printings = MapPrintingsByName(card.IsToken)[card.NameNormalized];
			}

			patchLegality();

			IsLoadingComplete.Signal();

			// release RAM
			_defaultSetsContent = null;
			Patch = null;
			Cards.Capacity = Cards.Count;

			foreach (var namesakeList in CardsByName.Values)
				namesakeList.Capacity = namesakeList.Count;

			foreach (var namesakeList in TokensByName.Values)
				namesakeList.Capacity = namesakeList.Count;

			IReadOnlyList<string> toPrintings(KeyValuePair<string, List<Card>> _) =>
				_.Value.Select(c => c.Set).Distinct().OrderBy(s => s.ReleaseDate).Select(s => s.Code).ToList();

			Dictionary<string, List<Card>> toNamesakesMap(IEnumerable<Card> cards) =>
				cards.GroupBy(_ => _.NameNormalized)
					.ToDictionary(
						gr => gr.Key,
						// card_by_name_sorting
						gr => gr.OrderByDescending(_ => _.ReleaseDate).ToList(),
						Str.Comparer);

			Dictionary<int, List<Card>> toTcgPlayerProductIdMap(IEnumerable<Card> cards) =>
				cards.GroupBy(_ => _.Identifiers.TcgPlayerProductId)
					.ToDictionary(
						gr => gr.Key,
						gr => gr//.OrderByDescending(_ => _.ReleaseDate)
						.ToList());
		}

		private void preProcessSet(Set set)
		{
			if (Str.Equals(set.Code, "fjmp"))
			{
				if ((set.Tokens?.Count ?? 0) == 0 && (set.ActualCards?.Count ?? 0) > 0)
				{
					set.Tokens = set.ActualCards;
					set.ActualCards = Empty<Card>.Array;

					foreach (Card card in set.Tokens)
						card.CardType = CardCardTypes.Card;
				}
			}
		}

		private static void preProcessCardOrToken(Card card)
		{
			card.NameNormalized = string.Intern(card.NameEn.RemoveDiacritics());

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

			card.TextEn = card.TextEn?.Invoke1(IncompleteChaosPattern.Replace, "{CHAOS}");
			card.FlavorEn = card.FlavorEn?.Invoke1(IncompleteChaosPattern.Replace, "{CHAOS}");

			card.Color = card.ColorsArr != null && card.ColorsArr.Count > 0
				? string.Intern(string.Join(" ", card.ColorsArr))
				: "Colorless";
		}

		private void preProcessCard(Card card)
		{
			applyPatch(card);

			if (!string.IsNullOrEmpty(card.OriginalText) && Str.Equals(card.OriginalText, card.TextEn))
				card.OriginalText = null;

			if (!string.IsNullOrEmpty(card.OriginalType) && Str.Equals(card.OriginalType, card.TypeEn))
				card.OriginalType = null;

			if (string.IsNullOrEmpty(card.Layout))
				card.Layout = "Normal";
			else if (Str.Equals(card.Layout, "Planar"))
			{
				if (card.TypesArr.Contains("Phenomenon"))
					card.Layout = CardLayouts.Phenomenon;
				else if (card.TypesArr.Contains("Plane"))
					card.Layout = CardLayouts.Plane;
			}

			const string timeshifted = "Timeshifted ";
			if (CardNames.BasicLands.Contains(card.NameEn))
				card.Rarity = "Basic land";
			else if (card.Rarity.StartsWith(timeshifted, Str.Comparison))
				card.Rarity = string.Intern(card.Rarity.Substring(timeshifted.Length));

			card.CardType = CardCardTypes.Normal;
		}

		private void preProcessToken(Card card)
		{
			applyPatch(card);

			if (Str.Equals(card.Layout, "double_faced_token"))
				card.Layout = CardLayouts.Transform;
			else if (
				Str.Equals(card.Layout, "art_series") ||
				Str.Equals(card.Layout, CardTypes.Token) ||
				Str.Equals(card.Layout, CardTypes.Emblem))
			{
				card.Layout = CardLayouts.Normal;
			}

			string type = null;
			if (card.TypesArr.Any(_ => CardCardTypes.ByType.TryGetValue(_, out type)))
				card.CardType = type;
			else if (CardCardTypes.ByName.TryGetValue(card.NameEn, out type))
				card.CardType = type;
		}

		private void applyPatch(Card card)
		{
			if (Patch.Cards.TryGetValue(card.SetCode, out var patch))
				card.Patch(patch);

			if (Patch.Cards.TryGetValue(card.NameEn, out patch))
			{
				if (
					string.IsNullOrEmpty(patch.Set) && patch.Sets == null ||
					Str.Equals(patch.Set, card.SetCode) ||
					patch.Sets != null && patch.Sets.Contains(card.SetCode, Str.Comparer)
				)
					card.Patch(patch);
			}
		}

		private void patchLegality()
		{
			if (Patch.Legality != null)
				foreach ((string format, var patch) in Patch.Legality)
				foreach (var card in Cards)
				{
					if (card.IsBannedIn(format) && !patch.Banned.Remove.Contains(card.NameEn) || patch.Banned.Add.Contains(card.NameEn))
						card.SetLegality(format, Legality.Banned);
					else if (card.IsRestrictedIn(format) && !patch.Restricted.Remove.Contains(card.NameEn) ||
						patch.Restricted.Add.Contains(card.NameEn))
						card.SetLegality(format, Legality.Restricted);
					else if (card.IsLegalIn(format) && !card.Printings.Any(patch.Sets.Remove.Contains) || card.Printings.Any(patch.Sets.Add.Contains))
						card.SetLegality(format, Legality.Legal);
					else
						card.SetLegality(format, Legality.Illegal);
				}

			foreach (var card in Cards)
				card.LegalityByFormat = card.LegalityByFormat?
					.ToDictionary(pair => string.Intern(pair.Key), pair => string.Intern(pair.Value), Str.Comparer);
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

		public void FillLocalizations()
		{
			//var generatedManaMismatchCards = new List<Card>();

			for (int i = 0; i < Cards.Count; i++)
			{
				var card = Cards[i];
				card.Localization = getLocalization(card);

				//if (!string.IsNullOrEmpty(card.Localization?.GeneratedMana) && string.IsNullOrEmpty(card.GeneratedMana))
				//{
				//	generatedManaMismatchCards.Add(card);
				//}
			}

			IsLocalizationLoadingComplete.Signal();
		}



		private static Dictionary<string, ForeignData> getLocalization(Card card)
		{
			if (card.ForeignData == null)
				return null;

			var result = new Dictionary<string, ForeignData>(Str.Comparer);

			foreach (var translation in card.ForeignData)
			{
				if (string.IsNullOrEmpty(translation.Name))
					translation.Name = translation.FaceName;

				if (string.Equals(translation.Name, card.NameEn, StringComparison.Ordinal))
					translation.Name = null;

				if (string.Equals(translation.Type, card.TypeEn, StringComparison.Ordinal))
					translation.Type = null;

				if (string.Equals(translation.Flavor, card.FlavorEn, StringComparison.Ordinal))
					translation.Flavor = null;

				if (string.Equals(translation.Text, card.TextEn, StringComparison.Ordinal))
					translation.Text = null;

				if (translation.Name != null || translation.Text != null || translation.Type != null || translation.Flavor != null)
				{
					var lang = CardLocalization.GetLang(translation.Language);
					if (lang != null && !result.ContainsKey(lang))
						result.Add(lang, translation);
				}
			}

			return result;
		}

		public event Action SetAdded;
		public AsyncSignal IsDownloadComplete { get; } = new AsyncSignal();

		public AsyncSignal IsFileLoadingComplete { get; } = new AsyncSignal();
		public AsyncSignal IsLoadingComplete { get; } = new AsyncSignal();
		public AsyncSignal IsLocalizationLoadingComplete { get; } = new AsyncSignal();

		internal FsPath SetsFile { get; set; }

		internal string[] CustomSetCodes
		{
			get => _customSetCodes;
			set
			{
				_customSetCodes = value;
				_customSetCodesSet = _customSetCodes.ToHashSet(Str.Comparer);
			}
		}

		public IDictionary<string, List<Card>> MapByName(bool tokens) =>
			tokens ? TokensByName : CardsByName;

		public IDictionary<string, HashSet<string>> MapIdByName(bool tokens) =>
			tokens ? TokenIdsByName : CardIdsByName;

		public IDictionary<string, IReadOnlyList<string>> MapPrintingsByName(bool tokens) =>
			tokens ? TokenPrintingsByName : CardPrintingsByName;

		private FsPath PatchFile { get; }

		public List<Card> Cards { get; }
		public IDictionary<string, Set> SetsByCode { get; } = new Dictionary<string, Set>(Str.Comparer);
		public IDictionary<string, Card> CardsById { get; } = new Dictionary<string, Card>(Str.Comparer);

		public IDictionary<string, List<Card>> CardsByName { get; private set; }
		public IDictionary<int, List<Card>> CardsAndTokensByTcgPlayerProductId { get; private set; }
		public IDictionary<string, List<Card>> TokensByName { get; private set; }
		public IDictionary<string, HashSet<string>> CardIdsByName { get; private set; }
		public IDictionary<string, HashSet<string>> TokenIdsByName { get; private set; }
		public IDictionary<string, IReadOnlyList<string>> CardPrintingsByName { get; private set; }
		public IDictionary<string, IReadOnlyList<string>> TokenPrintingsByName { get; private set; }

		private byte[] _defaultSetsContent;
		private byte[][] _customSetContents;
		private string[] _customSetCodes;
		private HashSet<string> _customSetCodesSet;

		private readonly CardFormatter _formatter;

		private readonly Func<IDataDownloader> _downloaderFactory;
		private IDataDownloader _downloader;
		private IDataDownloader Downloader => _downloader ??= _downloaderFactory();

		private Patch Patch { get; set; }

		public static readonly Regex IncompleteChaosPattern = new Regex("(?<!{)CHAOS(?!})");
	}
}
