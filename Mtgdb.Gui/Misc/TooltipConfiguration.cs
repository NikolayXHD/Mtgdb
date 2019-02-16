using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Mtgdb.Controls;
using Ninject;

namespace Mtgdb.Gui
{
	public class TooltipConfiguration
	{
		private readonly TooltipForm _defaultTooltip;
		private readonly TooltipForm _quickFilterTooltip;

		public TooltipConfiguration(
			[Named(GuiModule.DefaultTooltipScope)] TooltipForm defaultTooltip,
			[Named(GuiModule.QuickFilterTooltipScope)] TooltipForm quickFilterTooltip)
		{
			_defaultTooltip = defaultTooltip;
			_quickFilterTooltip = quickFilterTooltip;
		}

		public void Setup()
		{
			_quickFilterTooltip.BackColor = SystemColors.Window;
			_quickFilterTooltip.TooltipBorderStyle = DashStyle.Solid;
			_quickFilterTooltip.TextPadding = new Padding(1, 1, 1, 1);
			_quickFilterTooltip.VisibleBorders = AnchorStyles.None;

			// if TooltipMargin == 0 the tooltip shares 1 pixel border line with target
			// placing mouse there makes tooltip flicker
			_quickFilterTooltip.TooltipMargin = 1;

			_quickFilterTooltip.ScaleDpi();
			_defaultTooltip.ScaleDpi();
		}

		public void SetupQuickFilterTooltipController(TooltipController controller)
		{
			controller.DelayMs = 0;
			controller.ToggleOnAlt = false;
		}
	}
}