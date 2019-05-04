using System;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class ExtremumFinderTests
	{
		[TestCase(1, 3, 2, ExpectedResult = 3)]
		public int Max_is_found_correctly(params int[] values) =>
			values.AtMax(F.Self).Find();

		[TestCase(1, 3, 2, ExpectedResult = 1)]
		public int Min_is_found_correctly(params int[] values) =>
			values.AtMin(F.Self).Find();

		[TestCase( /* selectMinusValue */ false, 1, 2, ExpectedResult = 2)]
		[TestCase( /* selectMinusValue */ true, 1, 2, ExpectedResult = 1)]
		public int Max_applies_value_selector(bool selectMinusValue, params int[] values) =>
			selectMinusValue
				? values.AtMax(v => -v).Find()
				: values.AtMax(F.Self).Find();

		[TestCase( /* selectMinusValue */ false, 1, 2, ExpectedResult = 1)]
		[TestCase( /* selectMinusValue */ true, 1, 2, ExpectedResult = 2)]
		public int Min_applies_value_selector(bool selectMinusValue, params int[] values) =>
			selectMinusValue
				? values.AtMin(v => -v).Find()
				: values.AtMin(F.Self).Find();

		[TestCase(ExpectedResult = null)]
		[TestCase("a", "b", ExpectedResult = "b")]
		public string Max_returns_default_on_empty_enumerable(params string[] values) =>
			values.AtMax(F.Self).FindOrDefault();

		[TestCase(ExpectedResult = null)]
		[TestCase("a", "b", ExpectedResult = "a")]
		public string Min_returns_default_on_empty_enumerable(params string[] values) =>
			values.AtMin(F.Self).FindOrDefault();

		[TestCase(/* customComparer */ false, "a", "b", ExpectedResult = "b")]
		[TestCase(/* customComparer */ true, "a", "b", ExpectedResult = "a")]
		public string Max_supports_custom_comparer(bool customComparer, params string[] values) =>
			customComparer
				? values.AtMax(F.Self, new CustomComparer<string>((v1, v2) => -string.Compare(v1, v2, StringComparison.Ordinal))).Find()
				: values.AtMax(F.Self).Find();

		[TestCase(/* customComparer */ false, "a", "b", ExpectedResult = "a")]
		[TestCase(/* customComparer */ true, "a", "b", ExpectedResult = "b")]
		public string Min_supports_custom_comparer(bool customComparer, params string[] values) =>
			customComparer
				? values.AtMin(F.Self, new CustomComparer<string>((v1, v2) => -string.Compare(v1, v2, StringComparison.Ordinal))).Find()
				: values.AtMin(F.Self).Find();

		// odd -> even, then smallest
		[TestCase(2, 3, 4, 5, 6, ExpectedResult = 3)]
		public int Then_at_min_is_second_priority(params int[] values) =>
			values.AtMin(v => v % 2 == 0).ThenAtMin(F.Self).Find();

		// odd -> even, then largest
		[TestCase(2, 3, 4, 5, 6, ExpectedResult = 5)]
		public int Then_at_max_is_second_priority(params int[] values) =>
			values.AtMin(v => v % 2 == 0).ThenAtMax(F.Self).Find();
	}
}