using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using ButtonBase = Mtgdb.Controls.ButtonBase;

namespace Mtgdb.Gui
{
	public partial class SearchExamplesPanel : UserControl
	{
		public SearchExamplesPanel()
		{
			InitializeComponent();
		}

		public void ShowFindExamples() => _popupSubsystem.OpenPopup(_trigger);

		public void Setup(CardSearchSubsystem cardSearchSubsystem, PopupSubsystem popupSubsystem, ButtonBase trigger)
		{
			_trigger = trigger;
			_popupSubsystem = popupSubsystem;

			popupSubsystem.SetupPopup(new Popup(this, trigger, HorizontalAlignment.Right));

			var queryRows = Enumerable.Range(0, _panelExamples.RowCount)
				.Select(getFindExampleRow)
				.Where(r => r.Query != null)
				.ToList();

			var selectionBackColor = SystemColors.Highlight;

			foreach (var row in queryRows)
			{
				void mouseEnter(object sender, EventArgs args)
				{
					row.Query.BackColor = selectionBackColor;
					row.Comment.BackColor = selectionBackColor;
				}

				void mouseLeave(object sender, EventArgs args)
				{
					row.Query.BackColor = row.BackColor;
					row.Comment.BackColor = row.BackColor;
				}

				void mouseClick(object sender, EventArgs args)
				{
					cardSearchSubsystem.AppliedText = row.Query.Text;
					cardSearchSubsystem.Apply();
				}

				row.Query.MouseEnter += mouseEnter;
				row.Comment.MouseEnter += mouseEnter;

				row.Query.MouseLeave += mouseLeave;
				row.Comment.MouseLeave += mouseLeave;

				row.Query.MouseClick += mouseClick;
				row.Comment.MouseClick += mouseClick;
			}
		}

		private (Label Query, Label Comment, Color BackColor) getFindExampleRow(int i)
		{
			var queryLabel = (Label) _panelExamples.GetControlFromPosition(0, i);

			if (queryLabel.TextAlign != ContentAlignment.TopLeft)
				return (null, null, default);

			var commentLabel = (Label) _panelExamples.GetControlFromPosition(1, i);

			return (queryLabel, commentLabel, queryLabel.BackColor);
		}

		private PopupSubsystem _popupSubsystem;

		private ButtonBase _trigger;
	}
}
