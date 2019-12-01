namespace Mtgdb.Controls
{
	public partial class DeckListControl
	{
		private void setupTooltips(TooltipController controller)
		{
			controller.SetTooltip(_tooltipOwner, "Deck name", "Type deck name.\r\n" +
				"press Enter to apply\r\n" +
				"press Esc to cancel",
				_textboxRename,
				_panelRename);

			controller.SetTooltip(_tooltipOwner,
				() =>
				{
					if (!_searchSubsystem.IsApplied)
						return "Search bar for decks: receiving user input";

					string errorMessage = _searchSubsystem.SearchResult?.ParseErrorMessage;
					if (errorMessage != null)
						return "Search bar for decks: Syntax error";

					return "Search bar for decks";
				},
				() =>
				{
					if (!_searchSubsystem.IsApplied)
						return (_uiConfigRepository.Config.AutoApplySearchBar
								? "Press ENTER or wait 1 second to apply"
								: "Press ENTER to apply") + "\r\n\r\n" +
							"Auto-apply after 1 second can be turned on / off\r\n" +
							"from Advanced Settings menu in window title";

					string errorMessage = _searchSubsystem.SearchResult?.ParseErrorMessage;
					if (errorMessage != null)
						return errorMessage;

					return
						"Filter decks, example queries:\r\n" +
						// ReSharper disable once StringLiteralTypo
						"\tname: affin*\r\n" +
						"\tmana: \\{w\\} AND \\{u\\}\r\n\r\n" +
						"Ctrl+SPACE to get intellisense\r\n" +
						"Enter to apply\r\n" +
						"Ctrl+Backspace to delete one word\r\n\r\n" +
						"F1 to learn search bar query syntax\r\n" +
						"Middle mouse click to clear";
				},
				_searchBar,
				_searchBar.Input);

			controller.SetTooltip(_tooltipOwner,
				"Deck list sort order",
				"Sort buttons are located over textual fields of decks.\r\n" +
				"Click sort button to sort by field or change sort direction.\r\n" +
				"Shift + Click to add field to sort priorities,\r\n" +
				"Ctrl + Click to remove field from sort priorities.\r\n\r\n" +
				"When all explicit sort criteria are equal, decks are ordered " +
				"by relevance to search result, then by order they were created.\r\n\r\n" +
				"NOTE: currently opened deck is is always first in the list",
				_labelSortStatus);

			string filterMode(FilterByDeckMode mode) =>
				_menuFilterByDeckMode.MenuValues[(int) mode];

			controller.SetTooltip(_tooltipOwner,
				"Filter by deck mode",
				"Controls how search result of cards is affected by decks.\r\n\r\n" +
				$"- {filterMode(FilterByDeckMode.Ignored)}\r\n" +
				"    decks do not affect search result of cards\r\n" +
				$"- {filterMode(FilterByDeckMode.CurrentDeck)}\r\n" +
				"    show cards present in currently opened deck\r\n" +
				$"- {filterMode(FilterByDeckMode.FilteredSavedDecks)}\r\n" +
				"    show cards present in any saved deck from list below matching search criteria for " +
				"saved decks on the left",
				_menuFilterByDeckMode);

			controller.SetCustomTooltip(_layoutViewTooltip);
		}
	}
}
