using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class Translation
	{
		[JsonProperty("t", DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Type { get; set; }

		[JsonProperty("a", DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Text { get; set; }

		[JsonProperty("f", DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(InternedStringConverter))]
		public string Flavor { get; set; }

		[JsonProperty("#", DefaultValueHandling = DefaultValueHandling.Ignore)]
		[JsonConverter(typeof(InternedStringConverter))]
		public string CardNumber { get; set; }

		[JsonProperty("i")]
		public int Id { get; set; }

		public override string ToString()
		{
			return $@"{Id} #{CardNumber}
{Type}
------
{Text}
------
{Flavor}";
		}
	}
}