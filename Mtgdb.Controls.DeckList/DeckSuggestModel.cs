using JetBrains.Annotations;
using Mtgdb.Dal;
using Mtgdb.Index;

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
			((DeckSearcher) Searcher).Suggest(searchState, Ui);

		public UiModel Ui { get; set; }
	}
}