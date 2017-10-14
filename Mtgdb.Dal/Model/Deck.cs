using System.Collections.Generic;

namespace Mtgdb.Gui
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

		private Deck()
		{
		}

		public DeckZone MainDeck { get; set; }
		public DeckZone SideDeck { get; set; }

		public string Name { get; set; }
		public string File { get; set; }
		public string Error { get; set; }
	}
}