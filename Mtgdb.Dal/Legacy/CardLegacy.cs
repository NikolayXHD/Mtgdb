using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	/// <summary>
	/// http://www.mtgjson.com/documentation.html
	/// </summary>
	[JsonObject]
	public class CardLegacy
	{
		[JsonIgnore]
		public SetLegacy Set { get; set; }

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
		public string NameNormalized { get; internal set; }

		[JsonIgnore]
		public string ReleaseDate =>
			Set.ReleaseDate;

		/// <summary>
		/// A unique id for this card. It is made up by doing an SHA1 hash of
		/// setCode + cardName + cardImageName
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
		/// The card type. This is the type you would see on the card if printed today. Note: The dash is a UTF8 'long dash' as per the MTG rules
		/// </summary>
		[JsonProperty("type")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string TypeEn { get; set; }

		/// <summary>
		/// The text of the card. May contain mana symbols and other symbols.
		/// </summary>
		[JsonProperty("text")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string TextEn { get; set; }

		[JsonProperty("originalText")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string OriginalText { get; set; }

		/// <summary>
		/// The artist of the card. This may not match what is on the card as MTGJSON corrects many card misprints.
		/// </summary>
		[JsonProperty("artist")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Artist { get; set; }

		/// <summary>
		/// The card number. This is printed at the bottom-center of the card in small text. This is a string, not an integer, because some cards have letters in their numbers.
		/// </summary>
		[JsonProperty("number")]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Number { get; set; }

		/// <summary>
		/// This used to refer to the mtgimage.com file name for this card. mtgimage.com has been SHUT DOWN by Wizards of the Coast. This field will continue to be set correctly and is now only useful for UID purposes.
		/// </summary>
		[JsonConverter(typeof(InternedStringConverter))]
		[JsonProperty("imageName")]
		public string ImageNameOriginal { get; internal set; }

		/// <summary>
		/// The multiverseid of the card on Wizard's Gatherer web page. Cards from sets that do not exist on Gatherer will NOT have a multiverseid. Sets not on Gatherer are: ATH, ITP, DKM, RQS, DPA and all sets with a 4 letter code that starts with a lowercase 'p'.
		/// </summary>
		[JsonProperty("multiverseId")]
		public int? MultiverseId { get; set; }

		public override string ToString() =>
			string.Join(" ",
				Sequence
					.From(SetCode, Number, NameEn, TypeEn, MultiverseId?.ToString())
					.Where(F.IsNotNull));

		internal void PatchCard(CardPatch patch)
		{
			if (patch.Name != null)
				NameEn = patch.Name;

			if (patch.Text != null)
				TextEn = patch.Text;

			if (patch.FlipDuplicate)
				Remove = TextEn != OriginalText;

			if (patch.Type != null)
				TypeEn = patch.Type;

			if (patch.Names != null)
				Names = patch.Names;

			if (patch.Number != null)
				Number = patch.Number;

			if (patch.FullDuplicate && !_foundDuplicates.Add($"{SetCode}.{NameEn}"))
				Remove = true;
		}

		[JsonIgnore]
		internal bool Remove { get; set; }

		private static readonly HashSet<string> _foundDuplicates = new HashSet<string>(Str.Comparer);
	}
}