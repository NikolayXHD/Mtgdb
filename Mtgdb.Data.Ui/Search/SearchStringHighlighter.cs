using System.Drawing;
using System.Windows.Forms;
using Lucene.Net.Contrib;
using Mtgdb.Controls;
using Mtgdb.Data;

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
			if (Runtime.IsMono)
				return;

			var start = _findEditor.SelectionStart;
			var len = _findEditor.SelectionLength;

			var tokenizer = new MtgTolerantTokenizer(_findEditor.Text);
			tokenizer.Parse();

			_findEditor.BeginUpdate();

			try
			{
				setColor(0, _findEditor.TextLength, SystemColors.WindowText);

				foreach (var token in tokenizer.Tokens)
				{
					if (token.Type.IsAny(TokenType.FieldValue))
						setColor(token.Position, token.Value.Length, null);
					else if (token.Type.IsAny(TokenType.Field | TokenType.Colon))
						setColor(token.Position, token.Value.Length,
							SystemColors.HotTrack.TransformHsv(h: _ =>
								_ + Color.RoyalBlue.RotationTo(Color.DarkCyan)));
					else if (token.Type.IsAny(TokenType.RegexBody))
						setColor(token.Position, token.Value.Length,
							SystemColors.HotTrack.TransformHsv(h: _ =>
								_ + Color.RoyalBlue.RotationTo(Color.DarkRed)));
					else
						setColor(token.Position, token.Value.Length, SystemColors.Highlight);
				}

				_findEditor.SelectionStart = start;
				_findEditor.SelectionLength = len;
			}
			finally
			{
				_findEditor.EndUpdate();
			}
		}

		private readonly RichTextBox _findEditor;

		private void setColor(int from, int len, Color? foreColor)
		{
			_findEditor.SelectionStart = from;
			_findEditor.SelectionLength = len;

			if (foreColor.HasValue)
				_findEditor.SelectionColor = foreColor.Value;

			using var font = new Font(_findEditor.Font, FontStyle.Regular);
			_findEditor.SelectionFont = font;
		}
	}
}
