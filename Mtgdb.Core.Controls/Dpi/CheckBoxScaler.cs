namespace Mtgdb.Controls
{
	public static class CheckBoxScaler
	{
		public static void ScaleDpi(this CheckBox button)
		{
			button.ScaleDpiImages();
			button.ScaleDpiFont();
			button.ScaleDpiMargin();
			button.ScaleDpiPadding();
		}
	}
}