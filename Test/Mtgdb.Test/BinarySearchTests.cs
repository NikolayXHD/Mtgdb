using System.Collections.Generic;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class BinarySearchTests
	{
		[TestCase(
			ExpectedResult = -1)]
		[TestCase(false,
			ExpectedResult = -1)]
		[TestCase(false, false,
			ExpectedResult = -1)]
		[TestCase(true,
			ExpectedResult = 0)]
		[TestCase(true, true,
			ExpectedResult = 0)]
		[TestCase(false, true,
			ExpectedResult = 1)]
		[TestCase(false, true, true,
			ExpectedResult = 1)]
		[TestCase(false, false, false, true, true, true,
			ExpectedResult = 3)]
		public int First_index_result_is_expected(params bool[] input) =>
			((IReadOnlyList<bool>) input).BinarySearchFirstIndex(_ => _);

		[TestCase(
			ExpectedResult = -1)]
		[TestCase(true,
			ExpectedResult = 0)]
		[TestCase(true, true,
			ExpectedResult = 1)]
		[TestCase(false,
			ExpectedResult = -1)]
		[TestCase(false, false,
			ExpectedResult = -1)]
		[TestCase(true, false,
			ExpectedResult = 0)]
		[TestCase(true, false, false,
			ExpectedResult = 0)]
		[TestCase(true, true, true, false, false, false,
			ExpectedResult = 2)]
		public int Last_index_result_is_expected(params bool[] input) =>
			((IReadOnlyList<bool>) input).BinarySearchLastIndex(_ => _);
	}
}
