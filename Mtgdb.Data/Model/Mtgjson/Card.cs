using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Lucene.Net.Documents;
using Mtgdb.Data.Index;
using Newtonsoft.Json;

namespace Mtgdb.Data
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
		public List<ForeignData> ForeignData { get; set; }

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

		[JsonProperty("name")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string NameEn { get; set; }

		[JsonProperty("faceName")]
		[JsonConverter(typeof(InternedStringConverter))]
		internal string FaceName { get; set; }

		[JsonProperty("otherFaceIds")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		internal List<string> OtherFaceIdsMtgjson { get; set; }

		public IReadOnlyList<string> OtherFaceIds => _otherFaceIds ??= getOtherFaceIds();

		private IReadOnlyList<string> getOtherFaceIds()
		{
			if (OtherFaceIdsMtgjson == null || OtherFaceIdsMtgjson.Count == 0)
				return Empty<string>.Array;

			if (this.IsMeld() && this.IsSideA())
			{
				var meldResult = Set.MapById(IsToken)[OtherFaceIdsMtgjson[0]];
				return meldResult.OtherFaceIdsMtgjson.Where(F.IsNotEqualTo(MtgjsonId))
					.Append(meldResult.MtgjsonId)
					.ToArray();
			}

			return OtherFaceIdsMtgjson;
		}

		private IReadOnlyList<string> _otherFaceIds;

		public IEnumerable<Card> OtherFaces =>
			OtherFaceIds.Select(id => Set.MapById(IsToken)[id]);

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
		public HashSet<string> SubtypesArr { get; set; }

		/// <summary>
		/// List of card supertypes found before em-dash.
		/// </summary>
		[JsonProperty("supertypes")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public HashSet<string> SupertypesArr { get; set; }

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
		public HashSet<string> TypesArr { get; set; }

		/// <summary>
		/// A universal unique id generated for the card.
		/// </summary>
		[JsonProperty("uuid")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string MtgjsonId { get; set; }

		[JsonProperty("identifiers")]
		public CardIdentifiers Identifiers { get; set; }

		[JsonIgnore]
		public int? MultiverseId => Identifiers.MultiverseId;

		[JsonProperty("side")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Side { get; set; }

		[JsonIgnore]
		public string Id { get; internal set; }

		[JsonIgnore]
		public IReadOnlyList<string> Printings { get; set; }

		[JsonIgnore]
		public Set Set { get; set; }

		/// <summary>
		/// The name of the set
		/// </summary>
		[JsonIgnore]
		public string SetName =>
			Set?.Name;

		/// <summary>
		/// The set's abbreviated code
		/// </summary>
		[JsonIgnore]
		public string SetCode =>
			Set?.Code;

		[JsonIgnore]
		public string ReleaseDate =>
			Set?.ReleaseDate;

		[JsonIgnore]
		public string ReleaseMonth =>
			Set?.ReleaseDate?.Substring(0, 7);

		[JsonIgnore]
		public string ReleaseYear =>
			Set?.ReleaseDate?.Substring(0, 4);


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
		public string ImageNameBase =>
			ImageName.SplitTailingNumber().Item1;

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
		public string GeneratedMana =>
			_generatedMana ??= string.Concat(GeneratedManaArrExpanded);

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

		public Document Document =>
			_document ??= this.ToDocument();

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

					if (Str.Equals(SetCode, chinaOnlyStandardLegalSet) && (
							Printings == null || Printings.Count == 0 ||
							Printings.Count == 1 && Str.Equals(Printings[0], chinaOnlyStandardLegalSet)))
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
		public string LegalIn =>
			_legalIn ??= string.Intern(string.Join(@", ", LegalFormats));

		[JsonIgnore]
		public string RestrictedIn =>
			_restrictedIn ??= string.Intern(string.Join(@", ", RestrictedFormats));

		[JsonIgnore]
		public string BannedIn =>
			_bannedIn ??= string.Intern(string.Join(@", ", BannedFormats));

		[JsonIgnore]
		public string FutureIn =>
			_futureIn ??= string.Intern(string.Join(@", ", FutureFormats));

		[JsonIgnore]
		public IReadOnlyList<string> LegalFormats =>
			_legalFormats ??= getFormats(Legality.Legal);

		[JsonIgnore]
		public IReadOnlyList<string> RestrictedFormats =>
			_restrictedFormats ??= getFormats(Legality.Restricted);

		[JsonIgnore]
		public IReadOnlyList<string> BannedFormats =>
			_bannedFormats ??= getFormats(Legality.Banned);

		[JsonIgnore]
		public IReadOnlyList<string> FutureFormats =>
			_futureFormats ??= getFormats(Legality.Future);

		private IReadOnlyList<string> getFormats(string legality)
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
		public Dictionary<string, ForeignData> Localization { get; internal set; }

		[JsonIgnore]
		public float? Price { get; internal set; }

		public CardKeywords GetAllKeywords() =>
			_keywords ??= new CardKeywords(this);

		public ICollection<string> GetKeywords() =>
			GetAllKeywords().OtherKeywords;

		public ICollection<string> GetCastKeywords() =>
			GetAllKeywords().CastKeywords;

		[JsonIgnore]
		public string ImageName { get; set; }

		public bool IsLegalIn(string format) =>
			hasLegalityValueIn(format, Legality.Legal);

		public bool IsRestrictedIn(string format) =>
			hasLegalityValueIn(format, Legality.Restricted);

		public bool IsBannedIn(string format) =>
			hasLegalityValueIn(format, Legality.Banned);

		public bool IsFutureIn(string format) =>
			hasLegalityValueIn(format, Legality.Future);

		private bool hasLegalityValueIn(string format, string legalityValue) =>
			LegalityByFormat.TryGetValue(format, out string legality) &&
			Str.Equals(legality, legalityValue);

		public string GetName(string language) =>
			getLocalizedField(nameof(NameEn), language, loc => loc.Name, c => c.NameEn);

		public string GetType(string language) =>
			getLocalizedField(nameof(TypeEn), language, loc => loc.Type, c => c.TypeEn);

		public string GetText(string language) =>
			_textDeltaApplied
				? TextEn
				: getLocalizedField(nameof(TextEn), language, loc => loc.Text, c => c.TextEn);

		public string GetFlavor(string language) =>
			getLocalizedField(nameof(FlavorEn), language, loc => loc.Flavor, c => c.FlavorEn);

		private string getLocalizedField(string propertyName, string language,
			Func<ForeignData, string> getter, Func<Card, string> defaultGetter)
		{
			return Localization?.TryGet(language)?.Invoke0(getter) ?? defaultGetter(this);
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

			if (patch.Loyalty != null)
				Loyalty = patch.Loyalty;

			if (patch.Type != null)
				TypeEn = patch.Type;

			if (patch.OriginalText != null)
				OriginalText = patch.OriginalText;

			if (patch.OriginalType != null)
				OriginalType = patch.OriginalType;

			if (patch.Types != null)
				TypesArr = new HashSet<string>(patch.Types, Str.Comparer);

			if (patch.Subtypes != null)
				SubtypesArr = new HashSet<string>(patch.Subtypes, Str.Comparer);

			if (patch.Layout != null)
				Layout = patch.Layout;

			if (patch.Number != null)
				Number = patch.Number;

			Remove =
				patch.Remove ||
				patch.FullDuplicate && !_foundDuplicates.Add($"{SetCode}.{NameEn}") ||
				patch.FlipDuplicate && TextEn != OriginalText;

			if (patch.HasNoSide == true)
				Side = null;

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
				if (!ui.ImageRepo.IsLoadingSmallComplete.Signaled)
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
				if (!ui.ImageRepo.IsLoadingZoomComplete.Signaled)
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

		public bool HasImage(UiModel ui) =>
			ImageModel(ui) != null;

		public void PreloadImage(UiModel ui) =>
			ui.ImageLoader.GetSmallImage(ImageModel(ui));


		public string Name(UiModel ui) =>
			GetName(ui.LanguageController?.Language);

		public string Type(UiModel ui) =>
			GetType(ui.LanguageController?.Language);

		public string Text(UiModel ui) =>
			GetText(ui.LanguageController?.Language);

		public string Flavor(UiModel ui) =>
			GetFlavor(ui.LanguageController?.Language);


		public float? CollectionTotal(UiModel ui) =>
			CollectionCount(ui) == 0 ? (float?) null : (Price ?? 0) * CollectionCount(ui);

		public float? DeckTotal(UiModel ui) =>
			DeckCount(ui) == 0 ? (float?) null : (Price ?? 0) * DeckCount(ui);

		public int DeckCount(UiModel ui) =>
			ui.Deck?.GetCount(this) ?? 0;

		public int CollectionCount(UiModel ui) =>
			ui.Collection?.GetCount(this) ?? 0;

		/// <summary>
		/// all cards with same name, including this
		/// </summary>
		[JsonIgnore]
		public ICollection<Card> Namesakes { get; internal set; }

		/// <summary>
		/// ids of all cards with same name, including this
		/// </summary>
		[JsonIgnore]
		public HashSet<string> NamesakeIds { get; internal set; }

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
		private IReadOnlyList<string> _legalFormats;

		[JsonIgnore]
		private IReadOnlyList<string> _restrictedFormats;

		[JsonIgnore]
		private IReadOnlyList<string> _bannedFormats;

		[JsonIgnore]
		private IReadOnlyList<string> _futureFormats;

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
		private readonly Dictionary<(string PropertyName, string Language), string>
			_namesakeTranslations =
				new Dictionary<(string PropertyName, string Language), string>();

		[JsonIgnore]
		private CardKeywords _keywords;

		private static readonly HashSet<string> _foundDuplicates = new HashSet<string>(Str.Comparer);

		[JsonIgnore]
		private CardFaces _faces;

		[JsonIgnore]
		public CardFaces Faces =>
			_faces ??= new CardFaces(this);

		[JsonIgnore]
		public (int? Number, string Letter) SortableNumber =>
			_sortableNumber ??= Number.SplitTailingLetters();

		[JsonIgnore]
		public bool IsToken { get; internal set; }

		[JsonIgnore]
		public string CardType { get; internal set; }

		[JsonIgnore]
		internal CardFormatter Formatter { private get; set; }

		public override string ToString() =>
			Formatter.ToString(this);

		private (int? Number, string Letter)? _sortableNumber;
	}

	public class CardIdentifiers
	{
		/// <summary>
		/// An integer most cards have which Wizards uses as a card identifier.
		/// </summary>
		[JsonProperty("multiverseId")]
		public int? MultiverseId { get; set; }

		/// <summary>
		/// Id from mtgjson v 4.1.3 and earlier, non-unique per card face
		/// </summary>
		[JsonProperty("scryfallId")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string ScryfallId { get; set; }

		[JsonProperty("scryfallIllustrationId")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string ScryfallIllustrationId { get; set; }


		/// <summary>
		/// Unique by printing, alt/extended art, promo.  But NOT unique per foil.  The foil/non-foil version have same product ID.
		/// </summary>
		[JsonProperty("tcgplayerProductId")]
		public int TcgPlayerProductId { get; set; }
	}
}
