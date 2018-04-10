using System;

namespace Mtgdb.Dal.Index
{
	public class SuggestWord: IComparable<SuggestWord>
	{
		public string String { get; set; }
		public float Score { get; set; }

		public int CompareTo(SuggestWord other)
		{
			var compareScoreResult =  Score.CompareTo(other.Score);
			
			if (compareScoreResult != 0)
				return compareScoreResult;

			return -Str.Comparer.Compare(String, other.String);
		}
	}
}