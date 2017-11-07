using System;
using System.Linq;
using Mtgdb.Dal.EditDistance;
using SpellChecker.Net.Search.Spell;

namespace Mtgdb.Dal.Index
{
	public class DamerauLevenstineDistance : StringDistance
	{
		private readonly LevenstineDistance _editDistance;

		public DamerauLevenstineDistance()
		{
			_editDistance = new LevenstineDistance();
		}

		public float GetSimilarity(string s1, string s2)
		{
			float prefixDistance = getPrefixDistance(s1, s2);

			var s1Rev = new string(s1.Reverse().ToArray());
			var s2Rev = new string(s2.Reverse().ToArray());

			var postfixDistance = getPrefixDistance(s1Rev, s2Rev);

			var lengthDistance = (s2.Length - s1.Length).WithinRange(0, 4) / 12f;

			float typos = lengthDistance + 0.5f * Math.Min(prefixDistance, postfixDistance + 0.001f);
			

			float maxTypos = getMaxTypos(s1.Length);

			float result = Math.Min(1f, 1.001f - 0.5f * typos / maxTypos);
			return result;
		}

		private static float getMaxTypos(int length)
		{
			return (0.35f * length).WithinRange(0.01f, 4f);
		}

		private float getPrefixDistance(string s1, string s2)
		{
			if (s1.Length > LevenstineDistance.MaxInput)
				s1 = s1.Substring(0, LevenstineDistance.MaxInput);

			if (s2.Length > LevenstineDistance.MaxDict)
				s2 = s2.Substring(0, LevenstineDistance.MaxDict);

			lock (_editDistance)
			{
				var prefixDistance = _editDistance.GetPrefixDistance(s1, s2);
				return prefixDistance;
			}
		}
	}
}