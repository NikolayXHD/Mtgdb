namespace NConfiguration.Json.Parsing
{
	public sealed class JNull: JValue
	{
		public static readonly JNull Instance = new JNull();

		public JNull()
		{
		}

		public override TokenType Type
		{
			get
			{
				return TokenType.Null;
			}
		}

		public override string ToString()
		{
			return "null";
		}
	}
}
