using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	internal class LegalityPatch
	{
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public HashSet<string> Sets { get; set; }

		[JsonConverter(typeof(InternedStringArrayConverter))]
		public HashSet<string> Banned { get; set; }

		[JsonConverter(typeof(InternedStringArrayConverter))]
		public HashSet<string> Restricted { get; set; }
	}
}