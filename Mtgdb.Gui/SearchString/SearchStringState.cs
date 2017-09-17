namespace Mtgdb.Gui
{
	public class SearchStringState
	{
		public SearchStringState(string text, int caret)
		{
			Text = text;
			Caret = caret;
		}

		public string Text { get; private set; }

		public int Caret { get; private set; }
	}
}