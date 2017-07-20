using System;
using System.Collections.Generic;
using System.Linq;

namespace NConfiguration
{
	public abstract class CachedConfigNodeProvider : BaseConfigNodeProvider
	{
		private readonly Lazy<IList<KeyValuePair<string, ICfgNode>>> _list;
		private readonly Lazy<Dictionary<string, List<ICfgNode>>> _index;

		protected CachedConfigNodeProvider()
		{
			_list = new Lazy<IList<KeyValuePair<string, ICfgNode>>>(() => GetAllNodes().ToList().AsReadOnly());
			_index = new Lazy<Dictionary<string, List<ICfgNode>>>(CreateIndex);
		}

		protected abstract IEnumerable<KeyValuePair<string, ICfgNode>> GetAllNodes();

		public override IList<KeyValuePair<string, ICfgNode>> Items { get { return _list.Value; } }

		protected override Dictionary<string, List<ICfgNode>> Index { get { return _index.Value; } }
	}
}
