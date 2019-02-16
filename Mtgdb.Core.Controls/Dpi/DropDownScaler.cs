using Mtgdb.Controls.Properties;

namespace Mtgdb.Controls
{
	public static class DropDownScaler
	{
		public static void ScaleDpi(this DropDown dropDown)
		{
			DropDownBaseScaler.ScaleDpi(dropDown);

			new DpiScaler<DropDown>(cb =>
				{
					cb.ButtonImages = ButtonImages.ScaleDpi((null, Resources.drop_down_48));
				})
				.Setup(dropDown);
		}
	}
}