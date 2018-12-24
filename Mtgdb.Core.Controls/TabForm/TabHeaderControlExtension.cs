namespace Mtgdb.Controls
{
	public static class TabHeaderControlExtension
	{
		public static void ScaleDpi(this TabHeaderControl control)
		{
			control.Height = control.Height.ByDpiHeight();
			control.SlopeSize = control.SlopeSize.ByDpi();
			control.AddButtonSlopeSize = control.AddButtonSlopeSize.ByDpi();
			control.AddButtonWidth = control.AddButtonWidth.ByDpiWidth();
			control.ScaleDpiFont();
		}
	}
}