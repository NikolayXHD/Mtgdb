using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class ImageMappingTests : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadCards();

			ImgRepo.LoadFiles(Sequence.From("dev", "xlhq"));
			ImgRepo.LoadSmall();
			ImgRepo.LoadZoom();
		}

		[Test, Order(1)]
		public void No_cards_without_image()
		{
			var messages = new List<string>();

			foreach (var set in Repo.SetsByCode)
				foreach (var card in set.Value.Cards)
				{
					var small = Ui.GetSmallImage(card);
					var zooms = Ui.GetZoomImages(card);

					if (small == null || zooms == null || zooms.Count == 0)
						messages.Add($"{card.SetCode} {card.ImageName}");
				}

			Assert.That(messages, Is.Empty);
		}

		[Test, Order(2)]
		public void Zoom_images_match_small_ones()
		{
			var messages = new List<string>();

			foreach (var set in Repo.SetsByCode)
				foreach (var card in set.Value.Cards)
				{
					var small = Ui.GetSmallImage(card);
					var zooms = Ui.GetZoomImages(card);

					var smallPath = small.ImageFile.FullPath;
					var zoomPath = zooms[0].ImageFile.FullPath;

					smallPath = smallPath.ToLower(Str.Culture)
						.Replace("gatherer.original", "gatherer")
						.Replace("\\lq\\", string.Empty);

					zoomPath = zoomPath.ToLower(Str.Culture)
						.Replace("gatherer.preprocessed", "gatherer")
						.Replace("\\mq\\", string.Empty);

					if (!Str.Equals(smallPath, zoomPath))
						messages.Add($"{card.SetCode}: {smallPath}{Str.Endl}{zoomPath}");
				}

			Assert.That(messages, Is.Empty);
		}

		// ReSharper disable StringLiteralTypo
		[TestCase("UGL", XlhqTorrentsDir, "UGL", "UGL Tokens")]
		[TestCase("DDE", XlhqTorrentsDir, "DDE", "DDE Tokens")]
		[TestCase("C17", XlhqDir, "C17 - Commander 2017\\300DPI Cards")]
		[TestCase("IMA", XlhqDir, "IMA - Iconic Masters\\300DPI Cards")]
		[TestCase("UST", XlhqDir, "UST - Unstable\\300DPI Cards")]
		[TestCase("CED", XlhqDir, "CED - Collectors' Edition\\300DPI")]
		[TestCase("XLN", XlhqDir, "XLN - Ixalan\\300DPI Cards")]

		[TestCase("A25", XlhqDir, "A25 - 25 Masters\\300DPI Cards")]
		[TestCase("CMA", XlhqDir, "CMA - Commander Anthology\\300DPI Cards")]
		[TestCase("DDT", XlhqDir, "DDT - Duel Decks Merfolk vs Goblins\\300DPI Cards")]
		[TestCase("DDU", XlhqDir, "DDU - Duel Decks Elves vs Inventors\\300DPI Cards")]
		[TestCase("E02", XlhqDir, "E02 - Explorers of Ixalan\\300DPI Cards")]
		[TestCase("RIX", XlhqDir, "RIX - Rivals of Ixalan\\300DPI Cards")]
		[TestCase("V17", XlhqDir, "V17 - From the Vault Transform\\300DPI Cards")]

		[TestCase("BBD", XlhqDir, "BBD - Battlebond\\300DPI Cards")]
		[TestCase("DOM", XlhqDir, "DOM - Dominaria\\300DPI Cards")]
		[TestCase("GS1", XlhqDir, "GS1 - Global Series Jiang Yanggu & Mu Yanling\\300DPI Cards")]
		[TestCase("M19", XlhqDir, "M19 - Core 2019\\300DPI Cards")]
		[TestCase("SS1", XlhqDir, "SS1 - Signature Spellbook Jace\\300DPI Cards")]

		[TestCase("CM2", XlhqDir, "CM2 - Commander Anthology 2\\300DPI Cards")]
		[TestCase("C18", XlhqDir, "C18 - Commander 2018\\300DPI Cards")]
		[TestCase("GRN", XlhqDir, "GRN - Guilds of Ravnica\\300DPI Cards")]
		// ReSharper restore StringLiteralTypo
		public void Set_images_are_from_expected_directory(string setCode, string baseDir, params string[] expectedSubdirs)
		{
			var expectedDirsSet = expectedSubdirs
				.Select(_ => Path.Combine(baseDir, _))
				.ToList();

			var set = Repo.SetsByCode[setCode];
			foreach (var card in set.Cards)
			{
				var imageModel = Ui.GetSmallImage(card);
				var dir = Path.GetDirectoryName(imageModel.ImageFile.FullPath);
				Assert.That(expectedDirsSet, Does.Contain(dir).IgnoreCase, $"{card.ImageName} {dir}");
			}
		}

		private const string XlhqDir = "D:\\Distrib\\games\\mtg\\Mega\\XLHQ";
		private const string XlhqTorrentsDir = "D:\\Distrib\\games\\mtg\\XLHQ-Sets-Torrent.Unpacked";
		private const string GathererDir = "D:\\Distrib\\games\\mtg\\Gatherer.Original";
	}
}