using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Data
{
	public class CardFaceVariants : IReadOnlyList<IList<Card>>
	{
		public CardFaceVariants(Card c) =>
			Card = c;

		public IList<Card> this[int i] =>
			Card.Set.MapByName(Card.IsToken)[Card.GetFaceName(i)];

		public IList<Card> Main
		{
			get
			{
				if (Card.IsMeld())
				{
					if (Str.Equals(Card.NameNormalized, Card.Names[1])) // meld union
						// Protect from silent errors of replacing one meld card by another in deck transformations
						return null;

					// Each card in meld pair considers itself the main face, convenient in deck transformations
					if (Str.Equals(Card.NameNormalized, Card.Names[2]))
						return this[2];

					return this[0];
				}

				return this[0];
			}
		}

		public IEnumerator<IList<Card>> GetEnumerator() =>
			Enumerable.Range(0, Count)
				.Select(i => this[i])
				.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();

		public int Count =>
			Math.Max(1, Card.Names?.Count ?? 0);

		internal readonly Card Card;
	}
}
