using System;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Dal
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
					Count = new Dictionary<string, int>()
				},

				Sideboard = new DeckZone
				{
					Order = new List<string>(),
					Count = new Dictionary<string, int>()
				}
			};

			return result;
		}

		public static Deck Create(
			Dictionary<string, int> mainCountById,
			List<string> mainOrder,
			Dictionary<string, int> sideCountById,
			List<string> sideOrder)
		{
			var result = new Deck
			{
				MainDeck = new DeckZone
				{
					Count = mainCountById ?? new Dictionary<string, int>(),
					Order = mainOrder ?? new List<string>()
				},

				Sideboard = new DeckZone
				{
					Count = sideCountById ?? new Dictionary<string, int>(),
					Order = sideOrder ?? new List<string>()
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
				Sideboard.Order.ToList());

			result.Name = Name;
			result.File = File;
			result.Error = Error;
			result.Id = Id;

			return result;
		}

		public void Replace(Dictionary<string, string> replacements)
		{
			MainDeck = replace(MainDeck, replacements);
			Sideboard = replace(Sideboard, replacements);
		}

		public bool Contains(Card c) =>
			MainDeck.Count.ContainsKey(c.Id) || Sideboard.Count.ContainsKey(c.Id);

		public Deck()
		{
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

		public DeckZone GetZone(Zone zone)
		{
			switch (zone)
			{
				case Zone.Main:
					return MainDeck;
				case Zone.Side:
					return Sideboard;
				default:
					throw new ArgumentOutOfRangeException(nameof(zone), zone, null);
			}
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string File { get; set; }
		public string Error { get; set; }
	}
}