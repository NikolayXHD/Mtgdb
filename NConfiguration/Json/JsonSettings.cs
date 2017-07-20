using System.Collections.Generic;
using NConfiguration.Json.Parsing;

namespace NConfiguration.Json
{
	public abstract class JsonSettings : CachedConfigNodeProvider
	{
		protected abstract JObject Root { get; }

		protected override IEnumerable<KeyValuePair<string, ICfgNode>> GetAllNodes()
		{
			foreach (var pair in Root.Properties)
				foreach (var item in ViewObject.FlatArray(pair.Value))
					yield return new KeyValuePair<string, ICfgNode>(pair.Key, ViewObject.CreateByJsonValue(item));
		}
	}
}
