﻿using Mtgdb.Dal.Index;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class EditDistanceTests : TestsBase
	{
		[TestCase("vinira", "nevinyrral")]
		public static void Substring_to_superstring_distance_equals_0(string  val1, string val2)
		{
			var distance = new DamerauLevenstineDistance().GetDistance(val1, val2);
			Log.Debug($"{val1} -> {val2}: {distance:F4}");
		}
	}
}
