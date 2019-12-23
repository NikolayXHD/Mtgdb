using System;
using System.Collections.Generic;
using Mtgdb.Ui;

namespace Mtgdb.Data.Model
{
	public class DeckAggregateCache<TVal, TAggr, TResult>
	{
		public DeckAggregateCache(
			CardRepository repo,
			Func<Deck> deck,
			Func<TAggr> aggregationSeed,
			Func<TAggr, TVal, TAggr> add,
			Func<Card, int, TVal> value,
			Func<TAggr, TResult> transform)
		{
			_repo = repo;
			_deck = deck;
			_aggregationSeed = aggregationSeed;
			_add = add;
			_value = value;
			_transform = transform;
		}

		public void Clear() =>
			_cache.Clear();

		public TResult GetAggregate(Zone zone, Func<Card, bool> filter)
		{
			if (!_repo.IsLoadingComplete.Signaled)
				return _transform(_aggregationSeed());

			var key = (zone, filter);

			if (_cache.TryGetValue(key, out var result))
				return result;

			var aggregate = _aggregationSeed();

			foreach (var pair in _deck().GetZone(zone).Count)
			{
				var id = pair.Key;

				if (!_repo.CardsById.TryGetValue(id, out var card))
					continue;

				if (!filter(card))
					continue;

				var deckCount = pair.Value;
				aggregate = _add(aggregate, _value(card, deckCount));
			}

			result = _transform(aggregate);

			_cache.Add(key, result);

			return result;
		}

		private readonly Dictionary<(Zone Zone, Func<Card, bool> Filter), TResult> _cache =
			new Dictionary<(Zone Zone, Func<Card, bool> Filter), TResult>();

		private readonly CardRepository _repo;
		private readonly Func<Deck> _deck;
		private readonly Func<TAggr> _aggregationSeed;
		private readonly Func<TAggr, TVal, TAggr> _add;
		private readonly Func<Card, int, TVal> _value;
		private readonly Func<TAggr, TResult> _transform;
	}
}
