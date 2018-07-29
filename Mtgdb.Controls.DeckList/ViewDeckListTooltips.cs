using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	internal class ViewDeckListTooltips : ICustomTooltip
	{
		public event Action<TooltipModel> Show;
		public event Action Hide;

		public ViewDeckListTooltips(object owner, LayoutViewControl view)
		{
			Owner = owner;
			_view = view;
		}

		public void SubscribeToEvents()
		{
			_view.MouseMove += mouseMove;
			_view.CardIndexChanged += scrolled;
			_view.MouseLeave += mouseLeave;
		}

		private void scrolled(object sender)
		{
			var position = Cursor.Position;
			showFieldTooltip(position);
		}

		private void mouseMove(object sender, MouseEventArgs e)
		{
			if (_view.IsSelectingText())
			{
				Hide?.Invoke();
			}
			else
			{
				var position = Cursor.Position;
				showFieldTooltip(position);
			}
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			Hide?.Invoke();
		}

		private void showFieldTooltip(Point position)
		{
			var cursorPosition = _view.PointToClient(position);
			var hitInfo = _view.CalcHitInfo(cursorPosition);

			var dataSource = _view.FindRow(hitInfo.RowHandle);
			if (!hitInfo.AlignButtonDirection.HasValue && (dataSource == null || !hitInfo.FieldBounds.HasValue))
			{
				Hide?.Invoke();
				return;
			}

			if (hitInfo.IsSortButton)
			{
				Show?.Invoke(new TooltipModel
				{
					Id = $"{_view.Name}.{hitInfo.RowHandle}.{hitInfo.FieldName}.sort",
					ObjectBounds = hitInfo.ButtonBounds,
					Control = _view,
					Title = "Sort by " + hitInfo.FieldName,
					Text =
						"Click to sort by this field.\r\n" +
						"\r\n" +
						"Shift+Click to ADD this field to sorting. Currently sorted fields will have higher sort priority.\r\n" +
						"\r\n" +
						"Ctrl+Click to REMOVE this field from sorting. Other fields sort order will remain unchanged.\r\n" +
						"\r\n" +
						"Repeated click on sort button cycles sort order between Ascending, Descending, None.\r\n" +
						"\r\n" +
						"Hold Alt key when hovering to prevent showing this button. Helps selecting text in small fields.",
					Clickable = false
				});
			}
			else
			{
				if (hitInfo.IsSearchButton)
				{
					throw new NotSupportedException();
				}

				if (hitInfo.CustomButtonIndex >= 0)
				{
					var tooltip = new TooltipModel
					{
						Id = $"{_view.Name}.{hitInfo.RowHandle}.{hitInfo.FieldName}.",
						ObjectBounds = hitInfo.ButtonBounds,
						Control = _view,
						Clickable = false
					};

					if (hitInfo.IsAddButton())
					{
						tooltip.Id += "add";
						tooltip.Text = "Save this deck";
					}
					else if (hitInfo.IsRemoveButton())
					{
						tooltip.Id += "remove";
						tooltip.Title = "Remove this deck";
						tooltip.Text = "Removes this deck from the list.\r\n" +
							"NOTE: be careful, you cannot undo this.";
					}
					else if (hitInfo.IsRenameButton())
					{
						tooltip.Id += "rename";
						tooltip.Text = "Rename this deck";
					}
					else if (hitInfo.IsOpenButton())
					{
						tooltip.Id += "open";
						tooltip.Title = "Open deck";
						tooltip.Text = "Open the deck in it's original form as it was edited and saved.\r\n\r\n" +
							"Left-click to open in this tab, currently opened deck will be replaced.\r\n" +
							"Middle-click to open in new tab.\r\n\r\n";
					}
					else if (hitInfo.IsOpenTransformedButton())
					{
						tooltip.Id += "open_transformed";
						tooltip.Title = "Open transformed deck";
						tooltip.Text = "In transformed deck card variants from your collection and with known price are used whenever possible.\r\n\r\n" +
							"Left-click to open in this tab, currently opened deck will be replaced.\r\n" +
							"Middle-click to open in new tab.\r\n\r\n";
					}
					else
						return;

					Show?.Invoke(tooltip);
				}
				else if (hitInfo.AlignButtonDirection.HasValue)
				{
					Show?.Invoke(new TooltipModel
					{
						Id = $"{_view.Name}.{hitInfo.RowHandle}.{hitInfo.FieldName}.align",
						ObjectBounds = _view.GetAlignButtonBounds(hitInfo),
						Control = _view,
						Title = "Viewport alignment",
						Text = "Aligns viewport by this corner.\r\n" +
							"\r\n" +
							"If this corner would be truncated\r\n" +
							"viewport will shift to fit it into the screen.",
						Clickable = false
					});
				}
				else if (hitInfo.FieldName != null)
				{
					Show?.Invoke(new TooltipModel
					{
						Id = $"{_view.Name}.{hitInfo.RowHandle}.{hitInfo.FieldName}",
						ObjectBounds = hitInfo.FieldBounds.Value,
						Control = _view,
						Title = hitInfo.FieldName,
						Text = _view.GetText(hitInfo.RowHandle, hitInfo.FieldName),
						HighlightRanges = _view.GetHighlihgtTextRanges(hitInfo.RowHandle, hitInfo.FieldName),
						HighlightOptions = _view.ProbeCard.HighlightOptions,
						Clickable = true
					});
				}
			}
		}

		private readonly LayoutViewControl _view;

		public object Owner { get; }
	}
}