using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	internal class LegalityPatch
	{
		public Operation Sets { get; [UsedImplicitly] set; }

		public Operation Banned { get; [UsedImplicitly] set; }

		public Operation Restricted { get; [UsedImplicitly] set; }

		public class Operation
		{
			[JsonConverter(typeof(InternedStringArrayConverter))]
			public HashSet<string> Add { get; [UsedImplicitly] set; }

			[JsonConverter(typeof(InternedStringArrayConverter))]
			public HashSet<string> Remove { get; [UsedImplicitly] set; }
		}
	}
}