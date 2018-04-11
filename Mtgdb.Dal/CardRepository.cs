using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class CardRepository
	{
		public CardRepository()
		{
			SetsFile = AppDir.Data.AddPath("AllSets-x.json");
			BannedAndRestrictedFile = AppDir.Data.AddPath("patch.json");
			Cards = new List<Card>();
		}

		public void LoadFile()
		{
			_streamContent = File.ReadAllBytes(SetsFile);
			_patch = JsonConvert.DeserializeObject<Patch>(File.ReadAllText(BannedAndRestrictedFile));
			IsFileLoadingComplete = true;
		}

		public void Load()
		{
			var serializer = new JsonSerializer();

			Stream stream = new MemoryStream(_streamContent);
			using (stream)
			using (var stringReader = new StreamReader(stream))
			using (var jsonReader = new JsonTextReader(stringReader))
			{
				jsonReader.Read();

				while (true)
				{
					// пропустить имя сета
					jsonReader.Read();

					if (jsonReader.TokenType == JsonToken.EndObject)
						// сеты кончились, весь json прочитан
						break;

					// пропустить имя сета
					jsonReader.Read();

					var set = serializer.Deserialize<Set>(jsonReader);

					for (int i = 0; i < set.Cards.Count; i++)
					{
						var card = set.Cards[i];
						card.Set = set;

						preProcessCard(card);
					}

					for (int i = set.Cards.Count - 1; i >= 0; i--)
						if (set.Cards[i].Remove)
							set.Cards.RemoveAt(i);

					// после preProcessCard, чтобы было заполено поле NameNormalized
					set.CardsByName = set.Cards.GroupBy(_ => _.NameNormalized)
						.ToDictionary(
							gr => gr.Key,
							gr => gr.ToList(),
							Str.Comparer);

					lock (SetsByCode)
						SetsByCode.Add(set.Code, set);

					lock (Cards)
						foreach (var card in set.Cards)
							Cards.Add(card);

					foreach (var card in set.Cards)
						CardsById[card.Id] = card;

					SetAdded?.Invoke();
				}
			}

			CardsByName = Cards.GroupBy(_ => _.NameNormalized)
				.ToDictionary(
					gr => gr.Key,
					// card_by_name_sorting
					gr => gr.OrderByDescending(_ => _.ReleaseDate).ToList(),
					Str.Comparer);

			for (int i = 0; i < Cards.Count; i++)
				Cards[i].IndexInFile = i;

			patchLegality();

			IsLoadingComplete = true;
			LoadingComplete?.Invoke();

			// освободить память
			_streamContent = null;
			_patch = null;
			Cards.Capacity = Cards.Count;
		}

		private void preProcessCard(Card card)
		{
			card.NameNormalized = string.Intern(card.NameEn.RemoveDiacritics());

			ImageNamePatcher.Patch(card);

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

			if (_patch.Cards.TryGetValue(card.SetCode, out var patch))
				card.PatchCard(patch);

			if (_patch.Cards.TryGetValue(card.NameEn, out patch))
				card.PatchCard(patch);

			if (_patch.Cards.TryGetValue(card.Id, out patch))
				card.PatchCard(patch);

			card.PowerNum = getPower(card.Power);
			card.ToughnessNum = getPower(card.Toughness);
			card.LoyaltyNum = getLoyalty(card.Loyalty);

			if (card.TextEn != null)
				card.TextEn = LocalizationRepository.IncompleteChaosPattern.Replace(card.TextEn, "{CHAOS}");

			if (card.FlavorEn != null)
				card.FlavorEn = LocalizationRepository.IncompleteChaosPattern.Replace(card.FlavorEn, "{CHAOS}");

			if (card.MciNumber == null)
				card.MciNumber = card.Number;
			else if (card.MciNumber?.EndsWith(".html") == true)
				card.MciNumber = _mciNumberRegex.Match(card.MciNumber).Groups["id"].Value;

			card.Color = card.ColorsArr != null && card.ColorsArr.Count > 0
				? string.Join(" ", card.ColorsArr)
				: "Colorless";
		}

		private void patchLegality()
		{
			if (_patch.Legality == null)
				return;

			foreach (var pair in _patch.Legality)
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

		public ImageModel GetSmallImage(Card card, ImageRepository repository)
		{
			return repository.GetSmallImage(card, GetReleaseDateSimilarity);
		}

		public List<ImageModel> GetZoomImages(Card card, ImageRepository repository)
		{
			return repository.GetZooms(card, GetReleaseDateSimilarity);
		}

		public List<ImageModel> GetImagesArt(Card card, ImageRepository repository)
		{
			return repository.GetArts(card, GetReleaseDateSimilarity) ?? new List<ImageModel>();
		}

		public string GetReleaseDateSimilarity(string cardSet, string setCode)
		{
			var cardReleasDate = parseReleaseDate(SetsByCode.TryGet(cardSet)?.ReleaseDate);
			var setReleaseDate = parseReleaseDate(SetsByCode.TryGet(setCode)?.ReleaseDate);

			var n = (setReleaseDate - cardReleasDate).TotalDays;

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

		public List<Card> GetForms(Card card, UiModel ui)
		{
			var forms = new List<Card> { card };

			if (card.Names == null)
				return forms;

			foreach (string name in card.Names)
			{
				string namesakeName = name.RemoveDiacritics();

				if (card.NameNormalized == namesakeName)
					continue;

				var namesakeWithImage = CardsByName.TryGet(namesakeName)
					?.FirstOrDefault(c => c.HasImage(ui));

				if (namesakeWithImage != null)
					forms.Add(namesakeWithImage);
			}

			return forms;
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

				card.Namesakes = CardsByName[card.NameNormalized]
					.Where(c => c != card)
					.OrderByDescending(c => c.ReleaseDate)
					.ToArray();
			}

			IsLocalizationLoadingComplete = true;
			LocalizationLoadingComplete?.Invoke();
		}

		private static readonly Regex _mciNumberRegex = new Regex(@"(?<id>[\w\d]+).html", RegexOptions.Compiled);

		public void SetPrices(PriceRepository priceRepository)
		{
			foreach (var card in Cards)
				card.PricesValues = priceRepository.GetPrice(card);
		}



		public event Action SetAdded;
		public event Action LoadingComplete;
		public event Action LocalizationLoadingComplete;

		public bool IsFileLoadingComplete { get; private set; }
		public bool IsLoadingComplete { get; private set; }
		public bool IsLocalizationLoadingComplete { get; private set; }

		private string SetsFile { get; }
		private string BannedAndRestrictedFile { get; }

		public List<Card> Cards { get; }
		public IDictionary<string, Set> SetsByCode { get; } = new Dictionary<string, Set>(Str.Comparer);
		public IDictionary<string, Card> CardsById { get; } = new Dictionary<string, Card>(Str.Comparer);
		public IDictionary<string, List<Card>> CardsByName { get; private set; }

		private byte[] _streamContent;
		private Patch _patch;
	}
}