using System.Windows.Forms;
using Mtgdb.Data;
using Mtgdb.Data.Index;
using Mtgdb.Data.Model;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public class DeckSearchSubsystem : SearchSubsystem<long, DeckModel>
	{
		public DeckSearchSubsystem(
			Control parent,
			SearchBar searchBar,
			UiConfigRepository uiConfigRepository,
			DeckSearcher searcher,
			DeckDocumentAdapter adapter,
			params LayoutViewControl[] views)
			: base(
				parent,
				searchBar,
				uiConfigRepository,
				searcher,
				adapter,
				views)
		{
		}

		protected override string GetLanguage() =>
			null;

		protected override IntellisenseSuggest CycleValue(TextInputState currentState, bool backward) =>
			((DeckSearcher) Searcher).CycleValue(currentState, backward);

		protected override SearchResult<long> Search(string query) =>
			((DeckSearcher) Searcher).Search(query);

		public bool IsLoaded =>
			Searcher.IsLoaded;

		public bool IsUpdating =>
			((DeckSearcher) Searcher).IsUpdating;
	}
}
