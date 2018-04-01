using System;
using System.Collections.Generic;
using System.Drawing;

namespace Mtgdb.Controls
{
	internal class RenderBatch
	{
		public RenderBatch(bool isHighlighted)
		{
			IsHighlighted = isHighlighted;
		}

		public void Add(RectangleF rect, Action<RectangleF, bool, bool> action)
		{
			_actions.Add(new RenderAction(rect, action));
		}

		public void Offset(float x, float y)
		{
			foreach (var action in _actions)
				action.Offset(x, y);
		}

		public void Invoke(bool highlightBegin, bool highlightEnd)
		{
			foreach (var action in _actions)
				action.Invoke(highlightBegin, highlightEnd);
		}

		public bool IsHighlighted { get; }
		private readonly IList<RenderAction> _actions = new List<RenderAction>();
	}
}