using System;

namespace Mtgdb
{
	public static class Runtime
	{
		public static bool IsMono =>
			_isMono ??= Type.GetType ("Mono.Runtime") != null;

		private static bool? _isMono;
	}
}
