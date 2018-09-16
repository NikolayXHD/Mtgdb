using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Dal;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class MtgArenaFormatter : RegexDeckFormatter
	{
		public MtgArenaFormatter(CardRepository repo)
		{
			_repo = repo;
		}

		public override Deck ImportDeck(string serialized)
		{
			_isSideboard = false;
			var deck = base.ImportDeck(serialized);
			return deck;
		}

		protected override IList<string> SplitToLines(string serialized)
		{
			var lines = serialized.Trim().Split(Array.From("\r\n", "\r", "\n"), StringSplitOptions.None);
			return lines;
		}

		public override bool IsSideboard(Match match, string line)
		{
			_isSideboard |= line == string.Empty;
			return _isSideboard;
		}

		public override Card GetCard(Match match)
		{
			if (_repo.CardsByName.TryGetValue(match.Groups["name"].Value.RemoveDiacritics(), out var cards))
			{
				return cards
					.AtMax(c => Str.Equals(c.SetCode, match.Groups["set"].Value))
					.ThenAtMax(c => Str.Equals(c.Number, match.Groups["num"].Value))
					.Find();
			}

			return _repo.SetsByCode.TryGet(match.Groups["set"].Value)?.Cards
				.FirstOrDefault(c => Str.Equals(c.Number, match.Groups["num"].Value));
		}

		public override string ExportDeck(string name, Deck current)
		{
			var result = new StringBuilder();

			writeCards(result, current.MainDeck);
			result.AppendLine();
			writeCards(result, current.Sideboard);

			return result.ToString();
		}

		private void writeCards(StringBuilder result, DeckZone deckZone)
		{
			foreach (var cardId in deckZone.Order)
			{
				var count = deckZone.Count[cardId];
				var card = _repo.CardsById[cardId];

				result.AppendLine($"{count} {card.NameEn} ({card.SetCode}) {card.Number}");
			}
		}

		public override bool ValidateFormat(string serialized) =>
			SplitToLines(serialized).Any(_lineRegex.IsMatch);

		public override Regex LineRegex { get; } = _lineRegex;

		public override bool SupportsFile => false;
		public override string Description => "MTGArena {type}";
		public override string FileNamePattern => "*.txt";

		private bool _isSideboard;
		private readonly CardRepository _repo;
		private static readonly Regex _lineRegex = new Regex(
			@"^(?<count>\d+)\s+(?<name>.+) \((?<set>[^\)]+)\) (?<num>\d+\w*)$",
			RegexOptions.IgnoreCase);
	}
}