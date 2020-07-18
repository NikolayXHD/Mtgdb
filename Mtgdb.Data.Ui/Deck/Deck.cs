using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Mtgdb.Ui
{
	public class Deck
	{
		public static Deck Create()
		{
			var result = new Deck
			{
				MainDeck = new DeckZone
				{
					Order = new List<string>(),
					Count = new Dictionary<string, int>(Str.Comparer)
				},

				Sideboard = new DeckZone
				{
					Order = new List<string>(),
					Count = new Dictionary<string, int>(Str.Comparer)
				},

				Maybeboard = new DeckZone
				{
					Order = new List<string>(),
					Count = new Dictionary<string, int>(Str.Comparer)
				},

				SampleHand = new DeckZone
				{
					Order = new List<string>(),
					Count = new Dictionary<string, int>(Str.Comparer)
				}
			};

			return result;
		}

		public static Deck Create(
			Dictionary<string, int> mainCountById,
			List<string> mainOrder,
			Dictionary<string, int> sideCountById,
			List<string> sideOrder,
			Dictionary<string, int> maybeCountById,
			List<string> maybeOrder,
			Dictionary<string, int> sampleCountById,
			List<string> sampleOrder)
		{
			var result = new Deck
			{
				MainDeck = new DeckZone
				{
					Count = mainCountById ?? new Dictionary<string, int>(Str.Comparer),
					Order = mainOrder ?? new List<string>()
				},

				Sideboard = new DeckZone
				{
					Count = sideCountById ?? new Dictionary<string, int>(Str.Comparer),
					Order = sideOrder ?? new List<string>()
				},

				Maybeboard = new DeckZone
				{
					Count = maybeCountById ?? new Dictionary<string, int>(Str.Comparer),
					Order = maybeOrder ?? new List<string>()
				},

				SampleHand = new DeckZone
				{
					Count = sampleCountById ?? new Dictionary<string, int>(Str.Comparer),
					Order = sampleOrder ?? new List<string>()
				}
			};

			return result;
		}

		public Deck Copy()
		{
			var result = Create(
				MainDeck.Count.ToDictionary(),
				MainDeck.Order.ToList(),
				Sideboard.Count.ToDictionary(),
				Sideboard.Order.ToList(),
				Maybeboard.Count.ToDictionary(),
				Maybeboard.Order.ToList(),
				SampleHand.Count.ToDictionary(),
				SampleHand.Order.ToList());

			result.Name = Name;
			result.File = File;
			result.Error = Error;
			result.Id = Id;
			result.Saved = Saved;

			return result;
		}

		public bool IsEquivalentTo(Deck other) =>
			MainDeck.IsEquivalentTo(other.MainDeck) && Sideboard.IsEquivalentTo(other.Sideboard) && Maybeboard.IsEquivalentTo(other.Maybeboard);

		public void Replace(Dictionary<string, string> replacements)
		{
			MainDeck = replace(MainDeck, replacements);
			Sideboard = replace(Sideboard, replacements);
			Maybeboard = replace(Maybeboard, replacements);
		}

		[UsedImplicitly]  // to find usages in IDE
		public Deck()
		{
		}

		[OnDeserialized]
		public void OnDeserialized(StreamingContext context)
		{
			if (isMalformed(MainDeck))
				MainDeck = createEmptyZone();

			if (isMalformed(Sideboard))
				Sideboard = createEmptyZone();

			if (isMalformed(Maybeboard))
				Maybeboard = createEmptyZone();

			if (isMalformed(SampleHand))
				SampleHand = createEmptyZone();

			static bool isMalformed(DeckZone zone) =>
				zone == null || zone.Order == null || zone.Count == null;

			static DeckZone createEmptyZone() =>
				new DeckZone
				{
					Order = new List<string>(),
					Count = new Dictionary<string, int>()
				};
		}

		private static DeckZone replace(DeckZone original, Dictionary<string, string> replacements)
		{
			// replacements are not guaranteed to be unique

			return new DeckZone
			{
				Order = original.Order
					.Select(_ => replacements[_])
					.Distinct()
					.ToList(),

				Count = original.Count
					.GroupBy(_ => replacements[_.Key])
					.ToDictionary(
						gr => gr.Key,
						gr => gr.Sum(_ => _.Value))
			};
		}

		public DeckZone MainDeck { get; set; }
		public DeckZone Sideboard { get; set; }
		public DeckZone Maybeboard { get; set; }
		public DeckZone SampleHand { get; set; }

		public DeckZone GetZone(Zone zone)
		{
			switch (zone)
			{
				case Zone.Main:
					return MainDeck;
				case Zone.Side:
					return Sideboard;
				case Zone.Maybe:
					return Maybeboard;
				case Zone.SampleHand:
					return SampleHand;
				default:
					throw new ArgumentOutOfRangeException(nameof(zone), zone, null);
			}
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public FsPath File { get; set; }
		public string Error { get; set; }
		public DateTime? Saved { get; set; }
	}
}
