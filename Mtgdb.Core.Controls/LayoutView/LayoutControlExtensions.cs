namespace Mtgdb.Controls
{
	public static class LayoutControlExtensions
	{
		public static void ScaleDpi(this LayoutControl control)
		{
			foreach (var field in control.Fields)
				field.ScaleDpiFont();

			ControlHelpers.ScaleDpi(control);
		}
	}
}