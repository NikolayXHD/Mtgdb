using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Dal
{
	public class CardFaces : IReadOnlyList<Card>
	{
		public CardFaces(CardFaceVariants v) =>
			_v = v;

		public Card this[int i] =>
			select(_v[i]);

		public Card Main =>
			this[0];

		private Card select(IList<Card> list) =>
			Str.Equals(list[0].NameNormalized, _v.Card.NameNormalized)
				? _v.Card
				: list[0];

		public IEnumerator<Card> GetEnumerator() =>
			Enumerable.Range(0, Count)
				.Select(i => this[i])
				.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();

		public int Count =>
			_v.Count;

		private readonly CardFaceVariants _v;
	}
}