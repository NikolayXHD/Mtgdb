using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Mtgdb.Downloader;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	[Parallelizable(ParallelScope.Self)]
	public class YandexDiskImageTests : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadModules();
			_progressReader = Kernel.Get<ImageDownloadProgressReader>();
		}

		[Test]
		public void Yandex_image_lists_have_no_duplicate_directories()
		{
			foreach (var quality in _progressReader.Config.QualityGroups)
			{
				if (quality.YandexName == null)
					continue;

				var imageList = _progressReader.GetOnlineImages(quality);
				var duplicateDirectories = imageList
					.Select(_ => _.Path.Parent().Value)
					.Distinct(StringComparer.Ordinal)
					.GroupBy(_ => _, StringComparer.OrdinalIgnoreCase)
					.Where(gr => gr.Skip(1).Any())
					.Select(gr => gr.Key)
					.ToArray();

				Assert.That(duplicateDirectories, Is.Empty,
					"Duplicate directories in quality {0}", quality.YandexName);
			}
		}

		[Test]
		public async Task Published_yandex_images_have_no_missing_or_duplicate_sets()
		{
			var yandexDiskClient = new YandexDiskClient();

			foreach (var quality in _progressReader.Config.QualityGroups)
			{
				if (quality.YandexName == null)
					continue;

				var imageList = _progressReader.GetOnlineImages(quality);
				var publishedSets = imageList
					.Select(_ => _.Path.Parent().Value)
					.ToHashSet(StringComparer.Ordinal);

				var emptyFileName = _progressReader.Config.GetYandexDiskPath(quality, FsPath.Empty);
				var remoteDirectoryPath = emptyFileName.Substring(0, emptyFileName.Length - "/.7z".Length);
				var metadata = await yandexDiskClient.GetPathMetadata(remoteDirectoryPath);

				var fileNames = metadata.Directory.Items
					.Select(_ => _.Name)
					.ToArray();

				var setNames = fileNames
					.Select(_ => _.Substring(0, _.Length - ".7z".Length))
					.ToArray();

				var redundantSets = setNames
					.Where(set => !publishedSets.Contains(set))
					.OrderBy(_ => _)
					.ToArray();

				redundantSets.Should().BeEmpty();

				var missingSets = publishedSets.Except(setNames)
					.ToArray();

				missingSets.Should().BeEmpty();
			}
		}

		private ImageDownloadProgressReader _progressReader;
	}
}
