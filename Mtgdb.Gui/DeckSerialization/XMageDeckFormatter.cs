using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class XMageDeckFormatter : RegexDeckFormatter
	{
		private readonly CardRepository _repository;
		private static readonly Regex _idRegex = new Regex(@"^\d+", RegexOptions.Compiled);
		private const string Header = "NAME:";

		public XMageDeckFormatter(CardRepository repository)
		{
			_repository = repository;
		}

		public override Card GetCard(Match match)
		{
			var setCode = match.Groups["set"].Value;
			var id = match.Groups["id"].Value;
			string name = match.Groups["name"].Value;
			name = name.TrimComment();
			name = fromXMageName(name);

			var cards =
				_repository.SetsByCode.TryGet(setCode)?.CardsByName.TryGet(name) ??
				_repository.CardsByName.TryGet(name);

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
			var number = card.Number ?? card.MciNumber;

			if (number == null)
				return null;

			var match = _idRegex.Match(number);
			if (!match.Success)
				return null;

			return match.Value;
		}

		private static string fromXMageName(string xMageName)
		{
			if (Str.Equals(xMageName, @"Mindbreaker Demon"))
				return @"Mindwrack Demon";

			if (Str.Equals(xMageName, @"Kongming, 'Sleeping Dragon'"))
				return @"Kongming, ""Sleeping Dragon""";

			return xMageName;
		}

		public override string ExportDeck(string name, Deck current)
		{
			var result = new StringBuilder();
			result.AppendLine(Header + name);

			writeCards(result, current.MainDeck, string.Empty);
			writeCards(result, current.SideDeck, @"SB: ");

			return result.ToString();
		}

		private void writeCards(StringBuilder result, DeckZone zone, string prefix)
		{
			foreach (var cardId in zone.Order)
			{
				var count = zone.Count[cardId];
				var card = _repository.CardsById[cardId];

				result.AppendLine($"{prefix}{count} [{card.SetCode}:{0}] {card.NameNormalized}");
			}
		}

		public override bool ValidateFormat(string serialized)
		{
			return serialized.IndexOf(Header, Str.Comparison) >= 0;
		}

		public override Regex LineRegex { get; } = new Regex(
			@"^(?<sb>SB: )?(?<count>\d+) \[(?<set>[^:]+):(?<id>\d+)\] (?<name>.*)$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);
		
		public override string Description => "XMage {type}";
		public override string FileNamePattern => @"*.dck";
		public override bool SupportsExport => true;
		public override bool SupportsImport => true;
	}
}