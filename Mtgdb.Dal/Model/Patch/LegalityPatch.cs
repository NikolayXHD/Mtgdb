using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	internal class LegalityPatch
	{
		public Operation Sets { get; set; }

		public Operation Banned { get; set; }

		public Operation Restricted { get; set; }

		public class Operation
		{
			[JsonConverter(typeof(InternedStringArrayConverter))]
			public HashSet<string> Add { get; set; }

			[JsonConverter(typeof(InternedStringArrayConverter))]
			public HashSet<string> Remove { get; set; }
		}
	}
}