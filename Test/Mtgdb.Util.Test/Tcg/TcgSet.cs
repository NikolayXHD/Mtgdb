using Newtonsoft.Json;

namespace Mtgdb.Util
{
	public class TcgSet
	{
		[JsonProperty("n")]
		public string Name { get; set; }

		[JsonProperty("c")]
		public string Code { get; set; }
	}
}