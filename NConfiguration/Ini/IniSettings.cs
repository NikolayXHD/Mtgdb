using System.Collections.Generic;
using NConfiguration.Serialization;

namespace NConfiguration.Ini
{
	public abstract class IniSettings : CachedConfigNodeProvider
	{
		protected abstract IEnumerable<Section> Sections { get; }

		protected override IEnumerable<KeyValuePair<string, ICfgNode>> GetAllNodes()
		{
			foreach (var section in Sections)
			{
				if (section.Name == string.Empty)
					foreach (var pair in section.Pairs)
						yield return new KeyValuePair<string, ICfgNode>(pair.Key, new ViewPlainField(pair.Value));
				else
					yield return new KeyValuePair<string, ICfgNode>(section.Name, new ViewSection(section));
			}
		}
	}
}
