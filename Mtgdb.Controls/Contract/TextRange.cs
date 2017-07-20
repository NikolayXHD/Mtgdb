namespace Mtgdb.Controls
{
	public class TextRange
	{
		public TextRange(int index, int length, bool isContext)
		{
			Length = length;
			IsContext = isContext;
			Index = index;
		}

		public int Index { get; }

		public int Length { get; set; }

		public bool IsContext { get; private set; }

		public override string ToString()
		{
			return $"{Index}-{Index + Length - 1}";
		}
	}
}