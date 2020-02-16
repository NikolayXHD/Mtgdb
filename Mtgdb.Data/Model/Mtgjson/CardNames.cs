using System.Collections.Generic;

namespace Mtgdb.Data
{
	public static class CardNames
	{
		public const string Swamp = "Swamp";
		public const string Forest = "Forest";
		public const string Mountain = "Mountain";
		public const string Island = "Island";
		public const string Plains = "Plains";
		public const string Wastes = "Wastes";

		public const string Manifest = "Manifest";
		public const string Morph = "Morph";

		public static readonly HashSet<string> ColoredBasicLands =
			new HashSet<string>(Str.Comparer)
			{
				Swamp,
				Forest,
				Mountain,
				Island,
				Plains
			};

		public static readonly HashSet<string> BasicLands =
			new HashSet<string>(ColoredBasicLands.Append(Wastes), Str.Comparer);
	}
}
