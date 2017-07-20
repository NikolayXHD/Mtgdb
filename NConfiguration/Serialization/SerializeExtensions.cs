using System.Collections.Generic;
using System.Linq;

namespace NConfiguration.Serialization
{
	public static class SerializeExtensions
	{
		public static T Deserialize<T>(this IDeserializer deserializer, ICfgNode cfgNode)
		{
			return deserializer.Deserialize<T>(deserializer, cfgNode);
		}

		/// <summary>
		/// Returns the collection of child nodes with the specified name or empty if no match is found.
		/// </summary>
		/// <param name="name">node name is not case-sensitive.</param>
		/// <returns>Returns the collection of child nodes with the specified name or empty if no match is found.</returns>
		public static IEnumerable<ICfgNode> NestedByName(this ICfgNode parent, string name)
		{
			return parent.Nested
				.Where(p => NameComparer.Equals(p.Key, name))
				.Select(p => p.Value);
		}
	}
}
