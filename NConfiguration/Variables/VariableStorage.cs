using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NConfiguration.Serialization;

namespace NConfiguration.Variables
{
	public class VariableStorage : IVariableStorage
	{
		private readonly Dictionary<string, string> _map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		public string this[string name]
		{
			get
			{
				string value;
				if (_map.TryGetValue(name, out value))
					return value;

				throw new InvalidOperationException(string.Format("variable '{0}' not found", name));
			}
			set
			{
				if (_map.ContainsKey(name))
					return;

				_map.Add(name, value);
			}
		}

		public ICfgNode CfgNodeConverter(string name, ICfgNode candidate)
		{
			if (!NameComparer.Equals(name, "variable"))
				return new CfgNodeWrapper(candidate, this);
			
			var varConfig = DefaultDeserializer.Instance.Deserialize<VariableConfig>(candidate);
			this[varConfig.Name] = varConfig.Value;
			return null;
		}

		internal class VariableConfig
		{
			[DataMember(Name = "Name", IsRequired = true)]
			public string Name { get; set; }

			[DataMember(Name = "Value", IsRequired = true)]
			public string Value { get; set; }
		}
	}
}
