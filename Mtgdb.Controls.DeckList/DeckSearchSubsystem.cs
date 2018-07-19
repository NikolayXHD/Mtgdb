using System.Windows.Forms;
using Mtgdb.Index;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public class DeckSearchSubsystem : SearchSubsystem<int, DeckModel>
	{
		public DeckSearchSubsystem(
			Control parent,
			RichTextBox findEditor,
			Panel panelSearchIcon,
			ListBox listBoxSuggest,
			DeckSearcher searcher,
			DeckDocumentAdapter adapter,
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
			null;

		protected override IntellisenseSuggest CycleValue(TextInputState currentState, bool backward) =>
			((DeckSearcher) Searcher).CycleValue(currentState, backward);

		protected override SearchResult<int> Search(string query) =>
			((DeckSearcher) Searcher).Search(query);

		public bool IsLoaded =>
			Searcher.IsLoaded;

		public bool IsUpdating =>
			((DeckSearcher) Searcher).IsUpdating;
	}
}