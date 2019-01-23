using System.Collections.Generic;

namespace Mtgdb.Dal
{
	public static class CardLayouts
	{
		public const string Normal = "Normal";

		public const string Aftermath = "Aftermath";
		public const string Split = "Split";
		public const string Transform = "Transform";
		public const string Flip = "Flip";

		public const string Meld = "Meld";
		public const string Leveler = "Leveler";
		public const string Phenomenon = "Phenomenon";
		public const string Plane = "Plane";
		public const string Scheme = "Scheme";
		public const string Vanguard = "Vanguard";

		public static readonly HashSet<string> DoubleFacedLayouts =
			new HashSet<string>(Str.Comparer)
			{
				Aftermath, Split, Transform, Flip
			};
	}
}