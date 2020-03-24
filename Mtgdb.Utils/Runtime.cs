using System;
using System.Linq;

namespace Mtgdb
{
	public static class Runtime
	{
		public static bool IsMono =>
			_isMono ??= Type.GetType ("Mono.Runtime") != null;

		public static bool IsLinux =>
			_isLinux ??= _linuxPlatformIds.Contains((int) Environment.OSVersion.Platform);


		private static bool? _isMono;
		private static bool? _isLinux;

		private static readonly int[] _linuxPlatformIds = { 4, 6, 128 };
	}
}
