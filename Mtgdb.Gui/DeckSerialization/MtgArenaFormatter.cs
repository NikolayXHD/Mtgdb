using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Data;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class MtgArenaFormatter : RegexDeckFormatter
	{
		public MtgArenaFormatter(CardRepository repo)
			:base(repo)
		{
		}

		public static Dictionary<string, int> ImportCollection(MtgArenaIntegration integration, CardRepository repo)
		{
			var collectionData = integration.ImportCollection();
			var countById = new Dictionary<string, int>();

			if (collectionData == null)
				return countById;

			collectionData = addBasicLands(repo, collectionData);

			foreach (var data in collectionData)
			{
				if (!repo.SetsByCode.TryGetValue(data.Set, out var set))
					continue;

				var card = set.Cards.FirstOrDefault(c => Str.Equals(c.Number, data.Number));

				if (card == null)
					continue;

				countById.Add(card.Id, data.Count);
			}

			return countById;
		}

		private static IEnumerable<(string Set, string Number, int Count)> addBasicLands(CardRepository repo, IEnumerable<(string Set, string Number, int Count)> collectionData)
		{
			const int basicLandsCount = 40;

			var latestSet = repo.SetsByCode.Values
				.Where(s => !string.IsNullOrEmpty(s.ReleaseDate) && CardNames.ColoredBasicLands.All(s.ActualCardsByName.ContainsKey))
				.AtMax(s => s.ReleaseDate)
				.Find();

			var basicLands = CardNames.ColoredBasicLands.Select(land => (
				latestSet.Code,
				latestSet.ActualCardsByName[land].First().Number,
				basicLandsCount));

			return collectionData.Concat(basicLands);
		}

		public override Deck ImportDeck(string serialized)
		{
			_isSideboard = false;
			var deck = base.ImportDeck(serialized);
			return deck;
		}

		protected override IList<string> SplitToLines(string serialized)
		{
			var lines = serialized.Trim().Lines(StringSplitOptions.None);
			return lines;
		}

		public override bool IsSideboard(Match match, string line)
		{
			_isSideboard |= line == string.Empty;
			return _isSideboard;
		}

		public override Card GetCard(Match match)
		{
			string setCode = match.Groups["set"].Value;
			string actualSetCode = _setCodesByMtga.TryGet(setCode) ?? setCode;

			if (Repo.CardsByName.TryGetValue(match.Groups["name"].Value.RemoveDiacritics(), out var cards))
			{
				return cards
					.AtMax(c => Str.Equals(c.SetCode, actualSetCode))
					.ThenAtMax(c => Str.Equals(c.Number, match.Groups["num"].Value))
					.Find();
			}

			return Repo.SetsByCode.TryGet(actualSetCode)?.Cards
				.FirstOrDefault(c => Str.Equals(c.Number, match.Groups["num"].Value));
		}

		protected override string ExportDeckImplementation(string name, Deck current)
		{
			var result = new StringBuilder();

			writeCards(result, current.MainDeck);
			result.AppendLine();
			writeCards(result, current.Sideboard);
			// ignore maybeboard

			return result.ToString();
		}

		private void writeCards(StringBuilder result, DeckZone deckZone)
		{
			foreach (var cardId in deckZone.Order)
			{
				var count = deckZone.Count[cardId];
				var card = Repo.CardsById[cardId];

				string number = card.Number;

				if (card.Faces.Main == null)
					continue;

				string name = card.Faces.Main.NameEn;

				if (card.IsDoubleFace() && number.EndsWith("a", Str.Comparison))
					number = number.Substring(0, number.Length - 1);

				string setCode = _mtgaSetCodes.TryGet(card.SetCode) ?? card.SetCode;
				result.AppendLine($"{count} {name} ({setCode}) {number}");
			}
		}

		public override bool ValidateFormat(string serialized) =>
			SplitToLines(serialized).Any(_lineRegex.IsMatch);

		public override Regex LineRegex { get; } = _lineRegex;

		public override bool SupportsFile =>
			false;

		public override string Description =>
			"MTGArena {type}";

		public override string FileNamePattern =>
			"*.txt";

		private bool _isSideboard;

		private static readonly Regex _lineRegex = new Regex(
			@"^(?<count>\d+)\s+(?<name>.+) \((?<set>[^\)]+)\) (?<num>\d+\w*)$",
			RegexOptions.IgnoreCase);

		private static readonly Dictionary<string, string> _mtgaSetCodes =
			new Dictionary<string, string>(Str.Comparer)
			{
				["DOM"] = "DAR"
			};

		private static readonly Dictionary<string, string> _setCodesByMtga =
			_mtgaSetCodes.ToDictionary(pair => pair.Value, pair => pair.Key, Str.Comparer);
	}
}
