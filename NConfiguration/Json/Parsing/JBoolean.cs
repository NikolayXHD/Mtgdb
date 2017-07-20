namespace NConfiguration.Json.Parsing
{
	public sealed class JBoolean: JValue
	{
		public bool Value { get; private set; }

		public JBoolean(bool val)
		{
			Value = val;
		}

		public override TokenType Type
		{
			get
			{
				return TokenType.Boolean;
			}
		}

		public override string ToString()
		{
			return Value?"true":"false";
		}
	}
}
