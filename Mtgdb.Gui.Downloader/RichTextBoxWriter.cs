using System.IO;
using System.Text;
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

			TaskEx.Run(flushByTimeoutLoop);
		}

		protected override void Dispose(bool disposing)
		{
			_disposed = true;
			base.Dispose(disposing);
		}

		private async Task flushByTimeoutLoop()
		{
			while (!_disposed)
			{
				await TaskEx.Delay(200);
				flush();
			}
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
		private bool _disposed;

		public override Encoding Encoding => Encoding.UTF8;
	}
}