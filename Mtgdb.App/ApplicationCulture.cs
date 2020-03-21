using System.Globalization;
using System.Threading;

namespace Mtgdb
{
	public static class ApplicationCulture
	{
		public static void SetCulture(CultureInfo culture)
		{
			CultureInfo.DefaultThreadCurrentCulture = culture;
			CultureInfo.DefaultThreadCurrentUICulture = culture;
			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}
	}
}
