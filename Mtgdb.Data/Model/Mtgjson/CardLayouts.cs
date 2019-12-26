using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Data
{
	public static class CardLayouts
	{
		public const string Normal = "Normal";
		public const string Saga = "Saga";
		public const string Leveler = "Leveler";
		public const string Phenomenon = "Phenomenon";
		public const string Plane = "Plane";
		public const string Scheme = "Scheme";
		public const string Vanguard = "Vanguard";

		public const string Aftermath = "Aftermath";
		public const string Split = "Split";
		public const string Transform = "Transform";
		public const string Flip = "Flip";

		public const string Meld = "Meld";
		public const string Host = "Host";
		public const string Augment = "Augment";
		public const string Adventure = "Adventure";

		private static readonly HashSet<string> _doubleFaceLayouts =
			new HashSet<string>(Str.Comparer)
			{
				Aftermath, Split, Transform, Flip, Adventure,
			};

		private static readonly HashSet<string> _singleFaceLayouts =
			new HashSet<string>(Str.Comparer)
			{
				Normal, Leveler, Phenomenon, Plane, Scheme, Vanguard, Saga, Host, Augment,
			};

		private static readonly HashSet<string> _allLayouts =
			new HashSet<string>(
				_doubleFaceLayouts
					.Concat(_singleFaceLayouts)
					.Append(Meld),
				Str.Comparer);

		public static bool IsDoubleFace(this Card c) =>
			_doubleFaceLayouts.Contains(c.Layout);

		public static bool IsSingleFace(this Card c) =>
			_singleFaceLayouts.Contains(c.Layout);

		public static bool IsMultiFace(this Card c) =>
			!IsSingleFace(c);

		public static bool IsSingleSide(this Card c) =>
			!IsMeld(c) && !IsTransform(c);

		internal static bool IsKnownLayout(this Card c) =>
			_allLayouts.Contains(c.Layout);

		public static bool IsNormal(this Card c) =>
			Str.Equals(c.Layout, Normal);

		public static bool IsAftermath(this Card c) =>
			Str.Equals(c.Layout, Aftermath);

		public static bool IsSplit(this Card c) =>
			Str.Equals(c.Layout, Split);

		public static bool IsTransform(this Card c) =>
			Str.Equals(c.Layout, Transform);

		public static bool IsFlip(this Card c) =>
			Str.Equals(c.Layout, Flip);

		public static bool IsMeld(this Card c) =>
			Str.Equals(c.Layout, Meld);

		public static bool IsLeveler(this Card c) =>
			Str.Equals(c.Layout, Leveler);

		public static bool IsPhenomenon(this Card c) =>
			Str.Equals(c.Layout, Phenomenon);

		public static bool IsPlane(this Card c) =>
			Str.Equals(c.Layout, Plane);

		public static bool IsScheme(this Card c) =>
			Str.Equals(c.Layout, Scheme);

		public static bool IsSaga(this Card c) =>
			Str.Equals(c.Layout, Saga);

		public static bool IsVanguard(this Card c) =>
			Str.Equals(c.Layout, Vanguard);

		public static bool IsAdventure(this Card c) =>
			Str.Equals(c.Layout, Adventure);
	}
}
