using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mtgdb.Controls;

namespace Mtgdb.Downloader
{
	public class RichTextBoxWriter : TextWriter
	{
		public RichTextBoxWriter(RichTextBox textbox)
		{
			_textbox = textbox;
			_buffer = new StringBuilder();
			_cts = new CancellationTokenSource();
			_cts.Token.Run(async token =>
			{
				while (!token.IsCancellationRequested)
				{
					await Task.Delay(200, token);
					flush();
				}
			});
		}

		protected override void Dispose(bool disposing)
		{
			_cts.Cancel();
			base.Dispose(disposing);
		}

		public override void Write(char value)
		{
			base.Write(value);

			if (value == '\r')
				return;

			lock (_sync)
				_buffer.Append(value);
		}

		public override void Write(string value)
		{
			lock (_sync)
				_buffer.Append(value);
		}

		private void flush()
		{
			string text;
			lock (_sync)
			{
				if (_buffer.Length == 0)
					return;

				text = _buffer.ToString();
				_buffer.Clear();
			}

			_textbox.Invoke(delegate
			{
				_textbox.Focus();
				_textbox.AppendText(text);
			});
		}

		private readonly object _sync = new object();

		private readonly RichTextBox _textbox;
		private readonly StringBuilder _buffer;
		private readonly CancellationTokenSource _cts;

		public override Encoding Encoding => Encoding.UTF8;
	}
}
