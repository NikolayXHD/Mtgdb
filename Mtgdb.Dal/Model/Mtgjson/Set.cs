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
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// The set's abbreviated code
		/// </summary>
		[JsonProperty("code")]
		public string Code { get; set; }

		/// <summary>
		/// When the set was released (YYYY-MM-DD). For promo sets, the date the first card was released.
		/// </summary>
		[JsonProperty("releaseDate")]
		public string ReleaseDate { get; set; }

		/// <summary>
		/// The cards in the set
		/// </summary>
		[JsonProperty("cards")]
		public IList<Card> Cards { get; set; }

		[JsonIgnore]
		public Dictionary<string, List<Card>> CardsByName { get; set; }

		public override string ToString()
		{
			return $"{Code}: {Name}";
		}

		// [JsonProperty("tokens")]
		// public IList<Token> Tokens { get; set; }

/*
block	string
isOnlineOnly	bool
meta	object	{"date": "2018-10-13","version": "4.0.0"}
mtgoCode	string	"m19"
type	string	"core"
*/
	}
}