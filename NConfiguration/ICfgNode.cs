using System.Collections.Generic;

namespace NConfiguration
{
	/// <summary>
	/// A node in the document of configuration
	/// </summary>
	public interface ICfgNode
	{
		/// <summary>
		/// Gets all the child nodes with their names.
		/// </summary>
		IEnumerable<KeyValuePair<string, ICfgNode>> Nested { get; }

		/// <summary>
		/// Contain text value in this node.
		/// </summary>
		string Text { get; }
	}
}
