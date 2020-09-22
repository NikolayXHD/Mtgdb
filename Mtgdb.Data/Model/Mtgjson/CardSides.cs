namespace Mtgdb.Data
{
	public static class CardSides
	{
		public static readonly string[] Values = { A, B, C, D, E };

		public const string A = "a";
		public const string B = "b";
		public const string C = "c";
		public const string D = "d";
		public const string E = "e";

		public static bool IsSideA(this Card c) =>
			Str.Equals(c.Side, A);

		public static bool IsSideB(this Card c) =>
			Str.Equals(c.Side, B);
	}
}
