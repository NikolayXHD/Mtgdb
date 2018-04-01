using System.Collections.Generic;
using System.Drawing;

namespace Mtgdb.Controls
{
	public delegate void SelectionStateChanged(
		Rectangle previousRect,
		Point previousStart,
		bool previousSelecting,
		IEnumerable<Rectangle> invalidateAreas);
}