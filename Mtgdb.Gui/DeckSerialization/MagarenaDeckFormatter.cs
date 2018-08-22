using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Dal;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class MagarenaDeckFormatter : RegexDeckFormatter
	{
		public MagarenaDeckFormatter(CardRepository cardRepo)
		{
			_cardRepo = cardRepo;
		}

		public override Deck ImportDeck(string serialized)
		{
			var deck = base.ImportDeck(serialized);
			new XitaxDeckTransformation(_cardRepo).Transform(deck);
			return deck;
		}

		public override Card GetCard(Match match)
		{
			var card = _cardRepo.CardsByName.TryGet(match.Groups["name"].Value.RemoveDiacritics())
				// card_by_name_sorting
				?.First();

			// To verify reverse integration with test 
			//if (card != null && card.NameEn != match.Groups["name"].Value)
			//	return null;

			return card;
		}

		public override bool IsSideboard(Match match, string line)
		{
			return false;
		}



		public override string ExportDeck(string name, Deck current)
		{
			var creatures = new List<Card>();
			var lands = new List<Card>();
			var spells = new List<Card>();

			foreach (var cardId in current.MainDeck.Order)
			{
				var card = _cardRepo.CardsById[cardId];

				if (card.TypeEn.IndexOf(@"Creature", Str.Comparison) >= 0)
					creatures.Add(card);
				else if (card.TypeEn.IndexOf(@"Land", Str.Comparison) >=0)
					lands.Add(card);
				else
					spells.Add(card);
			}

			var result = new StringBuilder();
			result.AppendLine($"# {creatures.Count} creatures");
			foreach (var card in creatures)
			{
				int count = current.MainDeck.Count[card.Id];
				result.AppendLine($"{count} {card.NameEn}");
			}
			result.AppendLine();

			result.AppendLine($"# {spells.Count} spells");
			foreach (var card in spells)
			{
				int count = current.MainDeck.Count[card.Id];
				result.AppendLine($"{count} {card.NameEn}");
			}
			result.AppendLine();

			result.AppendLine($"# {lands.Count} lands");
			foreach (var card in lands)
			{
				int count = current.MainDeck.Count[card.Id];
				result.AppendLine($"{count} {card.NameEn}");
			}

			return result.ToString();
		}

		public override bool ValidateFormat(string serialized)
		{
			var lines = SplitToLines(serialized);
			return !lines.Any(l => l.StartsWith(DeckedBuilderDeckFormatter.SideboardPrefix));
		}



		public override Regex LineRegex { get; } = new Regex(
			@"^(?<count>\d+) (?<name>.+)$",
			RegexOptions.Compiled);

		public override string Description => "Magarena {type}";
		public override string FileNamePattern => "*.dec";
		public override bool SupportsExport => true;
		public override bool SupportsImport => true;

		private readonly CardRepository _cardRepo;
	}
}