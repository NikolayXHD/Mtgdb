using System.Globalization;
using System.Reflection;
using System.Threading;

namespace Mtgdb.Gui
{
	public static class ApplicationCulture
	{
		public static void SetCulture(CultureInfo culture)
		{
			var userDefaltCultureProperty = typeof(CultureInfo).GetField("s_userDefaultCulture", BindingFlags.Static | BindingFlags.NonPublic);
			var userDefaltUICultureProperty = typeof(CultureInfo).GetField("s_userDefaultUICulture", BindingFlags.Static | BindingFlags.NonPublic);

			userDefaltCultureProperty.SetValue(null, culture);
			userDefaltUICultureProperty.SetValue(null, culture);


			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}
	}
}