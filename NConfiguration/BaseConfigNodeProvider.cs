using System.Collections.Generic;

namespace NConfiguration
{
	public abstract class BaseConfigNodeProvider : IConfigNodeProvider
	{
		protected Dictionary<string, List<ICfgNode>> CreateIndex()
		{
			var result = new Dictionary<string, List<ICfgNode>>(NameComparer.Instance);

			List<ICfgNode> nodes;
			foreach (var section in Items)
			{
				if (!result.TryGetValue(section.Key, out nodes))
				{
					nodes = new List<ICfgNode>();
					result.Add(section.Key, nodes);
				}
				nodes.Add(section.Value);
			}

			return result;
		}

		protected abstract Dictionary<string, List<ICfgNode>> Index { get; }

		public abstract IList<KeyValuePair<string, ICfgNode>> Items { get; }

		public IEnumerable<ICfgNode> ByName(string sectionName)
		{
			List<ICfgNode> result;
			if (Index.TryGetValue(sectionName, out result))
				return result;

			return new ICfgNode[0];
		}
	}
}
