namespace Mtgdb.Gui
{
	partial class FormMain
	{
		private void setupTooltips(IFormRoot formRoot)
		{
			var controller = formRoot.TooltipController;

			controller.SetTooltip(this,
				"Deck areas",
				"Use right/middle mouse click to add/remove card.\r\n" +
				"Use Ctrl+Click or Ctrl+drag-n-drop to change quantity by 4.\r\n" +
				"Drag the card here to change deck area before dropping card.",
				_tabHeadersDeck);

			controller.SetTooltip(this,
				null,
				"Deck scroll position",
				_labelStatusScrollDeck,
				_panelIconStatusScrollDeck);

			controller.SetTooltip(this,
				null,
				"Search result scroll position",
				_labelStatusScrollCards,
				_panelIconStatusScrollCards);

			controller.SetTooltip(this,
				null,
				"Sets count",
				_panelIconStatusSets,
				_labelStatusSets);

			controller.SetTooltip(this,
				"Collection cards count",
				"Use Alt+right/middle mouse click to add/remove card to collection.\r\n" +
				"Use Ctrl+Alt+right/middle mouse click to add/remove 4 copies.",
				_panelIconStatusCollection,
				_labelStatusCollection);

			controller.SetTooltip(this,
				"Buttons filter mode",
				"Button filters are round checkable buttons grouped by 2 rows.\r\n" +
				"There are 3 possible modes: and, or, ignored.\r\n" +
				"To select between modes use Filter manager - the rightmost " +
				"buttons group in top panel.",
				_panelIconStatusFilterButtons,
				_labelStatusFilterButtons);

			controller.SetTooltip(this,
				"Search string mode",
				"Search string is a wide text input in top panel.\r\n" +
				"There are 3 possible modes: and, or, ignored.\r\n" +
				"To select between modes use Filter manager - the rightmost " +
				"buttons group in top panel.",
				_panelIconStatusSearch,
				_labelStatusSearch);

			controller.SetTooltip(this,
				"Filter by Collection mode",
				"When filtering by Collection, Search result only shows\r\n" +
				"cards present in your Collection.\r\n\r\n" +
				"Filtering by Collection can be enabled or disabled using Filter manager - " +
				"the rightmost buttons group in top panel.\r\n" +
				"There are 3 possible modes: and, or, ignored.\r\n",
				_panelIconStatusFilterCollection,
				_labelStatusFilterCollection);

			controller.SetTooltip(this,
				"Filter by Deck mode",
				"When filtering by Deck, Search result only shows\r\n" +
				"cards present in your Deck\r\n\r\n" +
				"Filtering by Deck can be enabled or disabled using Filter manager - " +
				"the rightmost buttons group in top panel.\r\n" +
				"There are 3 possible modes: and, or, ignored.\r\n",
				_panelIconStatusFilterDeck,
				_labelStatusFilterDeck);

			controller.SetTooltip(this,
				null,
				"Filter by Legality status",
				_panelIconStatusFilterLegality,
				_labelStatusFilterLegality);

			controller.SetTooltip(this,
				"Search result Sort order",
				"Sort buttons are located over textual fields in Search result.\r\n" +
				"Click sort button to sort by field or change sort direction.\r\n" +
				"Shift + Click to add field to sort priorities,\r\n" +
				"Ctrl + Click to remove field from sort priorities.\r\n\r\n" +
				"When all explicit sort criteria are equal, cards are ordered " +
				"by relevance to search result, then by order in AllSets-x.json file.",
				_panelIconStatusSort,
				_labelStatusSort);

			var keywordHint = "Search text lets you search by keywords too. It provides much more keyword values.\r\n" +
				"Use intellisense to see available values:\r\n" +
				"- type `keywords:`\r\n" +
				"- hit Ctrl+Space\r\n" +
				"- begin typing keyword";

			controller.SetTooltip(this,
				"Filter by keyword abilities",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use TOP row to REQUIRE ALL checked keywords in a card.\r\n" +
				"Use BOTTOM row to REQUIRE ANY checked keyword in a card.\r\n\r\n" +
				"Keyword examples: Flying, First Strike, Haste, ...\r\n\r\n"
				+ keywordHint,
				FilterAbility);

			controller.SetTooltip(this,
				"Filter by CAST RELATED keyword abilities",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use TOP row to REQUIRE ALL checked keywords in a card.\r\n" +
				"Use BOTTOM row to REQUIRE ANY checked keyword in a card.\r\n\r\n" +
				"Keyword examples: Cascade, Flashback, Madness, ...\r\n\r\n" +
				keywordHint,
				FilterCastKeyword);

			controller.SetTooltip(this,
				"Filter by spell Type",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use BOTTOM row to PROHIBIT any UNCHECKED types in a card.\r\n" +
				"Use TOP row to REQUIRE ALL checked types in a card.\r\n\r\n" +
				"Example: to see all artifacts check artifact in TOP row.\r\n\r\n" +
				"N/A means any Type different from values in this filter.",
				FilterType);

			controller.SetTooltip(this,
				"Filter Manager",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Filter manager selects between AND / OR mode or DISABLES the following filter sources:\r\n" +
				"  * Filter buttons\r\n" +
				"  * Search text\r\n" +
				"  * Legality filter\r\n" +
				"  * Filter to cards present in your collection\r\n" +
				"  * Filter to cards present in your deck\r\n\r\n" +
				"Use TOP row set AND mode.\r\n" +
				"Use BOTTOM row set OR mode.\r\n" +
				"Uncheck the source in both rows to set DISABLED mode.",
				FilterManager);

			controller.SetTooltip(this,
				"Filter by Rarity",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use LEFT row to PROHIBIT any UNCHECKED value.\r\n" +
				"The card cannot have more than 1 Rarity value so there is no\r\n" +
				"point in using the RIGHT row.\r\n\r\n" +
				"N/A means any Rarity different from values in this filter.",
				FilterRarity);

			controller.SetTooltip(this,
				"Filter by Mana symbol in card Text",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use RIGHT row to REQURE ALL checked values to be present in one card.\r\n" +
				"Use mode selector above to switch the way LEFT row acts:\r\n" +
				"  - mode: PROHIBIT UNCHECKED values\r\n" +
				"  + mode: REQUIRE ANY checked value\r\n\r\n" +
				"N/A means the card has NONE of the symbols from this filter.",
				FilterManaAbility);

			controller.SetTooltip(this,
				"Filter by Mana Cost",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use RIGHT row to REQURE ALL checked values to be present in one card.\r\n" +
				"Use mode selector above to switch the way LEFT row acts:\r\n" +
				"  - mode: PROHIBIT UNCHECKED values\r\n" +
				"  + mode: REQUIRE ANY checked value\r\n\r\n" +
				"N/A means the card has NONE of the symbols from this filter.",
				FilterManaCost);

			controller.SetTooltip(this,
				"Filter by Generated Mana",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use RIGHT row to REQURE ALL checked values to be present in one card.\r\n" +
				"Use LEFT row to REQUIRE ANY checked value",
				FilterGeneratedMana);

			controller.SetTooltip(this,
				"Filter by Converted Mana Cost",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use LEFT row to REQUIRE ANY checked value\r\n" +
				"The card cannot have more than 1 Converted Mana Cost value\r\n" +
				"so there is NO point in using the RIGHT row.",
				FilterCmc);

			controller.SetTooltip(this,
				"Filter by Layout",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use LEFT row to REQUIRE ANY checked value\r\n" +
				"The card cannot have more than 1 Layout value\r\n" +
				"so there is NO point in using the RIGHT row.",
				FilterLayout);

			controller.SetTooltip(this,
				"Visibility of Prohibiting buttons",
				"Use Prohibiting buttons to PROHIBIT checked values in a card\r\n" +
				"I guess you will not often need it so by default those buttons are hidden.",
				_buttonShowProhibit);

			controller.SetTooltip(this,
				"Mana Ability filter LEFT row mode",
				"- mode: PROHIBIT UNCHECKED values\r\n" +
				"+ mode: REQUIRE ANY checked value",
				_buttonExcludeManaAbility);

			controller.SetTooltip(this,
				"Mana Cost filter LEFT row mode",
				"- mode: PROHIBIT UNCHECKED values\r\n" +
				"+ mode: REQUIRE ANY checked value",
				_buttonExcludeManaCost);

			controller.SetTooltip(this,
				() => _searchStringSubsystem.SearchResult?.ParseErrorMessage != null
					? "Syntax error"
					: "Search text",
				() => _searchStringSubsystem.SearchResult?.ParseErrorMessage ??
					"Ctrl+SPACE to get intellisense\r\n" +
					"Enter to apply\r\n" +
					"Ctrl+Backspace to delete one word\r\n" +
					"F1 to learn searh string syntax\r\n\r\n" +
					"Ctrl+F to focus search input",
				_panelSearch,
				_searchEditor,
				_panelIconSearch);

			controller.SetTooltip(this,
				"Search text examples",
				"Opens menu with search text examples.\r\n" +
				"Same menu is opened by pressing F1.",
				_buttonSearchExamplesDropDown);

			controller.SetTooltip(this,
				"Filter by Legality",
				"Select format",
				_menuLegalityFormat);

			controller.SetTooltip(this,
				"Filter by Legality",
				"Show cards LEGAL in selected format",
				_buttonLegalityAllowLegal);

			controller.SetTooltip(this,
				"Filter by Legality",
				"Show cards RESTRICTED in selected format",
				_buttonLegalityAllowRestricted);

			controller.SetTooltip(this,
				"Filter by Legality",
				"Show cards BANNED in selected format",
				_buttonLegalityAllowBanned);

			controller.SetTooltip(this,
				"Show duplicates",
				"Card duplicates have the same name and by rules\r\n" +
				"they are considered to be the same card.\r\n\r\n" +
				"Some cards are released in muliple Sets. Also in most Sets\r\n" +
				"Basic lands are released in multiple variants.",
				_buttonShowDuplicates);

			controller.SetCustomTooltip(_tooltipViewCards);
			controller.SetCustomTooltip(_tooltipViewDeck);

			controller.SubscribeToEvents();
		}
	}
}