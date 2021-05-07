using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Mtgdb.Dev;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class ImageMappingTests: TestsBase
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
			var missingImageSets = new HashSet<string>();
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

						missingImageSets.Add(card.SetCode);
					}
					else
					{
						var smallPath = small.ImageFile.FullPath.Value;
						var zoomPath = zooms[0].ImageFile.FullPath.Value;

						smallPath = smallPath.ToLower(Str.Culture)
							.Replace("gatherer.original", "gatherer")
							.Replace("gatherer.preprocessed", "gatherer")
							.Replace($"{FsPath.Separator}lq{FsPath.Separator}", $"{FsPath.Separator}");

						zoomPath = zoomPath.ToLower(Str.Culture)
							.Replace("gatherer.preprocessed", "gatherer")
							.Replace($"{FsPath.Separator}mq{FsPath.Separator}", $"{FsPath.Separator}");

						if (!Str.Equals(smallPath, zoomPath))
							messages.Add($"{card.SetCode}: {smallPath}{Str.Endl}{zoomPath}");
					}
				}

			using (new AssertionScope())
			{
				missingImageSets.Should().BeEmpty();
				messages.Should().BeEmpty();
			}
		}

		private static IEnumerable<TestCaseData> Cases
		{
			get
			{
				// ReSharper disable StringLiteralTypo
				yield return new TestCaseData("UGL ", new[]
				{
					DevPaths.TorrentsDir.Join("UGL"),
					DevPaths.TorrentsDir.Join("UGL Tokens")
				});
				yield return new TestCaseData("DDE ", new[]
				{
					DevPaths.TorrentsDir.Join("DDE"),
					DevPaths.TorrentsDir.Join("DDE Tokens")
				});
				yield return new TestCaseData("C17 ", new[] {DevPaths.XlhqDir.Join("C17 - Commander 2017", "300DPI Cards")});
				yield return new TestCaseData("IMA ", new[] {DevPaths.XlhqDir.Join("IMA - Iconic Masters", "300DPI Cards")});
				yield return new TestCaseData("UST ", new[] {DevPaths.XlhqDir.Join("UST - Unstable", "300DPI Cards")});
				yield return new TestCaseData("CED ", new[] {DevPaths.XlhqDir.Join("CED - Collectors' Edition", "300DPI")});
				yield return new TestCaseData("XLN ", new[] {DevPaths.XlhqDir.Join("XLN - Ixalan", "300DPI Cards")});
				yield return new TestCaseData("A25 ", new[] {DevPaths.XlhqDir.Join("A25 - 25 Masters", "300DPI Cards")});
				yield return new TestCaseData("CMA ", new[] {DevPaths.XlhqDir.Join("CMA - Commander Anthology", "300DPI Cards")});
				yield return new TestCaseData("DDT ", new[] {DevPaths.XlhqDir.Join("DDT - Duel Decks Merfolk vs Goblins", "300DPI Cards")});
				yield return new TestCaseData("DDU ", new[] {DevPaths.XlhqDir.Join("DDU - Duel Decks Elves vs Inventors", "300DPI Cards")});
				yield return new TestCaseData("E02 ", new[] {DevPaths.XlhqDir.Join("E02 - Explorers of Ixalan", "300DPI Cards")});
				yield return new TestCaseData("RIX ", new[] {DevPaths.XlhqDir.Join("RIX - Rivals of Ixalan", "300DPI Cards")});
				yield return new TestCaseData("V17 ", new[] {DevPaths.XlhqDir.Join("V17 - From the Vault Transform", "300DPI Cards")});
				yield return new TestCaseData("BBD ", new[] {DevPaths.XlhqDir.Join("BBD - Battlebond", "300DPI Cards")});
				yield return new TestCaseData("DOM ", new[] {DevPaths.XlhqDir.Join("DOM - Dominaria", "300DPI Cards")});
				yield return new TestCaseData("GS1 ", new[] {DevPaths.XlhqDir.Join("GS1 - Global Series Jiang Yanggu & Mu Yanling", "300DPI Cards")});
				yield return new TestCaseData("M19 ", new[] {DevPaths.XlhqDir.Join("M19 - Core 2019", "300DPI Cards")});
				yield return new TestCaseData("SS1 ", new[] {DevPaths.XlhqDir.Join("SS1 - Signature Spellbook Jace", "300DPI Cards")});
				yield return new TestCaseData("CM2 ", new[] {DevPaths.XlhqDir.Join("CM2 - Commander Anthology 2", "300DPI Cards")});
				yield return new TestCaseData("C18 ", new[] {DevPaths.XlhqDir.Join("C18 - Commander 2018", "300DPI Cards")});
				yield return new TestCaseData("GRN ", new[] {DevPaths.XlhqDir.Join("GRN - Guilds of Ravnica", "300DPI Cards")});
				yield return new TestCaseData("GK1 ", new[] {DevPaths.XlhqDir.Join("GK1 - GRN Guild Kit", "300DPI Cards")});
				yield return new TestCaseData("UMA ", new[] {DevPaths.XlhqDir.Join("UMA - Ultimate Masters", "300DPI Cards")});
				yield return new TestCaseData("WAR ", new[] {DevPaths.XlhqDir.Join("WAR - War of the Spark", "300DPI Cards")});
				yield return new TestCaseData("M20 ", new[] {DevPaths.XlhqDir.Join("M20 - Core 2020", "300DPI Cards")});
				yield return new TestCaseData("SS2 ", new[] {DevPaths.XlhqDir.Join("SS2 - Signature Spellbook Gideon", "300DPI Cards")});
				yield return new TestCaseData("MH1 ", new[] {DevPaths.XlhqDir.Join("MH1 - Modern Horizons", "300DPI Cards")});
				yield return new TestCaseData("PWAR", new[] {DevPaths.GathererPreprocessedCardsDir.Join("PWAR")});
				yield return new TestCaseData("c19 ", new[] {DevPaths.GathererPreprocessedCardsDir.Join("c19")});
				yield return new TestCaseData("cmb1", new[] {DevPaths.GathererPreprocessedCardsDir.Join("cmb1")});
				yield return new TestCaseData("eld ", new[] {DevPaths.GathererPreprocessedCardsDir.Join("eld")});
				yield return new TestCaseData("gn2 ", new[] {DevPaths.GathererPreprocessedCardsDir.Join("gn2")});
				yield return new TestCaseData("ha1 ", new[] {DevPaths.GathererPreprocessedCardsDir.Join("ha1")});
				yield return new TestCaseData("peld", new[] {DevPaths.GathererPreprocessedCardsDir.Join("peld")});
				yield return new TestCaseData("ptg ", new[] {DevPaths.GathererPreprocessedCardsDir.Join("ptg")});
				yield return new TestCaseData("puma", new[] {DevPaths.GathererPreprocessedCardsDir.Join("puma")});
				yield return new TestCaseData("hho ", new[] {DevPaths.GathererPreprocessedCardsDir.Join("hho")});

				yield return new TestCaseData("PLGS", new[] {DevPaths.GathererPreprocessedCardsDir.Join("PLGS")});
				yield return new TestCaseData("HA3", new[] {DevPaths.GathererPreprocessedCardsDir.Join("HA3")});
				yield return new TestCaseData("SLU", new[] {DevPaths.GathererPreprocessedCardsDir.Join("SLU")});
				yield return new TestCaseData("SS3", new[] {DevPaths.GathererPreprocessedCardsDir.Join("SS3")});
				yield return new TestCaseData("2XM", new[] {DevPaths.GathererPreprocessedCardsDir.Join("2XM")});

				yield return new TestCaseData("CC1", new[] {DevPaths.GathererPreprocessedCardsDir.Join("CC1")});
				yield return new TestCaseData("CMR", new[] {DevPaths.GathererPreprocessedCardsDir.Join("CMR")});
				yield return new TestCaseData("AKR", new[] {DevPaths.GathererPreprocessedCardsDir.Join("AKR")});
				yield return new TestCaseData("ANB", new[] {DevPaths.GathererPreprocessedCardsDir.Join("ANB")});
				yield return new TestCaseData("AJMP", new[] {DevPaths.GathererPreprocessedCardsDir.Join("AJMP")});
				yield return new TestCaseData("JMP", new[] {DevPaths.GathererPreprocessedCardsDir.Join("JMP")});
				yield return new TestCaseData("FJMP", new[] {DevPaths.GathererPreprocessedCardsDir.Join("FJMP")});

				yield return new TestCaseData("M21", new[] {DevPaths.GathererPreprocessedCardsDir.Join("M21")});
				yield return new TestCaseData("HTR18", new[] {DevPaths.GathererPreprocessedCardsDir.Join("HTR18")});

				// ReSharper restore StringLiteralTypo
			}
		}

		[TestCaseSource(nameof(Cases))]
		public void Set_images_are_from_expected_directory(string setCode, params FsPath[] expectedDirsSet)
		{
			setCode = setCode.Trim();
			var set = Repo.SetsByCode[setCode];
			foreach (var card in set.ActualCards)
			{
				var imageModel = Ui.GetSmallImage(card);
				FsPath dir = imageModel.ImageFile.FullPath.Parent();
				using (new AssertionScope(card.ToString()))
					new[] {dir.Value}.Should().BeSubsetOf(expectedDirsSet.Select(_ => _.Value));
			}
		}
	}
}
