using Mtgdb.Dal;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class EditDistanceTests
	{
		[TestCase("M", "M16")]
		public static void Substring_to_superstring_distance_equals_0(string  val1, string val2)
		{
			var distance = new DamerauLevenshteinDistance().GetPrefixDistance(val1, val2);
			Assert.That(distance, Is.EqualTo(0));
		}

		[TestCase("asd")]
		public static void Test_same_string_distance_equals_0(string val)
		{
			var distance = new DamerauLevenshteinDistance().GetPrefixDistance(val, val);
			Assert.That(distance, Is.EqualTo(0));
		}

		[TestCase("asd", "890", 6)]
		[TestCase("aaaaa", "bbb", 10)]
		public static void Test_normal_edit_costs_2(string val1, string val2, float expectedDistance)
		{
			var distance = new DamerauLevenshteinDistance().GetPrefixDistance(val1, val2);
			const float epsilon = 0.1f;
			Assert.That(distance, Is.InRange(expectedDistance - epsilon, expectedDistance + epsilon));
		}

		[TestCase("{{{", "[[[", 1.5f)]
		[TestCase(",,,,,", "<<<<<<<<<", 2.5f)]
		public static void Test_CAPS_keyboard_typo_costs_0_5(string val1, string val2, float expectedDistance)
		{
			var distance = new DamerauLevenshteinDistance().GetPrefixDistance(val1, val2);
			const float epsilon = 0.1f;
			Assert.That(distance, Is.InRange(expectedDistance - epsilon, expectedDistance + epsilon));
		}

		[TestCase("jjj", "ооо", 1.5f)]
		[TestCase("ббббб", "<<<<<<<<<", 2.5f)]
		public static void Test_Language_keyboard_typo_costs_0_5(string val1, string val2, float expectedDistance)
		{
			var distance = new DamerauLevenshteinDistance().GetPrefixDistance(val1, val2);
			const float epsilon = 0.1f;
			Assert.That(distance, Is.InRange(expectedDistance - epsilon, expectedDistance + epsilon));
		}

		[TestCase("{{{", "}}}", 4.5f)]
		[TestCase("<<<<<", ">>>>>>>>>", 7.5f)]
		public static void Test_near_keyboard_typo_costs_1_5(string val1, string val2, float expectedDistance)
		{
			var distance = new DamerauLevenshteinDistance().GetPrefixDistance(val1, val2);
			const float epsilon = 0.1f;
			Assert.That(distance, Is.InRange(expectedDistance - epsilon, expectedDistance + epsilon));
		}

		[TestCase("aaa", "ddd", 6f)]
		[TestCase("yyyyy", "nnnnnnnnnn", 10f)]
		public static void Test_by_far_keys_typo_costs_2(string val1, string val2, float expectedDistance)
		{
			var distance = new DamerauLevenshteinDistance().GetPrefixDistance(val1, val2);
			const float epsilon = 0.1f;
			Assert.That(distance, Is.InRange(expectedDistance - epsilon, expectedDistance + epsilon));
		}
		
		/// <summary>
		/// 'о' русская, которая далеко от нуля на клавиатуре
		/// </summary>
		[TestCase("000", "ооо", 3f)]
		[TestCase("lllll", "1111111111", 5f)]
		public static void Test_optycal_typo_costs_1(string val1, string val2, float expectedDistance)
		{
			var distance = new DamerauLevenshteinDistance().GetPrefixDistance(val1, val2);
			const float epsilon = 0.1f;
			Assert.That(distance, Is.InRange(expectedDistance - epsilon, expectedDistance + epsilon));
		}

		[TestCase("ааа", "яяя", 3f)]
		[TestCase("ллллл", "ммммммм", 5f)]
		public static void Test_acoustic_typo_costs_1(string val1, string val2, float expectedDistance)
		{
			var distance = new DamerauLevenshteinDistance().GetPrefixDistance(val1, val2);
			const float epsilon = 0.1f;
			Assert.That(distance, Is.InRange(expectedDistance - epsilon, expectedDistance + epsilon));
		}

		[TestCase("ююю", "ыыы", 4.5f)]
		[TestCase("vvvvv", "jjjjjjjjj", 7.5f)]
		public static void Test_far_acoustic_typo_costs_1_5(string val1, string val2, float expectedDistance)
		{
			var distance = new DamerauLevenshteinDistance().GetPrefixDistance(val1, val2);
			const float epsilon = 0.1f;
			Assert.That(distance, Is.InRange(expectedDistance - epsilon, expectedDistance + epsilon));
		}

		[TestCase("ююю", "uuu", 6f)]
		[TestCase("zzzzz", "жжжжжжжжж", 10f)]
		public static void Test_translit_typo_costs_2(string val1, string val2, float expectedDistance)
		{
			var distance = new DamerauLevenshteinDistance().GetPrefixDistance(val1, val2);
			const float epsilon = 0.1f;
			Assert.That(distance, Is.InRange(expectedDistance - epsilon, expectedDistance + epsilon));
		}

		[TestCase("абвгд", "бавдг", 2f)]
		public static void Test_swap_typo_costs_1(string val1, string val2, float expectedDistance)
		{
			var distance = new DamerauLevenshteinDistance().GetPrefixDistance(val1, val2);
			const float epsilon = 0.1f;
			Assert.That(distance, Is.InRange(expectedDistance - epsilon, expectedDistance + epsilon));
		}
	}
}
