using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mtgdb.Dal
{
	public class ForgeSetRepository
	{
		public void Load()
		{
			var mappings = File.ReadAllLines(AppDir.Data.AddPath("forge_set_mapping.txt"))
				.Select(_ => _.Split('\t'))
				.ToArray();

			_setByForgeSet = mappings
				.ToDictionary(_ => _[0], _ => _[1]);

			_forgeSetBySet = mappings
				.ToDictionary(_ => _[1], _ => _[0]);
		}

		public void EnsureLoaded()
		{
			if (_setByForgeSet != null)
				return;

			Load();
		}

		public string ToForgeSet(string set)
		{
			return _forgeSetBySet.TryGet(set) ?? set;
		}

		public string FromForgeSet(string forgeSet)
		{
			return _setByForgeSet.TryGet(forgeSet) ?? forgeSet;
		}

		private Dictionary<string, string> _setByForgeSet;
		private Dictionary<string, string> _forgeSetBySet;
	}
}