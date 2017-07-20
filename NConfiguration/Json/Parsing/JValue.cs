using System;

namespace NConfiguration.Json.Parsing
{
	public abstract class JValue
	{
		public abstract TokenType Type { get; }

		public static JValue Parse(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException("text");

			var chars = new CharEnumerator(text);
			var result = chars.ReadValue(true);

			if(chars.MoveToNoWhite())
				throw new FormatException(string.Format("unexpected symbols '{0}' in the reading of end", chars.Tail));

			return result;
		}
	}
}
