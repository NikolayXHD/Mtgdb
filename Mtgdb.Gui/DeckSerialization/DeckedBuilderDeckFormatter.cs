using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class DeckedBuilderDeckFormatter : RegexDeckFormatter
	{
		public DeckedBuilderDeckFormatter(CardRepository cardRepo)
		{
			_cardRepo = cardRepo;
		}

		public override bool IsSideboard(Match match, string line)
		{
			return line.StartsWith(SideboardPrefix);
		}

		public override Card GetCard(Match match)
		{
			var card = _cardRepo.CardsByName.TryGet(match.Groups["name"].Value.RemoveDiacritics())
				// card_by_name_sorting
				?.First();

			return card;
		}

		public override string ExportDeck(string name, Deck current)
		{
			var result = new StringBuilder();

			if (!string.IsNullOrEmpty(current.Name))
			{
				result.Append("// ");
				result.AppendLine(current.Name);
			}

			writeDeckZone(result, current.MainDeck, prefix: string.Empty);
			result.AppendLine();
			writeDeckZone(result, current.Sideboard, prefix: SideboardPrefix);

			return result.ToString();
		}

		private void writeDeckZone(StringBuilder result, DeckZone zone, string prefix)
		{
			foreach (var id in zone.Order)
			{
				result.Append(prefix);
				result.AppendLine($"{zone.Count[id]} {_cardRepo.CardsById[id].NameEn}");
			}
		}

		public override bool ValidateFormat(string serialized)
		{
			var lines = SplitToLines(serialized);
			return lines.Any(l => l.StartsWith(SideboardPrefix));
		}

		public override Regex LineRegex { get; } = new Regex(
			@"^(SB: )?(?<count>\d+) (?<name>.+)$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public const string SideboardPrefix = "SB: ";

		public override bool SupportsExport => true;
		public override bool SupportsImport => true;
		public override string Description => "Decked builder {type}";
		public override string FileNamePattern => "*.dec";

		private readonly CardRepository _cardRepo;
	}
}