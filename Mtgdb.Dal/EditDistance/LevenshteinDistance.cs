using System;
using System.Collections.Generic;
using System.Text;

namespace Mtgdb.Dal
{
	public class LevenshteinDistance
	{
		public float GetPrefixDistance(string userStr, string dictStr)
		{
			return GetDistances(userStr, dictStr).PrefixDistance;
		}

		public EditDistances GetDistances(string userStr, string dictStr)
		{
			userStr = userStr.ToLower(Str.Culture);
			dictStr = dictStr.ToLower(Str.Culture);

			int userStrLength = userStr.Length;
			int dictStrLength = dictStr.Length;

			fillTrace(userStr, dictStr, userStrLength, dictStr.Length);

			// Возьмем минимальное
			// префиксное расстояние
			float minPrefixDist = _trace[userStrLength, 0];
			
			for (int i = 1; i <= dictStrLength; i++)
			{
				float dist = _trace[userStrLength, i];

				if (dist < minPrefixDist)
					minPrefixDist = dist;
			}

			return new EditDistances(minPrefixDist, _trace[userStrLength, dictStr.Length]);
		}

		private void fillTrace(string userStr, string dictStr, int userLength, int dictLength)
		{
			for (int i = 0; i <= userLength; i++)
				_trace[i, 0] = i * 2f;

			for (int j = 0; j <= dictLength; j++)
				_trace[0, j] = j * 2f;

			for (int j = 1; j <= dictLength; j++)
				for (int i = 1; i <= userLength; i++)
				{
					// Учтем вставки, удаления и замены
					float rcost = replaceCostCached(userStr[i - 1], dictStr[j - 1]);

					float dist0 = _trace[i - 1, j] + 2;
					float dist1 = _trace[i, j - 1] + 2;
					float dist2 = _trace[i - 1, j - 1] + rcost;

					_trace[i, j] = Math.Min(dist0, Math.Min(dist1, dist2));

					// Учтем обмен
					if (i > 1 && j > 1 && userStr[i - 1] == dictStr[j - 2] && userStr[i - 2] == dictStr[j - 1])
						_trace[i, j] = Math.Min(_trace[i, j], _trace[i - 2, j - 2] + 1);
				}
		}

		private string printTrace(int sizeUser, int sizeDict)
		{
			var result = new StringBuilder();

			for (int i = 0; i <= sizeUser; i++)
			{
				for (int j = 0; j <= sizeDict; j++)
					result.AppendFormat("{0:00.0} ", _trace[i, j]);

				result.AppendLine();
			}

			return result.ToString();
		}

		private static float replaceCostCached(char c1, char c2)
		{
			float result;
			var key = new Tuple<char, char>(c1, c2);

			lock (_replaceCostCached)
			{
				if (_replaceCostCached.TryGetValue(key, out result))
					return result;

				if (_replaceCostCached.TryGetValue(new Tuple<char, char>(c2, c1), out result))
					return result;

				result = ReplaceCost.Get(c1, c2);
				_replaceCostCached[key] = result;
			}

			return result;
		}

		public const int MaxInput = 40;
		public const int MaxDict = 100;
		private readonly float[,] _trace = new float[MaxInput + 1, MaxDict + 1];
		private static readonly Dictionary<Tuple<char, char>, float> _replaceCostCached = new Dictionary<Tuple<char, char>, float>();
	}

	public struct EditDistances
	{
		public EditDistances(float prefixDistance, float distance)
		{
			PrefixDistance = prefixDistance;
			Distance = distance;
		}

		public readonly float PrefixDistance;

		public readonly float Distance;
	}
}
