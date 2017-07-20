using System;
using System.Collections.Generic;
using System.Linq;

namespace NConfiguration
{
	public abstract class CfgNode: ICfgNode
	{
		private readonly Lazy<string> _textLazy;
		private readonly Lazy<IEnumerable<KeyValuePair<string, ICfgNode>>> _nestedNodes;

		protected CfgNode()
		{
			_textLazy = new Lazy<string>(GetNodeText);
			_nestedNodes = new Lazy<IEnumerable<KeyValuePair<string, ICfgNode>>>(copyNestedNodes);
		}

		private IEnumerable<KeyValuePair<string, ICfgNode>> copyNestedNodes()
		{
			return GetNestedNodes().ToList().AsReadOnly();
		}

		public abstract string GetNodeText();

		public abstract IEnumerable<KeyValuePair<string, ICfgNode>> GetNestedNodes();

		public string Text
		{
			get
			{
				return _textLazy.Value;
			}
		}

		public IEnumerable<KeyValuePair<string, ICfgNode>> Nested
		{
			get
			{
				return _nestedNodes.Value;
			}
		}
	}
}

