using Mtgdb.Dal.Index;
using NLog;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class EditDistanceTests : TestsBase
	{
		[TestCase("vinira", "nevinyrral")]
		[TestCase("thalia", "thalia's lieutenant")]
		public void Substring_to_superstring_distance_equals_0(string  val1, string val2)
		{
			var distance = new DamerauLevenstineSimilarity().GetSimilarity(val1, val2);
			Log.Info($"{val1} -> {val2}: {distance:F4}");
		}
	}
}
