using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal.EditDistance
{
	public class LevenstineDistance
	{
		private static readonly Regex _wordRegex = new Regex(@"\b\w+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private static readonly ConcurrentStack<LevenstineDistance> _instances = new ConcurrentStack<LevenstineDistance>();
		private static readonly Dictionary<Tuple<string, string>, float> _distanceCache = new Dictionary<Tuple<string, string>, float>();
		private static readonly Dictionary<string, Dictionary<string, int>> _userWordsCache = new Dictionary<string, Dictionary<string, int>>(Str.Comparer);
		private static readonly Dictionary<string, string[]> _dictWordsCache = new Dictionary<string, string[]>(Str.Comparer);

		public static float GetEditDistance(Dictionary<string, int> userWords, string[] dictWords)
		{
			LevenstineDistance instance;
			bool createInstance = !_instances.TryPop(out instance);
			if (createInstance)
				instance = new LevenstineDistance();

			float totalDist = getTotalDist(instance, userWords, dictWords);

			if (createInstance)
				_instances.Push(instance);

			return totalDist;
		}

		private static float getTotalDist(LevenstineDistance instance, Dictionary<string, int> userWords, string[] dictWords)
		{
			float totalDist = 0;
			foreach (var pair in userWords)
			{
				var userWord = pair.Key;
				var count = pair.Value;

				float minDist = float.MaxValue;
				foreach (var dictWord in dictWords)
				{
					float distance;
					var key = new Tuple<string, string>(userWord.ToLower(), dictWord.ToLower());
					if (!_distanceCache.TryGetValue(key, out distance))
					{
						distance = instance.GetPrefixDistance(userWord, dictWord);
						_distanceCache.Add(key, distance);
					}

					var dist = distance;
					if (dist < minDist)
					{
						minDist = dist;
						if (minDist <= 0f)
							break;
					}
				}

				totalDist += minDist*count;
			}

			return totalDist;
		}

		public static string[] GetDictionaryWords(string dictStr)
		{
			string[] result;

			lock (_dictWordsCache)
				if (_dictWordsCache.TryGetValue(dictStr, out result))
					return result;

			result = _wordRegex.Matches(dictStr)
				.OfType<Match>()
				.Where(_ => _.Success)
				.Select(_ => addEndOfWordSuffix(_.Value))
				.Distinct()
				.ToArray();

			lock (_dictWordsCache)
				_dictWordsCache[dictStr] = result;

			return result;
		}

		public static Dictionary<string, int> GetInputWords(string userInput)
		{
			lock (_userWordsCache)
			{
				Dictionary<string, int> result;

				if (_userWordsCache.TryGetValue(userInput, out result))
					return result;

				result = _wordRegex.Matches(userInput)
					.OfType<Match>()
					.Where(_ => _.Success)
					.Select(_ => _.Value.Length < 4 ? _.Value : addEndOfWordSuffix(_.Value))
					.GroupBy(_ => _, Str.Comparer)
					.ToDictionary(
						_ => _.Key,
						_ => _.Count(),
						Str.Comparer);

				_userWordsCache.Add(userInput, result);
				return result;
			}
		}

		private static string addEndOfWordSuffix(string userInput)
		{
			var inputBuilder = new StringBuilder();
			inputBuilder.Append(userInput);

			if (userInput[userInput.Length - 1] != ReplaceCost.EndOfWord)
				inputBuilder.Append(ReplaceCost.EndOfWord);

			var result = inputBuilder.ToString();

			if (result.Length > MaxInput)
				return result.Substring(0, MaxInput);

			return result;
		}

		public const int MaxInput = 80;
		public const int MaxDict = 200;
		private readonly float[,] _trace = new float[MaxInput + 1, MaxDict + 1];
		private static readonly Dictionary<Tuple<char, char>, float> _replaceCostCached = new Dictionary<Tuple<char, char>, float>();

		public float GetPrefixDistance(string userStr, string dictStr)
		{
			validate(userStr, dictStr);

			userStr = userStr.ToLower(CultureInfo.InvariantCulture);
			dictStr = dictStr.ToLower(CultureInfo.InvariantCulture);

			int userLength = userStr.Length;
			var dictLength = dictStr.Length;

			fillTrace(userStr, dictStr, userLength, dictLength);

			// Возьмем минимальное
			// префиксное расстояние
			float minPrefixDist = _trace[userLength, 0];
			for (int i = 1; i <= dictLength; ++i)
			{
				float dist = _trace[userLength, i];

				if (dist < minPrefixDist)
					minPrefixDist = dist;
			}

			return minPrefixDist;
		}

		public float GetDistance(string userStr, string dictStr)
		{
			userStr = userStr.ToLower(CultureInfo.InvariantCulture);
			dictStr = dictStr.ToLower(CultureInfo.InvariantCulture);

			int userLength = userStr.Length;
			var dictLength = dictStr.Length;

			fillTrace(userStr, dictStr, userLength, dictLength);

			// Возьмем минимальное
			// префиксное расстояние
			float dist = _trace[userLength, dictLength];
			return dist;
		}

		private static void validate(string userStr, string dictStr)
		{
			if (string.IsNullOrEmpty(userStr))
				throw new ArgumentException("user string empty");

			if (string.IsNullOrEmpty(dictStr))
				throw new ArgumentException("dict string empty");

			if (userStr.Length > MaxInput)
				throw new ArgumentException($"user string length exceeds {MaxInput}: {userStr.Length}");

			if (dictStr.Length > MaxDict)
				throw new ArgumentException($"dict string length exceeds {MaxDict}: {dictStr.Length}");
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

		private void fillTraceSimple(string userStr, string dictStr, int userLength, int dictLength)
		{
			for (int i = 0; i <= userLength; i++)
				_trace[i, 0] = i * 2f;

			for (int j = 0; j <= dictLength; j++)
				_trace[0, j] = j * 2f;

			for (int j = 1; j <= dictLength; j++)
				for (int i = 1; i <= userLength; i++)
				{
					// Учтем вставки, удаления и замены
					float dist0 = _trace[i - 1, j] + 2;
					float dist1 = _trace[i, j - 1] + 2;

					int replaceCost;
					if (userStr[i - 1] == dictStr[j - 1])
						replaceCost = 0;
					else
						replaceCost = 2;

					float dist2 = _trace[i - 1, j - 1] + replaceCost;

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
	}
}
