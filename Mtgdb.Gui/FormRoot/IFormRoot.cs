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

		void AddTab(Action<object> onCreated);
		void AddTab();

		void CloseTab();

		int TabsCount { get; }

		bool LoadedGuiSettings { get; set; }
		bool ShowTextualFields { get; set; }
		bool ShowDeck { get; set; }
		bool ShowPartialCards { get; set; }

		SuggestModel SuggestModel { get; }
		TooltipController TooltipController { get; }

		UiModel UiModel { get; }
		Direction? SnapDirection { get; set; }
		Rectangle WindowArea { get; set; }
	}
}
