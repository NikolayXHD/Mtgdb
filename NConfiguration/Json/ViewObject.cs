using System;
using System.Collections.Generic;
using NConfiguration.Serialization;
using NConfiguration.Json.Parsing;

namespace NConfiguration.Json
{
	/// <summary>
	/// The mapping JSON-document to nodes of configuration
	/// </summary>
	public sealed class ViewObject : CfgNode
	{
		private JObject _obj;

		/// <summary>
		/// The mapping JSON-document to nodes of configuration
		/// </summary>
		/// <param name="converter">string converter into a simple values</param>
		/// <param name="obj">JSON object</param>
		public ViewObject(JObject obj)
		{
			_obj = obj;
		}

		internal static ICfgNode CreateByJsonValue(JValue val)
		{
			if (val == null)
				return null;

			switch (val.Type)
			{
				case TokenType.Null:
					return new ViewPlainField(null);

				case TokenType.Object:
					return new ViewObject((JObject)val);

				case TokenType.String:
				case TokenType.Boolean:
				case TokenType.Number:
					return new ViewPlainField(val.ToString());
				
				default:
					throw new NotSupportedException(string.Format("JSON type {0} not supported", val.Type));
			}
		}

		internal static IEnumerable<JValue> FlatArray(JValue val)
		{
			if (val == null)
				yield break;
			
			if (val.Type != TokenType.Array)
			{
				yield return val;
				yield break;
			}

			foreach (var item in ((JArray)val).Items)
			{
				foreach (var innerItem in FlatArray(item)) //HACK: remove recursion
				{
					yield return innerItem;
				}
			}
		}

		public override string GetNodeText()
		{
			throw new NotSupportedException("JSON document can't contain value");
		}

		public override IEnumerable<KeyValuePair<string, ICfgNode>> GetNestedNodes()
		{
			foreach (var el in _obj.Properties)
				foreach (var val in FlatArray(el.Value))
					yield return new KeyValuePair<string, ICfgNode>(el.Key, CreateByJsonValue(val));
		}
	}
}
