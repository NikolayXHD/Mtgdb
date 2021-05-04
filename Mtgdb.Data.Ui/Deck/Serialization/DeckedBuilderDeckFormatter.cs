using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Data;

namespace Mtgdb.Ui
{
	public class DeckedBuilderDeckFormatter : RegexDeckFormatter
	{
		public DeckedBuilderDeckFormatter(CardRepository repo)
			:base(repo)
		{
		}

		public override bool IsSideboard(Match match, string line)
		{
			return line.StartsWith(SideboardPrefix);
		}

		public override Card GetCard(Match match)
		{
			var card = Repo.CardsByName.TryGet(match.Groups["name"].Value.RemoveDiacritics())
				// card_by_name_sorting
				?.First();

			return card;
		}

		protected override string ExportDeckImplementation(string name, Deck current, bool exact = false)
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
			// ignore maybeboard

			return result.ToString();
		}

		private void writeDeckZone(StringBuilder result, DeckZone zone, string prefix)
		{
			foreach (var id in zone.Order)
			{
				result.Append(prefix);
				result.AppendLine($"{zone.Count[id]} {Repo.CardsById[id].NameEn}");
			}
		}

		public override bool ValidateFormat(string serialized)
		{
			var lines = SplitToLines(serialized);
			return lines.Any(l => l.StartsWith(SideboardPrefix));
		}

		public override Regex LineRegex { get; } = new Regex(
			@"^(SB: )?(?<count>\d+) (?<name>.+)$", RegexOptions.IgnoreCase);

		public const string SideboardPrefix = "SB: ";

		public override string Description => "Decked builder {type}";
		public override string FileNamePattern => "*.dec";
	}
}
