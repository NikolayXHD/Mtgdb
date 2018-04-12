using System;

namespace Mtgdb.Dal.Index
{
	public class DamerauLevenshteinSimilarity : IStringSimilarity
	{
		private readonly DamerauLevenshteinDistance _editDistance;

		public DamerauLevenshteinSimilarity()
		{
			_editDistance = new DamerauLevenshteinDistance();
		}

		public float GetSimilarity(string s1, string s2)
		{
			float minDist = float.MaxValue;
			var prefixes = (s2.Length - s1.Length + 1).WithinRange(1, null);

			for (int l = 0; l < prefixes; l++)
			{
				const float deepness = 1.6f;
				int minDeep = 2;

				int r = Math.Min(s2.Length, l + Math.Max(minDeep, (int) Math.Round(deepness * s1.Length)));
				var prefixDistance = getPrefixDistance(s1, s2.Substring(l, r - l)).PrefixDistance;

				// value of 1 means similarity(ab, zzzzzab) = similarity(ab, zabzzzz)
				float prefixPreference = 1f;

				// positionTerm favors prefixes and suffixes
				// over infixes
				// ^          p = prefixes
				// |     + +
				// |   +     +
				// | +         + + prefixPreference
				// +-----+-----+--------------->
				// 0     p/2   p               l
				float positionTerm = 0.001f * Math.Min(l, prefixPreference + Math.Max(0, prefixes - l - 1));

				var dist = prefixDistance + positionTerm;

				if (dist < minDist)
					minDist = dist;
			}

			float typos = 0.5f * minDist;
			float maxTypos = getMaxTypos(s1.Length);

			float result = 1f - typos / maxTypos;
			return result;
		}

		private static float getMaxTypos(int length)
		{
			return (0.7f * length).WithinRange(null, 8f);
		}

		private EditDistances getPrefixDistance(string s1, string s2)
		{
			if (s1.Length > DamerauLevenshteinDistance.MaxInput)
				s1 = s1.Substring(0, DamerauLevenshteinDistance.MaxInput);

			if (s2.Length > DamerauLevenshteinDistance.MaxDict)
				s2 = s2.Substring(0, DamerauLevenshteinDistance.MaxDict);

			lock (_editDistance)
			{
				var distances = _editDistance.GetDistances(s1, s2);
				return distances;
			}
		}
	}
}