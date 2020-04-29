using System.IO;
using System.Linq;
using Mtgdb.Data;
using Mtgdb.Dev;
using Mtgdb.Gui;
using Mtgdb.Test;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class DeckFormattersTests : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadCards();
		}

		[Test]
		public void XMage()
		{
			FsPath decksLocation = DevPaths.DataDrive.Join("games", "xmage", "mage-client", "sample-decks");
			findCards(decksLocation, new XMageDeckFormatter(Repo));
		}

		[Test]
		public void Forge()
		{
			var locations = new[]
			{
				DevPaths.DataDrive.Join("games", "forge", "res", "quest", "world"),
				DevPaths.WindowsDrive.Join("Users", "Kolia", "AppData", "Roaming", "Forge", "decks")
			};

			foreach (var decksLocation in locations)
				findCards(decksLocation, new ForgeDeckFormatter(Repo, Kernel.Get<ForgeSetRepository>()));
		}

		[Test]
		public void Magarena()
		{
			FsPath decksLocation = DevPaths.DataDrive.Join("games", "Magarena-1.81", "Magarena", "decks");
			findCards(decksLocation, new MagarenaDeckFormatter(Repo));
		}

		[Test]
		public void Mtgo()
		{
			var mtgoCardsFile = new FsPath(TestContext.CurrentContext.TestDirectory, "Resources", "Mtgo", "cards.txt");
			var mtgoCardNames = mtgoCardsFile.ReadAllLines().Distinct().OrderBy(Str.Comparer);

			Log.Debug("Unmatched mtgo cards");

			var cardsByMtgoName = Repo.Cards.GroupBy(MtgoDeckFormatter.ToMtgoName)
				.ToDictionary(_ => _.Key, _ => _.ToList());

			foreach (string name in mtgoCardNames)
				if (!cardsByMtgoName.ContainsKey(name))
					Log.Debug(name);
		}

		private void findCards(FsPath decksLocation, RegexDeckFormatter formatter)
		{
			var matches = decksLocation.EnumerateFiles(formatter.FileNamePattern, SearchOption.AllDirectories)
				.SelectMany(file => file.ReadAllLines().Select(line => new { line, file }))
				.GroupBy(_ => _.line)
				.Select(gr =>
				{
					string line = gr.Key;
					var match = formatter.LineRegex.Match(line);

					var card = match.Success
						? formatter.GetCard(match)
						: null;

					return new { match, card, files = gr.Select(_ => _.file).ToArray() };
				})
				.Where(_ => _.match.Success)
				.ToArray();

			if (matches.Length == 0)
				Assert.Fail("Decks not found");

			foreach (var match in matches)
			{
				if (match.card == null)
					Log.Debug("NOT FOUND {0} in\r\n{1}", match.match.Value, string.Join("\r\n", match.files.Select(_ => '\t' + _.Value)));
			}
		}
	}
}
