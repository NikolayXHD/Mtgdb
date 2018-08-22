using System.Globalization;
using System.Reflection;
using System.Threading;

namespace Mtgdb
{
	public static class ApplicationCulture
	{
		public static void SetCulture(CultureInfo culture)
		{
			var userDefaultCultureProperty = typeof(CultureInfo).GetField("s_userDefaultCulture", BindingFlags.Static | BindingFlags.NonPublic);
			var userDefaultUICultureProperty = typeof(CultureInfo).GetField("s_userDefaultUICulture", BindingFlags.Static | BindingFlags.NonPublic);

			userDefaultCultureProperty.SetValue(null, culture);
			userDefaultUICultureProperty.SetValue(null, culture);

			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}
	}
}