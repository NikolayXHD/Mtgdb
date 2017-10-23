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
		private readonly UiModel _uiModel;

		public event Action SetAdded;
		public event Action LoadingComplete;
		public event Action ImageLoadingComplete;
		public event Action LocalizationLoadingComplete;

		public bool IsFileLoadingComplete { get; private set; }
		public bool IsLoadingComplete { get; private set; }
		public bool IsImageLoadingComplete { get; private set; }
		public bool IsLocalizationLoadingComplete { get; private set; }

		private string SetsFile { get; }
		private string BannedAndRestrictedFile { get; }

		public IDictionary<string, Set> SetsByCode { get; } = new Dictionary<string, Set>(Str.Comparer);
		public IDictionary<string, Card> CardsById { get; } = new Dictionary<string, Card>(Str.Comparer);
		public IDictionary<string, List<Card>> CardsByName { get; private set; }

		private byte[] _streamContent;
		private Dictionary<string, CardDelta> _cardLegailityDeltas;

		public CardRepository(UiModel uiModel)
		{
			SetsFile = AppDir.Data.AddPath("AllSets-x.json");
			BannedAndRestrictedFile = AppDir.Data.AddPath("patch.json");

			Cards = new List<Card>();
			
			_uiModel = uiModel;
		}

		public List<Card> Cards { get; }

		public void LoadFile()
		{
			_streamContent = File.ReadAllBytes(SetsFile);
			_cardLegailityDeltas = JsonConvert.DeserializeObject<Dictionary<string, CardDelta>>(File.ReadAllText(BannedAndRestrictedFile));
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
						card.UiModel = _uiModel;
						
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
					gr => gr.OrderByDescending(_=>_.ReleaseDate).ToList(),
					Str.Comparer);

			for (int i = 0; i < Cards.Count; i++)
				Cards[i].IndexInFile = i;

			IsLoadingComplete = true;
			LoadingComplete?.Invoke();

			// освободить память
			_streamContent = null;
			_cardLegailityDeltas = null;
			Cards.Capacity = Cards.Count;
		}

		private void preProcessCard(Card card)
		{
			card.NameNormalized = string.Intern(card.NameEn.RemoveDiacritics());

			if (Str.Equals(card.SetCode, "ZEN"))
			{
				// Plains1a.xlhq.jpg -> Plains5.xlhq.jpg

				string modifiedName = null;

				if (card.ImageName.EndsWith("1a", Str.Comparison))
					modifiedName = card.ImageName.Substring(0, card.ImageName.Length - 2) + '5';
				else if (card.ImageName.EndsWith("2a", Str.Comparison))
					modifiedName = card.ImageName.Substring(0, card.ImageName.Length - 2) + '6';
				else if (card.ImageName.EndsWith("3a", Str.Comparison))
					modifiedName = card.ImageName.Substring(0, card.ImageName.Length - 2) + '7';
				else if (card.ImageName.EndsWith("4a", Str.Comparison))
					modifiedName = card.ImageName.Substring(0, card.ImageName.Length - 2) + '8';

				if (modifiedName != null)
					card.ImageName = string.Intern(modifiedName);
			}
			else if (Str.Equals(card.SetCode, "AKH"))
			{
				if (Str.Equals(card.Rarity, "Basic Land"))
				{
					var parts = card.ImageName.SplitTalingNumber();
					card.ImageName = parts.Item1 + (1 + (parts.Item2 - 1 + 3) % 4);
				}
			}
			else if (Str.Equals(card.SetCode, "DD3_DVD"))
			{
				if (Str.Equals(card.ImageName, "swamp3"))
					card.ImageName = "swamp4";
				else if (Str.Equals(card.ImageName, "swamp4"))
					card.ImageName = "swamp3";
			}
			else if (Str.Equals(card.ImageName, "Sultai Ascendacy"))
			{
					card.ImageName = "Sultai Ascendancy";
			}

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

			CardDelta delta;
			if (_cardLegailityDeltas.TryGetValue(card.SetCode, out delta))
				card.ApplyDelta(delta);

			if (_cardLegailityDeltas.TryGetValue(card.NameEn, out delta))
				card.ApplyDelta(delta);

			if (_cardLegailityDeltas.TryGetValue(card.Id, out delta))
				card.ApplyDelta(delta);

			if (card.GeneratedMana == null)
				card.GeneratedMana = string.Intern(GeneratedManaParser.ParseGeneratedMana(card.TextEn));

			card.PowerNum = getPower(card.Power);
			card.ToughnessNum = getPower(card.Toughness);
			card.LoyaltyNum = getLoyalty(card.Loyalty);

			if (card.TextEn != null)
				card.TextEn = LocalizationRepository.IncompleteChaosPattern.Replace(card.TextEn, "{CHAOS}");

			if (card.FlavorEn != null)
				card.FlavorEn = LocalizationRepository.IncompleteChaosPattern.Replace(card.FlavorEn, "{CHAOS}");

			if (card.MciNumber != null && card.MciNumber.EndsWith(".html"))
				card.MciNumber = _mciNumberRegex.Match(card.MciNumber).Groups["id"].Value;
		}

		private static float? getPower(string power)
		{
			if (string.IsNullOrEmpty(power))
				return null;

			float result;
			if (float.TryParse(power, out result))
				return result;

			var parts = power.Split('+');
			float sum = 0;
			foreach (string part in parts)
			{
				float.TryParse(part, out result);
				sum += result;
			}

			return sum;
		}

		private static int? getLoyalty(string loyalty)
		{
			if (string.IsNullOrEmpty(loyalty))
				return null;

			int result;
			if (int.TryParse(loyalty, out result))
				return result;

			return 0;
		}

		public void SelectCardImages(ImageRepository repository)
		{
			foreach (var card in Cards)
				card.ImageModel = GetSmallImage(card, repository);

			//var withoutImages = Cards.Where(_ => _.ImageModel == null).ToArray();

			IsImageLoadingComplete = true;
			ImageLoadingComplete?.Invoke();
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
			return repository.GetArts(card, GetReleaseDateSimilarity)
				?? new List<ImageModel>();
		}

		public string GetReleaseDateSimilarity(string cardSet, string setCode)
		{
			var cardReleasDate = parseReleaseDate(SetsByCode.TryGet(cardSet)?.ReleaseDate);
			var setReleaseDate = parseReleaseDate(SetsByCode.TryGet(setCode)?.ReleaseDate);

			var n = (setReleaseDate - cardReleasDate).TotalDays;

			if (n < 0)
				n = 1000000 + n;

			return ((int) n).ToString("D7", CultureInfo.InvariantCulture);
		}

		private static DateTime parseReleaseDate(string releaseDate)
		{
			if (!string.IsNullOrEmpty(releaseDate))
			{
				DateTime result;

				if (DateTime.TryParseExact(releaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
					return result;
			}

			return DateTime.MinValue;
		}

		public List<Card> GetForms(Card card)
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
					?.FirstOrDefault(_ => _.ImageModel != null);

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
	}
}