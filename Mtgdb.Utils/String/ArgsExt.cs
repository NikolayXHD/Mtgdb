using System.Collections.Generic;

namespace Mtgdb
{
	public static class ArgsExt
	{
		public static bool GetFlag(this IList<string> argsList, string name)
		{
			return argsList.Contains(name);
		}

		public static string GetParam(this IList<string> argsList, string name)
		{
			string value;

			int argIndex = argsList.IndexOf(name);
			if (argIndex == -1 || argIndex + 1 >= argsList.Count)
				value = null;
			else
				value = argsList[argIndex + 1];

			return value;
		}
	}
}