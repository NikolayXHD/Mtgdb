using System;
using System.Collections.Generic;
using NConfiguration.Serialization;

namespace NConfiguration.Ini
{
	/// <summary>
	/// The mapping section in INI-document to nodes of configuration
	/// </summary>
	public sealed class ViewSection: CfgNode
	{
		private List<KeyValuePair<string, string>> _pairs;

		/// <summary>
		/// The mapping INI-document to nodes of configuration
		/// </summary>
		/// <param name="converter">string converter into a simple values</param>
		/// <param name="section">section in INI-document</param>
		public ViewSection(Section section)
		{
			_pairs = section.Pairs;
		}

		public override string GetNodeText()
		{
			throw new NotSupportedException("section can't contain value");
		}

		public override IEnumerable<KeyValuePair<string, ICfgNode>> GetNestedNodes()
		{
			foreach (var pair in _pairs)
				yield return new KeyValuePair<string, ICfgNode>(pair.Key, new ViewPlainField(pair.Value));
		}
	}
}

