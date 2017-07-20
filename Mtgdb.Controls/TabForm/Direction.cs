using System;

namespace Mtgdb.Controls
{
	[Flags]
	public enum Direction
	{
		None = 0,
		North = 1 << 0,
		NorthEast = North | East,
		East = 1 << 1,
		SouthEast = East | South,
		South = 1 << 2,
		SouthWest = South | West,
		West = 1 << 3,
		NorthWest = North | West
	}

	public static class DirectionExtension
	{
		public static int ToWmNChittest(this Direction value)
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
	}
}