using System;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class RichTextBoxSelectionSubsystem
	{
		public RichTextBoxSelectionSubsystem(FixedRichTextBox tooltipTextbox)
		{
			_tooltipTextbox = tooltipTextbox;
			SelectionEnabled = true;
		}

		public void SubsribeToEvents()
		{
			_tooltipTextbox.MouseDown += text_MouseDown;
			_tooltipTextbox.MouseUp += text_MouseUp;
			_tooltipTextbox.MouseMove += text_MouseMove;
		}

		public void UnsubsribeFromEvents()
		{
			_tooltipTextbox.MouseDown -= text_MouseDown;
			_tooltipTextbox.MouseUp -= text_MouseUp;
			_tooltipTextbox.MouseMove -= text_MouseMove;
		}

		private void text_MouseDown(object sender, MouseEventArgs e)
		{
			if (!SelectionEnabled)
				return;

			if (e.Button == MouseButtons.Left)
			{
				int current = _tooltipTextbox.GetTrueIndexPositionFromPoint(e.Location);

				if (current == _selectionStart)
					_selectionManual = true;
			}
		}

		private void text_MouseUp(object sender, MouseEventArgs e)
		{
			if (!SelectionEnabled)
				return;

			_selectionStart = -1;
			_selectionManual = false;
		}

		private void text_MouseMove(object sender, MouseEventArgs e)
		{
			if (!SelectionEnabled)
				return;

			if (_selectionManual)
			{
				var current = _tooltipTextbox.GetTrueIndexPositionFromPoint(e.Location);

				if (current != _selectionStart)
				{
					_tooltipTextbox.SelectionStart = Math.Min(current, _selectionStart);
					_tooltipTextbox.SelectionLength = Math.Abs(current - _selectionStart);
				}
			}
			else if (!_tooltipTextbox.Focused)
			{
				_tooltipTextbox.SelectionStart = _selectionStart =
					_tooltipTextbox.GetTrueIndexPositionFromPoint(e.Location);
			}
		}

		public void Reset()
		{
			_selectionStart = -1;
			_selectionManual = false;
		}

		public bool SelectionEnabled { get; set; }

		private int _selectionStart;
		private bool _selectionManual;

		private readonly FixedRichTextBox _tooltipTextbox;
	}
}