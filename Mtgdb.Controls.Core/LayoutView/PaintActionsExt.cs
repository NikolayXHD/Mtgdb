using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	internal static class PaintActionsExt
	{
		public static void Paint(this IEnumerable<Action<PaintEventArgs>> actions, PaintEventArgs e)
		{
			foreach (var action in actions)
				action(e);
		}
	}
}