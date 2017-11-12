using System;
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
	}
}