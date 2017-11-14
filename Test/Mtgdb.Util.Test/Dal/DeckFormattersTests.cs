using System.IO;
using System.Linq;
using Mtgdb.Dal;
using Mtgdb.Gui;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class DeckFormattersTests : TestsBase
	{
		private readonly IKernel _kernel = new StandardKernel();
		private CardRepository _cardRepo;

		[OneTimeSetUp]
		public void Setup()
		{
			_kernel.Load<CoreModule>();
			_kernel.Load<DalModule>();

			_cardRepo = _kernel.Get<CardRepository>();
			_cardRepo.LoadFile();
			_cardRepo.Load();
		}

		[TestCase(@"D:\Games\xmage\mage-client\sample-decks")]
		public void XMage(string decksLocation)
		{
			findCards(decksLocation, new XMageDeckFormatter(_cardRepo));
		}

		[TestCase(@"D:\Games\Forge\res\quest\world")]
		[TestCase(@"C:\Users\Kolia\AppData\Roaming\Forge\decks")]
		public void Forge(string decksLocation)
		{
			findCards(decksLocation, new ForgeDeckFormatter(_cardRepo, _kernel.Get<ForgeSetRepository>()));
		}

		[TestCase(@"D:\games\Magarena-1.81\Magarena\decks")]
		public void Magarena(string decksLocation)
		{
			findCards(decksLocation, new MagarenaDeckFormatter(_cardRepo));
		}

		[Test]
		public void Mtgo()
		{
			var mtgoCardsFile = TestContext.CurrentContext.TestDirectory.AddPath("Resources\\Mtgo\\cards.txt");
			var mtgoCardNames = File.ReadAllLines(mtgoCardsFile).Distinct().OrderBy(_ => _);

			Log.Debug("Unmatched mtgo cards");
			
			var cardsByMtgoName = _cardRepo.Cards.GroupBy(MtgoDeckFormatter.ToMtgoName)
				.ToDictionary(_ => _.Key, _ => _.ToList());

			foreach (string name in mtgoCardNames)
				if (!cardsByMtgoName.ContainsKey(name))
					Log.Debug(name);
		}

		private static void findCards(string decksLocation, RegexDeckFormatter formatter)
		{
			var matches = Directory.GetFiles(decksLocation, formatter.FileNamePattern, SearchOption.AllDirectories)
				.SelectMany(file => File.ReadAllLines(file).Select(line => new { line, file }))
				.GroupBy(_ => _.line)
				.Select(gr =>
				{
					string line = gr.Key;
					var match = formatter.LineRegex.Match(line);

					Card card;
					if (match.Success)
						card = formatter.GetCard(match);
					else
						card = null;

					return new { match, card, files = gr.Select(_ => _.file).ToArray() };
				})
				.Where(_ => _.match.Success)
				.ToArray();

			if (matches.Length == 0)
				Assert.Fail("Decks not found");

			foreach (var match in matches)
			{
				if (match.card == null)
					Log.Debug("NOT FOUND {0} in\r\n{1}", match.match.Value, string.Join("\r\n", match.files.Select(_ => '\t' + _)));
			}
		}
	}
}
