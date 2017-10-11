using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Dal;
using Mtgdb.Gui.Resx;

namespace Mtgdb.Gui
{
	public class MtgoDeckFormatter : RegexDeckFormatter
	{
		public MtgoDeckFormatter(CardRepository repository)
		{
			_repository = repository;
		}

		public override GuiSettings ImportDeck(string serialized)
		{
			ensureLoaded();

			_isSideboard = false;
			return base.ImportDeck(serialized);
		}

		public override Card GetCard(Match match)
		{
			string name = match.Groups["name"].Value;
			name = FromMtgoName(name);

			var cards = _cardsByName.TryGet(name);

			var card = cards
				?.OrderByDescending(_ => _.ReleaseDate)
				.ThenBy(_ => _.ImageName)
				.FirstOrDefault();

			return card;
		}

		public override bool IsSideboard(Match match, string line)
		{
			_isSideboard |= line == string.Empty;
			return _isSideboard;
		}

		public static string FromMtgoName(string mtgoName)
		{
			var separatorIndex = mtgoName.IndexOf('/');

			if (separatorIndex >= 0)
				mtgoName = mtgoName.Substring(0, separatorIndex);

			string result;
			if (_nameByMtgoName.TryGetValue(mtgoName, out result))
				return result;

			return mtgoName;
		}

		public static string ToMtgoName(Card card)
		{
			string name;
			if (Str.Equals(card.Layout, "split") || Str.Equals(card.Layout, "aftermath"))
				name = card.Names[0] + "/" + card.Names[1];
			else
				name = card.NameEn;

			string result;
			if (_mtgoNameByName.TryGetValue(name, out result))
				return result;

			return name;
		}

		public override string ExportDeck(string name, GuiSettings current)
		{
			var result = new StringBuilder();
			
			writeCards(result, current.Deck, current.DeckOrder);
			result.AppendLine();
			writeCards(result, current.SideDeck, current.SideDeckOrder);

			return result.ToString();
		}

		private void writeCards(
			StringBuilder result,
			Dictionary<string, int> deck,
			List<string> deckOrder)
		{
			foreach (var cardId in deckOrder)
			{
				var count = deck[cardId];
				var card = _repository.CardsById[cardId];

				result.AppendLine($"{count} {ToMtgoName(card)}");
			}
		}

		public override bool ValidateFormat(string serialized)
		{
			var lines = serialized.Split(new [] {Environment.NewLine}, StringSplitOptions.None);
			if (lines.Length == 0)
				return false;

			int blankLinesCount = 0;

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];

				if (line == string.Empty && i < lines.Length - 1)
					blankLinesCount++;

				if (blankLinesCount > 1)
					return false;

				if (line == string.Empty)
					continue;

				for (int j = 0; j < lines[j].Length; j++)
				{
					if (char.IsDigit(line[j]))
						continue;

					if (char.IsWhiteSpace(line[j]))
					{
						if (j > 0)
							break;

						return false;
					}

					return false;
				}
			}

			return true;
		}

		public override Regex LineRegex { get; } = new Regex(
			@"^(?<count>\d+)\s+(?<name>.+)$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private void ensureLoaded()
		{
			if (_cardsByName == null)
				_cardsByName = _repository.Cards
					.GroupBy(_ => _.NameEn)
					.ToDictionary(_ => _.Key, _ => _.ToList());
		}

		public override string Description => Resources.Formatter_MtgoDeck_Description;
		public override string FileNamePattern => @"*.txt";
		public override bool SupportsExport => true;
		public override bool SupportsImport => true;

		private bool _isSideboard;
		private readonly CardRepository _repository;
		private Dictionary<string, List<Card>> _cardsByName;

		private static readonly Dictionary<string, string> _mtgoNameByName = 
			new Dictionary<string, string>
			{
				{ "Jötun Grunt", "Jotun Grunt" },
				{ "Jötun Owl Keeper", "Jotun Owl Keeper" },
				{ "Bösium Strip", "Bosium Strip" }
			};

		private static readonly Dictionary<string, string> _nameByMtgoName =
			_mtgoNameByName.ToDictionary(_ => _.Value, _ => _.Key);
	}
}