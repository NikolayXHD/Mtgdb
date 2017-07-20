using System;
using System.Drawing;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	internal class LayoutViewTooltip : ICustomTooltip
	{
		public event Action<TooltipModel> Show;
		public event Action Hide;

		public LayoutViewTooltip(LayoutView layoutView)
		{
			_layoutView = layoutView;
		}

		public void SubscribeToEvents()
		{
			_layoutView.MouseMove += mouseMove;
			_layoutView.VisibleRecordIndexChanged += scrolled;
			_layoutView.MouseLeave += mouseLeave;
		}

		public void UnsubscribeFromEvents()
		{
			_layoutView.MouseMove -= mouseMove;
			_layoutView.VisibleRecordIndexChanged -= scrolled;
		}

		private void scrolled(object sender)
		{
			var position = Cursor.Position;
			showFieldTooltip(position);
		}

		private void mouseMove(object sender, MouseEventArgs e)
		{
			var position = Cursor.Position;
			showFieldTooltip(position);
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			Hide?.Invoke();
		}

		private void showFieldTooltip(Point position)
		{
			var cursorPosition = _layoutView.Control.PointToClient(position);
			var hitInfo = _layoutView.CalcHitInfo(cursorPosition);

			var card = (Card)_layoutView.GetRow(hitInfo.RowHandle);
			if (card == null || hitInfo.IsOverImage() || !hitInfo.FieldBounds.HasValue)
			{
				Hide?.Invoke();
				return;
			}

			Show?.Invoke(new TooltipModel
			{
				Id = $"{_layoutView.Control.Name}.{hitInfo.RowHandle}.{hitInfo.FieldName}",
				ObjectBounds = hitInfo.FieldBounds.Value,
				Control = _layoutView.Control,
				Title = hitInfo.FieldName,
				Text = _layoutView.GetFieldText(hitInfo.RowHandle, hitInfo.FieldName),
				HighlightRanges = _layoutView.GetHiglightRanges(hitInfo.RowHandle, hitInfo.FieldName),
				HighlightSettings = _layoutView.GetHighlightSettings(),
				Clickable = true
			});
		}

		private readonly LayoutView _layoutView;
	}
}