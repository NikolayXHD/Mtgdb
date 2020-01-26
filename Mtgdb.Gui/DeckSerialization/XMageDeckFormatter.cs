using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Data;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class XMageDeckFormatter : RegexDeckFormatter
	{
		public XMageDeckFormatter(CardRepository repo)
			:base(repo)
		{
		}

		public override Card GetCard(Match match)
		{
			var setCode = match.Groups["set"].Value;
			var id = match.Groups["id"].Value;
			string name = match.Groups["name"].Value;
			name = name.TrimComment();
			name = fromXMageName(name);

			var cards =
				Repo.SetsByCode.TryGet(setCode)?.ActualCardsByName.TryGet(name) ??
				Repo.CardsByName.TryGet(name);

			var card = cards
				?.OrderByDescending(_ => _.ReleaseDate)
				.ThenByDescending(_ => Str.Equals(id, getCardId(_)))
				.FirstOrDefault();

			return card;
		}

		public override bool IsSideboard(Match match, string line)
		{
			return match.Success && match.Groups["sb"].Success;
		}



		private static string getCardId(Card card)
		{
			if (card.Number == null)
				return null;

			var match = _idRegex.Match(card.Number);
			if (!match.Success)
				return null;

			return match.Value;
		}

		private static string fromXMageName(string xMageName)
		{
			// ReSharper disable StringLiteralTypo

			if (Str.Equals(xMageName, @"Mindbreaker Demon"))
				return @"Mindwrack Demon";

			if (Str.Equals(xMageName, @"Kongming, 'Sleeping Dragon'"))
				return @"Kongming, ""Sleeping Dragon""";

			// ReSharper restore StringLiteralTypo

			return xMageName;
		}

		protected override string ExportDeckImplementation(string name, Deck current)
		{
			var result = new StringBuilder();
			result.AppendLine(Header + name);

			writeCards(result, current.MainDeck, string.Empty);
			writeCards(result, current.Sideboard, @"SB: ");
			// ignore maybeboard

			return result.ToString();
		}

		private void writeCards(StringBuilder result, DeckZone zone, string prefix)
		{
			foreach (var cardId in zone.Order)
			{
				var count = zone.Count[cardId];
				var card = Repo.CardsById[cardId];

				result.AppendLine($"{prefix}{count} [{card.SetCode}:{0}] {card.NameNormalized}");
			}
		}

		public override bool ValidateFormat(string serialized)
		{
			var lines = SplitToLines(serialized);
			bool valid = lines.Any(LineRegex.IsMatch);
			return valid;
		}

		public override Regex LineRegex { get; } = new Regex(
			@"^(?<sb>SB: )?(?<count>\d+) \[(?<set>[^:]+):(?<id>\d+)\] (?<name>.*)$",
			RegexOptions.IgnoreCase);

		public override string Description => "XMage {type}";
		public override string FileNamePattern => @"*.dck";

		private static readonly Regex _idRegex = new Regex(@"^\d+");
		private const string Header = "NAME:";
	}
}
