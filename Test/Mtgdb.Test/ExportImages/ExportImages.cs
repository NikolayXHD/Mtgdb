using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Util;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class ExportImages
	{
		private ForgeIntegration _integration;

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

			repo.SelectCardImages(imgRepo);

			_integration = kernel.Get<ForgeIntegration>();
		}

		//[TestCase("XLN", @"D:\Distrib\games\mtg\Mtgdb.Pictures", "lq", "mq")]
		[TestCase("C17", @"D:\Distrib\games\mtg\Mtgdb.Pictures", "lq", "mq")]
		public void Export(string set, string targetDir, string smallSubdir, string zoomSubdir)
		{
			_integration.ExportCardImages(
				targetDir,
				small: true,
				zoomed: true,
				code: set,
				smallSubdir: smallSubdir,
				zoomedSubdir: zoomSubdir);
		}
	}
}
