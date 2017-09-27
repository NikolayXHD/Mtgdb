namespace Mtgdb.Gui
{
	partial class FormMain
	{
		private void setupTooltips()
		{
			_toolTipController.SetTooltip("Deck areas",
				"Use right/middle mouse click to add/remove card.\r\n" +
				"Use Ctrl+Click or Ctrl+drag-n-drop to change quantity by 4.\r\n" +
				"Drag the card here to change deck area before dropping card.",
				_tabHeadersDeck);

			_toolTipController.SetTooltip(null,
				"Deck scroll position",
				_labelStatusScrollDeck,
				_panelIconStatusScrollDeck);

			_toolTipController.SetTooltip(null,
				"Search result scroll position",
				_labelStatusScrollCards,
				_panelIconStatusScrollCards);

			_toolTipController.SetTooltip(null,
				"Sets count",
				_panelIconStatusSets,
				_labelStatusSets);

			_toolTipController.SetTooltip("Collection cards count",
				"Use Alt+right/mdiddle mouse click to add/remove card to collection.\r\n" +
				"Use Alt+right/mdiddle mouse click to add/remove 4 copies.",
				_panelIconStatusCollection,
				_labelStatusCollection);

			_toolTipController.SetTooltip("Buttons filter mode",
				"Button filters are round checkable buttons grouped by 2 rows.\r\n" +
				"There are 3 possible modes: and, or, ignored.\r\n" +
				"To select between modes use Filter manager - the rightmost " +
				"buttons group in top panel.",
				_panelIconStatusFilterButtons,
				_labelStatusFilterButtons);

			_toolTipController.SetTooltip("Search string mode",
				"Search string is a wide text input in top panel.\r\n" +
				"There are 3 possible modes: and, or, ignored.\r\n" +
				"To select between modes use Filter manager - the rightmost " +
				"buttons group in top panel.",
				_panelIconStatusSearch,
				_labelStatusSearch);

			_toolTipController.SetTooltip("Filter by Collection mode",
				"When filtering by Collection, Search result only shows\r\n" +
				"cards present in your Collection.\r\n\r\n" +
				"Filtering by Collection can be enabled or disabled using Filter manager - " +
				"the rightmost buttons group in top panel.\r\n" +
				"There are 3 possible modes: and, or, ignored.\r\n",
				_panelIconStatusFilterCollection,
				_labelStatusFilterCollection);

			_toolTipController.SetTooltip("Filter by Deck mode",
				"When filtering by Deck, Search result only shows\r\n" +
				"cards present in your Deck\r\n\r\n" +
				"Filtering by Deck can be enabled or disabled using Filter manager - " +
				"the rightmost buttons group in top panel.\r\n" +
				"There are 3 possible modes: and, or, ignored.\r\n",
				_panelIconStatusFilterDeck,
				_labelStatusFilterDeck);

			_toolTipController.SetTooltip(null,
				"Filter by Legality status",
				_panelIconStatusFilterLegality,
				_labelStatusFilterLegality);

			_toolTipController.SetTooltip("Filter by keyword abilities",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use TOP row to REQUIRE ALL checked keywords in a card.\r\n" +
				"Use BOTTOM row to REQUIRE ANY checked keyword in a card.\r\n\r\n"+
				"Keyword examples: Flying, First Strike, Haste, ...",
				FilterAbility);

			_toolTipController.SetTooltip("Filter by spell Type",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use BOTTOM row to PROHIBIT any UNCHECKED types in a card.\r\n" +
				"Use TOP row to REQUIRE ALL checked types in a card.\r\n\r\n" +
				"Example: to see all artifacts check artifact in TOP row.\r\n\r\n" +
				"N/A means any Type different from values in this filter.",
				FilterType);

			_toolTipController.SetTooltip("Filter by Rarity",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use BOTTOM row to PROHIBIT any UNCHECKED value.\r\n" +
				"The card cannot have more than 1 Rarity value so there is no\r\n" +
				"point in using the TOP row.\r\n\r\n" +
				"N/A means any Rarity different from values in this filter.",
				FilterRarity);

			_toolTipController.SetTooltip("Filter Manager",
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

			_toolTipController.SetTooltip("Filter by Mana symbol in card Text",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use RIGHT row to REQURE ALL checked values to be present in one card.\r\n" +
				"Use mode selector above to switch the way LEFT row acts:\r\n" +
				"  - mode: PROHIBIT UNCHECKED values\r\n" +
				"  + mode: REQUIRE ANY checked value\r\n\r\n" +
				"N/A means the card has NONE of the symbols from this filter.",
				FilterManaAbility);

			_toolTipController.SetTooltip("Filter by Mana Cost",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use RIGHT row to REQURE ALL checked values to be present in one card.\r\n" +
				"Use mode selector above to switch the way LEFT row acts:\r\n" +
				"  - mode: PROHIBIT UNCHECKED values\r\n" +
				"  + mode: REQUIRE ANY checked value\r\n\r\n" +
				"N/A means the card has NONE of the symbols from this filter.",
				FilterManaCost);

			_toolTipController.SetTooltip("Filter by Generated Mana",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use RIGHT row to REQURE ALL checked values to be present in one card.\r\n" +
				"Use LEFT row to REQUIRE ANY checked value",
				FilterGeneratedMana);

			_toolTipController.SetTooltip("Filter by Converted Mana Cost",
				"Use Middle mouse click to RESET.\r\n\r\n" +
				"Use LEFT row to REQUIRE ANY checked value\r\n" +
				"The card cannot have more than 1 Converted Mana Cost value\r\n" +
				"so there is no point in using the RIGHT row.",
				FilterCmc);

			_toolTipController.SetTooltip("Visibility of Prohibiting buttons",
				"Use Prohibiting buttons to PROHIBIT checked values in a card\r\n" +
				"I guess you will not often need it so by default those buttons are hidden.",
				_buttonShowProhibit);

			_toolTipController.SetTooltip("Mana Ability filter LEFT row mode",
				"- mode: PROHIBIT UNCHECKED values\r\n" +
				"+ mode: REQUIRE ANY checked value",
				_buttonExcludeManaAbility);

			_toolTipController.SetTooltip("Mana Cost filter LEFT row mode",
				"- mode: PROHIBIT UNCHECKED values\r\n" +
				"+ mode: REQUIRE ANY checked value",
				_buttonExcludeManaCost);

			_toolTipController.SetTooltip("Search string",
				"Ctrl+SPACE to get intellisense\r\n" +
				"Enter to apply\r\n" +
				"Ctrl+Backspace to delete one word\r\n" +
				"F1 to learn searh string syntax\r\n\r\n" +
				"Ctrl+F to focus search input",
				_findCustomPanel,
				_findEditor,
				_panelIconSearch);

			_toolTipController.SetTooltip("Filter by Legality",
				"Select format",
				_menuLegalityFormat);

			_toolTipController.SetTooltip("Filter by Legality",
				"Show cards LEGAL in selected format",
				_buttonLegalityAllowLegal);

			_toolTipController.SetTooltip("Filter by Legality",
				"Show cards RESTRICTED in selected format",
				_buttonLegalityAllowRestricted);

			_toolTipController.SetTooltip("Filter by Legality",
				"Show cards BANNED in selected format",
				_buttonLegalityAllowBanned);

			_toolTipController.SetTooltip("Show duplicates",
				"Card duplicates have the same name and by rules\r\n" +
				"they are considered to be the same card.\r\n\r\n" +
				"Some cards are released in muliple Sets. Also in most Sets\r\n" +
				"Basic lands are released in multiple variants.",
				_buttonShowDuplicates);

			_toolTipController.SetCustomTooltip(_tooltipViewCards);
		}
	}
}
