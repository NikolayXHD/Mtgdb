using System;
using System.Collections.Generic;
using System.Linq;
using Mtgdb.Index;

namespace Mtgdb.Dal.Index
{
	public class CardSpellcheckerState : LuceneSpellcheckerState<int, Card>
	{
		public CardSpellcheckerState(
			CardRepository repo,
			Func<Set, bool> filterSet,
			Spellchecker spellchecker,
			CardSearcherState searcherState,
			IDocumentAdapter<int, Card> adapter,
			Func<int> maxCount,
			bool loaded) :
			base(spellchecker, searcherState, adapter, maxCount, loaded)
		{
			_repo = repo;
			_filterSet = filterSet;
		}

		protected override IEnumerable<Card> GetObjectsToIndex()
		{
			if (!_repo.IsLocalizationLoadingComplete)
				throw new InvalidOperationException();

			return _repo.SetsByCode.Values.Where(_filterSet).SelectMany(_ => _.Cards);
		}

		private readonly CardRepository _repo;
		private readonly Func<Set, bool> _filterSet;
	}
}