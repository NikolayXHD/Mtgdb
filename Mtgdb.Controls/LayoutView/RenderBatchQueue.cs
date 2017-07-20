using System.Collections.Generic;

namespace Mtgdb.Controls
{
	internal class RenderBatchQueue
	{
		private readonly List<RenderBatch> _list = new List<RenderBatch>();

		public int Count => _list.Count;
		public void Clear() => _list.Clear();

		public void Add(RenderBatch renderAction)
		{
			_list.Add(renderAction);
		}

		public RenderBatch this[int i] => _list[i];
	}
}