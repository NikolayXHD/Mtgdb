using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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

			var zoomImages = TestLoadingUtil.ImageRepository.GetAllImagesZoom();

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
	}
}
