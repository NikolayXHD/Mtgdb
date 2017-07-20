namespace NConfiguration.Json.Parsing
{
	public sealed class JNumber: JValue
	{
		public string Value { get; private set; }

		public JNumber(string text)
		{
			Value = text;
		}

		public override TokenType Type
		{
			get
			{
				return TokenType.Number;
			}
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
