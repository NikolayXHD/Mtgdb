namespace Lucene.Net.Contrib
{
	public class EscapedChar
	{
		public EscapedChar(string value, int position)
		{
			Value = value;
			Position = position;
		}

		public int Position { get; }

		public string Value { get; }
	}
}