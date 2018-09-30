using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using Mtgdb.Test;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class ImageMappingUtils : TestsBase
	{
		[SetUp]
		public void Setup()
		{
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

			var detectedColors = new BitArray(1 << 24);
			for (int i = 0; i < zoomImages.Count; i++)
			{
				var img = new Bitmap(zoomImages[i].FullPath);
				new ColorDetector(img, detectedColors).Execute();
			}

			int color = Enumerable.Range(0, detectedColors.Length)
				.Where(i => !detectedColors.Get(i))
				.AtMax(getBrightness)
				.Find();

			Log.Debug($"R: {color << 16 >> 16} G: {color << 8 >> 16} B: {color >> 16}");
		}

		private static int getBrightness(int color) =>
			color << 16 >> 16 +
			color << 8 >> 16 +
			color >> 16;
	}
}
