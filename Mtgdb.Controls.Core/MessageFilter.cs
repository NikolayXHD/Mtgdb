using System;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class MessageFilter : IMessageFilter
	{
		public static MessageFilter Instance { get; } = new MessageFilter();

		private MessageFilter()
		{
		}

		public bool PreFilterMessage(ref Message m)
		{
			const int WM_MOUSEMOVE = 0x0200;
			const int WM_LBUTTONDOWN = 0x0201;
			const int WM_MBUTTONDOWN = 0x0207;
			const int WM_RBUTTONDOWN = 0x0204;

			switch (m.Msg)
			{
				case WM_MOUSEMOVE:
					GlobalMouseMove?.Invoke(this, EventArgs.Empty);
					break;

				case WM_LBUTTONDOWN:
				case WM_MBUTTONDOWN:
				case WM_RBUTTONDOWN:
					GlobalMouseDown?.Invoke(this, new MouseEventArgs(
						toMouseButtons(m.Msg),
						0,
						m.LParam.LowWord(),
						m.LParam.HighWord(),
						0));
					break;
			}

			return false;

			MouseButtons toMouseButtons(int msg)
			{
				switch (msg)
				{
					case WM_LBUTTONDOWN:
						return MouseButtons.Left;
					case WM_MBUTTONDOWN:
						return MouseButtons.Middle;
					case WM_RBUTTONDOWN:
						return MouseButtons.Right;
				}

				throw new ArgumentException();
			}
		}

		public event MouseEventHandler GlobalMouseDown;
		public event EventHandler GlobalMouseMove;
	}
}