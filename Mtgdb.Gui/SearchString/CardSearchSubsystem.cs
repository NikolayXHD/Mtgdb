using System.Windows.Forms;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Mtgdb.Controls;
using Mtgdb.Index;

namespace Mtgdb.Gui
{
	public class CardSearchSubsystem : SearchSubsystem<int, Card>
	{
		public CardSearchSubsystem(
			Control parent,
			RichTextBox findEditor,
			Panel panelSearchIcon,
			ListBox listBoxSuggest,
			CardSearcher searcher,
			CardDocumentAdapter adapter,
			params LayoutViewControl[] layoutViews)
			: base(
				parent,
				findEditor,
				panelSearchIcon,
				listBoxSuggest,
				searcher,
				adapter,
				layoutViews)
		{
		}

		protected override string GetLanguage() =>
			Ui.LanguageController.Language;

		protected override IntellisenseSuggest CycleValue(TextInputState currentState, bool backward) =>
			((CardSearcher) Searcher).Spellchecker.CycleValue(currentState, backward, ((CardSuggestModel) SuggestModel).Language);

		protected override SearchResult<int> Search(string query) =>
			Searcher.Search(query, GetLanguage());

		public override string GetFieldValueQuery(string fieldName, string fieldValue, bool useAndOperator = false)
		{
			if (fieldName == nameof(Card.Image))
				fieldName = CardQueryParser.Like;

			return base.GetFieldValueQuery(fieldName, fieldValue, useAndOperator);
		}

		public UiModel Ui { get; set; }
	}
}