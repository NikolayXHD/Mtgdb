using JetBrains.Annotations;
using Mtgdb.Index;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public class DeckSuggestModel : SuggestModel<long, DeckModel>
	{
		[UsedImplicitly]
		public DeckSuggestModel(DeckSearcher searcher)
			: base(searcher)
		{
		}

		protected override IntellisenseSuggest Suggest(TextInputState searchState) =>
			((DeckSearcher) Searcher).Suggest(searchState);
	}
}