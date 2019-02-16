using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	partial class FormMain
	{
		private void setupTooltips(TooltipController defaultTooltips, TooltipController quickFilterTooltips)
		{
			defaultTooltips.SetTooltip(this,
				"Deck zones",
				"Drag the card here to change deck area before dropping it.\r\n" +
				"Right/middle mouse click a card to add/remove it.\r\n" +
				"Ctrl+Click or Ctrl+drag-n-drop a card to change its quantity by 4.",
				_tabHeadersDeck);

			defaultTooltips.SetTooltip(this,
				null,
				"Sets count",
				_panelIconStatusSets,
				_labelStatusSets);

			defaultTooltips.SetTooltip(this,
				"Collection cards count",
				"Use Alt+right/middle mouse click to add/remove card to collection.\r\n" +
				"Use Ctrl+Alt+right/middle mouse click to add/remove 4 copies.",
				_panelIconStatusCollection,
				_labelStatusCollection);

			defaultTooltips.SetTooltip(this,
				"Buttons filter mode",
				"Button filters are round checkable buttons grouped by 2 rows.\r\n" +
				"There are 3 possible modes: and, or, ignored.\r\n" +
				"To select between modes use Filter manager - the rightmost " +
				"buttons group in top panel.",
				_panelIconStatusFilterButtons,
				_labelStatusFilterButtons);

			defaultTooltips.SetTooltip(this,
				"Search bar mode",
				"Search bar is a wide text input in top panel.\r\n" +
				"There are 3 possible modes: and, or, ignored.\r\n" +
				"To select between modes use Filter manager - the rightmost " +
				"buttons group in top panel.",
				_panelIconStatusSearch,
				_labelStatusSearch);

			defaultTooltips.SetTooltip(this,
				"Filter by Collection mode",
				"When filtering by Collection, Search result only shows\r\n" +
				"cards present in your Collection.\r\n\r\n" +
				"Filtering by Collection can be enabled or disabled using Filter manager - " +
				"the rightmost buttons group in top panel.\r\n" +
				"There are 3 possible modes: and, or, ignored.\r\n",
				_panelIconStatusFilterCollection,
				_labelStatusFilterCollection);

			defaultTooltips.SetTooltip(this,
				"Filter by Deck mode",
				"When filtering by Deck, Search result only shows\r\n" +
				"cards present in your current Deck.\r\n\r\n" +
				"Alternatively you can search cards present in any of your decks shown in `deck list` " +
				"tab. To do this\r\n" +
				"- open `deck list` tab (it's next to `main deck` / `sideboard` / `sample hand` tabs)\r\n" +
				"- if necessary narrow down the list of visible saved decks by using search bar " +
				"in `deck list` tab\r\n" +
				"- change selected value in `filter by deck` menu located in top right " +
				"of `deck list` panel.\r\n\r\n" +
				"Filtering by Deck can be enabled or disabled using Filter manager - " +
				"the rightmost buttons group in top panel.\r\n" +
				"There are 3 possible modes: and, or, ignored.\r\n",
				_panelIconStatusFilterDeck,
				_labelStatusFilterDeck);

			defaultTooltips.SetTooltip(this,
				null,
				"Filter by Legality status",
				_panelIconStatusFilterLegality,
				_labelStatusFilterLegality);

			defaultTooltips.SetTooltip(this,
				"Search result Sort order",
				"Sort buttons are located over textual fields in Search result.\r\n" +
				"Click sort button to sort by field or change sort direction.\r\n" +
				"Shift + Click to add field to sort priorities,\r\n" +
				"Ctrl + Click to remove field from sort priorities.\r\n\r\n" +
				"When all explicit sort criteria are equal, cards are ordered " +
				"by relevance to search result, then by order in AllSets-x.json file.",
				_panelIconStatusSort,
				_labelStatusSort);

			setFilterButtonTooltips(defaultTooltips, quickFilterTooltips);

			defaultTooltips.SetTooltip(this,
				"Visibility of Prohibiting buttons",
				"Use Prohibiting buttons to PROHIBIT checked values in a card\r\n" +
				"I guess you will not often need it so by default those buttons are hidden.",
				_buttonShowProhibit);

			defaultTooltips.SetTooltip(this,
				"Mana Ability filter LEFT row mode",
				"- mode: PROHIBIT UNCHECKED values\r\n" +
				"+ mode: REQUIRE ANY checked value",
				_buttonExcludeManaAbility);

			defaultTooltips.SetTooltip(this,
				"Mana Cost filter LEFT row mode",
				"- mode: PROHIBIT UNCHECKED values\r\n" +
				"+ mode: REQUIRE ANY checked value",
				_buttonExcludeManaCost);

			defaultTooltips.SetTooltip(this,
				() => _cardSearch.SearchResult?.ParseErrorMessage != null
					? "Syntax error"
					: "Search bar",
				() => _cardSearch.SearchResult?.ParseErrorMessage ??
					"Narrows down the list of cards below based on a query you type. Example query:\r\n" +
					"TextEn: \"counter target spell\"\r\n\r\n" +
					"Ctrl+SPACE to get intellisense\r\n" +
					"Enter to apply\r\n" +
					"Ctrl+Backspace to delete one word\r\n" +
					"F1 to learn search bar query syntax\r\n\r\n" +
					"Ctrl+F to focus search bar\r\n" +
					"Middle mouse click to clear",
				_searchBar,
				_searchBar.Input,
				_panelIconSearch);

			defaultTooltips.SetTooltip(this,
				"Search query examples",
				"Opens menu with search query examples.\r\n" +
				"Same menu is opened by pressing F1.",
				_dropdownSearchExamples);

			defaultTooltips.SetTooltip(this,
				"Filter by Legality",
				"Select format\r\n\r\n" +
				"Middle mouse click to reset filter by legality",
				_menuLegality,
				_panelIconLegality);

			defaultTooltips.SetTooltip(this,
				"Filter by Legality",
				"Show cards LEGAL in selected format\r\n\r\n" +
				"Middle mouse click to reset filter by legality",
				_buttonLegalityAllowLegal);

			defaultTooltips.SetTooltip(this,
				"Filter by Legality",
				"Show cards RESTRICTED in selected format\r\n\r\n" +
				"Middle mouse click to reset filter by legality",
				_buttonLegalityAllowRestricted);

			defaultTooltips.SetTooltip(this,
				"Filter by Legality",
				"Show cards BANNED in selected format\r\n\r\n" +
				"Middle mouse click to reset filter by legality",
				_buttonLegalityAllowBanned);

			defaultTooltips.SetTooltip(this,
				"Filter by Legality",
				"Show cards which will SOON BECOME LEGAL in selected format",
				_buttonLegalityAllowFuture);

			defaultTooltips.SetTooltip(this,
				"Show duplicates",
				"Card duplicates have the same name and by rules\r\n" +
				"they are considered to be the same card.\r\n\r\n" +
				"Some cards are released in multiple Sets. Also in most Sets\r\n" +
				"Basic lands are released in multiple variants.",
				_buttonShowDuplicates);

			defaultTooltips.SetTooltip(this,
				null,
				"Deck panel visibility",
				_buttonHideDeck);

			const string hideScrollText = "Click to toggle scrollbar visibility";

			defaultTooltips.SetTooltip(this,
				null,
				hideScrollText,
				_buttonShowScrollCards);

			defaultTooltips.SetTooltip(this,
				null,
				hideScrollText,
				_buttonShowScrollDeck);

			defaultTooltips.SetTooltip(this,
				null,
				"Deck panel scroll position",
				_labelStatusScrollDeck);

			defaultTooltips.SetTooltip(this,
				null,
				"Search result scroll position",
				_labelStatusScrollCards);

			defaultTooltips.SetTooltip(this,
				"Partial cards visibility",
				"Depending on window size displayed cards may lack some horizontal or vertical " +
				"space at window borders.\r\n\r\n" +
				"Enable partial cards to display more cards.\r\n" +
				"Disable partial cards to completely display card image and text whenever possible.",
				_buttonShowPartialCards);

			defaultTooltips.SetTooltip(this,
				null,
				"Card text visibility",
				_buttonShowText);

			defaultTooltips.SetTooltip(this,
				"Reset all filters",
				"Resets all search / filter controls to their default state " +
				"in order to begin new search from scratch.",
				_buttonResetFilters);

			defaultTooltips.SetCustomTooltip(_tooltipViewCards);
			defaultTooltips.SetCustomTooltip(_tooltipViewDeck);
		}

		private void setFilterButtonTooltips(TooltipController defaultTooltips, TooltipController quickFilterTooltips)
		{
			setRegularTooltip(defaultTooltips, FilterAbility, "Filter by keyword abilities", "Use Middle mouse click to RESET.\r\n\r\n" +
				"Use TOP row to REQUIRE ALL checked keywords in a card.\r\n" +
				"Use BOTTOM row to REQUIRE ANY checked keyword in a card.\r\n\r\n" +
				"Keyword examples: Flying, First Strike, Haste, ...", true);
			quickFilterTooltips.SetCustomTooltip(new QuickFilterTooltip(FilterAbility, this)
			{
				PreferHorizontalShift = true
			});

			setRegularTooltip(defaultTooltips, FilterCastKeyword, "Filter by CAST RELATED keyword abilities", "Use Middle mouse click to RESET.\r\n\r\n" +
				"Use TOP row to REQUIRE ALL checked keywords in a card.\r\n" +
				"Use BOTTOM row to REQUIRE ANY checked keyword in a card.\r\n\r\n" +
				"Keyword examples: Cascade, Flashback, Madness, ...", true);
			quickFilterTooltips.SetCustomTooltip(new QuickFilterTooltip(FilterCastKeyword, this)
			{
				PreferHorizontalShift = true
			});

			setRegularTooltip(defaultTooltips, FilterType, "Filter by spell Type", "Use Middle mouse click to RESET.\r\n\r\n" +
				"Use BOTTOM row to PROHIBIT any UNCHECKED types in a card.\r\n" +
				"Use TOP row to REQUIRE ALL checked types in a card.\r\n\r\n" +
				"Example: to see all artifacts check artifact in TOP row.\r\n\r\n" +
				"N/A means any Type different from values in this filter.", false);
			quickFilterTooltips.SetCustomTooltip(new QuickFilterTooltip(FilterType, this));

			setRegularTooltip(defaultTooltips, FilterManager, "Filter Manager", "Use Middle mouse click to RESET.\r\n\r\n" +
				"Filter manager selects between AND / OR mode or DISABLES the following filter sources:\r\n" +
				"  * Filter buttons\r\n" +
				"  * Search bar\r\n" +
				"  * Legality filter\r\n" +
				"  * Filter to cards present in your collection\r\n" +
				"  * Filter to cards present in your deck\r\n\r\n" +
				"Use TOP row set AND mode.\r\n" +
				"Use BOTTOM row set OR mode.\r\n" +
				"Uncheck the source in both rows to set DISABLED mode.", false);
			quickFilterTooltips.SetCustomTooltip(new QuickFilterTooltip(FilterManager, this));

			setRegularTooltip(defaultTooltips, FilterRarity, "Filter by Rarity", "Use Middle mouse click to RESET.\r\n\r\n" +
				"Use LEFT row to PROHIBIT any UNCHECKED value.\r\n" +
				"The card cannot have more than 1 Rarity value so there is no\r\n" +
				"point in using the RIGHT row.\r\n\r\n" +
				"N/A means any Rarity different from values in this filter.", false);
			quickFilterTooltips.SetCustomTooltip(new QuickFilterTooltip(FilterRarity, this));

			setRegularTooltip(defaultTooltips, FilterManaAbility, "Filter by Mana symbol in card Text", "Use Middle mouse click to RESET.\r\n\r\n" +
				"Use RIGHT row to REQUIRE ALL checked values to be present in one card.\r\n" +
				"Use mode selector above to switch the way LEFT row acts:\r\n" +
				"  - mode: PROHIBIT UNCHECKED values\r\n" +
				"  + mode: REQUIRE ANY checked value\r\n\r\n" +
				"N/A means the card has NONE of the symbols from this filter.", false);
			quickFilterTooltips.SetCustomTooltip(new QuickFilterTooltip(FilterManaAbility, this));

			setRegularTooltip(defaultTooltips, FilterManaCost, "Filter by Mana Cost", "Use Middle mouse click to RESET.\r\n\r\n" +
				"Use RIGHT row to REQUIRE ALL checked values to be present in one card.\r\n" +
				"Use mode selector above to switch the way LEFT row acts:\r\n" +
				"  - mode: PROHIBIT UNCHECKED values\r\n" +
				"  + mode: REQUIRE ANY checked value\r\n\r\n" +
				"N/A means the card has NONE of the symbols from this filter.", false);
			quickFilterTooltips.SetCustomTooltip(new QuickFilterTooltip(FilterManaCost, this));

			setRegularTooltip(defaultTooltips, FilterGeneratedMana, "Filter by Generated Mana", "Use Middle mouse click to RESET.\r\n\r\n" +
				"Use RIGHT row to REQUIRE ALL checked values to be present in one card.\r\n" +
				"Use LEFT row to REQUIRE ANY checked value", false);
			quickFilterTooltips.SetCustomTooltip(new QuickFilterTooltip(FilterGeneratedMana, this));

			setRegularTooltip(defaultTooltips, FilterCmc, "Filter by Converted Mana Cost", "Use Middle mouse click to RESET.\r\n\r\n" +
				"Use LEFT row to REQUIRE ANY checked value\r\n" +
				"The card cannot have more than 1 Converted Mana Cost value\r\n" +
				"so there is NO point in using the RIGHT row.", false);

			setRegularTooltip(defaultTooltips, FilterLayout, "Filter by Layout", "Use Middle mouse click to RESET.\r\n\r\n" +
				"Use LEFT row to REQUIRE ANY checked value\r\n" +
				"The card cannot have more than 1 Layout value\r\n" +
				"so there is NO point in using the RIGHT row.", false);
			quickFilterTooltips.SetCustomTooltip(new QuickFilterTooltip(FilterLayout, this));
		}

		private void setRegularTooltip(TooltipController defaultTooltips, QuickFilterControl control, string name, string description, bool isKeyword)
		{
			if (isKeyword)
			{
				description += "\r\n\r\n" +
					"Search bar lets you search by keywords too. It provides much more keyword values.\r\n" +
					"Use intellisense to see available values:\r\n" +
					"- type keywords:\r\n" +
					"- hit Ctrl + Space\r\n" +
					"- begin typing keyword";
			}

			if (control.EnableCostBehavior)
			{
				description += "\r\n\r\n" +
					"Left-clicking these buttons may lead to some buttons different from the one you clicked " +
					"become checked or unchecked automatically.\r\n\r\n" +
					"Right-click the button to avoid auto checking / unchecking other buttons.";
			}

			defaultTooltips.SetTooltip(this,
				name,
				description,
				control);
		}
	}
}