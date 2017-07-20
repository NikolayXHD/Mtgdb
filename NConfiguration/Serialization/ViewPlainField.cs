using System.Collections.Generic;

namespace NConfiguration.Serialization
{
	/// <summary>
	/// Representation of a simple text value.
	/// </summary>
	public sealed class ViewPlainField: ICfgNode
	{
		/// <summary>
		/// Representation of a simple text value.
		/// </summary>
		/// <param name="converter">string converte</param>
		/// <param name="text">text value</param>
		public ViewPlainField(string text)
		{
			Text = text;
		}

		public string Text { get; private set; }

		/// <summary>
		/// Return empty collection
		/// </summary>
		public IEnumerable<KeyValuePair<string, ICfgNode>> Nested
		{
			get
			{
				yield break;
			}
		}
	}
}

