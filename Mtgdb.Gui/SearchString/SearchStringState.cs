using System;

namespace Mtgdb.Gui
{
	public class SearchStringState : IEquatable<SearchStringState>
	{
		public SearchStringState(string text, int caret)
		{
			Text = text;
			Caret = caret;
		}

		public string Text { get; }

		public int Caret { get; }

		public bool Equals(SearchStringState other)
		{
			return other != null && string.Equals(Text, other.Text) && Caret == other.Caret;
		}
	}
}