using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Dal
{
	public class CardFaceVariants : IEnumerable<IList<Card>>
	{
		public CardFaceVariants(Card c) =>
			Card = c;

		public IList<Card> this[int i] =>
			Card.Set.CardsByName[Card.GetFaceName(i)];

		public IList<Card> Main =>
			this[0];

		public IEnumerator<IList<Card>> GetEnumerator() =>
			Enumerable.Range(0, Math.Max(1, Card.Names?.Count ?? 0))
				.Select(i => this[i])
				.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();

		internal readonly Card Card;
	}
}