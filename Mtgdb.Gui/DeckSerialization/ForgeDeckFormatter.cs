using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Data;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class ForgeDeckFormatter : RegexDeckFormatter
	{
		public ForgeDeckFormatter(CardRepository repo, ForgeSetRepository forgeSetRepo)
			:base(repo)
		{
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

				var cards = Repo.SetsByCode.TryGet(setCode)
					?.ActualCardsByName.TryGet(name);

				if (cards != null)
				{
					if (variantGroup.Success)
						card = cards.FirstOrDefault(_ => Str.Equals(_.ImageName, name + variantGroup.Value));

					return card ?? cards.First();
				}
			}

			card = Repo.CardsByName.TryGet(name)
				// card_by_name_sorting
				?.First();

			return card;
		}

		public override Deck ImportDeck(string serialized)
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
			// ReSharper disable once StringLiteralTypo
			if (Str.Equals(forgeName, @"Seal of Cleansings"))
				return @"Seal of Cleansing";

			return forgeName.TrimEnd('+');
		}


		protected override string ExportDeckImplementation(string name, Deck current)
		{
			_forgeSetRepo.EnsureLoaded();

			var result = new StringBuilder();
			result.AppendLine(Header);
			result.AppendLine($@"Name={name}");

			result.AppendLine(@"[Main]");
			writeCards(result, current.MainDeck);

			result.AppendLine(@"[Sideboard]");
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
				var set = _forgeSetRepo.ToForgeSet(card.SetCode);
				result.AppendLine($"{count} {card.NameNormalized}|{set}");
			}
		}

		public override bool ValidateFormat(string serialized)
		{
			return serialized.IndexOf(Header, Str.Comparison) >= 0;
		}



		public override Regex LineRegex { get; } = new Regex(
			@"^(?<count>\d+) (?<name>[^|]+)(\|(?<set>[^|]+)(\|(?<variant>[^|]+))?)?", RegexOptions.IgnoreCase);

		public override string Description => "Forge {type}";
		public override string FileNamePattern => @"*.dck";

		private const string SideboardMark = @"[sideboard]";
		private const string Header = @"[metadata]";

		private readonly ForgeSetRepository _forgeSetRepo;
		private bool _isSideboard;
	}
}
