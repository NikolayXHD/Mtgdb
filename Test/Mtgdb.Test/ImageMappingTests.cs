using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
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

		[TestCase("", "")]
		public void No_missing_images_And_zoom_images_match_small_ones(string setsToSkipList, string partialSetsList)
		{
			// one test for 2 checks for performance reason

			var setsToSkip = setsToSkipList?.Split(',').ToHashSet(Str.Comparer)
				?? Empty<string>.Set;
			var partialSets = partialSetsList?.Split(',').ToHashSet(Str.Comparer)
				?? Empty<string>.Set;

			var messages = new List<string>();
			foreach (var set in Repo.SetsByCode.Where(_ => !setsToSkip.Contains(_.Key)))
			foreach (var card in set.Value.Cards)
			{
				var small = Ui.GetSmallImage(card);
				var zooms = Ui.GetZoomImages(card);
				bool noZooms = zooms == null || zooms.Count == 0;

				if (small == null || noZooms)
				{
					if (partialSets.Contains(set.Value.Code) && small == null && noZooms)
						continue;

					messages.Add($"missing " +
						string.Join(", ", new[]
							{
								small == null ? "small" : null,
								noZooms ? "zoom" : null
							}.Where(_ => _ != null)) +
						$" {card.SetCode} {card.ImageName}");
				}
				else
				{
					var smallPath = small.ImageFile.FullPath;
					var zoomPath = zooms[0].ImageFile.FullPath;

					smallPath = smallPath.ToLower(Str.Culture)
						.Replace("gatherer.original", "gatherer")
						.Replace("gatherer.preprocessed", "gatherer")
						.Replace("\\lq\\", string.Empty);

					zoomPath = zoomPath.ToLower(Str.Culture)
						.Replace("gatherer.preprocessed", "gatherer")
						.Replace("\\mq\\", string.Empty);

					if (!Str.Equals(smallPath, zoomPath))
						messages.Add($"{card.SetCode}: {smallPath}{Str.Endl}{zoomPath}");
				}
			}

			messages.Should().BeEmpty();
		}

		// ReSharper disable StringLiteralTypo
		[TestCase("UGL ", TorrentsDir, "UGL", "UGL Tokens")]
		[TestCase("DDE ", TorrentsDir, "DDE", "DDE Tokens")]
		[TestCase("C17 ", XlhqDir, "C17 - Commander 2017\\300DPI Cards")]
		[TestCase("IMA ", XlhqDir, "IMA - Iconic Masters\\300DPI Cards")]
		[TestCase("UST ", XlhqDir, "UST - Unstable\\300DPI Cards")]
		[TestCase("CED ", XlhqDir, "CED - Collectors' Edition\\300DPI")]
		[TestCase("XLN ", XlhqDir, "XLN - Ixalan\\300DPI Cards")]
		[TestCase("A25 ", XlhqDir, "A25 - 25 Masters\\300DPI Cards")]
		[TestCase("CMA ", XlhqDir, "CMA - Commander Anthology\\300DPI Cards")]
		[TestCase("DDT ", XlhqDir, "DDT - Duel Decks Merfolk vs Goblins\\300DPI Cards")]
		[TestCase("DDU ", XlhqDir, "DDU - Duel Decks Elves vs Inventors\\300DPI Cards")]
		[TestCase("E02 ", XlhqDir, "E02 - Explorers of Ixalan\\300DPI Cards")]
		[TestCase("RIX ", XlhqDir, "RIX - Rivals of Ixalan\\300DPI Cards")]
		[TestCase("V17 ", XlhqDir, "V17 - From the Vault Transform\\300DPI Cards")]
		[TestCase("BBD ", XlhqDir, "BBD - Battlebond\\300DPI Cards")]
		[TestCase("DOM ", XlhqDir, "DOM - Dominaria\\300DPI Cards")]
		[TestCase("GS1 ", XlhqDir, "GS1 - Global Series Jiang Yanggu & Mu Yanling\\300DPI Cards")]
		[TestCase("M19 ", XlhqDir, "M19 - Core 2019\\300DPI Cards")]
		[TestCase("SS1 ", XlhqDir, "SS1 - Signature Spellbook Jace\\300DPI Cards")]
		[TestCase("CM2 ", XlhqDir, "CM2 - Commander Anthology 2\\300DPI Cards")]
		[TestCase("C18 ", XlhqDir, "C18 - Commander 2018\\300DPI Cards")]
		[TestCase("GRN ", XlhqDir, "GRN - Guilds of Ravnica\\300DPI Cards")]
		[TestCase("GK1 ", XlhqDir, "GK1 - GRN Guild Kit\\300DPI Cards")]
		[TestCase("UMA ", XlhqDir, "UMA - Ultimate Masters\\300DPI Cards")]
		[TestCase("WAR ", XlhqDir, "WAR - War of the Spark\\300DPI Cards")]
		[TestCase("M20 ", XlhqDir, "M20 - Core 2020\\300DPI Cards")]
		[TestCase("SS2 ", XlhqDir, "SS2 - Signature Spellbook Gideon\\300DPI Cards")]
		[TestCase("MH1 ", XlhqDir, "MH1 - Modern Horizons\\300DPI Cards")]
		[TestCase("PWAR", XlhqDir, "Promos\\pWAR - War of the Spark Promos", "WAR - War of the Spark\\300DPI Cards")]

		[TestCase("c19 ", PreprocesedDir, "c19")]
		[TestCase("cmb1", PreprocesedDir, "cmb1")]
		[TestCase("eld ", PreprocesedDir, "eld")]
		[TestCase("gn2 ", PreprocesedDir, "gn2")]
		[TestCase("ha1 ", PreprocesedDir, "ha1")]
		[TestCase("peld", PreprocesedDir, "peld")]
		[TestCase("ptg ", PreprocesedDir, "ptg")]
		[TestCase("puma", PreprocesedDir, "puma")]
		[TestCase("hho ", PreprocesedDir, "hho")]
		// ReSharper restore StringLiteralTypo
		public void Set_images_are_from_expected_directory(
			string setCode, string baseDir, params string[] expectedSubdirs)
		{
			setCode = setCode.Trim();
			var expectedDirsSet = expectedSubdirs
				.Select(_ => Path.Combine(baseDir, _))
				.ToList();

			var set = Repo.SetsByCode[setCode];
			foreach (var card in set.ActualCards)
			{
				var imageModel = Ui.GetSmallImage(card);
				var dir = Path.GetDirectoryName(imageModel.ImageFile.FullPath);
				Assert.That(expectedDirsSet, Does.Contain(dir).IgnoreCase,
					$"{card.ImageName} {dir}");
			}
		}

		private const string XlhqDir = "D:\\distrib\\games\\mtg\\Mega\\XLHQ";

		private const string TorrentsDir =
			"D:\\distrib\\games\\mtg\\XLHQ-Sets-Torrent.Unpacked";

		private const string OriginalDir =
			"D:\\distrib\\games\\mtg\\Gatherer.Original";
		private const string PreprocesedDir =
			"D:\\distrib\\games\\mtg\\Gatherer.Preprocessed";
	}
}
