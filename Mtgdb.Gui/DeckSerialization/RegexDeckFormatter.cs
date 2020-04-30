using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mtgdb.Data;
using Mtgdb.Ui;
using NLog;

namespace Mtgdb.Gui
{
	public abstract class RegexDeckFormatter : IDeckFormatter
	{
		protected RegexDeckFormatter(CardRepository repo)
		{
			Repo = repo;
		}

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
					add(card, count, result.Sideboard);
				else
					add(card, count, result.MainDeck);

				// ignore maybeboard
			}

			_log.Info("Unmatched cards:\r\n{0}", string.Join("\r\n", unmatched));

			return result;
		}

		protected virtual IList<string> SplitToLines(string serialized)
		{
			return serialized.Lines(StringSplitOptions.RemoveEmptyEntries);
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

		public string ExportDeck(string name, Deck current)
		{
			if (!Repo.IsLoadingComplete.Signaled)
				throw new InvalidOperationException();

			var deckToExport = DeckConverter.ConvertDeck(current,
				cardId => Repo.CardsById[cardId].Faces.Main?.Id);

			return ExportDeckImplementation(name, deckToExport);
		}

		protected abstract string ExportDeckImplementation(string name, Deck current);
		public abstract string Description { get; }
		public abstract string FileNamePattern { get; }
		public abstract bool ValidateFormat(string serialized);

		public virtual bool SupportsExport => true;
		public virtual bool SupportsImport => true;
		public virtual bool SupportsFile => true;
		public virtual bool UseBom => false;
		public virtual string FormatHint => null;

		protected readonly CardRepository Repo;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
