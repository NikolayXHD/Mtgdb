using System;
using System.Drawing;
using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public interface IFormRoot
	{
		bool CanUndo { set; }
		bool CanRedo { set; }

		bool HideTooltips { get; set; }
		bool ShowFilterPanels { get; set; }

		event Action ShowFilterPanelsChanged;

		void SelectNextTab();
		void SelectPreviousTab();

		void AddTab();

		void CloseTab();

		int TabsCount { get; }

		void OpenDeckInNewTab(Deck deck);

		bool LoadedGuiSettings { get; set; }
		bool ShowTextualFields { get; set; }
		bool ShowDeck { get; set; }
		bool ShowPartialCards { get; set; }
		bool ShowScroll { get; set; }

		CardSuggestModel CardSuggestModel { get; }
		DeckSuggestModel DeckSuggestModel { get; }

		TooltipController TooltipController { get; }

		UiModel UiModel { get; }
		Direction? SnapDirection { get; set; }
		Rectangle WindowArea { get; set; }
	}
}
