namespace Mtgdb.Gui
{
	public class SearhStringState
	{
		public SearhStringState(string text, int caret)
		{
			Text = text;
			Caret = caret;
		}

		public string Text { get; private set; }

		public int Caret { get; private set; }
	}
}