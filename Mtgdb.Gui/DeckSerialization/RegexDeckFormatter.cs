using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public abstract class RegexDeckFormatter : IDeckFormatter
	{
		public abstract Regex LineRegex { get; }
		public abstract bool IsSideboard(Match match, string line);
		public abstract Card GetCard(Match match);

		public virtual GuiSettings ImportDeck(string serialized)
		{
			var result = new GuiSettings();
			
			var lines = serialized.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			//var unmatched = new HashSet<string>();

			foreach (string line in lines)
			{
				var match = LineRegex.Match(line);

				bool isSideboard = IsSideboard(match, line);

				if (!match.Success)
					continue;

				var count = int.Parse(match.Groups["count"].Value);
				var card = GetCard(match);

				if (card == null)
				{
					//unmatched.Add(match.Value);
					continue;
				}
				
				if (isSideboard)
					setCount(card, count, result.SideDeck, result.SideDeckOrder);
				else
					setCount(card, count, result.Deck, result.DeckOrder);
			}

			//string unmatchedStr = string.Join(Environment.NewLine, unmatched);

			return result;
		}

		private static void setCount(Card card, int count, Dictionary<string, int> deck, List<string> deckOrder)
		{
			if (deck.ContainsKey(card.Id))
				deck[card.Id] += count;
			else
			{
				deck[card.Id] = count;
				deckOrder.Add(card.Id);
			}
		}

		public abstract string ExportDeck(string name, GuiSettings current);
		public abstract string Description { get; }
		public abstract string FileNamePattern { get; }
		public abstract bool ValidateFormat(string serialized);
		public abstract bool SupportsExport { get; }
		public abstract bool SupportsImport { get; }
		public virtual bool UseBom => false;
	}
}