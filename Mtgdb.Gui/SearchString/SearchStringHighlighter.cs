using System.Drawing;
using System.Windows.Forms;
using Lucene.Net.Contrib;
using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	public class SearchStringHighlighter
	{
		private readonly RichTextBox _findEditor;
		public bool HighlightingInProgress { get; private set; }

		public SearchStringHighlighter(RichTextBox findEditor)
		{
			_findEditor = findEditor;
		}

		public void Highlight()
		{
			HighlightingInProgress = true;

			_findEditor.Visible = false;

			var start = _findEditor.SelectionStart;
			var len = _findEditor.SelectionLength;
			
			var tokenizer = new TolerantTokenizer(_findEditor.Text);
			tokenizer.Parse();

			setColor(0, _findEditor.TextLength, Color.Black, false);

			foreach (var token in tokenizer.Tokens)
			{
				if (token.Type.IsAny(TokenType.FieldValue))
					setColor(token.Position, token.Value.Length, null, true);
				else if (token.Type.IsAny(TokenType.Field | TokenType.Colon))
					setColor(token.Position, token.Value.Length, Color.Teal, false);
				else if (token.Type.IsAny(TokenType.RegexBody))
					setColor(token.Position, token.Value.Length, Color.DarkRed, false);
				else
					setColor(token.Position, token.Value.Length, Color.MediumBlue, false);
			}

			_findEditor.SelectionStart = start;
			_findEditor.SelectionLength = len;

			_findEditor.Visible = true;
			HighlightingInProgress = false;
		}

		private void setColor(int from, int len, Color? foreColor, bool underline)
		{
			_findEditor.SelectionStart = from;
			_findEditor.SelectionLength = len;

			if (foreColor.HasValue)
				_findEditor.SelectionColor = foreColor.Value;

			if (underline && !_findEditor.SelectedText.IsCjk())
				_findEditor.SelectionFont = new Font(_findEditor.Font, FontStyle.Underline);
			else
				_findEditor.SelectionFont = new Font(_findEditor.Font, FontStyle.Regular);
		}
	}
}