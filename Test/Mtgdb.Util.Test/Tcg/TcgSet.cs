using Newtonsoft.Json;

namespace Mtgdb.Test
{
	public class TcgSet
	{
		[JsonProperty("n")]
		public string Name { get; set; }

		[JsonProperty("c")]
		public string Code { get; set; }
	}
}