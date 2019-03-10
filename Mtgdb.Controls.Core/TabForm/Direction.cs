using System;

namespace Mtgdb.Controls
{
	[Flags]
	public enum Direction
	{
		MiddleCenter = 0,

		Left = 1 << 0,
		Top = 1 << 1,
		Right = 1 << 2,
		Bottom = 1 << 3,

		TopRight = Top | Right,
		BottomRight = Bottom | Right,
		BottomLeft = Bottom | Left,
		TopLeft = Top | Left
	}

	public static class DirectionExtension
	{
		public static int ToWmNcHitTest(this Direction value)
		{
			switch (value)
			{
				// ReSharper disable CommentTypo

				case Direction.MiddleCenter:
					return 1; // HTCLIENT
				

				case Direction.Top:
					return 12; // HTTOP

				case Direction.TopRight:
					return 14; //HTTOPRIGHT

				case Direction.Right:
					return 11; // HTRIGHT

				case Direction.BottomRight:
					return 17; // HTBOTTOMRIGHT

				case Direction.Bottom:
					return 15; // HTBOTTOM

				case Direction.BottomLeft:
					return 16; // HTBOTTOMLEFT

				case Direction.Left:
					return 10; // HTLEFT

				case Direction.TopLeft:
					return 13; //HTTOPLEFT
				
				// ReSharper restore CommentTypo
				
				default:
					throw new ArgumentOutOfRangeException(nameof(value), value, null);
			}
		}

		public static bool IsVertical(this Direction value)
		{
			return value.HasFlag(Direction.Top) || value.HasFlag(Direction.Bottom);
		}

		public static bool IsHorizontal(this Direction value)
		{
			return value.HasFlag(Direction.Top) || value.HasFlag(Direction.Bottom);
		}
	}
}