using System.IO;
using System.ServiceModel;
using System.Web.ModelBinding;
using FluentAssertions;
using Mtgdb.Controls;
using Mtgdb.Data;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class DeploymentUtils
	{
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

			imgRepo.LoadFiles(Sequence.From("dev", "xlhq"));
			imgRepo.LoadSmall();
			imgRepo.LoadZoom();

			_export = kernel.Get<ImageExport>();
		}

		[TestCase(null, PicturesDir, LqTokenDir, MqTokenDir, true, true)]
		public void SelectImages(string setcode, string targetDir, string smallSubdir, string zoomSubdir, bool forceRemoveCorner, bool token) =>
			_export.ExportCardImages(targetDir, small: true, zoomed: true, setcode, smallSubdir, zoomSubdir, forceRemoveCorner, token);

		[TestCase(LqTokenDir, LqTokenDir + ListDirSuffix + "\\" + Signer.SignaturesFile, null)]
		[TestCase(MqTokenDir, MqTokenDir + ListDirSuffix + "\\" + Signer.SignaturesFile, null)]
		// [TestCase("lq      ", @"lq-list\filelist.txt      ", SetCodes)]
		// [TestCase("mq      ", @"mq-list\filelist.txt      ", SetCodes)]
		public void SignImages(string packagePath, string outputFile, string setCodes) =>
			_export.SignFiles(
				PicturesDir.AddPath(packagePath.TrimEnd()),
				PicturesDir.AddPath(outputFile.TrimEnd()),
				setCodes);

		[TestCase(PicturesDir + "\\" + LqTokenDir, PicturesDir + "\\" + LqTokenDir + ZipDirSuffix, null)]
		[TestCase(PicturesDir + "\\" + MqTokenDir, PicturesDir + "\\" + MqTokenDir + ZipDirSuffix, null)]
		public void ZipImages(string sourceRoot, string compressedRoot, string setcodeList)
		{
			var setCodes = setcodeList?.Split(',').ToHashSet(Str.Comparer);
			Directory.CreateDirectory(compressedRoot);

			foreach (var subdir in new DirectoryInfo(sourceRoot).EnumerateDirectories())
			{
				if (setCodes?.Contains(subdir.Name) == false)
					continue;

				var targetFile = new FileInfo(compressedRoot.AddPath(subdir.Name + ".7z"));
				if (targetFile.Exists)
					targetFile.Delete();

				new SevenZip(false).Compress(subdir.FullName, targetFile.FullName)
					.Should().BeTrue();
			}
		}

		private ImageExport _export;
		private const string PicturesDir = @"D:\distrib\games\mtg\Mtgdb.Pictures";
		private const string SetCodes = "c19,celd,cmb1,eld,gn2,ha1,hho,peld,ptg,puma";
		private const string LqTokenDir = "lq-token";
		private const string MqTokenDir = "mq-token";
		private const string ListDirSuffix = "-list";
		private const string ZipDirSuffix = "-7z";
	}
}
