using System.Collections.Generic;
using System.Linq;

namespace Mtgdb
{
	public static class Cjk
	{
		public static bool IsCjk(this string s)
		{
			return s.Any(IsCjk);
		}

		/// <summary>
		/// chinese japanese korean
		/// </summary>
		public static bool IsCjk(this char c)
		{
			var rangeIndex = _cjkCharacterRanges.BinarySearchFirstIndexOf(r => r.Max >= c);

			if (rangeIndex < 0)
				return false;

			if (_cjkCharacterRanges[rangeIndex].Min <= c)
				return true;

			return false;
		}

		/// <summary>
		/// https://en.wikipedia.org/wiki/Unicode_block
		/// </summary>
		private static readonly List<CharRange> _cjkCharacterRanges = new List<CharRange>
		{
			new CharRange('\u1100', '\u11FF'), // Hangul Jamo
			new CharRange('\u2E80', '\u2EFF'), // CJK Radicals Supplement

			// new CharRange('\u3000', '\u303F'), // CJK Symbols and Punctuation
			// new CharRange('\u3040', '\u309F'), // Hiragana
			// new CharRange('\u30A0', '\u30FF'), // Katakana
			// new CharRange('\u3100', '\u312F'), // Bopomofo
			// new CharRange('\u3130', '\u318F'), // Hangul Compatibility Jamo
			// new CharRange('\u3190', '\u319F'), // Kanbun
			// new CharRange('\u31A0', '\u31BF'), // Bopomofo Extended
			// new CharRange('\u31C0', '\u31EF'), // CJK Strokes
			// new CharRange('\u31F0', '\u31FF'), // Katakana Phonetic Extensions
			// new CharRange('\u3200', '\u32FF'), // Enclosed CJK Letters and Months
			// new CharRange('\u3300', '\u33FF'), // CJK Compatibility
			// new CharRange('\u3400', '\u4DBF'), // CJK Unified Ideographs ExtensionA
			// new CharRange('\u4DC0', '\u4DFF'), // Yijing Hexagram Symbols
			// new CharRange('\u4E00', '\u9FFF'), // CJK Unified Ideographs
			new CharRange('\u3000', '\u9FFF'), // merged 14 adjacent spans from above
			
			new CharRange('\uA960', '\uA97F'), // Hangul Jamo Extended-A

			// new CharRange('\uAC00', '\uD7AF'), // Hangul Syllables
			// new CharRange('\uD7B0', '\uD7FF'), // Hangul Jamo Extended-B
			new CharRange('\uAC00', '\uD7FF'), // merged 2 adjacent spans from above


			new CharRange('\uF900', '\uFAFF'), // CJK Compatibility Ideographs
			new CharRange('\uFE30', '\uFE4F'), // CJK Compatibility Forms
			new CharRange('\uFF00', '\uFFEF') // Halfwidth and Fullwidth Forms
		};

		private struct CharRange
		{
			public readonly char Min;
			public readonly char Max;

			public CharRange(char min, char max)
			{
				Min = min;
				Max = max;
			}
		}
	}
}