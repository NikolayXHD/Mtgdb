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

				SideDeck = new DeckZone
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

				SideDeck = new DeckZone
				{
					Count = sideCountById ?? new Dictionary<string, int>(),
					Order = sideOrder ?? new List<string>()
				}
			};

			return result;
		}

		public void Replace(Dictionary<string, string> replacements)
		{
			MainDeck = replace(MainDeck, replacements);
			SideDeck = replace(SideDeck, replacements);
		}

		private Deck()
		{
		}

		private static DeckZone replace(DeckZone original, Dictionary<string, string> replacements)
		{
			return new DeckZone
			{
				Order = original.Order.Select(_ => replacements[_]).ToList(),
				Count = original.Count.ToDictionary(_ => replacements[_.Key], _ => _.Value)
			};
		}

		public DeckZone MainDeck { get; private set; }
		public DeckZone SideDeck { get; private set; }

		public string Name { get; set; }
		public string File { get; set; }
		public string Error { get; set; }
	}
}