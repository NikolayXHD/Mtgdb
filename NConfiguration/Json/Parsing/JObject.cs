using System.Collections.Generic;
using System.Linq;

namespace NConfiguration.Json.Parsing
{
	public sealed class JObject : JValue
	{
		public List<KeyValuePair<string, JValue>> Properties { get; private set; }

		public JObject()
		{
			Properties = new List<KeyValuePair<string, JValue>>();
		}

		public JValue this[string name]
		{
			get
			{
				return Properties
					.Where(p => p.Key == name)
					.Select(p => p.Value)
					.FirstOrDefault();
			}
		}

		public override TokenType Type
		{
			get
			{
				return TokenType.Object;
			}
		}
	}
}
