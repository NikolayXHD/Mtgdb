using System;
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

			var queryRows = Enumerable.Range(0, _panelExamples.RowCount)
				.Select(getFindExampleRow)
				.Where(r => r.Query != null)
				.ToList();

			var selectionBackColor = SystemColors.Highlight;

			foreach (var (query, comment, backColor) in queryRows)
			{
				void mouseEnter(object sender, EventArgs args)
				{
					query.BackColor = selectionBackColor;
					comment.BackColor = selectionBackColor;
				}

				void mouseLeave(object sender, EventArgs args)
				{
					query.BackColor = backColor;
					comment.BackColor = backColor;
				}

				void mouseClick(object sender, MouseEventArgs args)
				{
					if (args.Button != MouseButtons.Left)
						return;

					QueryClicked?.Invoke(query.Text);
				}

				query.MouseEnter += mouseEnter;
				comment.MouseEnter += mouseEnter;

				query.MouseLeave += mouseLeave;
				comment.MouseLeave += mouseLeave;

				query.MouseClick += mouseClick;
				comment.MouseClick += mouseClick;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.Clear(SystemColors.ActiveBorder);
			PostPaint?.Invoke(this, e);
		}

		private (Label Query, Label Comment, Color BackColor) getFindExampleRow(int i)
		{
			var queryLabel = (Label) _panelExamples.GetControlFromPosition(0, i);

			if (queryLabel.TextAlign != ContentAlignment.TopLeft)
				return default;

			var commentLabel = (Label) _panelExamples.GetControlFromPosition(1, i);

			return (queryLabel, commentLabel, queryLabel.BackColor);
		}

		public event Action<string> QueryClicked;

		public event PaintEventHandler PostPaint;
	}
}
