using System;

namespace Mtgdb.Controls
{
	public interface ICustomTooltip
	{
		event Action<TooltipModel> Show;
		event Action Hide;
	}
}