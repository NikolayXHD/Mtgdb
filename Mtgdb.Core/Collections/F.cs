namespace Mtgdb
{
	public static class F
	{
		public static bool IsNull<T>(T val) where T : class => val == null;
		public static bool IsNull<T>(T? val) where T : struct => val == null;
		public static bool IsNotNull<T>(T val) where T : class => val != null;
		public static bool IsNotNull<T>(T? val) where T : struct => val != null;
		public static bool False(bool val) => !val;
		public static bool True(bool val) => val;
	}
}