namespace Mtgdb
{
	internal readonly struct Range
	{
		public readonly ushort From;
		public readonly ushort Length;

		public Range(int from, int length)
		{
			From = (ushort) from;
			Length = (ushort) length;
		}

		public Range(ushort from, ushort length)
		{
			From = from;
			Length = length;
		}
	}
}
