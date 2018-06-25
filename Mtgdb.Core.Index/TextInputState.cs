using System;

namespace Mtgdb.Index
{
	public class TextInputState : IEquatable<TextInputState>
	{
		public TextInputState(string text, int caret, int selectionLength)
		{
			Text = text;
			Caret = caret;
			SelectionLength = selectionLength;
		}

		public string Text { get; }
		public int Caret { get; }
		public int SelectionLength { get; }

		public bool Equals(TextInputState other)
		{
			return other != null && string.Equals(Text, other.Text) && Caret == other.Caret && SelectionLength == other.SelectionLength;
		}
	}
}