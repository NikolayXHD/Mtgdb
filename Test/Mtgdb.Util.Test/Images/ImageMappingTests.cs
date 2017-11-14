using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Mtgdb.Dal;
using Mtgdb.Test;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class ImageMappingTests : TestsBase
	{
		[SetUp]
		public void Setup()
		{
			LoadModules();
			LoadCards();

			ImgRepo.LoadFiles();
			ImgRepo.LoadZoom();
		}

		[Test]
		public void Map_xlhq_sets()
		{
			var zoomImages = ImgRepo.GetAllZooms();

			var imagesBySet = zoomImages.GroupBy(_ => _.SetCode ?? string.Empty, Str.Comparer)
				.ToDictionary(
					gr => gr.Key,
					gr => gr.OrderBy(_ => Path.GetFileNameWithoutExtension(_.FullPath)).ToList(),
					Str.Comparer
				);

			var sets = Repo.SetsByCode.Values.OrderBy(_ => _.ReleaseDate);
			foreach (var set in sets)
			{
				Log.Debug($"{set.Code}\t{set.Cards.Count}\t{set.Name}");

				var entry = imagesBySet.TryGet(set.Code);

				if (entry == null)
					Log.Debug($"\t{0}\t");
				else
				{
					foreach (var dir in entry
						.GroupBy(_ => Path.GetDirectoryName(_.FullPath))
						.ToDictionary(gr => gr.Key, gr => gr.Count())
						.OrderBy(_ => _.Key))
					{
						Log.Debug($"\t{dir.Value}\t{dir.Key}");
					}
				}
			}
		}

		[Test]
		public void Find_non_used_transparency_key()
		{
			var zoomImages = ImgRepo.GetAllZooms().ToList();

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

			Log.Debug($"R: {color / 0x10000} G: {color % 0x10000 / 0x100} B: {color % 0x100}");
		}

		private static int getBrightness(int color)
		{
			return color / 0x10000 + color % 0x10000 / 0x100 + color % 0x100;
		}
	}
}
