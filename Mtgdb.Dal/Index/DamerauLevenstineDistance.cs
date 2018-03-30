using System;

namespace Mtgdb.Dal.Index
{
	public class DamerauLevenstineDistance : IStringDistance
	{
		private readonly LevenshteinDistance _editDistance;

		public DamerauLevenstineDistance()
		{
			_editDistance = new LevenshteinDistance();
		}

		/// <summary>
		/// In fact it is not distance, it is similarity
		/// </summary>
		public float GetDistance(string s1, string s2)
		{
			float minDist = float.MaxValue;
			var prefixes = 1 + Math.Max(0, s2.Length - s1.Length);

			for (int l = 0; l < prefixes; l++)
			{
				int r = Math.Min(s2.Length, l + 2 * s1.Length);

				var prefixDistance = getPrefixDistance(s1, s2.Substring(l, r - l)).PrefixDistance;

				var dist = prefixDistance + 0.0001f * Math.Min(l, 0.5f + Math.Max(0, s2.Length - s1.Length - l) + 0.001f * Math.Abs(s2.Length - s1.Length));

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
			if (s1.Length > LevenshteinDistance.MaxInput)
				s1 = s1.Substring(0, LevenshteinDistance.MaxInput);

			if (s2.Length > LevenshteinDistance.MaxDict)
				s2 = s2.Substring(0, LevenshteinDistance.MaxDict);

			lock (_editDistance)
			{
				var distances = _editDistance.GetDistances(s1, s2);
				return distances;
			}
		}
	}
}