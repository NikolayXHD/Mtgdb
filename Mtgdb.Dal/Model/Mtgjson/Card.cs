using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using Mtgdb.Dal.Index;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	/// <summary>
	/// https://mtgjson.com/v4/docs.html
	/// </summary>
	[JsonObject]
	public class Card
	{
		/// <summary>
		/// Name of artist.
		/// </summary>
		[JsonProperty("artist")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Artist { get; set; }

		/// <summary>
		/// List of all colors in card’s mana cost and any color indicator. Some cards are special (such as Devoid cards or other cards with certain rules text).
		/// </summary>
		[JsonProperty("colors")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public IList<string> ColorsArr { get; set; }

		/// <summary>
		/// The converted mana cost of the card.
		/// </summary>
		[JsonProperty("convertedManaCost")]
		public float Cmc { get; set; }

		/// <summary>
		/// Italicized text found below the rules text that has no game function.
		/// </summary>
		[JsonProperty("flavorText")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string FlavorEn { get; set; }

		/// <summary>
		/// Foreign language names for the card, if this card in this set was printed in another language. An array of objects, each object having 'language', 'name' and 'multiverseid' keys. Not available for all sets.
		/// </summary>
		[JsonProperty("foreignData")]
		public List<ForeignName> ForeignNames { get; set; }

		/// <summary>
		/// Type of card. Can be normal, split, flip, transform, meld, leveler, saga, planar, scheme, vanguard, token, double_faced_token, emblem, augment, or host. (If normal, it is usually omitted.)
		/// </summary>
		[JsonConverter(typeof(InternedStringConverter))]
		[JsonProperty("layout")]
		public string Layout { get; set; }

		/// <summary>
		/// Keys are Magic play formats. Can be
		/// 1v1, brawl, commander, duel, frontier, future, legacy, modern, pauper, penny, standard, or vintage.
		/// Values can be Legal, Restricted, Banned, or Future.
		/// “Future” is used for a revision of the format in which the card will be legal soon.
		/// If the format is not listed, it is assumed the card is not legal in that format.
		/// </summary>
		[JsonProperty("legalities")]
		public Dictionary<string, string> LegalityByFormat { get; set; }

		/// <summary>
		/// Planeswalker loyalty value.
		/// </summary>
		[JsonProperty("loyalty")]
		[JsonConverter(typeof(IntToInternedStringConverter))]
		public string Loyalty { get; set; }

		/// <summary>
		/// Mana cost of the card.
		/// </summary>
		[JsonProperty("manaCost")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string ManaCost { get; set; }

		/// <summary>
		/// An integer most cards have which Wizards uses as a card identifier.
		/// </summary>
		[JsonProperty("multiverseId")]
		public int? MultiverseId { get; set; }

		/// <summary>
		/// Name of the card. (If the card is in an Un-set and has multiple printings, a space and letter enclosed in parentheses, starting with (b), follows the name.)
		/// </summary>
		[JsonProperty("name")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string NameEn { get; set; }

		/// <summary>
		/// Names of each face on the card. Meld cards are listed in the order of CardA, Meld, CardB.
		/// </summary>
		[JsonProperty("names")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		internal IList<string> Names { get; set; }

		/// <summary>
		/// Number of the card.
		/// </summary>
		[JsonProperty("number")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Number { get; set; }

		/// <summary>
		/// Text on the card as originally printed.
		/// </summary>
		[JsonProperty("originalText")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string OriginalText { get; set; }

		/// <summary>
		/// Type as originally printed. Includes any supertypes and subtypes.
		/// </summary>
		[JsonProperty("originalType")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string OriginalType { get; set; }

		/// <summary>
		/// List of sets the card was printed in, in uppercase.
		/// </summary>
		[JsonProperty("printings")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public IList<string> Printings { get; set; }

		/// <summary>
		/// Power of the creature.
		/// </summary>
		[JsonProperty("power")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Power { get; set; }

		/// <summary>
		/// Rarity. Can be common, uncommon, rare, or mythic
		/// </summary>
		[JsonProperty("rarity")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Rarity { get; set; }

		/// <summary>
		/// The rulings for the card. An array of objects, each object having 'date' and 'text' keys.
		/// </summary>
		[JsonProperty("rulings")]
		public List<Ruling> RulingsList { get; set; }

		/// <summary>
		/// List of card subtypes found after em-dash.
		/// </summary>
		[JsonProperty("subtypes")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public IList<string> SubtypesArr { get; set; }

		/// <summary>
		/// List of card supertypes found before em-dash.
		/// </summary>
		[JsonProperty("supertypes")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public IList<string> SupertypesArr { get; set; }

		/// <summary>
		/// Rules text of the card.
		/// </summary>
		[JsonProperty("text")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string TextEn { get; set; }

		/// <summary>
		/// Toughness of the creature.
		/// </summary>
		[JsonProperty("toughness")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Toughness { get; set; }

		/// <summary>
		/// Type of the card. Includes any supertypes and subtypes.
		/// </summary>
		[JsonProperty("type")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string TypeEn { get; set; }

		/// <summary>
		/// List of types of the card.
		/// </summary>
		[JsonProperty("types")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public IList<string> TypesArr { get; set; }

		/// <summary>
		/// A universal unique id generated for the card.
		/// </summary>
		[JsonProperty("uuid")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string MtgjsonId { get; set; }

		/// <summary>
		/// Id from mtgjson v 4.1.3 and earlier, non-unique per card face
		/// </summary>
		[JsonProperty("scryfallId")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string ScryfallId { get; set; }

		[JsonIgnore]
		public string Id { get; set; }

		/*

		/// <summary>
		/// Color of the border. Can be black, borderless, gold, silver, or white.
		/// </summary>
		public string BorderColor { get; set; }

		/// <summary>
		/// List of all colors in card’s mana cost, rules text and any color indicator.
		/// </summary>
		public List<string> ColorIdentity { get; set; }

		/// <summary>
		/// List of all colors in card’s color indicator. Usually found only on cards without mana costs and other special cards.
		/// </summary>
		public List<string> ColorIndicator { get; set; }

		/// <summary>
		/// The converted mana cost of the face (half, or part) of the card.
		/// </summary>
		[JsonProperty("faceConvertedManaCost")]
		public float FaceCmc { get; set; }

		/// <summary>Style of the card frame. Can be 1993, 1997, 2003, 2015, or future.</summary>
		[JsonProperty("frameVersion")]
		public string FrameVersion { get; set; }

		/// <summary>Can the card be found in foil? Can be true or false. (If false, it is usually omitted.)</summary>
		[JsonProperty("hasFoil")]
		public bool hasFoil { get; set; }

		/// <summary>Can the card be found in foil? Can be true or false. (If false, it is usually omitted.)</summary>
		[JsonProperty("hasFoil")]
		public bool HasFoil { get; set; }

		/// <summary>Can the card be found in non-foil? Can be true or false. (If false, it is usually omitted.)</summary>
		[JsonProperty("hasNoFoil")]
		public bool HasNoFoil { get; set; }

		/// <summary>Can the card only be found in foil? true or false. (If false, it is usually omitted.)</summary>
		[JsonProperty("isFoilOnly")]
		public bool IsFoilOnly { get; set; }

		/// <summary>Is the card only available online? Can be true or false. (If false, it is usually omitted.)</summary>
		[JsonProperty("isOnlineOnly")]
		public bool IsOnlineOnly { get; set; }

		/// <summary>Is the card oversized? Can be true or false. (If false, it is usually omitted.)</summary>
		[JsonProperty("isOversized")]
		public bool IsOversized { get; set; }

		/// <summary>Is the card on the Reserved List? Can be true or false. (If false, isReserved is usually omitted.)</summary>
		[JsonProperty("isReserved")]
		public bool IsReserved { get; set; }

		/// <summary>Card is “timeshifted”, a feature from Time Spiral block. Can be true or false. (If false, it is usually omitted.)</summary>
		[JsonProperty("isTimeshifted")]
		public bool IsTimeshifted { get; set; }

		/// <summary>
		/// UUIDs of cards with alternate printings with the same set code (excluding Un-sets).
		/// </summary>
		[JsonProperty("variations")]
		public List<string> Variations { get; set; }

		/// <summary>
		/// Name of the watermark on the card. Can be one of many different values, including a guild name, clan name, or wotc for the shooting star. (If there isn’t one, it can be an empty string, but it is usually omitted.)
		/// </summary>
		[JsonProperty("watermark")]
		public string Watermark { get; set; }
		*/



		[JsonIgnore]
		public Set Set { get; set; }

		/// <summary>
		/// The name of the set
		/// </summary>
		[JsonIgnore]
		public string SetName => Set?.Name;

		/// <summary>
		/// The set's abbreviated code
		/// </summary>
		[JsonIgnore]
		public string SetCode => Set?.Code;

		[JsonIgnore]
		public string ReleaseDate => Set?.ReleaseDate;

		[JsonIgnore]
		public string ReleaseMonth => Set?.ReleaseDate?.Substring(0, 7);

		[JsonIgnore]
		public string ReleaseYear => Set?.ReleaseDate?.Substring(0, 4);


		[JsonIgnore]
		public string Color { get; set; }

		/// <summary>
		/// Maximum hand size modifier. Only exists for Vanguard cards.
		/// </summary>
		[JsonIgnore]
		public int? Hand { get; set; }

		/// <summary>
		/// Starting life total modifier. Only exists for Vanguard cards.
		/// </summary>
		[JsonIgnore]
		public int? Life { get; set; }

		[JsonIgnore]
		public string ImageNameBase => ImageName.SplitTailingNumber().Item1;

		[JsonIgnore]
		public string NameNormalized { get; internal set; }

		[JsonIgnore]
		public int IndexInFile { get; set; }

		[JsonIgnore]
		public bool IsSearchResult { get; set; }



		[JsonIgnore]
		public string Types { get; internal set; }

		[JsonIgnore]
		public string Subtypes { get; internal set; }

		[JsonIgnore]
		public string Supertypes { get; internal set; }

		[JsonIgnore]
		public float? PowerNum { get; internal set; }

		[JsonIgnore]
		public float? ToughnessNum { get; internal set; }

		[JsonIgnore]
		public int? LoyaltyNum { get; internal set; }

		[JsonIgnore]
		public string GeneratedMana => _generatedMana ?? (_generatedMana = string.Concat(GeneratedManaArrExpanded));

		[JsonIgnore]
		public IList<string> GeneratedManaArrExpanded
		{
			get
			{
				parseGeneratedManaOnce();
				return _generatedManaArrExpanded;
			}
			private set
			{
				_generatedManaParsed = true;
				_generatedManaArrExpanded = value;
			}
		}

		[JsonIgnore]
		public IList<string> GeneratedManaArr
		{
			get
			{
				parseGeneratedManaOnce();
				return _generatedManaArr;
			}
			private set
			{
				_generatedManaParsed = true;
				_generatedManaArr = value;
			}
		}

		private void parseGeneratedManaOnce()
		{
			if (_generatedManaParsed)
				return;

			(_generatedManaArr, _generatedManaArrExpanded) = GeneratedManaParser.ParseGeneratedMana(this);
			_generatedManaParsed = true;
		}

		public Document Document => _document ?? (_document = this.ToDocument());

		[JsonIgnore]
		public string Rulings
		{
			get
			{
				if (_rulings != null)
					return _rulings;

				var resultBuilder = new List<string>();

				if (!string.IsNullOrEmpty(LegalIn))
				{
					const string chinaOnlyStandardLegalSet = "GS1";

					if (Str.Equals(SetCode, chinaOnlyStandardLegalSet) && (Printings == null || Printings.Count == 0 || Printings.Count == 1 && Str.Equals(Printings[0], chinaOnlyStandardLegalSet)))
						resultBuilder.Add($"legal: {LegalIn.Replace("Standard", "Standard (China only)")}");
					else
						resultBuilder.Add($"legal: {LegalIn}");
				}

				if (!string.IsNullOrEmpty(RestrictedIn))
					resultBuilder.Add($"restricted: {RestrictedIn}");

				if (!string.IsNullOrEmpty(BannedIn))
					resultBuilder.Add($"banned: {BannedIn}");

				if (RulingsList != null)
					foreach (var ruling in RulingsList)
						resultBuilder.Add($"{ruling.Date}: {ruling.Text}");

				_rulings = string.Intern(string.Join("\n", resultBuilder));
				return _rulings;
			}
		}

		[JsonIgnore]
		public string LegalIn => _legalIn ?? (_legalIn = string.Intern(string.Join(@", ", LegalFormats)));

		[JsonIgnore]
		public string RestrictedIn => _restrictedIn ?? (_restrictedIn = string.Intern(string.Join(@", ", RestrictedFormats)));

		[JsonIgnore]
		public string BannedIn => _bannedIn ?? (_bannedIn = string.Intern(string.Join(@", ", BannedFormats)));

		[JsonIgnore]
		public string FutureIn => _futureIn ?? (_futureIn = string.Intern(string.Join(@", ", FutureFormats)));

		[JsonIgnore]
		public string[] LegalFormats => _legalFormats ?? (_legalFormats = getFormats(Legality.Legal));

		[JsonIgnore]
		public string[] RestrictedFormats => _restrictedFormats ?? (_restrictedFormats = getFormats(Legality.Restricted));

		[JsonIgnore]
		public string[] BannedFormats => _bannedFormats ?? (_bannedFormats = getFormats(Legality.Banned));

		[JsonIgnore]
		public string[] FutureFormats => _futureFormats ?? (_futureFormats = getFormats(Legality.Future));

		private string[] getFormats(string legality)
		{
			return LegalityByFormat
				.Where(_ => Str.Equals(_.Value, legality))
				.Select(_ => _.Key)
				.OrderBy(_ => Legality.Formats.IndexOf(_, Str.Comparer))
				.ToArray();
		}

		internal void SetLegality(string format, string legality)
		{
			if (Str.Equals(legality, Legality.Illegal))
				LegalityByFormat.Remove(format);
			else
				LegalityByFormat[format] = legality;
		}

		[JsonIgnore]
		public CardLocalization Localization { get; internal set; }

		[JsonIgnore]
		internal PriceValues PricesValues { get; set; }

		[JsonIgnore]
		public float? PricingLow => PricesValues?.Low;

		[JsonIgnore]
		public float? PricingMid => PricesValues?.Mid;

		[JsonIgnore]
		public float? PricingHigh => PricesValues?.High;

		[JsonIgnore]
		public float? PriceLow => PricingLow ?? PricingMid ?? PricingHigh;

		[JsonIgnore]
		public float? PriceMid => PricingMid ?? PricingLow ?? PricingHigh;

		[JsonIgnore]
		public float? PriceHigh => PricingHigh ?? PricingMid ?? PricingLow;

		public CardKeywords GetAllKeywords() => _keywords ?? (_keywords = new CardKeywords(this));

		public ICollection<string> GetKeywords() => GetAllKeywords().OtherKeywords;
		public ICollection<string> GetCastKeywords() => GetAllKeywords().CastKeywords;

		[JsonIgnore]
		public string ImageName { get; set; }

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.AppendLine($"{ManaCost} {NameEn}");
			builder.Append($"{TypeEn} {Layout}");

			if (LoyaltyNum.HasValue)
				builder.Append($" {LoyaltyNum}");

			if (!string.IsNullOrEmpty(Power) || !string.IsNullOrEmpty(Toughness))
				builder.Append($" {Power}/{Toughness}");

			builder.AppendLine();

			if (!string.IsNullOrEmpty(TextEn))
				builder.AppendLine(TextEn);

			string rulingsText = Rulings;
			if (!string.IsNullOrEmpty(rulingsText))
				builder.AppendLine(rulingsText);

			if (!string.IsNullOrEmpty(FlavorEn))
				builder.AppendLine(FlavorEn);

			if (!string.IsNullOrEmpty(Artist))
				builder.AppendLine(Artist);

			return builder.ToString();
		}



		public bool IsLegalIn(string format) =>
			hasLegalityValueIn(format, Legality.Legal);

		public bool IsRestrictedIn(string format) =>
			hasLegalityValueIn(format, Legality.Restricted);

		public bool IsBannedIn(string format) =>
			hasLegalityValueIn(format, Legality.Banned);

		public bool IsFutureIn(string format) =>
			hasLegalityValueIn(format, Legality.Future);

		private bool hasLegalityValueIn(string format, string legalityValue) =>
			LegalityByFormat.TryGetValue(format, out var legality) &&
			Str.Equals(legality, legalityValue);

		public string GetName(string language) => getLocalizedField(nameof(NameEn), language, (loc, lang) => loc.GetName(lang), c => c.NameEn);

		public string GetType(string language) => getLocalizedField(nameof(TypeEn), language, (loc, lang) => loc.GetType(lang), c => c.TypeEn);

		public string GetText(string language) => _textDeltaApplied
			? TextEn
			: getLocalizedField(nameof(TextEn), language, (loc, lang) => loc.GetAbility(lang), c => c.TextEn);

		public string GetFlavor(string language) => getLocalizedField(nameof(FlavorEn), language, (loc, lang) => loc.GetFlavor(lang), c => c.FlavorEn);

		private string getLocalizedField(string propertyName, string language, Func<CardLocalization, string, string> getter, Func<Card, string> defaultGetter)
		{
			string result =
				Localization?.Invoke1(getter, language) ??
				findNamesakeTranslation(propertyName, language, getter) ??
				defaultGetter(this);

			return result;
		}

		private string findNamesakeTranslation(string propertyName, string language, Func<CardLocalization, string, string> getter)
		{
			if (Namesakes == null)
				return null;

			string result;

			lock (_namesakeTranslations)
				if (_namesakeTranslations.TryGetValue((propertyName, language), out result))
					return result;

			result = Namesakes
				.Select(namesake => namesake.Localization?.Invoke1(getter, language))
				.FirstOrDefault(transl => transl != null);

			lock (_namesakeTranslations)
				_namesakeTranslations[(propertyName, language)] = result;

			return result;
		}



		internal void Patch(CardPatch patch)
		{
			if (patch.Name != null)
				NameEn = patch.Name;

			if (patch.Text != null)
			{
				_textDeltaApplied = true;
				TextEn = patch.Text;
			}

			if (patch.Flavor != null)
				FlavorEn = patch.Flavor;

			if (patch.GeneratedMana != null)
			{
				GeneratedManaArrExpanded = patch.GeneratedMana;
				GeneratedManaArr = patch.GeneratedMana;
			}

			if (patch.FlipDuplicate)
				Remove = TextEn != OriginalText;

			if (patch.Loyalty != null)
				Loyalty = patch.Loyalty;

			if (patch.Type != null)
				TypeEn = patch.Type;

			if (patch.OriginalType != null)
				OriginalType = patch.OriginalType;

			if (patch.Types != null)
				TypesArr = patch.Types;

			if (patch.Subtypes != null)
				SubtypesArr = patch.Subtypes;

			if (patch.Layout != null)
				Layout = patch.Layout;

			if (patch.Names != null)
				Names = patch.Names;

			if (patch.Number != null)
				Number = patch.Number;

			if (patch.FullDuplicate && !_foundDuplicates.Add($"{SetCode}.{NameEn}"))
				Remove = true;

			if (patch.Life != null)
				Life = patch.Life;

			if (patch.Hand != null)
				Hand = patch.Hand;
		}



		public Bitmap Image(UiModel ui)
		{
			var imageModel = ImageModel(ui);

			if (imageModel == null)
				return null;

			return ui.ImageLoader.GetSmallImage(imageModel);
		}

		public ImageModel ImageModel(UiModel ui)
		{
			return ui.Config.DisplaySmallImages 
				? getImageModel(ui) 
				: getZoomImageModel(ui);
		}

		private ImageModel getImageModel(UiModel ui)
		{
			if (!_imageModelSelected)
			{
				if (!ui.ImageRepo.IsLoadingSmallComplete)
					return null;

				_imageModel = ui.GetSmallImage(this);
				_imageModelSelected = true;
			}

			return _imageModel;
		}

		private ImageModel getZoomImageModel(UiModel ui)
		{
			if (!_zoomImageModelSelected)
			{
				if (!ui.ImageRepo.IsLoadingZoomComplete)
					return null;

				_zoomImageModel = ui.GetZoomImages(this)?.FirstOrDefault();
				_zoomImageModelSelected = true;
			}

			return _zoomImageModel;
		}

		public void ResetImageModel()
		{
			_imageModelSelected = false;
		}

		public bool HasImage(UiModel ui) => ImageModel(ui) != null;

		public void PreloadImage(UiModel ui)
		{
			ui.ImageLoader.GetSmallImage(ImageModel(ui));
		}



		public string Name(UiModel ui) => GetName(ui.LanguageController?.Language);

		public string Type(UiModel ui) => GetType(ui.LanguageController?.Language);

		public string Text(UiModel ui) => GetText(ui.LanguageController?.Language);

		public string Flavor(UiModel ui) => GetFlavor(ui.LanguageController?.Language);



		public float? CollectionTotalLow(UiModel ui) => CollectionCount(ui) == 0 ? (float?) null : (PriceLow ?? 0) * CollectionCount(ui);

		public float? CollectionTotalMid(UiModel ui) => CollectionCount(ui) == 0 ? (float?) null : (PriceMid ?? 0) * CollectionCount(ui);

		public float? CollectionTotalHigh(UiModel ui) => CollectionCount(ui) == 0 ? (float?) null : (PriceHigh ?? 0) * CollectionCount(ui);

		public float? DeckTotalLow(UiModel ui) => DeckCount(ui) == 0 ? (float?) null : (PriceLow ?? 0) * DeckCount(ui);

		public float? DeckTotalMid(UiModel ui) => DeckCount(ui) == 0 ? (float?) null : (PriceMid ?? 0) * DeckCount(ui);

		public float? DeckTotalHigh(UiModel ui) => DeckCount(ui) == 0 ? (float?) null : (PriceHigh ?? 0) * DeckCount(ui);


		public int DeckCount(UiModel ui) => ui.Deck?.GetCount(this) ?? 0;

		public int CollectionCount(UiModel ui) => ui.Collection?.GetCount(this) ?? 0;

		[JsonIgnore]
		public ICollection<Card> Namesakes { get; internal set; }

		[JsonIgnore]
		internal bool Remove { get; set; }

		[JsonIgnore]
		private bool _generatedManaParsed;

		[JsonIgnore]
		private string _generatedMana;

		[JsonIgnore]
		private IList<string> _generatedManaArrExpanded;

		private IList<string> _generatedManaArr;

		[JsonIgnore]
		private string _rulings;

		[JsonIgnore]
		private string _legalIn;

		[JsonIgnore]
		private string _restrictedIn;

		[JsonIgnore]
		private string _bannedIn;

		[JsonIgnore]
		private string _futureIn;

		[JsonIgnore]
		private string[] _legalFormats;

		[JsonIgnore]
		private string[] _restrictedFormats;

		[JsonIgnore]
		private string[] _bannedFormats;

		[JsonIgnore]
		private string[] _futureFormats;

		[JsonIgnore]
		private bool _textDeltaApplied;

		[JsonIgnore]
		private Document _document;

		[JsonIgnore]
		private ImageModel _imageModel;

		[JsonIgnore]
		private ImageModel _zoomImageModel;

		[JsonIgnore]
		private bool _imageModelSelected;

		[JsonIgnore]
		private bool _zoomImageModelSelected;

		[JsonIgnore]
		private readonly Dictionary<(string PropertyName, string Language), string> _namesakeTranslations =
			new Dictionary<(string PropertyName, string Language), string>();

		[JsonIgnore]
		private CardKeywords _keywords;

		private static readonly HashSet<string> _foundDuplicates = new HashSet<string>(Str.Comparer);



		[JsonIgnore]
		private CardFaceVariants _faceVariants;

		[JsonIgnore]
		public CardFaceVariants FaceVariants =>
			_faceVariants ?? (_faceVariants = new CardFaceVariants(this));

		[JsonIgnore]
		private CardFaces _faces;

		[JsonIgnore]
		public CardFaces Faces =>
			_faces ?? (_faces = new CardFaces(FaceVariants));



		public static readonly HashSet<string> ColoredBasicLandNames =
			new HashSet<string>(Str.Comparer)
			{
				"swamp",
				"forest",
				"mountain",
				"island",
				"plains"
			};

		public static readonly HashSet<string> BasicLandNames =
			new HashSet<string>(ColoredBasicLandNames.Append("wastes"), Str.Comparer);
	}
}