using Mtgdb.Controls.Properties;

namespace Mtgdb.Controls
{
	public static class CheckBoxScaler
	{
		public static void ScaleDpi(this CheckBox checkBox)
		{
			checkBox.ScaleDpiFont();
			checkBox.ScaleDpiPadding();

			new DpiScaler<CheckBox>(b => 
					b.ButtonImages = ButtonImages.ScaleDpi(
						(null, Resources.unchecked_32),
						(null, Resources.checked_32)))
				.Setup(checkBox);
		}
	}
}