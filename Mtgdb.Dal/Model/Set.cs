using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	/// <summary>
	/// http://www.mtgjson.com/documentation.html
	/// </summary>
	[JsonObject]
	public class Set
	{
		/// <summary>
		/// The name of the set
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The set's abbreviated code
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// When the set was released (YYYY-MM-DD). For promo sets, the date the first card was released.
		/// </summary>
		public string ReleaseDate { get; set; }

		/// <summary>
		/// The cards in the set
		/// </summary>
		public IList<Card> Cards { get; set; }

		[JsonIgnore]
		public Dictionary<string, List<Card>> CardsByName { get; set; }

		/*
		/// <summary>
		/// The code that Gatherer uses for the set. Only present if different than 'code'
		/// </summary>
		public string GathererCode { get; set; }

		/// <summary>
		/// An old style code used by some Magic software. Only present if different than 'gathererCode' and 'code'
		/// </summary>
		public string OldCode { get; set; }

		/// <summary>
		/// The code that magiccards.info uses for the set. Only present if magiccards.info has this set
		/// </summary>
		public string MagicCardsInfoCode { get; set; }

		/// <summary>
		/// The type of border on the cards, either "white", "black" or "silver"
		/// </summary>
		public string Border { get; set; }

		/// <summary>
		/// Type of set. One of: "core", "expansion", "reprint", "box", "un", "from the vault", "premium deck", "duel deck", "starter", "commander", "planechase", "archenemy", "promo", "vanguard", "masters", "conspiracy"
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// The block this set is in,
		/// </summary>
		public string Block { get; set; }

		/// <summary>
		/// Present and set to true if the set was only released online
		/// </summary>
		public bool OnlineOnly { get; set; }

		/// <summary>
		/// Booster contents for this set, see below for details.
		/// The 'booster' key is present for each set that has physical boosters (so not present for box sets, duel decks, digital masters editions, etc.). It is an array containing one item per card in the booster. Thus the array length is how many cards are in a booster. Each item in the array is either a string representing the type of booster card or an array of strings representing possible types for that booster card.
		/// The common booster card types are: common, uncommon, rare, mythic rare, land, marketing, checklist, double faced 
		/// The Time Spiral block has some additional types: timeshifted common, timeshifted uncommon, timeshifted rare, timeshifted purple
		/// </summary>
		//public IList<string> Booster { get; set; }

		*/
	}
}