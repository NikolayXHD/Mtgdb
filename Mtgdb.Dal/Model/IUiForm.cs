using System;

namespace Mtgdb.Dal
{
	public interface IUiForm
	{
		bool CanUndo { set; }
		bool CanRedo { set; }
		string Language { get; set; }

		bool HideTooltips { get; set; }
		event Action LanguageChanged;

		void NextTab();
		void NewTab();
		void CloseTab();
	}
}