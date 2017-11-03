using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Mtgdb.Dal;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class DalTests
	{
		[Test]
		public void Test_repository_is_not_empty()
		{
			TestLoader.LoadModules();
			var repo = TestLoader.CardRepository;

			repo.LoadFile();
			repo.Load();

			Assert.That(repo.Cards, Is.Not.Null);
			Assert.That(repo.Cards, Is.Not.Empty);

			Assert.That(repo.SetsByCode, Is.Not.Null);
			Assert.That(repo.SetsByCode.Count, Is.GreaterThan(0));
		}

		[Test]
		public void Map_xlhq_sets()
		{
			TestLoader.LoadModules();
			
			TestLoader.CardRepository.LoadFile();
			TestLoader.CardRepository.Load();

			TestLoader.ImageRepository.LoadFiles();
			TestLoader.ImageRepository.LoadZoom();

			var zoomImages = TestLoader.ImageRepository.GetAllZooms();

			var imagesBySet = zoomImages.GroupBy(_ => _.SetCode ?? string.Empty, Str.Comparer)
				.ToDictionary(
					gr => gr.Key,
					gr => gr.OrderBy(_ => Path.GetFileNameWithoutExtension(_.FullPath)).ToList(),
					Str.Comparer
				);

			var sets = TestLoader.CardRepository.SetsByCode.Values.OrderBy(_=>_.ReleaseDate);
			foreach (var set in sets)
			{
				Console.WriteLine($"{set.Code}\t{set.Cards.Count}\t{set.Name}");

				var entry = imagesBySet.TryGet(set.Code);

				if (entry == null)
					Console.WriteLine($"\t{0}\t");
				else
				{
					foreach (var dir in entry
						.GroupBy(_ => Path.GetDirectoryName(_.FullPath))
						.ToDictionary(gr => gr.Key, gr => gr.Count())
						.OrderBy(_ => _.Key))
					{
						Console.WriteLine($"\t{dir.Value}\t{dir.Key}");
					}
				}

				Console.WriteLine();
			}
		}

		[Test]
		public void Zoom_images_match_small_ones()
		{
			TestLoader.LoadModules();

			var cardRepo = TestLoader.CardRepository;
			var imgRepo = TestLoader.ImageRepository;

			cardRepo.LoadFile();
			cardRepo.Load();

			imgRepo.LoadFiles();

			imgRepo.LoadSmall();
			imgRepo.LoadZoom();

			foreach (var set in cardRepo.SetsByCode.OrderBy(_ => _.Value.ReleaseDate))
				foreach (var card in set.Value.Cards)
				{
					var small = cardRepo.GetSmallImage(card, imgRepo);
					var zooms = cardRepo.GetZoomImages(card, imgRepo);

					Assert.That(small, Is.Not.Null);
					Assert.That(zooms, Is.Not.Null);
					Assert.That(zooms, Is.Not.Empty);

					var smallPath = small.FullPath;
					var zoomPath = zooms[0].FullPath;

					smallPath = smallPath.ToLowerInvariant()
						.Replace("gatherer.original", "gatherer")
						.Replace("\\lq\\", string.Empty);

					zoomPath = zoomPath.ToLowerInvariant()
						.Replace("gatherer.preprocessed", "gatherer")
						.Replace("\\mq\\", string.Empty);

					if (!Str.Equals(smallPath, zoomPath))
						Assert.Fail();
				}
		}

		[Test]
		public void Find_non_used_transparency_key()
		{
			TestLoader.LoadModules();
			TestLoader.ImageRepository.LoadFiles();
			TestLoader.ImageRepository.LoadZoom();

			var zoomImages = TestLoader.ImageRepository.GetAllZooms().ToList();

			var detectedColors = new bool[0x1000000];

			for (int i = 0; i < zoomImages.Count; i++)
			{
				var img = new Bitmap(zoomImages[i].FullPath);
				new ColorDetector(img, detectedColors).Execute();
			}

			int color = Enumerable.Range(0, detectedColors.Length)
				.Where(i => !detectedColors[i])
				.AtMax(getBrightness)
				.Find();

			Console.WriteLine($"R: {color / 0x10000} G: {color % 0x10000 / 0x100} B: {color % 0x100}");
		}

		private static int getBrightness(int color)
		{
			return color / 0x10000 + color % 0x10000 / 0x100 + color % 0x100;
		}
	}
}
