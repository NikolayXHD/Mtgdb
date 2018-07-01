using System;
using System.Drawing;
using System.Windows.Forms;
using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	public class TabHeadersTooltip : ICustomTooltip
	{
		public TabHeadersTooltip(TabHeaderControl control, Control owner)
		{
			_control = control;
			Owner = owner;
		}

		public void SubscribeToEvents()
		{
			_control.MouseMove += mouseMove;
			_control.MouseLeave += mouseLeave;
		}

		private void mouseLeave(object sender, EventArgs e) =>
			Hide?.Invoke();

		private void mouseMove(object sender, MouseEventArgs e)
		{
			if (_control.IsDragging())
			{
				Hide?.Invoke();
				return;
			}

			_control.GetTabIndex(e.Location, out int hoveredIndex, out bool hoveredClose);

			var model = new TooltipModel
			{
				Id = "tab",
				Control = _control
			};

			model.Text = "Add tab: Ctrl+T or click '+' button\r\n" +
				"Remove tab: Ctrl + F4 or click 'x' button or Middle mouse click\r\n" +
				"Select next tab: Ctrl + Tab\r\n\r\n" +
				"Drag tab headers to reorder tabs.\r\n\r\n" +
				"Move the tab to another window by dragging it to another window title.\r\n\r\n" +
				"Drag the card here to select or create another tab where you want to drop the card.";

			if ((hoveredIndex < 0 || hoveredIndex >= _control.Count) && hoveredIndex != _control.AddButtonIndex)
			{
				model.ObjectBounds = new Rectangle(default(Point), _control.Size);

				model.Id += ".none";
				model.Title = "Tabbed Document Interface (TDI)";
			}
			else
			{
				model.ObjectBounds = _control.GetTabPolygon(hoveredIndex).GetBoundingRectangle();
				model.Id += $".{hoveredIndex}";

				if (hoveredClose)
				{
					model.Id += ".close";
					model.Title = "Close this tab";
				}
				else if (hoveredIndex == _control.AddButtonIndex)
				{
					model.Id += ".open";
					model.Title = "Open new tab";
				}
				else
				{
					var formMain = (FormMain) _control.TabIds[hoveredIndex];

					var deckName = formMain.DeckName;
					var title = formMain.Text;

					model.Title = deckName != null && deckName != title
						? deckName
						: formMain.Text;
				}
			}

			Show?.Invoke(model);
		}

		public object Owner { get; }
		public event Action<TooltipModel> Show;
		public event Action Hide;

		private readonly TabHeaderControl _control;
	}
}