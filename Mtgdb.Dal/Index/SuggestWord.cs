using System;

namespace Mtgdb.Dal.Index
{
	public class SuggestWord: IComparable<SuggestWord>
	{
		public string String { get; set; }

		public float Score { get; set; }

		public float Freq { get; set; }

		public int CompareTo(SuggestWord other)
		{
			return Score.CompareTo(other.Score);
		}
	}
}