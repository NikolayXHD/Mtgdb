using Mtgdb.Controls;
using Mtgdb.Data;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class ExportImages
	{
		private ImageExport _export;

		[OneTimeSetUp]
		public void Setup()
		{
			BitmapExtensions.CustomScaleStrategy = ImageMagickScaler.Scale;

			var kernel = new StandardKernel();
			kernel.Load<CoreModule>();
			kernel.Load<DalModule>();
			kernel.Load<UtilModule>();

			var repo = kernel.Get<CardRepository>();
			repo.LoadFile();
			repo.Load();

			var imgRepo = kernel.Get<ImageRepository>();

			imgRepo.LoadFiles();
			imgRepo.LoadZoom();

			_export = kernel.Get<ImageExport>();
		}

		//[TestCase("XLN", @"D:\Distrib\games\mtg\Mtgdb.Pictures", "lq", "mq")]
		[TestCase("C17", @"D:\Distrib\games\mtg\Mtgdb.Pictures", "lq", "mq", /* forceRemoveCorner */false)]
		public void Export(string set, string targetDir, string smallSubdir, string zoomSubdir, bool forceRemoveCorner)
		{
			_export.ExportCardImages(
				targetDir,
				small: true,
				zoomed: true,
				code: set,
				smallSubdir,
				zoomSubdir,
				forceRemoveCorner);
		}
	}
}
