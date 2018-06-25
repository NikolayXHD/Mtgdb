using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	public partial class SearchExamplesPanel : UserControl
	{
		public SearchExamplesPanel()
		{
			InitializeComponent();
		}

		public void ShowFindExamples() => _buttonSubsystem.OpenPopup(_trigger);

		public void Setup(CardSearchSubsystem cardSearchSubsystem, ButtonSubsystem buttonSubsystem, ButtonBase trigger)
		{
			_trigger = trigger;
			_buttonSubsystem = buttonSubsystem;

			buttonSubsystem.SetupPopup(
				new Popup(this,
					trigger,
					HorizontalAlignment.Right,
					openOnHover: false,
					borderOnHover: false));

			var queryRows = Enumerable.Range(0, _panelExamples.RowCount)
				.Select(getFindExampleRow)
				.Where(r => r.Query != null)
				.ToList();

			var selectionBackColor = Color.LightBlue;

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
					buttonSubsystem.ClosePopup(trigger);
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
				return (null, null, default(Color));

			var commentLabel = (Label) _panelExamples.GetControlFromPosition(1, i);

			return (queryLabel, commentLabel, queryLabel.BackColor);
		}

		private ButtonSubsystem _buttonSubsystem;

		private ButtonBase _trigger;
	}
}
