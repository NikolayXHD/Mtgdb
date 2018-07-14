using System;

namespace Mtgdb.Controls
{
	[Flags]
	public enum Direction
	{
		None = 0,
		MiddleCenter = None,

		North = 1 << 0,
		Top = North,

		NorthEast = North | East,
		TopRight = NorthEast,

		East = 1 << 1,
		Right = East,

		SouthEast = East | South,
		BottomRight = SouthEast,

		South = 1 << 2,
		Bottom = South,

		SouthWest = South | West,
		BottomLeft = SouthWest,

		West = 1 << 3,
		Left = West,

		NorthWest = North | West,
		TopLeft = NorthWest
	}

	public static class DirectionExtension
	{
		public static int ToWmNcHitTest(this Direction value)
		{
			switch (value)
			{
				case Direction.None:
					return 1; // HTCLIENT

				case Direction.North:
					return 12; // HTTOP

				case Direction.NorthEast:
					return 14; //HTTOPRIGHT

				case Direction.East:
					return 11; // HTRIGHT

				case Direction.SouthEast:
					return 17; // HTBOTTOMRIGHT

				case Direction.South:
					return 15; // HTBOTTOM

				case Direction.SouthWest:
					return 16; // HTBOTTOMLEFT

				case Direction.West:
					return 10; // HTLEFT

				case Direction.NorthWest:
					return 13; //HTTOPLEFT

				default:
					throw new ArgumentOutOfRangeException(nameof(value), value, null);
			}
		}

		public static bool IsVertical(this Direction value)
		{
			return value.HasFlag(Direction.North) | value.HasFlag(Direction.South);
		}

		public static bool IsHorizontal(this Direction value)
		{
			return value.HasFlag(Direction.East) | value.HasFlag(Direction.West);
		}
	}
}