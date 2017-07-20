using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		private string MagicDuelsFile { get; }
		private string BannedAndRestrictedFile { get; }

		private List<Set> Sets { get; }

		public IDictionary<string, Set> SetsByCode { get; } = new Dictionary<string, Set>(Str.Comparer);
		public IDictionary<string, Card> CardsById { get; } = new Dictionary<string, Card>(Str.Comparer);
		public IDictionary<string, List<Card>> CardsByName { get; private set; }

		private byte[] _streamContent;
		private Dictionary<string, CardDelta> _cardLegailityDeltas;

		public CardRepository(UiModel uiModel)
		{
			SetsFile = Path.Combine(AppDir.Data, "AllSets-x.json");
			MagicDuelsFile = Path.Combine(AppDir.Data, "MagicDuelsCards.txt");
			BannedAndRestrictedFile = Path.Combine(AppDir.Data, "banned_and_restricted.json");

			Cards = new List<Card>();
			Sets = new List<Set>();
			
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
			var countInDuels = readMagicDuelsCards();
			
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
						card.IsMagicDuels = countInDuels.Contains(card.NameEn) || card.TypeEn.StartsWith("Basic Land", Str.Comparison);

						preProcessCard(card);
					}

					// после preProcessCard, чтобы было заполено поле NameNormalized
					set.CardsByName = set.Cards.GroupBy(_ => _.NameNormalized)
						.ToDictionary(
							gr => gr.Key,
							gr => gr.ToList(),
							Str.Comparer);

					lock (SetsByCode)
					{
						SetsByCode.Add(set.Code, set);
						Sets.Add(set);
					}

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

			foreach (var group in CardsByName.Values)
				group[0].IsNonDuplicate = true;


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

			if (card.GeneratedMana == null)
				card.GeneratedMana = string.Intern(GeneratedManaParser.ParseGeneratedMana(card.TextEn));

			card.PowerNum = getPower(card.Power);
			card.ToughnessNum = getPower(card.Toughness);
			card.LoyaltyNum = getLoyalty(card.Loyalty);
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
				card.ImageModel = selectCardImage(card, repository);

			//var withoutImages = Cards.Where(_ => _.ImageModel == null).ToArray();

			IsImageLoadingComplete = true;
			ImageLoadingComplete?.Invoke();
		}

		private ImageModel selectCardImage(Card card, ImageRepository repository)
		{
			return repository.GetImageSmall(card, GetReleaseDate);
		}

		private HashSet<string> readMagicDuelsCards()
		{
			if (!File.Exists(MagicDuelsFile))
				return new HashSet<string>();

			var lines = File.ReadAllLines(MagicDuelsFile);
			var result = new HashSet<string>(lines.Select(_ => _.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries)[0]));
			return result;
		}

		public List<ImageModel> GetImagesZoom(Card card, ImageRepository repository)
		{
			return repository.GetImagesZoom(card, GetReleaseDate);
		}

		public List<ImageModel> GetImagesArt(Card card, ImageRepository repository)
		{
			return repository.GetImagesArt(card, GetReleaseDate) 
				?? new List<ImageModel>();
		}

		public string GetReleaseDate(string setCode)
		{
			Set set;
			if (setCode == null || !SetsByCode.TryGetValue(setCode, out set))
				return @"0000-00-00";

			return set.ReleaseDate;
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
				card.Localization = localizationRepository.GetLocalization(card.SetCode, card.NameEn);

				//if (!string.IsNullOrEmpty(card.Localization?.GeneratedMana) && string.IsNullOrEmpty(card.GeneratedMana))
				//{
				//	generatedManaMismatchCards.Add(card);
				//}
			}

			IsLocalizationLoadingComplete = true;
			LocalizationLoadingComplete?.Invoke();
		}
	}
}