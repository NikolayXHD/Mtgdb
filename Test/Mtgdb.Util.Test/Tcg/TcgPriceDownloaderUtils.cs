using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Data;
using Mtgdb.Test;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class TcgPriceDownloaderUtils: TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadCards();
		}

		[Test]
		public void MapSets()
		{
			var tcgSets = new TcgParser().ParseSets();

			var notMapped = new List<TcgSet>();
			var mapped = new Dictionary<string, Set>();

			foreach (var set in tcgSets)
			{
				var matched = Repo.SetsByCode.Values
					.Where(_ => Str.Equals(_.Name, set.Name))
					.ToArray();

				if (matched.Length != 1)
					notMapped.Add(set);
				else
					mapped.Add(set.Code, matched[0]);
			}

			var mappedReport = string.Join(Str.Endl, mapped.Select(pair => $"{pair.Value.Code}\t{pair.Key}"));

			Log.Debug(mappedReport);
		}

		[Test]
		public void ParseTcgCards()
		{
			var tcgParser = new TcgParser();
			var cardsBySet = tcgParser.ParseCards();

			Log.Debug(() => Json.Serialize(cardsBySet));
		}

		[Test]
		public void PrintNotMappedSets()
		{
			var tcgParser = new TcgParser();

			var tcgSetBySet = tcgParser.GetTcgSetBySet();

			var mappedCodes = new HashSet<string>(tcgSetBySet.Keys);
			var mappedTcgCodes = new HashSet<string>(tcgSetBySet.Values);

			var notMappedSets = Repo.SetsByCode.Values.Select(_ => _.Code)
				.Except(mappedCodes)
				.ToArray();

			var notMappedReport =
				string.Join(Str.Endl,
					notMappedSets.Select(code => Repo.SetsByCode[code])
						.OrderByDescending(set => set.ReleaseDate)
						.Select(set => $"{set.ReleaseDate}\t{set.Code}\t{set.Name}")
						.ToArray());

			Log.Debug("Not mapped sets:");
			Log.Debug(notMappedReport);

			var tcgSets = tcgParser.ParseSets();

			var notMappedTcgSetsReport = string.Join(Str.Endl,
				tcgSets
					.Where(s => !mappedTcgCodes.Contains(s.Code))
					.Select(s => $"{s.Code}\t{s.Name}")
			);

			Log.Debug("Not mapped TCG sets:");
			Log.Debug(notMappedTcgSetsReport);
		}

		[TestCase(null)]
		//[TestCase("pMEI")]
		public void MapCards(string minSetCode)
		{
			var tcgParser = new TcgParser();

			var tcgSetsBySet = tcgParser.GetTcgSetBySet();
			var tcgCardsByTcgSet = tcgParser.GetTcgCardsByTcgSet();
			var orderByCard = tcgParser.GetOrderByCard();

			var preProcessedTcgNameByCode = new Dictionary<TcgCard, string>();

			foreach (var cardsByCode in tcgCardsByTcgSet.Values)
				foreach (var tcgCard in cardsByCode.Values)
					preProcessedTcgNameByCode.Add(tcgCard, preProcessTcgName(tcgCard));

			foreach (var set in Repo.SetsByCode.Values.OrderBy(_ => _.ReleaseDate))
			{
				if (minSetCode != null && Str.Compare(set.Code, minSetCode) < 0)
					continue;

				if (!tcgSetsBySet.TryGetValue(set.Code, out string tcgSet))
					continue;

				var tcgCards = tcgCardsByTcgSet[tcgSet];

				foreach (var pair in set.CardsByName.OrderBy(_ => _.Key))
				{
					var cards = pair.Value
						.OrderBy(_ => _.Number)
						.ThenBy(card => card.ImageName)
						.ToList();

					string name = preProcessMtgjsonName(cards[0]);

					var matchingTcgCards = tcgCards
						.Where(_ => preProcessedTcgNameByCode[_.Value].Equals(name, Str.Comparison))
						.OrderBy(_ => orderByCard.TryGet(tcgSet)?.TryGet(_.Key))
						.ThenBy(_ => _.Value.Number)
						.ThenBy(_ => _.Value.Name)
						.ToList();

					removeFoilDuplicates(matchingTcgCards);

					if (matchingTcgCards.Count != 1 || cards.Count != 1)
					{
						var message = new StringBuilder();
						message.Append(set.Code);
						message.Append(" -> ");
						message.AppendLine(tcgSet);

						for (int i = 0; i < cards.Count || i < matchingTcgCards.Count; i++)
						{
							message.Append("\t");
							if (i < cards.Count)
							{
								message.Append(cards[i].ImageName);
								message.Append(" [");
								message.Append(cards[i].Artist);
								message.Append(']');
							}
							else
								message.Append("?");

							message.Append(" -> ");

							if (i < matchingTcgCards.Count)
								message.Append($"{matchingTcgCards[i].Key} #{matchingTcgCards[i].Value.Number} {matchingTcgCards[i].Value.Name}");
							else
								message.Append("?");

							message.AppendLine();
						}

						Log.Info(message.ToString());
					}
				}
			}
		}

		private static string preProcessMtgjsonName(Card card)
		{
			string name = card.NameNormalized;

			if (Str.Equals(card.Layout, CardLayouts.Split) || Str.Equals(card.Layout, CardLayouts.Aftermath))
				return string.Join("/", card.Faces.Select(c=>c.NameNormalized));

			if (Str.Equals(card.Layout, CardLayouts.Flip))
				return card.Faces.Main.NameNormalized;

			if (name.EndsWith(" token card", Str.Comparison))
				name = name.Substring(0, name.Length - " card".Length);

			return name;
		}

		private string preProcessTcgName(TcgCard tcgCard)
		{
			string name = tcgCard.Name;

			if (Str.Equals(name, "B.F.M. (Big Furry Monster Left)") || Str.Equals(name, "B.F.M. (Big Furry Monster Right)"))
				return "B.F.M. (Big Furry Monster)";

			if (Str.Equals(name, "Our Market Research Shows That Players Like Really Long Card Names...."))
				return "Our Market Research Shows That Players Like Really Long Card Names So We Made this Card to Have the Absolute Longest Card Name Ever Elemental";

			if (Str.Equals(name, "Ach! Hans, Run!"))
				return "\"Ach! Hans, Run!\"";

			if (Str.Equals(name, "The Ultimate Nightmare of Wizards of the Coast Customer Service"))
				return "The Ultimate Nightmare of Wizards of the Coast® Customer Service";

			name = name.Replace(" // ", "/");

			// Sauté
			name = name.RemoveDiacritics();

			// (FOIL), (263), (b)
			name = _preProcessRegex.Replace(name, string.Empty);

			return name;
		}

		private static void removeFoilDuplicates(List<KeyValuePair<string, TcgCard>> matchingTcgCards)
		{
			for (int i = matchingTcgCards.Count - 1; i >= 0; i--)
			{
				var m1 = matchingTcgCards[i];
				string foil = " (foil)";

				if (!m1.Value.Name.EndsWith(foil, Str.Comparison))
					continue;

				for (int j = i - 1; j >= 0; j--)
				{
					var m2 = matchingTcgCards[j];
					if (m2.Value.Number == m1.Value.Number && Str.Equals(m2.Value.Name + foil, m1.Value.Name))
					{
						matchingTcgCards.RemoveAt(i);
						break;
					}
				}
			}
		}

		private readonly Regex _preProcessRegex = new Regex(
			@" ?((?:\[[^\]]+\] ?)+|(?:\([^\)]+\) ?)+|- ?full art|\(?SDCC \d* EXCLUSIVE\)?)$",
			RegexOptions.IgnoreCase);
	}
}