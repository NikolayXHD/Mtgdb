using System;
using System.Collections.Generic;
using System.Linq;
using FileHelpers;
using JetBrains.Annotations;
using Mtgdb.Data;
using Mtgdb.Ui;
using NLog;

namespace Mtgdb.Gui
{
	/// <summary>
	/// Supports importing the CSV format created by exporting a list from TCG Player's mobile app.
	/// </summary>
	public class TcgCsvDeckFormatter : IDeckFormatter
	{
		public TcgCsvDeckFormatter(CardRepository cardRepository)
		{
			_fileHelperEngine = new FileHelperEngine<CsvCardModel>();
			_cardRepository = cardRepository;
		}

		public string Description => "TCGPlayer Mobile App CSV";
		public string FileNamePattern => "*.csv";
		public bool SupportsExport => false;
		public bool SupportsImport => true;
		public bool SupportsFile => true;
		public bool UseBom => false;
		public string FormatHint => null;

		public bool ValidateFormat(string serialized)
		{
			string header = serialized.Lines(StringSplitOptions.RemoveEmptyEntries).First();
			var headers = header.Split(',');

			// validate some of the more unique headers, to differentiate from other possible non-TCGPlayer CSV's
			return headers.Contains("Simple Name") && headers.Contains("Product ID") && headers.Contains("SKU");
		}

		public Deck ImportDeck(string serialized, bool exact = false)
		{
			var result = Deck.Create();
			var unmatched = new HashSet<string>();

			var csvCards = _fileHelperEngine.ReadStringAsList(serialized);
			foreach (CsvCardModel csvCard in csvCards)
			{
				var card = getCard(csvCard.SimpleName, csvCard.ProductId, csvCard.SetCode,csvCard.CardNumber);

				if (card == null)
				{
					unmatched.Add(csvCard.SimpleName);
					continue;
				}

				add(card, csvCard.Quantity, result.MainDeck);
			}

			_log.Info($"Unmatched cards:{Str.Endl}{string.Join(Str.Endl, unmatched)}");
			return result;
		}


		private static void add(Card card, int count, DeckZone collection)
		{
			if (collection.Count.ContainsKey(card.Id))
				collection.Count[card.Id] += count;
			else
			{
				collection.Count[card.Id] = count;
				collection.Order.Add(card.Id);
			}
		}

		public string ExportDeck(string name, Deck current, bool exact = false) =>
			throw new NotSupportedException();

		private Card getCard(string cardName, int tcgPlayerProductId, string setCode= null, string cardNumber=null)
		{
			// Note TCGPlayer set codes are unreliable, there's some rogue codes like PPELD and PRE that don't exist mtgjson data models

			// Instead, use TCG product ID.
			// Unique by printing, alt / extended art, promo.  But not by foil.
			var cardVariants = CardsAndTokensByProductId.TryGet(tcgPlayerProductId);

			// TODO: Foil variant from the "Printing" CSV field which has values "Normal" or "Foil"
			var card = cardVariants?.FirstOrDefault();
			return card;
		}

		private Dictionary<int, List<Card>> _cardsAndTokensByProductId;

		private Dictionary<int, List<Card>> CardsAndTokensByProductId
		{
			get
			{
				if (_cardsAndTokensByProductId != null)
					return _cardsAndTokensByProductId;

				if (!_cardRepository.IsLoadingComplete.Signaled)
					throw new InvalidOperationException();

				_cardsAndTokensByProductId = _cardRepository.Cards
					.GroupBy(_ => _.Identifiers.TcgPlayerProductId)
					.ToDictionary(gr => gr.Key, gr => gr.ToList());

				return _cardsAndTokensByProductId;
			}
		}

		private readonly CardRepository _cardRepository;
		private readonly FileHelperEngine<CsvCardModel> _fileHelperEngine;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();

		[DelimitedRecord(",")]
		[IgnoreEmptyLines]
		[IgnoreFirst]
		[UsedImplicitly]
		private class CsvCardModel
		{
			[FieldCaption("Quantity")]
			public int Quantity { get; set; }

			// E.g.  Castle Locthwain (Extended Art), Faerie Guidemother (Showcase), "Goat // Food (16)"
			[FieldCaption("Name")]
			public string Name { get; set; }

			// E.g. Castle Locthwain
			[FieldCaption("SimpleName")]
			public string SimpleName { get; set; }

			[FieldCaption("Set")]
			public string Set { get; set; }

			[FieldCaption("CardNumber")]
			public string CardNumber { get; set; }

			[FieldCaption("SetCode")]
			public string SetCode { get; set; }

			[FieldCaption("Printing")]
			public string Printing { get; set; }

			[FieldCaption("Condition")]
			public string Condition { get; set; }

			[FieldCaption("Language")]
			public string Language { get; set; }

			[FieldCaption("Rarity")]
			public string Rarity { get; set; }

			[FieldCaption("ProductID")]
			public int ProductId { get; set; }

			[FieldCaption("SKU")]
			public string Sku { get; set; }
		}
	}
}
