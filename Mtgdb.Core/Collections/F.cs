namespace Mtgdb
{
	public static class F
	{
		public static bool NotNull<T>(T val) where T : class => val != null;
		public static bool False(bool val) => !val;
		public static bool True(bool val) => val;
	}
}