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
		internal IList<Card> ActualCards { get; set; }

		[JsonProperty("tokens")]
		internal IList<Card> Tokens { get; set; }

		[JsonIgnore]
		public IList<Card> Cards { get; set; }

		[JsonIgnore]
		public Dictionary<string, List<Card>> ActualCardsByName { get; internal set; }

		[JsonIgnore]
		public Dictionary<string, List<Card>> TokensByName { get; internal set; }

		public Dictionary<string, List<Card>> MapByName(bool tokens) =>
			tokens ? TokensByName : ActualCardsByName;

		public override string ToString() =>
			$"{Code}: {Name} {ActualCards?.Count ?? 0}+{Tokens?.Count ?? 0} {ReleaseDate}";
		/*
block	string
isOnlineOnly	bool
meta	object	{"date": "2018-10-13","version": "4.0.0"}
mtgoCode	string	"m19"
type	string	"core"
*/
	}
}
