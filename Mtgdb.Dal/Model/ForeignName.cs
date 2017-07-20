using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	[JsonObject]
	public class ForeignName
	{
		public string Language { get; set; }

		public string Name { get; set; }

		public int Multiverseid { get; set; }
	}
}