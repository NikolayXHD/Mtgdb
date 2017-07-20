namespace NConfiguration.Json.Parsing
{
	public sealed class JString: JValue
	{
		public string Value { get; private set; }

		public JString(string text)
		{
			Value = text;
		}

		public override TokenType Type
		{
			get
			{
				return TokenType.String;
			}
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
