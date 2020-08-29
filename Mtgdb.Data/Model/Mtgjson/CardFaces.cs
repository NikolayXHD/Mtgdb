using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Data
{
	public class CardFaces : IEnumerable<Card>
	{
		public CardFaces(Card c) =>
			_c = c;

		public Card this[int i]
		{
			get
			{
				if (_faces.TryGetValue(i, out var card))
					return card;

				card = getFace(i);
				_faces.Add(i, card);
				return card;
			}
		}

		private Card getFace(int i)
		{
			if (_c.OtherFaceIds == null || _c.OtherFaceIds.Count == 0)
			{
				if (i == 0)
					return _c;

				return null;
			}

			var side = CardSides.Values[i];
			if (Str.Equals(_c.Side, side))
				return _c;

			return _c.OtherFaces.FirstOrDefault(_ => Str.Equals(_.Side, side));
		}


		public Card Main
		{
			get
			{
				if (_c.IsMeld() && _c.Side == CardSides.B /* meld union */)
					// Protect from silent errors of replacing one meld card by another in deck transformations
					return null;

				return this[0];
			}
		}

		public IEnumerator<Card> GetEnumerator() =>
			Enumerable.Range(0, Count)
				.Select(i => this[i])
				.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();

		public int Count =>
			Math.Max(1, 1 + (_c.OtherFaceIds?.Count ?? 0));

		private readonly Dictionary<int, Card> _faces = new Dictionary<int, Card>();
		private readonly Card _c;
	}
}
