using System.Collections.Generic;

namespace NConfiguration.Json.Parsing
{
	public sealed class JArray: JValue
	{
		public List<JValue> Items { get; private set; }

		public JArray()
		{
			Items = new List<JValue>();
		}

		public override TokenType Type
		{
			get
			{
				return TokenType.Array;
			}
		}
	}
}
