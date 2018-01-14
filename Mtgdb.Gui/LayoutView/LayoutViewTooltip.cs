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

		public LayoutViewTooltip(LayoutView layoutView, SearchStringSubsystem searchStringSubsystem)
		{
			_layoutView = layoutView;
			_searchStringSubsystem = searchStringSubsystem;
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
			if (!hitInfo.AlignButtonDirection.HasValue && (card == null || hitInfo.IsOverImage() || !hitInfo.FieldBounds.HasValue))
			{
				Hide?.Invoke();
				return;
			}

			if (hitInfo.IsSortButton)
			{
				Show?.Invoke(new TooltipModel
				{
					Id = $"{_layoutView.Control.Name}.{hitInfo.RowHandle}.{hitInfo.FieldName}.sort",
					ObjectBounds = _layoutView.GetSortButtonBounds(hitInfo),
					Control = _layoutView.Control,
					Title = "Sort by " + hitInfo.FieldName,
					Text = "Click to sort by this field.\r\n\r\n" +
					       "Shift+Click to ADD this field to sorting. Currently sorted fields will have higher sort priority.\r\n\r\n" +
					       "Ctrl+Click to REMOVE this field from sorting. Other fields sort order will remain unchanged.\r\n\r\n" +
					       "Repeated click on sort button cycles sort order between Ascending, Descending, None.",
					Clickable = false
				});
			}
			else if (hitInfo.IsSearchButton)
			{
				Show?.Invoke(new TooltipModel
				{
					Id = $"{_layoutView.Control.Name}.{hitInfo.RowHandle}.{hitInfo.FieldName}.search",
					ObjectBounds = _layoutView.GetSearchButtonBounds(hitInfo),
					Control = _layoutView.Control,
					Title = "Add to search",
					Text = "Click to EXTEND search result by cards matching this value\r\n" +
					       "Shift+Click to NARROW DOWN search result by cards matching this value\r\n\r\n" +
					       "Following term will be added to search text\r\n" +
					       _searchStringSubsystem.GetFieldValueQuery(hitInfo.FieldName, _layoutView.GetFieldText(hitInfo.RowHandle, hitInfo.FieldName)),
					Clickable = false
				});
			}
			else if (hitInfo.AlignButtonDirection.HasValue)
			{
				Show?.Invoke(new TooltipModel
				{
					Id = $"{_layoutView.Control.Name}.{hitInfo.RowHandle}.{hitInfo.FieldName}.align",
					ObjectBounds = _layoutView.GetAlignButtonBounds(hitInfo),
					Control = _layoutView.Control,
					Title = "Viewport alignment",
					Text = "Aligns viewport by this corner.\r\n\r\n" +
					       "If this corner would be truncated\r\n"+
					       "viewport will shift to fit it into the screen.",
					Clickable = false
				});
			}
			else
			{
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
		}

		private readonly LayoutView _layoutView;
		private readonly SearchStringSubsystem _searchStringSubsystem;
	}
}