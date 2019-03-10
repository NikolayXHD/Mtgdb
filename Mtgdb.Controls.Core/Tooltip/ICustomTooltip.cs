using System;

namespace Mtgdb.Controls
{
	public interface ICustomTooltip
	{
		object Owner { get; }
		event Action<TooltipModel> Show;
		event Action Hide;

		void SubscribeEvents();
		void UnsubscribeEvents();
	}
}