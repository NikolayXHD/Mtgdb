using System;
using System.Collections.Generic;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	internal class Evaluators : List<KeyValuePair<int, Func<Card, bool>>>
	{
		public void Add(int index, Func<Card, bool> value)
		{
			Add(new KeyValuePair<int, Func<Card, bool>>(index, value));
		}
	}
}