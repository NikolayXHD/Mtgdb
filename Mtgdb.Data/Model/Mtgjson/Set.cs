using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mtgdb.Data
{
	/// <summary>
	/// http://www.mtgjson.com/documentation.html
	/// </summary>
	[JsonObject]
	public class Set
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("code")]
		public string Code { get; set; }

		/// <summary>
		/// When the set was released (YYYY-MM-DD). For promo sets, the date the first card was released.
		/// </summary>
		[JsonProperty("releaseDate")]
		public string ReleaseDate { get; set; }

		[JsonProperty("cards")]
		public IList<Card> Cards { get; set; }

		[JsonProperty("cards")]
		public IList<Card> Tokens { get; set; }

		[JsonIgnore]
		public Dictionary<string, List<Card>> CardsByName { get; set; }

		public override string ToString() =>
			$"{Code}: {Name} {Cards?.Count ?? 0}+{Tokens?.Count ?? 0} {ReleaseDate}";
		/*
block	string
isOnlineOnly	bool
meta	object	{"date": "2018-10-13","version": "4.0.0"}
mtgoCode	string	"m19"
type	string	"core"
*/
	}
}
