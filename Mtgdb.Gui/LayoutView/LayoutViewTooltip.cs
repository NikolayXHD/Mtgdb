using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Data;

namespace Mtgdb.Gui
{
	internal class LayoutViewTooltip : ICustomTooltip
	{
		public event Action<TooltipModel> Show;
		public event Action Hide;

		public LayoutViewTooltip(
			object owner,
			LayoutViewControl view,
			CardSearchSubsystem cardSearchSubsystem,
			CountInputSubsystem countInput)
		{
			Owner = owner;
			_view = view;
			_cardSearchSubsystem = cardSearchSubsystem;
			_countInput = countInput;
		}

		public void SubscribeEvents()
		{
			_view.MouseMove += mouseMove;
			_view.MouseLeave += mouseLeave;
			_view.CardIndexChanged += scrolled;
		}

		public void UnsubscribeEvents()
		{
			_view.MouseMove -= mouseMove;
			_view.MouseLeave -= mouseLeave;
			_view.CardIndexChanged -= scrolled;
		}

		private void scrolled(object sender) =>
			showFieldTooltip(Cursor.Position);

		private void mouseMove(object sender, MouseEventArgs e)
		{
			if (_view.IsSelectingText())
				Hide?.Invoke();
			else
				showFieldTooltip(Cursor.Position);
		}

		private void mouseLeave(object sender, EventArgs e) =>
			Hide?.Invoke();

		private void showFieldTooltip(Point position)
		{
			var cursorPosition = _view.PointToClient(position);
			var hitInfo = _view.CalcHitInfo(cursorPosition);

			if (hitInfo.AlignButtonDirection.HasValue)
			{
				Show?.Invoke(new TooltipModel
				{
					Id =
						$"{_view.Name}.{hitInfo.RowHandle}.{hitInfo.FieldName}.align",
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
			else if (_countInput.IsCountRectangle(hitInfo, cursorPosition, out var countRect))
			{
				Show?.Invoke(new TooltipModel
				{
					Id = $"{_view.Name}.{hitInfo.RowHandle}.edit_count",
					ObjectBounds = countRect,
					Control = _view,
					Title = "Edit count",
					Text =
						"Left click here: set count in deck\r\n" +
						"Alt + Left click here: set count in collection\r\n" +
						"When editing, Tab / Shift + Tab: switch to next / previous card\r\n\r\n" +
						"Right click: add 1 to deck\r\n" +
						"Middle click: remove 1 from deck\r\n" +
						"Alt + Right click: add 1 to collection\r\n" +
						"Alt + Middle click: remove 1 from collection\r\n" +
						"Ctrl + any above shortcut: add/remove 4 cards instead of 1\r\n\r\n" +
						"Note: clicks work anywhere inside card, not necessarily at the count label",
					Clickable = false
				});
			}
			else if (hitInfo.IsSortButton)
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
			else if (hitInfo.IsSearchButton)
			{
				string text;
				string title;
				string query = _cardSearchSubsystem.GetFieldValueQuery(
					hitInfo.FieldName,
					_view.GetText(hitInfo.RowHandle, hitInfo.FieldName));

				if (hitInfo.FieldName == nameof(Card.Image))
				{
					title = "Search similar cards";
					text = "Click to search cards similar to this one.\r\n" +
						"Similarity is determined by Text and GeneratedMana fields.\r\n" +
						"\r\n" +
						"Following term will be added to search bar\r\n" +
						query;
				}
				else
				{
					title = "Add to search";
					text =
						"Click to NARROW DOWN search result by cards matching this value\r\n\r\n" +
						"Following term will be added to search bar\r\n" +
						query + "\r\n\r\n" +
						"Hold Alt key when hovering to prevent showing this button. Helps selecting text in small fields.";
				}

				Show?.Invoke(new TooltipModel
				{
					Id =
						$"{_view.Name}.{hitInfo.RowHandle}.{hitInfo.FieldName}.search",
					ObjectBounds = hitInfo.ButtonBounds,
					Control = _view,
					Title = title,
					Text = text,
					Clickable = false
				});
			}
			else if (hitInfo.CustomButtonIndex >= 0)
			{
				bool isDeck = DeckEditorButtons.IsDeck(hitInfo.CustomButtonIndex);
				int delta = DeckEditorButtons.GetCountDelta(hitInfo.CustomButtonIndex);
				int absDelta = Math.Abs(delta);

				Show?.Invoke(new TooltipModel
				{
					Id = $"{_view.Name}.{hitInfo.RowHandle}.{hitInfo.CustomButtonIndex}",
					ObjectBounds = hitInfo.ButtonBounds,
					Control = _view,
					Title = $"{(delta > 0 ? "Add" : "Remove")} {absDelta} card{(absDelta == 1 ? string.Empty : "s")} {(delta > 0 ? "to" : "from")} {(isDeck ? "Deck" : "Collection")}",
					Text =
						$"{(absDelta == 1 ? string.Empty : "Ctrl + ")}{(isDeck ? string.Empty : "Alt + ")}{(delta > 0 ? "Right" : "Middle")} " +
						"mouse click on card image does the same",
					Clickable = false
				});
			}
			else if (
				hitInfo.RowDataSource != null &&
				hitInfo.FieldBounds.HasValue &&
				!Str.Equals(hitInfo.FieldName, nameof(Card.Image)))
			{
				Show?.Invoke(new TooltipModel
				{
					Id =
						$"{_view.Name}.{hitInfo.RowHandle}.{hitInfo.FieldName}",
					ObjectBounds = hitInfo.FieldBounds.Value,
					Control = _view,
					Title = hitInfo.FieldName,
					Text = getFieldTooltipText(hitInfo.RowHandle, hitInfo.FieldName),
					HighlightRanges = _view.GetHighlightTextRanges(hitInfo.RowHandle, hitInfo.FieldName),
					HighlightOptions = _view.HighlightOptions,
					Clickable = true
				});
			}
			else
			{
				Hide?.Invoke();
			}
		}

		private string getFieldTooltipText(int rowHandle, string field)
		{
			var text = new StringBuilder(_view.GetText(rowHandle, field));

			var card = (Card) _view.FindRow(rowHandle);

			if (Str.Equals(field, nameof(Card.Text)))
			{
				if (!string.IsNullOrEmpty(card.OriginalText))
					text
						.AppendLine()
						.AppendLine()
						.Append(nameof(Card.OriginalText)).Append(":")
						.AppendLine()
						.Append(card.OriginalText);

				var additionalFields = new StringBuilder();

				if (!string.IsNullOrEmpty(card.GeneratedMana))
					additionalFields
						.AppendLine()
						.Append(nameof(Card.GeneratedMana)).Append(": ").Append(card.GeneratedMana);

				var keywords = card.GetKeywords()
					.Concat(card.GetCastKeywords())
					.ToHashSet(Str.Comparer);

				if (keywords.Count > 0)
					additionalFields
						.AppendLine()
						.Append(nameof(KeywordDefinitions.Keywords)).Append(": ")
						.Append(string.Join(", ", keywords));

				additionalFields
					.AppendLine()
					.Append(nameof(KeywordDefinitions.Layout)).Append(": ").Append(card.Layout);

				if (!string.IsNullOrEmpty(card.ImageName))
					additionalFields
						.AppendLine()
						.Append(nameof(Card.ImageName)).Append(": ")
						.Append(card.ImageName);

				if (card.OtherFaceIds != null && card.OtherFaceIds.Count > 0)
					additionalFields.AppendLine()
						.Append(nameof(Card.Faces)).Append(": ")
						.Append(string.Join(", ", card.OtherFaces.Select(_=>_.NameEn)));

				if (additionalFields.Length > 0)
					text
						.AppendLine()
						.Append(additionalFields);
			}
			else if (Str.Equals(field, nameof(Card.Type)) && !string.IsNullOrEmpty(card.OriginalType))
				text
					.AppendLine()
					.AppendLine()
					.Append(nameof(Card.OriginalType)).Append(":")
					.AppendLine()
					.Append(card.OriginalType);
			else if (Str.Equals(field, nameof(Card.Number)))
			{
				if (card.MultiverseId.HasValue)
					text
						.AppendLine()
						.AppendLine()
						.Append(nameof(card.MultiverseId)).Append(": ").Append(card.MultiverseId.Value);
				if (!string.IsNullOrEmpty(card.Side))
				{
					text
						.AppendLine()
						.AppendLine()
						.Append(nameof(card.Side)).Append(": ").Append(card.Side);
				}
			}

			return text.ToString();
		}

		private readonly LayoutViewControl _view;
		private readonly CardSearchSubsystem _cardSearchSubsystem;
		private readonly CountInputSubsystem _countInput;

		public object Owner { get; }
	}
}
