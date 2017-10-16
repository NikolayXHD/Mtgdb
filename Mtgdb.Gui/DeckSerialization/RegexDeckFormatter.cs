using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mtgdb.Dal;
using NLog;

namespace Mtgdb.Gui
{
	public abstract class RegexDeckFormatter : IDeckFormatter
	{
		public abstract Regex LineRegex { get; }
		public abstract bool IsSideboard(Match match, string line);
		public abstract Card GetCard(Match match);

		public virtual Deck ImportDeck(string serialized)
		{
			var result = Deck.Create();
			
			var lines = SplitToLines(serialized);

			var unmatched = new HashSet<string>();

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
					unmatched.Add(match.Value);
					continue;
				}
				
				if (isSideboard)
					add(card, count, result.SideDeck);
				else
					add(card, count, result.MainDeck);
			}

			_log.Info("Unmatched cards:\r\n{0}", string.Join("\r\n", unmatched));

			return result;
		}

		protected virtual IList<string> SplitToLines(string serialized)
		{
			return serialized.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		}

		private static void add(Card card, int count, DeckZone collection)
		{
			if (collection.Count.ContainsKey(card.Id))
				collection.Count[card.Id] += count;
			else
			{
				collection.Count[card.Id] = count;
				collection.Order.Add(card.Id);
			}
		}

		public abstract string ExportDeck(string name, Deck current);
		public abstract string Description { get; }
		public abstract string FileNamePattern { get; }
		public abstract bool ValidateFormat(string serialized);
		public abstract bool SupportsExport { get; }
		public abstract bool SupportsImport { get; }
		public virtual bool UseBom => false;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}