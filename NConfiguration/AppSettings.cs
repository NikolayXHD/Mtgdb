using NConfiguration.Combination;
using NConfiguration.Serialization;
using System.Collections.Generic;

namespace NConfiguration
{
	public sealed class AppSettings : IAppSettings
	{
		private readonly IConfigNodeProvider _nodeProvider;
		private readonly IDeserializer _deserializer;
		private readonly ICombiner _combiner;

		public AppSettings(IConfigNodeProvider nodeProvider)
			: this(nodeProvider, DefaultDeserializer.Instance, DefaultCombiner.Instance)
		{
		}

		public AppSettings(IConfigNodeProvider nodeProvider, IDeserializer deserializer, ICombiner combiner)
		{
			_nodeProvider = nodeProvider;
			_deserializer = deserializer;
			_combiner = combiner;
		}

		public IList<KeyValuePair<string, ICfgNode>> Items
		{
			get { return _nodeProvider.Items; }
		}

		public IEnumerable<ICfgNode> ByName(string sectionName)
		{
			return _nodeProvider.ByName(sectionName);
		}

		public T Deserialize<T>(IDeserializer context, ICfgNode cfgNode)
		{
			return _deserializer.Deserialize<T>(context, cfgNode);
		}

		public T Combine<T>(ICombiner combiner, T x, T y)
		{
			return _combiner.Combine<T>(combiner, x, y);
		}
	}
}
