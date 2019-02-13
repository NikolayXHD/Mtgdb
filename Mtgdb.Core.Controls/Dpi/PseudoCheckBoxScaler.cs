using Mtgdb.Controls.Properties;

namespace Mtgdb.Controls
{
	public static class PseudoCheckBoxScaler
	{
		public static void ScaleDpi(this PseudoCheckBox checkBox)
		{
			checkBox.Box.ScaleDpiFont();
			checkBox.Box.ScaleDpiPadding();

			new DpiScaler<PseudoCheckBox>(b => 
					b.ButtonSubsystem.SetupButton(b.Box, ButtonImages.ScaleDpi(
						(null, Resources.unchecked_32),
						(null, Resources.checked_32))))
				.Setup(checkBox);
		}
	}
}