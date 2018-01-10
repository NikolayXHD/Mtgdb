﻿using System;
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
			LoadModules();
			LoadCards();

			ImgRepo.LoadFiles();
			ImgRepo.LoadSmall();
			ImgRepo.LoadZoom();
		}

		[Test, Order(1)]
		public void No_cards_without_image()
		{
			foreach (var set in Repo.SetsByCode)
				foreach (var card in set.Value.Cards)
				{
					var small = Repo.GetSmallImage(card, ImgRepo);
					var zooms = Repo.GetZoomImages(card, ImgRepo);

					Assert.That(small, Is.Not.Null);
					Assert.That(zooms, Is.Not.Null);
					Assert.That(zooms, Is.Not.Empty);
				}
		}

		[Test, Order(2)]
		public void Zoom_images_match_small_ones()
		{
			foreach (var set in Repo.SetsByCode)
				foreach (var card in set.Value.Cards)
				{
					var small = Repo.GetSmallImage(card, ImgRepo);
					var zooms = Repo.GetZoomImages(card, ImgRepo);

					var smallPath = small.FullPath;
					var zoomPath = zooms[0].FullPath;

					smallPath = smallPath.ToLowerInvariant()
						.Replace("gatherer.original", "gatherer")
						.Replace("\\lq\\", string.Empty);

					zoomPath = zoomPath.ToLowerInvariant()
						.Replace("gatherer.preprocessed", "gatherer")
						.Replace("\\mq\\", string.Empty);

					if (!Str.Equals(smallPath, zoomPath))
						Assert.Fail(smallPath + Environment.NewLine + zoomPath);
				}
		}

		[TestCase("c17", XlhqDir, "C17 - Commander 2017\\300DPI Cards")]
		[TestCase("ima", XlhqDir, "IMA - Iconic Masters\\300DPI Cards")]
		[TestCase("ust", XlhqDir, "UST - Unstable\\300DPI Cards")]
		[TestCase("ced", XlhqDir, "CED - Collectors\' Edition\\300DPI")]
		[TestCase("xln", XlhqDir, "XLN - Ixalan\\300DPI Cards")]
		[TestCase("ugl", XlhqTorrentsDir, "UGL", "UGL Tokens")]
		[TestCase("dde", XlhqTorrentsDir, "DDE", "DDE Tokens")]
		public void Set_images_are_from_expected_directory(string setCode, string baseDir, params string[] expectedSubdirs)
		{
			var expectedDirsSet = expectedSubdirs
				.Select(_ => Path.Combine(baseDir, _))
				.ToList();
				
			var set = Repo.SetsByCode[setCode];
			foreach (var card in set.Cards)
			{
				var imageModel = Repo.GetSmallImage(card, ImgRepo);
				var dir = Path.GetDirectoryName(imageModel.FullPath);
				Assert.That(expectedDirsSet, Does.Contain(dir).IgnoreCase, card.ImageName);
			}
		}

		private const string XlhqDir = "D:\\Distrib\\games\\mtg\\Mega\\XLHQ";
		private const string XlhqTorrentsDir = "D:\\Distrib\\games\\mtg\\XLHQ-Sets-Torrent.Unpacked";
	}
}