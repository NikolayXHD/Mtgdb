namespace Mtgdb.Gui
{
	public class UiConfig
	{
		public UiConfig() =>
			UiScalePercent = DefaultUiScalePercent;

		public int UiScalePercent { get; set; }

		public const int DefaultUiScalePercent = 100;
	}
}