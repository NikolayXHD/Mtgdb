using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FileHelpers;
using Mtgdb.Data;
using Mtgdb.Ui;
using NLog;

namespace Mtgdb.Gui
{

	[DelimitedRecord(",")][IgnoreEmptyLines][IgnoreFirst]
	public class CardCsvModel
	{
		public int Quantity			{get;set;}

		// E.g.  Castle Locthwain (Extended Art), Faerie Guidemother (Showcase), "Goat // Food (16)"
		public string Name			{get;set;}

		// E.g. Castle Locthwain
		public string SimpleName	{get;set;}
		public string Set			{get;set;}
		public string CardNumber	{get;set;}
		public string SetCode		{get;set;}
		public string Printing		{get;set;}
		public string Condition		{get;set;}
		public string Language		{get;set;}
		public string Rarity		{get;set;}
		public int    ProductID		{get;set;}
		public string SKU			{get;set;}
	}

	/// <summary>
	/// Supports importing the CSV format created by exporting a list from TCG Player's mobile app.
	/// </summary>
	public class TcgCsvDeckFormatter : IDeckFormatter
	{
		private FileHelperEngine<CardCsvModel> fileHelperEngine;

		public TcgCsvDeckFormatter(CardRepository cardRepository)
		{
			fileHelperEngine = new FileHelperEngine<CardCsvModel>();
			this.cardRepository = cardRepository;
		}
		
		public string Description => "TCGPlayer Mobile App CSV";
		public string FileNamePattern => "*.csv";
		public bool ValidateFormat(string serialized)
		{
			string header = serialized.Lines(StringSplitOptions.RemoveEmptyEntries).First();
			var headers = header.Split(',');

			// validate some of the more unique headers, to differentiate from other possible non-TCGPlayer CSV's
			return headers.Contains("Simple Name") && headers.Contains("Product ID") && headers.Contains("SKU");
		}
			    
		public bool SupportsExport => false;
		public bool SupportsImport => true;
		public bool SupportsFile => true;
		public bool UseBom => false;
		public string FormatHint => null;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
		private readonly CardRepository cardRepository;

		public virtual Deck ImportDeck(string serialized, bool exact = false)
		{
			var result = Deck.Create();

			List<CardCsvModel> cards = fileHelperEngine.ReadStringAsList(serialized);

			var unmatched = new HashSet<string>();
						
			foreach (CardCsvModel csvCard in cards)
			{
				int count = csvCard.Quantity;
				var card = GetCard(csvCard.SimpleName, csvCard.ProductID, csvCard.SetCode,csvCard.CardNumber);

				if (card == null) {
					unmatched.Add(csvCard.SimpleName);// error tracking
					continue;
				}

				//if (isSideboard)
				//	add(card, count, result.Sideboard);
				//else
				add(card, count, result.MainDeck);
				
			}

			_log.Info("Unmatched cards:\r\n{0}", string.Join("\r\n", unmatched));

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

		public string ExportDeck(string name, Deck current, bool exact = false)
		{
			throw new NotImplementedException();
		}

		private Card GetCard(string cardName, int tcgPlayerProductId, string setCode= null, string cardNumber=null)
		{
			// Note TCGPlayer set codes are unreiable, there's some rogue codes like PPELD and PRE that don't exist mtgjson data models

			// Instead, use TCG product ID.
			//Unique by printing, alt / extended art, promo.  But not by foil.
			var cardVariants = cardRepository.CardsAndTokensByTcgPlayerProductId.TryGet(tcgPlayerProductId);
						
			// TODO: Foil variant from the "Printing" CSV fiel?d which has values "Normal" or "Foil"
		
			var card = cardVariants?.FirstOrDefault();
			return card;
		}

	}
}
