using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Mtgdb.Util
{
	internal static class ReplaceCost
	{
		public const char EndOfWord = ' ';

		private static readonly Dictionary<char, HashSet<int>>[] _soundGroupsByChar =
		{
			toGroupByChar("ae,iye,iyu,o,,bp,,ckqx,,dt,,l,r,m,n,,fv,gj,sxz"),
			toGroupByChar("ая,ао,оё,эеё,ую,ыий,,лмн,,кгх,йцсзжчшщ,дтц,бпфв,,р,,ъь")
		};

		private static readonly Dictionary<char, HashSet<int>> _opticalGroupsByChar =
			toGroupByChar("0oо,1li,3з,2z,4ч,5s,б6,8bвg,9gд, _:-\t\u00ad");

		private static readonly Dictionary<char, HashSet<int>> _translitGroupsByChar = getTranslitGroupsByChar(new[]
		{
			// ru en
			// v  v
			// en ru
			"abcdefghijklmnopqrstuvwxyz абвгдеёжзийклмнопрстуфхцчшщьыъэюя",
			"абсдефгхижклмнопкрстуввэыз abvgdyyzziyklmnoprstufhtcss'y'eyy",
			"эбсдифжэажклмну ьа  ю уки  u  u eoh e'c   e  c opkshhcy yaua",
			"й к    чйие     ю      са     h ae        i     hhctch      "
		});

		private static readonly Dictionary<char, HashSet<Point>> _keyboardLocationsByChar = getKeyboardLocationsByChar(
			@"`1234567890-= 
 qwertyuiop[]\
 asdfghjkl;'  
 zxcvbnm,./   ",
			@"~!@#$%^&*()_+ 
 QWERTYUIOP{}|
 ASDFGHJKL:""  
 ZXCVBNM<>?   ",
			@"ё1234567890-= 
 йцукенгшщзхъ\
 фывапролджэ  
 ячсмитьбю.   ",
			@"Ё!""№;%:?*()_+ 
 ЙЦУКЕНГШЩЗХЪ
 ФЫВАПРОЛДЖЭ/  
 ЯЧСМИТЬБЮ,   ");

		public static float Get(char c1, char c2)
		{
			c1 = char.ToLower(c1, Str.Culture);
			c2 = char.ToLower(c2, Str.Culture);

			if (c1 == c2)
				return 0;

			if (c1 == EndOfWord || c2 == EndOfWord)
				return Similarity.High;

			// Даёт High

			// пока индекс не учитывает такую возможность, в этом нет смысла
			//var translitCost = translitReplaceCost(c1, c2);
			//if (translitCost <= Similarity.High)
			//	return translitCost;

			// Даёт High, Light
			var keyboardCost = keyboardReplaceCost(c1, c2);
			if (keyboardCost <= Similarity.Normal)
				return keyboardCost;

			// Даёт Normal
			var opticalCost = opticalReplaceCost(c1, c2);
			if (opticalCost <= Similarity.Normal)
				return opticalCost;

			// Даёт Normal, Light
			var soundCost = soundReplaceCost(c1, c2);

			float result = Math.Min(keyboardCost, soundCost);

			return result;
		}

		private static float soundReplaceCost(char c1, char c2)
		{
			var result = Enumerable.Range(0, _soundGroupsByChar.Length)
				.Min(i => soundReplaceCost(c1, c2, _soundGroupsByChar[i]));

			return result;
		}

		private static float soundReplaceCost(char c1, char c2, Dictionary<char, HashSet<int>> soundGroupsByChar)
		{
			if (!soundGroupsByChar.TryGetValue(c1, out var c1Groups))
				return Similarity.None;

			if (!soundGroupsByChar.TryGetValue(c2, out var c2Groups))
				return Similarity.None;

			// Входят в одну фонетическую группу
			if (c1Groups.Overlaps(c2Groups))
				return Similarity.Normal;

			// Входят в соседние фонетические группы
			if (c1Groups.Any(gr1 => c2Groups.Any(gr2 => Math.Abs(gr1 - gr2) <= 1)))
				return Similarity.Light;

			return Similarity.None;
		}

		private static float opticalReplaceCost(char c1, char c2)
		{
			if (!_opticalGroupsByChar.TryGetValue(c1, out var c1Groups))
				return Similarity.None;

			if (!_opticalGroupsByChar.TryGetValue(c2, out var c2Groups))
				return Similarity.None;

			// Входят в одну оптическую группу
			if (c1Groups.Overlaps(c2Groups))
				return Similarity.Normal;

			return Similarity.None;
		}

		private static float translitReplaceCost(char c1, char c2)
		{
			if (!_translitGroupsByChar.TryGetValue(c1, out var c1TranslitGroups))
				return Similarity.None;

			if (!_translitGroupsByChar.TryGetValue(c2, out var c2TranslitGroups))
				return Similarity.None;

			if (c1TranslitGroups.Overlaps(c2TranslitGroups))
				return Similarity.High;

			return Similarity.None;
		}

		private static float keyboardReplaceCost(char c1, char c2)
		{
			if (!_keyboardLocationsByChar.TryGetValue(c1, out var c1KeyboardLocations))
				return Similarity.None;

			if (!_keyboardLocationsByChar.TryGetValue(c2, out var c2KeyboardLocations))
				return Similarity.None;

			float minDistance = float.MaxValue;

			foreach (var c1Location in c1KeyboardLocations)
				foreach (var c2Location in c2KeyboardLocations)
				{
					int keyboardDistance = Math.Max(
						Math.Abs(c1Location.X - c2Location.X),
						Math.Abs(c1Location.Y - c2Location.Y));

					float dist;

					switch (keyboardDistance)
					{
						case 0:
							return Similarity.High;
						case 1:
							dist = Similarity.Light;
							break;
						default:
							dist = Similarity.None;
							break;
					}

					if (dist < minDistance)
						minDistance = dist;
				}

			return minDistance;
		}

		private static Dictionary<char, HashSet<int>> toGroupByChar(string groups)
		{
			var parts = groups.Split(Array.From(','), StringSplitOptions.None);
			var result = Enumerable.Range(0, parts.Length)
				.Select(i => new { index = i, chars = parts[i] })
				.SelectMany(_ => _.chars.Select(c => new { chr = c, _.index }))
				.GroupBy(_ => _.chr)
				.ToDictionary(
					_ => _.Key,
					_ => new HashSet<int>(_.Select(x => x.index)));

			return result;
		}

		private static Dictionary<char, HashSet<int>> getTranslitGroupsByChar(string[] alphabets)
		{
			var result = Enumerable.Range(0, alphabets.Length)
				.SelectMany(j =>
					Enumerable.Range(0, alphabets[j].Length)
						.Where(i => alphabets[j][i] != ' ')
						.Select(i => new { i, ch = alphabets[j][i] }))
				.GroupBy(_ => _.ch)
				.ToDictionary(
					gr => gr.Key,
					gr => new HashSet<int>(gr.Select(_ => _.i)));

			return result;
		}

		private static Dictionary<char, HashSet<Point>> getKeyboardLocationsByChar(params string[] keyboardLayouts)
		{
			var result = Enumerable.Range(0, keyboardLayouts.Length)
				.SelectMany(l =>
				{
					var lines = keyboardLayouts[l].Split(
						Array.From(Str.Endl),
						StringSplitOptions.RemoveEmptyEntries);

					return Enumerable
						.Range(0, lines.Length)
						.SelectMany(y => Enumerable
							.Range(0, lines[y].Length)
							.Where(x => lines[y][x] != ' ')
							.Select(x => new
							{
								location = new Point(x, y),
								ch = lines[y][x]
							})
						);
				})
				.GroupBy(_ => _.ch)
				.ToDictionary(
					gr => gr.Key,
					gr => new HashSet<Point>(gr.Select(_ => _.location)));

			return result;
		}
	}
}