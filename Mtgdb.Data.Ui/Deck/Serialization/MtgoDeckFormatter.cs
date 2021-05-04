﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Data;

namespace Mtgdb.Ui
{
	public class MtgoDeckFormatter : RegexDeckFormatter
	{
		public MtgoDeckFormatter(CardRepository repo)
			:base(repo)
		{
		}

		public override Deck ImportDeck(string serialized, bool exact = false)
		{
			ensureLoaded();

			_isSideboard = false;
			_sideboardIndicator = getSideboardIndicator(serialized);
			var deck = base.ImportDeck(serialized, exact);
			if (!exact)
				new XitaxDeckTransformation(Repo).Transform(deck);
			return deck;
		}

		private static string getSideboardIndicator(string serialized)
		{
			string sideboardIndicator;
			if (serialized.IndexOf("sideboard", Str.Comparison) >= 0)
				sideboardIndicator = "sideboard";
			else
				sideboardIndicator = string.Empty;
			return sideboardIndicator;
		}



		public override Card GetCard(Match match)
		{
			string name = match.Groups["name"].Value;
			name = FromMtgoName(name);

			var cards = _cardsByName.TryGet(name);

			var card = cards
				?.OrderByDescending(_ => _.ReleaseDate)
				.ThenBy(_ => _.ImageName)
				.FirstOrDefault();

			return card;
		}

		protected override IList<string> SplitToLines(string serialized)
		{
			_sideboardIndicator = getSideboardIndicator(serialized);

			var lines = serialized.Trim().Lines(StringSplitOptions.None);

			var result = new List<string>();

			foreach (string line in lines)
			{
				var match = LineRegex.Match(line);

				if (line == string.Empty)
				{
					if (result.Count > 0 && result[result.Count - 1] != string.Empty)
						result.Add(line);
				}
				else if (isSideboardIndicator(line))
					result.Add(line);
				else if (match.Success)
					result.Add(line);
				else if (isKnownMtgoName(line.Trim()))
					result.Add("1 " + line.Trim());
				else if (line.IndexOf("\t", Str.Comparison) >= 0)
				{
					var parts = line.Split(Sequence.Array('\t'), StringSplitOptions.RemoveEmptyEntries);

					for (int i = 0; i < parts.Length; i++)
					{
						if (isKnownMtgoName(parts[i]))
						{
							if (i > 0 && parts[i - 1].All(char.IsDigit))
								result.Add(parts[i - 1] + ' ' + parts[i]);
							else if (i < parts.Length - 1 && parts[i + 1].All(char.IsDigit))
								result.Add(parts[i + 1] + ' ' + parts[i]);
							break;
						}
					}
				}
				else
				{
					var splits = _splitterRegex.Matches(line)
						.OfType<Match>()
						.Select(m => m.Index)
						.ToList();

					if (splits.Count > 0)
						splits.Add(line.Length);

					for (int i = 0; i < splits.Count - 1; i++)
					{
						string substring = line.Substring(splits[i], splits[i + 1] - splits[i]).TrimEnd();

						var substringMatch = LineRegex.Match(substring);
						if (substringMatch.Success)
							result.Add(substring);
					}
				}
			}

			return result;
		}

		private bool isKnownMtgoName(string name)
		{
			return _cardsByName.ContainsKey(FromMtgoName(name));
		}

		public override bool IsSideboard(Match match, string line)
		{
			_isSideboard |= isSideboardIndicator(line);
			return _isSideboard;
		}

		private bool isSideboardIndicator(string line)
		{
			return
				_sideboardIndicator == string.Empty && line == string.Empty ||
				_sideboardIndicator != string.Empty && line.IndexOf(_sideboardIndicator, Str.Comparison) >= 0;
		}

		public static string FromMtgoName(string mtgoName)
		{
			var separators = Sequence.Array(" // ", " / ", "//", "/");

			int separatorIndex = -1;

			for (int i = 0; i < separators.Length; i++)
			{
				separatorIndex = mtgoName.IndexOf(separators[i], Str.Comparison);
				if (separatorIndex >= 0)
					break;
			}

			if (separatorIndex >= 0)
				mtgoName = mtgoName.Substring(0, separatorIndex);

			if (_nameByMtgoName.TryGetValue(mtgoName, out string result))
				return result;

			return mtgoName;
		}

		public static string ToMtgoName(Card card)
		{
			string name;
			if (Str.Equals(card.Layout, CardLayouts.Split) || Str.Equals(card.Layout, CardLayouts.Aftermath))
				name = card.Faces[0].NameEn + "/" + card.Faces[1].NameEn;
			else
				name = card.NameEn;

			if (_mtgoNameByName.TryGetValue(name, out string result))
				return result;

			return name;
		}

		protected override string ExportDeckImplementation(string name, Deck current, bool exact = false)
		{
			var result = new StringBuilder();

			writeCards(result, current.MainDeck, exact);
			result.AppendLine();
			writeCards(result, current.Sideboard, exact);
			// ignore maybeboard

			return result.ToString();
		}

		private void writeCards(StringBuilder result, DeckZone deckZone, bool exact = false)
		{
			foreach (var cardId in deckZone.Order)
			{
				var card = exact
					? Repo.CardsById[cardId]
					: Repo.CardsById[cardId].Faces.Main;

				if (card == null)
					continue;

				var count = deckZone.Count[cardId];
				result.AppendLine($"{count} {ToMtgoName(card)}");
			}
		}

		public override bool ValidateFormat(string serialized)
		{
			ensureLoaded();
			var lines = SplitToLines(serialized);
			return lines.Count != 0;
		}

		public override Regex LineRegex { get; } = _lineRegex;

		private void ensureLoaded()
		{
			if (_cardsByName == null)
				_cardsByName = Repo.Cards
					.GroupBy(_ => _.NameEn)
					.ToDictionary(_ => _.Key, _ => _.ToList());
		}

		public override string Description => "Magic The Gathering Online {type}";
		public override string FileNamePattern => @"*.txt";

		private bool _isSideboard;
		private string _sideboardIndicator;

		private Dictionary<string, List<Card>> _cardsByName;

		private static readonly Dictionary<string, string> _mtgoNameByName =
			new Dictionary<string, string>
			{
				// ReSharper disable StringLiteralTypo

				{ "Jötun Grunt", "Jotun Grunt" },
				{ "Jötun Owl Keeper", "Jotun Owl Keeper" },
				{ "Bösium Strip", "Bosium Strip" }

				// ReSharper restore StringLiteralTypo
			};

		private static readonly Dictionary<string, string> _nameByMtgoName =
			_mtgoNameByName.ToDictionary(_ => _.Value, _ => _.Key);

		private static readonly Regex _splitterRegex =
			new Regex(@"\b(?<count>\d+)x?\s+\b", RegexOptions.IgnoreCase);

		private static readonly Regex _lineRegex =
			new Regex(@"^(?<count>\d+)x?\s+(?<name>[\S ]+)(?:\t.*)?$", RegexOptions.IgnoreCase);
	}
}
