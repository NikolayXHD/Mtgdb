using NConfiguration.Combination;
using NConfiguration.Serialization;
using System;
using System.Collections.Generic;

namespace NConfiguration
{
	public sealed class ChangeableAppSettings : IAppSettings, IChangeable
	{
		private readonly IChangeable _changeable;
		private readonly IAppSettings _source;

		public ChangeableAppSettings(IAppSettings source, IChangeable changeable)
		{
			_source = source;
			_changeable = changeable;
		}

		public event EventHandler Changed
		{
			add { _changeable.Changed += value; }
			remove { _changeable.Changed -= value; }
		}

		public IList<KeyValuePair<string, ICfgNode>> Items
		{
			get { return _source.Items; }
		}

		public IEnumerable<ICfgNode> ByName(string sectionName)
		{
			return _source.ByName(sectionName);
		}

		public T Deserialize<T>(IDeserializer context, ICfgNode cfgNode)
		{
			return _source.Deserialize<T>(context, cfgNode);
		}

		public T Combine<T>(ICombiner combiner, T x, T y)
		{
			return _source.Combine<T>(combiner, x, y);
		}
	}
}
