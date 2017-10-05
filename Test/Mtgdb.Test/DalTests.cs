﻿using System;
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
			TestLoadingUtil.LoadModules();
			var repo = TestLoadingUtil.CardRepository;

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
			TestLoadingUtil.LoadModules();
			
			TestLoadingUtil.CardRepository.LoadFile();
			TestLoadingUtil.CardRepository.Load();

			TestLoadingUtil.ImageRepository.LoadFiles();
			TestLoadingUtil.ImageRepository.LoadZoom();

			var zoomImages = TestLoadingUtil.ImageRepository.GetAllZooms();

			var imagesBySet = zoomImages.GroupBy(_ => _.SetCode ?? string.Empty, Str.Comparer)
				.ToDictionary(
					gr => gr.Key,
					gr => gr.OrderBy(_ => Path.GetFileNameWithoutExtension(_.FullPath)).ToList(),
					Str.Comparer
				);

			var sets = TestLoadingUtil.CardRepository.SetsByCode.Values.OrderBy(_=>_.ReleaseDate);
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
			TestLoadingUtil.LoadModules();

			var cardRepo = TestLoadingUtil.CardRepository;
			var imgRepo = TestLoadingUtil.ImageRepository;

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
			TestLoadingUtil.LoadModules();
			TestLoadingUtil.ImageRepository.LoadFiles();
			TestLoadingUtil.ImageRepository.LoadZoom();

			var zoomImages = TestLoadingUtil.ImageRepository.GetAllZooms().ToList();

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

		public class ColorDetector : BmpProcessor
		{
			public ColorDetector(Bitmap bmp, bool[] detectedColors) : base(bmp)
			{
				_detectedColors = detectedColors;
			}

			protected override void ExecuteRaw()
			{
				for (int x = 0; x < Rect.Width; x++)
					for (int y = 0; y < Rect.Height; y++)
					{
						int l = GetLocation(x, y);
						var r = RgbValues[l];
						var g = RgbValues[l + 1];
						var b = RgbValues[l + 2];

						_detectedColors[r * 0x10000 + g * 0x100 + b] = true;
					}
			}

			private readonly bool[] _detectedColors;
		}
	}
}
