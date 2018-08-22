using System.IO;
using System.Text;
using System.Windows.Forms;
using Mtgdb.Controls;

namespace Mtgdb.Downloader
{
	public class RichTextBoxWriter : TextWriter
	{
		private readonly RichTextBox _textbox;

		public RichTextBoxWriter(RichTextBox textbox)
		{
			_textbox = textbox;
		}

		public override void Write(char value)
		{
			if (value == '\r')
				return;

			_textbox.Invoke(delegate
			{
				_textbox.Focus();
				_textbox.AppendText(new string(value, 1));
			});
		}

		public override void Write(string value)
		{
			_textbox.Invoke(delegate
			{
				_textbox.Focus();
				_textbox.AppendText(value);
			});
		}

		public override Encoding Encoding => Encoding.UTF8;
	}
}