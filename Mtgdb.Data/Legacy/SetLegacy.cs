using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mtgdb.Data
{
	/// <summary>
	/// http://www.mtgjson.com/documentation.html
	/// </summary>
	[JsonObject]
	public class SetLegacy
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
		public IList<CardLegacy> Cards { get; set; }

		[JsonIgnore]
		public Dictionary<string, List<CardLegacy>> CardsByName { get; set; }

		public override string ToString() =>
			$"{Code}: {Name}";
	}
}