using System;

namespace Mtgdb.Gui
{
	public class SearchInputState : IEquatable<SearchInputState>
	{
		public SearchInputState(string text, int caret, int selectionLength)
		{
			Text = text;
			Caret = caret;
			SelectionLength = selectionLength;
		}

		public string Text { get; }
		public int Caret { get; }
		public int SelectionLength { get; }

		public bool Equals(SearchInputState other)
		{
			return other != null && string.Equals(Text, other.Text) && Caret == other.Caret && SelectionLength == other.SelectionLength;
		}
	}
}