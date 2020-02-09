using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	public partial class SearchExamplesMenu : UserControl, IPostPaintEvent
	{
		public SearchExamplesMenu()
		{
			InitializeComponent();

			if (DesignMode)
				return;

			_numberByIndex = new Dictionary<int, int>();

			for (int i = 0; i < _panelExamples.RowCount; i++)
			{
				(Label query, Label comment) = getLabels(i);
				bool isRow = query.TextAlign == ContentAlignment.TopLeft;
				if (isRow)
					_numberByIndex[i] = _numberByIndex.Count;

				(Color back, Color fore) = getColors(i);

				query.BackColor = back;
				query.ForeColor = fore;
				if (comment != null)
				{
					comment.BackColor = back;
					comment.ForeColor = fore;
				}

				if (isRow)
				{
					query.MouseEnter += mouseEnter;
					query.MouseLeave += mouseLeave;
					query.MouseClick += mouseClick;

					if (comment != null)
					{
						comment.MouseEnter += mouseEnter;
						comment.MouseLeave += mouseLeave;
						comment.MouseClick += mouseClick;
					}
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.Clear(SystemColors.ActiveBorder);
			PostPaint?.Invoke(this, e);
		}

		public void Scale() =>
			_panelExamples.Controls.OfType<Label>().ForEach(ControlScaler.ScaleDpiFont);

		private void mouseEnter(object sender, EventArgs args)
		{
			var  position = _panelExamples.GetCellPosition((Control) sender);
			(Label query, Label comment) = getLabels(position.Row);

			query.BackColor = comment.BackColor = _selectionBackColor;
			query.ForeColor = comment.ForeColor = _selectionForeColor;
		}

		private void mouseLeave(object sender, EventArgs args)
		{
			var position = _panelExamples.GetCellPosition((Control) sender);
			int i = position.Row;

			(Label query, Label comment) = getLabels(i);
			(Color back, Color fore) = getColors(i);

			query.BackColor = comment.BackColor = back;
			query.ForeColor = comment.ForeColor = fore;
		}

		private void mouseClick(object sender, MouseEventArgs args)
		{
			if (args.Button != MouseButtons.Left)
				return;

			var position = _panelExamples.GetCellPosition((Control) sender);
			(Label query, _) = getLabels(position.Row);
			QueryClicked?.Invoke(query.Text);
		}



		private (Color Back, Color Fore) getColors(int i)
		{
			if (_numberByIndex.TryGetValue(i, out int n))
				return n % 2 == 0
					? (SystemColors.Window, SystemColors.WindowText)
					: (SystemColors.Control, SystemColors.ControlText);

			return (SystemColors.ActiveCaption, SystemColors.ActiveCaptionText);
		}

		private (Label query, Label comment) getLabels(int i) =>
		(
			(Label) _panelExamples.GetControlFromPosition(0, i),
			(Label) _panelExamples.GetControlFromPosition(1, i)
		);



		public event Action<string> QueryClicked;

		public event PaintEventHandler PostPaint;


		private readonly Dictionary<int, int> _numberByIndex;
		private static readonly Color _selectionBackColor = SystemColors.Highlight;
		private static readonly Color _selectionForeColor = SystemColors.HighlightText;
	}
}
