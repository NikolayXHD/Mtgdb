using System.Drawing;
using System.Windows.Forms;
using Lucene.Net.Contrib;
using Mtgdb.Index;

namespace Mtgdb.Ui
{
	public class SearchStringHighlighter
	{
		public SearchStringHighlighter(RichTextBox findEditor)
		{
			_findEditor = findEditor;
		}

		public void Highlight()
		{
			bool hasFocus = _findEditor.Focused;

			_findEditor.Parent.SuspendLayout();
			_findEditor.Visible = false;

			var start = _findEditor.SelectionStart;
			var len = _findEditor.SelectionLength;

			var tokenizer = new MtgTolerantTokenizer(_findEditor.Text);
			tokenizer.Parse();

			setColor(0, _findEditor.TextLength, SystemColors.WindowText, false);

			foreach (var token in tokenizer.Tokens)
			{
				if (token.Type.IsAny(TokenType.FieldValue))
					setColor(token.Position, token.Value.Length, null, true);
				else if (token.Type.IsAny(TokenType.Field | TokenType.Colon))
					setColor(token.Position, token.Value.Length, SystemColors.HotTrack.TransformHsv(h: _ => _ + Color.RoyalBlue.RotationTo(Color.DarkCyan)), false);
				else if (token.Type.IsAny(TokenType.RegexBody))
					setColor(token.Position, token.Value.Length, SystemColors.HotTrack.TransformHsv(h: _ => _ + Color.RoyalBlue.RotationTo(Color.DarkRed)), false);
				else
					setColor(token.Position, token.Value.Length, SystemColors.Highlight, false);
			}

			_findEditor.SelectionStart = start;
			_findEditor.SelectionLength = len;

			_findEditor.Visible = true;
			_findEditor.Parent.ResumeLayout(false);

			if (hasFocus)
				_findEditor.Focus();
		}

		private readonly RichTextBox _findEditor;

		private void setColor(int from, int len, Color? foreColor, bool underline)
		{
			_findEditor.SelectionStart = from;
			_findEditor.SelectionLength = len;

			if (foreColor.HasValue)
				_findEditor.SelectionColor = foreColor.Value;

			string selectedText = _findEditor.SelectedText;

			if (underline && !selectedText.IsCjk() && selectedText.IndexOf('_') < 0)
				_findEditor.SelectionFont = new Font(_findEditor.Font, FontStyle.Underline);
			else
				_findEditor.SelectionFont = new Font(_findEditor.Font, FontStyle.Regular);
		}
	}
}