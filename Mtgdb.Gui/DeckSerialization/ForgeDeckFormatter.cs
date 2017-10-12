using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Dal;
using Mtgdb.Gui.Resx;

namespace Mtgdb.Gui
{
	public class ForgeDeckFormatter : RegexDeckFormatter
	{
		public ForgeDeckFormatter(CardRepository cardRepo, ForgeSetRepository forgeSetRepo)
		{
			_cardRepo = cardRepo;
			_forgeSetRepo = forgeSetRepo;
		}

		public override Card GetCard(Match match)
		{
			Card card = null;
			string name = match.Groups["name"].Value;
			name = name.TrimComment();
			name = fromForgeName(name);

			var setGroup = match.Groups["set"];
			var variantGroup = match.Groups["variant"];

			if (setGroup.Success)
			{
				var setCode = _forgeSetRepo.FromForgeSet(setGroup.Value);

				var cards = _cardRepo.SetsByCode.TryGet(setCode)
					?.CardsByName.TryGet(name);

				if (cards != null)
				{
					if (variantGroup.Success)
						card = cards.FirstOrDefault(_ => Str.Equals(_.ImageName, name + variantGroup.Value));

					if (card == null)
						card = cards.First();

					return card;
				}
			}

			card = _cardRepo.CardsByName.TryGet(name)
				// card_by_name_sorting
				?.First();

			return card;
		}

		public override GuiSettings ImportDeck(string serialized)
		{
			_forgeSetRepo.EnsureLoaded();

			_isSideboard = false;
			return base.ImportDeck(serialized);
		}

		public override bool IsSideboard(Match match, string line)
		{
			_isSideboard |= line.StartsWith(SideboardMark, Str.Comparison);
			return _isSideboard;
		}

		private static string fromForgeName(string forgeName)
		{
			if (Str.Equals(forgeName, @"Seal of Cleansings"))
				return @"Seal of Cleansing";

			return forgeName.TrimEnd('+');
		}


		public override string ExportDeck(string name, GuiSettings current)
		{
			_forgeSetRepo.EnsureLoaded();

			var result = new StringBuilder();
			result.AppendLine(Header);
			result.AppendLine($@"Name={name}");

			result.AppendLine(@"[Main]");
			writeCards(result, current.Deck, current.DeckOrder);

			result.AppendLine(@"[Sideboard]");
			writeCards(result, current.SideDeck, current.SideDeckOrder);

			return result.ToString();
		}

		private void writeCards(StringBuilder result, Dictionary<string, int> deck, List<string> deckOrder)
		{
			if (deckOrder == null)
				return;

			foreach (var cardId in deckOrder)
			{
				var count = deck[cardId];
				var card = _cardRepo.CardsById[cardId];
				var set = _forgeSetRepo.ToForgeSet(card.SetCode);
				result.AppendLine($"{count} {card.NameNormalized}|{set}");
			}
		}

		public override bool ValidateFormat(string serialized)
		{
			return serialized.IndexOf(Header, Str.Comparison) >= 0;
		}



		public override Regex LineRegex { get; } = new Regex(
			@"^(?<count>\d+) (?<name>[^|]+)(\|(?<set>[^|]+)(\|(?<variant>[^|]+))?)?",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public override string Description => Resources.Formatter_ForgeDeck_Description;
		public override string FileNamePattern => @"*.dck";
		public override bool SupportsExport => true;
		public override bool SupportsImport => true;



		private const string SideboardMark = @"[sideboard]";
		private const string Header = @"[metadata]";

		private readonly CardRepository _cardRepo;
		private readonly ForgeSetRepository _forgeSetRepo;
		private bool _isSideboard;
	}
}
