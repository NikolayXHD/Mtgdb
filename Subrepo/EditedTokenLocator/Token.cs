namespace Lucene.Net.Contrib
{
	public class Token
	{
		public Token(int position, string value, TokenType type, string parentField)
		{
			Position = position;
			Type = type;
			ParentField = parentField;
			Value = value;
		}

		public int Position { get; }
		public string ParentField { get; }
		public string NextTokenField { get; internal set; }

		public TokenType Type { get; }
		public string Value { get; }

		public Token PhraseStart { get; set; }

		private Token PhraseEnd
		{
			get
			{
				if (!IsPhrase)
					return null;

				var current = this;

				while (true)
				{
					if (current.Next?.PhraseStart != PhraseStart)
						return current;

					current = current.Next;
				}
			}
		}

		public Token PhraseOpen =>
			PhraseStart?.Previous ?? PhraseStart ?? this;

		public Token PhraseClose
		{
			get
			{
				var end = PhraseEnd;
				return end?.Next ?? end ?? this;
			}
		}

		public bool PhraseHasSlop { get; set; }
		public bool IsPhraseComplex { get; set; }

		public Token Next { get; internal set; }
		public Token Previous { get; internal set; }

		public override string ToString() =>
			Type.IsAny(TokenType.Field)
				? $"{Position:D3}: {Value}"
				: $"{Position:D3}: {Value}    {Type} of: {ParentField}";

		public bool IsLeftToCaret(int caret)
		{
			return Position + Value.Length <= caret;
		}

		public bool IsRightToCaret(int caret)
		{
			return Position >= caret;
		}

		public bool OverlapsCaret(int caret)
		{
			return Position + Value.Length > caret && Position < caret;
		}

		public bool TouchesCaret(int caret)
		{
			return Position == caret || Position + Value.Length == caret;
		}

		public bool IsConnectedToCaret(int caret)
		{
			if (!TouchesCaret(caret))
				return false;

			if (Type.IsAny(TokenType.Open | TokenType.Close | TokenType.Colon | TokenType.Quote | TokenType.RegexDelimiter))
				return false;

			if (Type.IsAny(TokenType.FieldValue))
			{
				if (Position == caret && Value[0].IsCj())
					return false;

				if (Position + Value.Length == caret && Value[Value.Length - 1].IsCj())
					return false;
			}

			return true;
		}

		public bool IsPhraseStart => PhraseStart == this;

		public bool IsPhrase => PhraseStart != null;

		public string GetPhraseText(string queryText)
		{
			var start = PhraseStart ?? this;
			var end = PhraseEnd ?? this;

			return queryText.Substring(start.Position, end.Position + end.Value.Length - start.Position);
		}
	}
}