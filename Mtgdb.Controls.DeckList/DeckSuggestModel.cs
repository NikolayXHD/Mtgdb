using JetBrains.Annotations;
using Mtgdb.Dal;
using Mtgdb.Index;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public class DeckSuggestModel : SuggestModel<int, DeckModel>
	{
		[UsedImplicitly]
		public DeckSuggestModel(DeckSearcher searcher)
			: base(searcher)
		{
		}

		protected override IntellisenseSuggest Suggest(TextInputState searchState) =>
			((DeckSearcher) Searcher).Suggest(searchState);

		public UiModel Ui { get; set; }
	}
}