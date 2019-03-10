using System;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Data.Index
{
	public class CardSpellcheckerState : LuceneSpellcheckerState<int, Card>
	{
		public CardSpellcheckerState(
			CardRepository repo,
			Spellchecker spellchecker,
			CardSearcherState searcherState,
			IDocumentAdapter<int, Card> adapter,
			Func<int> maxCount,
			bool loaded) :
			base(spellchecker, searcherState, adapter, maxCount, loaded)
		{
			_repo = repo;
		}

		protected override IEnumerable<Card> GetObjectsToIndex()
		{
			if (!_repo.IsLocalizationLoadingComplete)
				throw new InvalidOperationException();

			return _repo.SetsByCode.Values.SelectMany(_ => _.Cards);
		}

		private readonly CardRepository _repo;
	}
}