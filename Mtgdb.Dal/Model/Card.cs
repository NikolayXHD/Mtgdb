using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	/// <summary>
	/// http://www.mtgjson.com/documentation.html
	/// </summary>
	[JsonObject]
	public class Card
	{
		[JsonIgnore]
		public UiModel UiModel { get; internal set; }

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
		public Bitmap Image => UiModel?.ImageCache.GetImage(ImageModel, UiModel.ImageCache.CardSize);

		[JsonIgnore]
		public ImageModel ImageModel { get; set; }

		[JsonIgnore]
		public string ImageNameBase => ImageName.SplitTalingNumber().Item1;

		[JsonIgnore]
		public string NameNormalized { get; internal set; }

		[JsonIgnore]
		public int IndexInFile { get; set; }

		[JsonIgnore]
		public bool IsNonDuplicate { get; internal set; }

		[JsonIgnore]
		public bool IsSearchResult { get; set; }



		[JsonIgnore]
		public int DeckCount => UiModel?.Deck?.GetCount(this) ?? 0;

		[JsonIgnore]
		public int CollectionCount => UiModel?.Collection?.GetCount(this) ?? 0;



		[JsonIgnore]
		public string Types { get; internal set; }

		[JsonIgnore]
		public string Subtypes { get; internal set; }

		[JsonIgnore]
		public string Supertypes { get; internal set; }



		[JsonIgnore]
		public bool HasImage => ImageModel != null;

		[JsonIgnore]
		public float? PowerNum { get; internal set; }

		[JsonIgnore]
		public float? ToughnessNum { get; internal set; }

		[JsonIgnore]
		public int? LoyaltyNum { get; internal set; }

		[JsonIgnore]
		public string GeneratedMana { get; set; }



		[JsonIgnore]
		public string Rulings
		{
			get
			{
				if (_rulings != null)
					return _rulings;

				var resultBuilder = new List<string>();

				if (RulingsList != null)
					foreach (var ruling in RulingsList)
						resultBuilder.Add($"{ruling.Date}: {ruling.Text}");

				if (!string.IsNullOrEmpty(LegalIn))
					resultBuilder.Add($"legal: {LegalIn}");

				if (!string.IsNullOrEmpty(RestrictedIn))
					resultBuilder.Add($"restricted: {RestrictedIn}");

				if (!string.IsNullOrEmpty(BannedIn))
					resultBuilder.Add($"banned: {BannedIn}");

				_rulings = string.Intern(string.Join("\n", resultBuilder));
				return _rulings;
			}
		}

		[JsonIgnore]
		public string LegalIn
		{
			get
			{
				if (_legalIn != null)
					return _legalIn;

				var legalFormats = LegalityByFormat
					.Where(_ => Str.Equals(_.Value.Legality, "legal"))
					.Select(_ => _.Key);

				_legalIn = string.Intern(string.Join(@", ", legalFormats));
				return _legalIn;
			}
		}

		[JsonIgnore]
		public string RestrictedIn
		{
			get
			{
				if (_restrictedIn != null)
					return _restrictedIn;

				var legalFormats = LegalityByFormat
					.Where(_ => Str.Equals(_.Value.Legality, "restricted"))
					.Select(_ => _.Key);

				_restrictedIn = string.Intern(string.Join(@", ", legalFormats));
				return _restrictedIn;
			}
		}

		[JsonIgnore]
		public string BannedIn
		{
			get
			{
				if (_bannedIn != null)
					return _bannedIn;

				var legalFormats = LegalityByFormat
					.Where(_ => Str.Equals(_.Value.Legality, "banned"))
					.Select(_ => _.Key);

				_bannedIn = string.Intern(string.Join(@", ", legalFormats));
				return _bannedIn;
			}
		}

		[JsonIgnore]
		public Dictionary<string, LegalityNote> LegalityByFormat
		{
			get
			{
				if (_legalityByFormat != null)
					return _legalityByFormat;

				if (LegalitiesList == null)
					_legalityByFormat = new Dictionary<string, LegalityNote>(Str.Comparer);
				else
					_legalityByFormat = LegalitiesList
						//it was retired in June 12, when Vintage was introduced into Magic Online
						.Where(_ => !Str.Equals(_.Format, "classic"))
						.ToDictionary(_ => _.Format, Str.Comparer);

				return _legalityByFormat;
			}
		}



		[JsonIgnore]
		public CardLocalization Localization { get; internal set; }

		[JsonIgnore]
		internal PriceValues PricesValues { get; set; }

		[JsonIgnore]
		public string Name => GetName(UiModel?.Language);

		[JsonIgnore]
		public string Type => GetType(UiModel?.Language);

		[JsonIgnore]
		public string Text => GetText(UiModel?.Language);

		[JsonIgnore]
		public string Flavor => GetFlavor(UiModel?.Language);



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

		[JsonIgnore]
		public float? CollectionTotalLow => CollectionCount == 0 ? (float?) null : (PriceLow ?? 0)*CollectionCount;

		[JsonIgnore]
		public float? CollectionTotalMid => CollectionCount == 0 ? (float?) null : (PriceMid ?? 0)*CollectionCount;

		[JsonIgnore]
		public float? CollectionTotalHigh => CollectionCount == 0 ? (float?) null : (PriceHigh ?? 0)*CollectionCount;

		[JsonIgnore]
		public float? DeckTotalLow => DeckCount == 0 ? (float?) null : (PriceLow ?? 0)*DeckCount;

		[JsonIgnore]
		public float? DeckTotalMid => DeckCount == 0 ? (float?) null : (PriceMid ?? 0)*DeckCount;

		[JsonIgnore]
		public float? DeckTotalHigh => DeckCount == 0 ? (float?) null : (PriceHigh ?? 0)*DeckCount;




		/// <summary>
		/// A unique id for this card. It is made up by doing an SHA1 hash of setCode + cardName + cardImageName
		/// </summary>
		[JsonProperty("id")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Id { get; set; }

		/// <summary>
		/// The card name. For split, double-faced and flip cards, just the name of one side of the card. Basically each 'sub-card' has its own record.
		/// </summary>
		[JsonProperty("name")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string NameEn { get; set; }

		/// <summary>
		/// Only used for split, flip, double-faced, and meld cards. Will contain all the names on this card, front or back. For meld cards, the first name is the card with the meld ability, which has the top half on its back, the second name is the card with the reminder text, and the third name is the melded back face.
		/// </summary>
		[JsonProperty("names")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public IList<string> Names { get; set; }

		/// <summary>
		/// The mana cost of this card. Consists of one or more mana symbols.
		/// </summary>
		[JsonProperty("manaCost")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string ManaCost { get; set; }

		/// <summary>
		/// Converted mana cost. Always a number. NOTE: cmc may have a decimal point as cards from unhinged may contain "half mana" (such as 'Little Girl' with a cmc of 0.5). Cards without this field have an implied cmc of zero as per rule 202.3a
		/// </summary>
		[JsonProperty("cmc")]
		public float Cmc { get; set; }

		/// <summary>
		/// The card type. This is the type you would see on the card if printed today. Note: The dash is a UTF8 'long dash' as per the MTG rules
		/// </summary>
		[JsonProperty("type")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string TypeEn { get; set; }

		/// <summary>
		/// The types of the card. These appear to the left of the dash in a card type. Example values: Instant, Sorcery, Artifact, Creature, Enchantment, Land, Planeswalker
		/// </summary>
		[JsonProperty("types")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public IList<string> TypesArr { get; set; }

		/// <summary>
		/// The supertypes of the card. These appear to the far left of the card type. Example values: Basic, Legendary, Snow, World, Ongoing 
		/// </summary>
		[JsonProperty("supertypes")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public IList<string> SupertypesArr { get; set; }

		/// <summary>
		/// The subtypes of the card. These appear to the right of the dash in a card type. Usually each word is its own subtype. Example values: Trap, Arcane, Equipment, Aura, Human, Rat, Squirrel, etc.
		/// </summary>
		[JsonProperty("subtypes")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public IList<string> SubtypesArr { get; set; }

		/// <summary>
		/// The rarity of the card. Examples: Common, Uncommon, Rare, Mythic Rare, Special, Basic Land
		/// </summary>
		[JsonProperty("rarity")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Rarity { get; set; }

		/// <summary>
		/// The text of the card. May contain mana symbols and other symbols.
		/// </summary>
		[JsonProperty("text")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string TextEn { get; set; }

		[JsonProperty("originalText")]
		[JsonConverter(typeof(InternedStringConverter))]
		private string OriginalText { get; set; }

		/// <summary>
		/// The flavor text of the card.
		/// </summary>
		[JsonProperty("flavor")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string FlavorEn { get; set; }

		/// <summary>
		/// The artist of the card. This may not match what is on the card as MTGJSON corrects many card misprints.
		/// </summary>
		[JsonProperty("artist")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Artist { get; set; }

		/// <summary>
		/// The power of the card. This is only present for creatures. This is a string, not an integer, because some cards have powers like: "1+*"
		/// </summary>
		[JsonProperty("power")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Power { get; set; }

		/// <summary>
		/// The toughness of the card. This is only present for creatures. This is a string, not an integer, because some cards have toughness like: "1+*"
		/// </summary>
		[JsonProperty("toughness")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Toughness { get; set; }

		/// <summary>
		/// The loyalty of the card. This is only present for planeswalkers.
		/// </summary>
		[JsonProperty("loyalty")]
		[JsonConverter(typeof (IntToInternedStringConverter))]
		public string Loyalty { get; set; }

		/// <summary>
		/// The rulings for the card. An array of objects, each object having 'date' and 'text' keys.
		/// </summary>
		[JsonProperty("rulings")]
		public List<Ruling> RulingsList { get; set; }

		/// <summary>
		/// Which formats this card is legal, restricted or banned in. An array of objects, each object having 'format' and 'legality'. A 'condition' key may be added in the future if Gatherer decides to utilize it again.
		/// </summary>
		[JsonProperty("legalities")]
		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		public List<LegalityNote> LegalitiesList { get; set; }

		/// <summary>
		/// The card number. This is printed at the bottom-center of the card in small text. This is a string, not an integer, because some cards have letters in their numbers.
		/// </summary>
		[JsonProperty("number")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Number { get; set; }

		/// <summary>
		/// Number used by MagicCards.info for their indexing URLs (Most often it is the card number in the set)
		/// </summary>
		[JsonProperty("mciNumber")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string MciNumber { get; set; }

		/// <summary>
		/// This used to refer to the mtgimage.com file name for this card. mtgimage.com has been SHUT DOWN by Wizards of the Coast. This field will continue to be set correctly and is now only useful for UID purposes.
		/// </summary>
		[JsonConverter(typeof(InternedStringConverter))]
		[JsonProperty("imageName")]
		public string ImageName { get; internal set; }

		[JsonConverter(typeof(InternedStringConverter))]
		[JsonProperty("layout")]
		public string Layout { get; set; }

		/*
		/// <summary>
		/// Foreign language names for the card, if this card in this set was printed in another language. An array of objects, each object having 'language', 'name' and 'multiverseid' keys. Not available for all sets.
		/// </summary>
		public List<ForeignName> ForeignNames { get; set; }
		
		

		/// <summary>
		/// The card colors. Usually this is derived from the casting cost, but some cards are special (like the back of double-faced cards and Ghostfire).
		/// </summary>
		public List<string> Colors { get; set; }

		/// <summary>
		/// This is created reading all card color information and costs. It is the same for double-sided cards (if they have different colors, the identity will have both colors). It also identifies all mana symbols in the card (cost and text). Mostly used on commander decks.
		/// </summary>
		public List<string> ColorIdentity { get; set; }
		
		/// <summary>
		/// The multiverseid of the card on Wizard's Gatherer web page. Cards from sets that do not exist on Gatherer will NOT have a multiverseid. Sets not on Gatherer are: ATH, ITP, DKM, RQS, DPA and all sets with a 4 letter code that starts with a lowercase 'p'.
		/// </summary>
		public int? Multiverseid { get; set; }
		
		/// <summary>
		/// If a card has alternate art (for example, 4 different Forests, or the 2 Brothers Yamazaki) then each other variation's multiverseid will be listed here, NOT including the current card's multiverseid. NOTE: Only present for sets that exist on Gatherer.
		/// </summary>
		public List<int> Variations { get; set; }
		
		/// <summary>
		/// The watermark on the card. Note: Split cards don't currently have this field set, despite having a watermark on each side of the split card.
		/// </summary>
		public string Watermark { get; set; }

		/// <summary>
		/// If the border for this specific card is DIFFERENT than the border specified in the top level set JSON, then it will be specified here. (Example: Unglued has silver borders, except for the lands which are black bordered)
		/// </summary>
		public string Border { get; set; }

		/// <summary>
		/// If this card was a timeshifted card in the set.
		/// </summary>
		public bool Timeshifted { get; set; }

		/// <summary>
		/// Maximum hand size modifier. Only exists for Vanguard cards.
		/// </summary>
		public int? Hand { get; set; }

		/// <summary>
		/// Starting life total modifier. Only exists for Vanguard cards.
		/// </summary>
		public int? Life { get; set; }

		/// <summary>
		/// Set to true if this card is reserved by Wizards Official Reprint Policy
		/// </summary>
		public bool Reserved { get; set; }

		/// <summary>
		/// The date this card was released. This is only set for promo cards. The date may not be accurate to an exact day and month, thus only a partial date may be set (YYYY-MM-DD or YYYY-MM or YYYY). Some promo cards do not have a known release date.
		/// </summary>
		public string ReleaseDate { get; set; }
		
		/// <summary>
		/// Set to true if this card was only released as part of a core box set. These are technically part of the core sets and are tournament legal despite not being available in boosters.
		/// </summary>
		public bool Starter { get; set; }

		/// <summary>
		/// The sets that this card was printed in, expressed as an array of set codes.
		/// </summary>
		public List<string> Printings { get; set; }

		/// <summary>
		/// The original text on the card at the time it was printed. This field is not available for promo cards.
		/// </summary>
		public string OriginalText { get; set; }

		/// <summary>
		/// The original type on the card at the time it was printed. This field is not available for promo cards.
		/// </summary>
		public string OriginalType { get; set; }

		/// <summary>
		/// For promo cards, this is where this card was originally obtained. For box sets that are theme decks, this is which theme deck the card is from. For clash packs, this is which deck it is from.
		/// </summary>
		public string Source { get; set; }
		*/



		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.AppendLine($"{ManaCost} {Name}");
			builder.Append($"{Type}");

			if (LoyaltyNum.HasValue)
				builder.Append($" {LoyaltyNum}");

			if (!string.IsNullOrEmpty(Power) || !string.IsNullOrEmpty(Toughness))
				builder.Append($" {Power}/{Toughness}");

			builder.AppendLine();

			if (!string.IsNullOrEmpty(Text))
				builder.AppendLine(Text);

			string rulingsText = Rulings;
			if (!string.IsNullOrEmpty(rulingsText))
				builder.AppendLine(rulingsText);

			if (!string.IsNullOrEmpty(Flavor))
				builder.AppendLine(Flavor);

			if (!string.IsNullOrEmpty(Artist))
				builder.AppendLine(Artist);

			return builder.ToString();
		}

		public void PreloadImage()
		{
			UiModel?.ImageCache.GetImage(ImageModel, UiModel.ImageCache.CardSize);
		}



		public bool IsLegalIn(string format)
		{
			LegalityNote legality;
			if (LegalityByFormat.TryGetValue(format, out legality))
				return Str.Equals(legality.Legality, Legality.Legal);

			return false;
		}

		public bool IsRestrictedIn(string format)
		{
			LegalityNote legality;
			if (LegalityByFormat.TryGetValue(format, out legality))
				return Str.Equals(legality.Legality, Legality.Restricted);

			return false;
		}

		public bool IsBannedIn(string format)
		{
			LegalityNote legality;
			if (LegalityByFormat.TryGetValue(format, out legality))
				return Str.Equals(legality.Legality, Legality.Banned);

			return false;
		}



		public string GetName(string language) => Localization?.GetName(language) ?? NameEn;

		public string GetType(string language) => Localization?.GetType(language) ?? TypeEn;

		public string GetText(string language) => _textDeltaApplied ? TextEn : (Localization?.GetAbility(language) ?? TextEn);

		public string GetFlavor(string language) => Localization?.GetFlavor(language) ?? FlavorEn;



		internal void ApplyDelta(CardDelta cardDelta)
		{
			if (cardDelta.Legality != null)
				foreach (var delta in cardDelta.Legality)
				{
					LegalityNote legality;
					if (LegalityByFormat.TryGetValue(delta.Format, out legality))
						legality.Legality = delta.Legality;
					else
						LegalityByFormat.Add(delta.Format, delta);
				}

			if (cardDelta.Text != null)
			{
				_textDeltaApplied = true;
				TextEn = cardDelta.Text;
			}

			if (cardDelta.GeneratedMana != null)
			{
				GeneratedMana = cardDelta.GeneratedMana;
			}

			if (cardDelta.FlipDuplicate)
				Remove = TextEn != OriginalText;

			if (cardDelta.MciNumber != null)
				MciNumber = cardDelta.MciNumber;
		}

		internal bool Remove { get; private set; }

		[JsonIgnore]
		private Dictionary<string, LegalityNote> _legalityByFormat;

		[JsonIgnore]
		private string _rulings;

		[JsonIgnore]
		private string _legalIn;

		[JsonIgnore]
		private string _restrictedIn;

		[JsonIgnore]
		private string _bannedIn;

		[JsonIgnore]
		private bool _textDeltaApplied;
	}
}