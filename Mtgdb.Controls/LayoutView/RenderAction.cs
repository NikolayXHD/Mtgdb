using System;
using System.Drawing;

namespace Mtgdb.Controls
{
	internal class RenderAction
	{
		private RectangleF _rect;
		private readonly Action<RectangleF, bool, bool> _action;

		public RenderAction(RectangleF rect, Action<RectangleF, bool, bool> action)
		{
			_rect = rect;
			_action = action;
		}

		public void Offset(PointF point)
		{
			_rect.Offset(point);
		}

		public void Offset(float x, float y)
		{
			_rect.Offset(x, y);
		}

		public void Invoke(bool highlightBegin, bool highlightEnd)
		{
			_action.Invoke(_rect, highlightBegin, highlightEnd);
		}
	}
}