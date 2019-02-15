using System;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class PopupSubsystem : IMessageFilter
	{
		public static PopupSubsystem Instance { get; } = new PopupSubsystem();

		private PopupSubsystem()
		{
		}

		public bool PreFilterMessage(ref Message m)
		{
			// ReSharper disable InconsistentNaming
			// ReSharper disable IdentifierTypo
			const int WM_LBUTTONDOWN = 0x0201;
			const int WM_MBUTTONDOWN = 0x0207;
			const int WM_RBUTTONDOWN = 0x0204;
			// ReSharper restore IdentifierTypo
			// ReSharper restore InconsistentNaming

			switch (m.Msg)
			{
				case WM_LBUTTONDOWN:
				case WM_MBUTTONDOWN:
				case WM_RBUTTONDOWN:
					GlobalMouseDown?.Invoke(this, EventArgs.Empty);
					break;
			}

			return false;
		}

		public event EventHandler GlobalMouseDown;
	}
}