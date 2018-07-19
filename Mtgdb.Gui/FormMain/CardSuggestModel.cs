using JetBrains.Annotations;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Mtgdb.Index;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class CardSuggestModel : SuggestModel<int, Card>
	{
		[UsedImplicitly]
		public CardSuggestModel(CardSearcher searcher)
		: base(searcher)
		{
		}

		protected override IntellisenseSuggest Suggest(TextInputState searchState)
		{
			_language = Language;
			return Searcher.Spellchecker.Suggest(searchState, _language);
		}

		protected override bool IsSuggestUpToDate() =>
			_language == Language &&
			base.IsSuggestUpToDate();

		private string _language;

		public string Language =>
			Ui.LanguageController.Language;

		public UiModel Ui { get; set; }
	}
}