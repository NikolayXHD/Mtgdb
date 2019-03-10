using JetBrains.Annotations;
using Mtgdb.Data.Model;
using Mtgdb.Ui;

namespace Mtgdb.Data.Index
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