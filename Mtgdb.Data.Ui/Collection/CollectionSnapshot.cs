using System.Collections.Generic;
using JetBrains.Annotations;
using Mtgdb.Data;

namespace Mtgdb.Ui
{
	public class CollectionSnapshot : ICardCollection
	{
		public static CollectionSnapshot Empty() =>
			new CollectionSnapshot(new Dictionary<string, int>(Str.Comparer));

		[UsedImplicitly] // when deserializing
		public CollectionSnapshot()
		{
		}

		private CollectionSnapshot(Dictionary<string, int> countById) =>
			CountById = countById;

		public CollectionSnapshot(CollectionEditorModel original)
			: this(original.CountById.ToDictionary(Str.Comparer))
		{
		}

		public int GetCount(Card c) =>
			CountById.TryGet(c.Id);

		public HashSet<string> GetAffectedCardIds(CollectionSnapshot previousSnapshot)
		{
			if (previousSnapshot == null)
				return CountById.Keys.ToHashSet(Str.Comparer);

			var result = new HashSet<string>(Str.Comparer);

			foreach (var currentPair in CountById)
				if (!previousSnapshot.CountById.TryGetValue(currentPair.Key, out var previousCount) || previousCount != currentPair.Value)
					result.Add(currentPair.Key);

			foreach (var oldPair in previousSnapshot.CountById)
				if (!CountById.TryGetValue(oldPair.Key, out var currentCount) || currentCount != oldPair.Value)
					result.Add(oldPair.Key);

			return result;
		}

		[UsedImplicitly] // by deserializer
		public Dictionary<string, int> CountById { get; set; }
	}
}