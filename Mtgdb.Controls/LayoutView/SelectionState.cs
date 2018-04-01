using System.Collections.Generic;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class SelectionState
	{
		public void ResetSelection()
		{
			var previousStart = Start;
			var previousRect = Rectangle;
			var previousSelecting = Selecting;

			Rectangle = Rectangle.Empty;
			Start = Point.Empty;
			End = Point.Empty;
			Selecting = false;
			SelectAll = false;

			onChanged(previousRect, previousStart, previousSelecting);
		}

		public void StartSelectionAt(Point location)
		{
			var previousStart = Start;
			var previousRect = Rectangle;
			var previousSelecting = Selecting;

			Rectangle = new Rectangle(location, default(Size));
			Start = location;
			End = location;
			Selecting = true;
			SelectAll = false;

			onChanged(previousRect, previousStart, previousSelecting);
		}

		public void MoveSelectionTo(Point location)
		{
			var previousRect = Rectangle;

			var direction = location.Minus(Start)
				.ToSize();

			var x = Start.X;
			var y = Start.Y;
			var width = direction.Width;
			var height = direction.Height;

			if (width < 0)
			{
				x += width;
				width *= -1;
			}

			if (height < 0)
			{
				y += height;
				height *= -1;
			}

			Rectangle = new Rectangle(x, y, width, height);
			End = location;
			SelectAll = false;

			onChanged(previousRect, Start, Selecting);
		}

		public void EndSelection()
		{
			var previousSelecting = Selecting;
			Selecting = false;
			onChanged(Rectangle, Start, previousSelecting);
		}

		private static IEnumerable<Rectangle> getDelta(Rectangle rect1, Rectangle rect2)
		{
			if (!rect1.IntersectsWith(rect2))
			{
				yield return rect1;
				yield return rect2;
				yield break;
			}

			int leftMin, leftMax;
			int rightMin, rightMax;
			int topMin, topMax;
			int bottomMin, bottomMax;

			if (rect1.Left < rect2.Left)
			{
				leftMin = rect1.Left;
				leftMax = rect2.Left;
			}
			else
			{
				leftMin = rect2.Left;
				leftMax = rect1.Left;
			}

			if (rect1.Right < rect2.Right)
			{
				rightMin = rect1.Right;
				rightMax = rect2.Right;
			}
			else
			{
				rightMin = rect2.Right;
				rightMax = rect1.Right;
			}

			if (rect1.Top < rect2.Top)
			{
				topMin = rect1.Top;
				topMax = rect2.Top;
			}
			else
			{
				topMin = rect2.Top;
				topMax = rect1.Top;
			}

			if (rect1.Bottom < rect2.Bottom)
			{
				bottomMin = rect1.Bottom;
				bottomMax = rect2.Bottom;
			}
			else
			{
				bottomMin = rect2.Bottom;
				bottomMax = rect1.Bottom;
			}

			if (leftMin < leftMax)
				yield return new Rectangle(leftMin, topMin, leftMax - leftMin, bottomMin - topMin);

			if (topMin < topMax)
				yield return new Rectangle(leftMax, topMin, rightMax - leftMax, topMax - topMin);

			if (rightMin < rightMax)
				yield return new Rectangle(rightMin, topMax, rightMax - rightMin, bottomMax - topMax);

			if (bottomMin < bottomMax)
				yield return new Rectangle(leftMin, bottomMin, rightMin - leftMin, bottomMax - bottomMin);
		}

		private void onChanged(Rectangle previousRect, Point previousStart, bool previousSelecting)
		{
			if (previousRect != Rectangle || previousStart != Start || previousSelecting != Selecting)
				Changed?.Invoke(previousRect, previousStart, previousSelecting, getDelta(Rectangle, previousRect));
		}

		public event SelectionStateChanged Changed;

		public Point Start { get; private set; } = Point.Empty;
		public Point End { get; private set; } = Point.Empty;
		public Rectangle Rectangle { get; private set; } = Rectangle.Empty;
		public bool Selecting { get; private set; }

		public bool SelectAll { get; set; }
	}
}