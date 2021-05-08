using System;
using System.Collections.Generic;
using System.IO;

namespace Mtgdb
{
	public static class FsPathPersistence
	{
		public static void RegisterPathSubstitution(Func<FsPath, FsPath> substitution) =>
			_substitutions.Add(substitution);

		public static FsPath Deserialize(string value)
		{
			if (Path.DirectorySeparatorChar != '\\')
				value = value.Replace('\\', Path.DirectorySeparatorChar);
			if (Path.DirectorySeparatorChar != '/')
				value = value.Replace('/', Path.DirectorySeparatorChar);

			var result = new FsPath(value);

			foreach (var substitution in _substitutions)
				result = substitution(result);

			return result;
		}

		private static readonly HashSet<Func<FsPath, FsPath>> _substitutions =
			new HashSet<Func<FsPath, FsPath>>();
	}
}
