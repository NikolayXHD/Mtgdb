using System.ComponentModel;

namespace Mtgdb.Dal
{
	public class UiConfig
	{
		[DefaultValue(DefaultUiScalePercent)]
		public int UiScalePercent { get; set; } = DefaultUiScalePercent;
		
		[DefaultValue(true)]
		public bool DisplaySmallImages { get; set; } = true;

		public const int DefaultUiScalePercent = 100;
	}
}