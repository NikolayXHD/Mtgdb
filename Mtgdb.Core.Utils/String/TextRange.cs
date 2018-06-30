using System.Text.RegularExpressions;

namespace Mtgdb.Controls
{
	public class TextRange
	{
		public static TextRange Copy(Match m)
		{
			return new TextRange(m.Index, m.Length);
		}

		public TextRange(int index, int length)
		{
			Length = length;
			Index = index;
		}

		public int Index { get; }

		public int Length { get; set; }

		public bool IsContext { get; set; }

		public override string ToString()
		{
			return $"{Index}-{Index + Length - 1}";
		}
	}
}