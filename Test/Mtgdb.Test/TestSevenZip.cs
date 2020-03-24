using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class TestSevenZip
	{
		[SetUp]
		public void Setup()
		{
			_tempDirectoryPath = Path.GetTempPath().AddPath(Path.GetRandomFileName());
			Directory.CreateDirectory(_tempDirectoryPath);
		}

		[TearDown]
		public void Teardown()
		{
			if (Directory.Exists(_tempDirectoryPath))
				Directory.Delete(_tempDirectoryPath, recursive: true);
		}

		[Test]
		public void Test_compress_to_extract_roundtrip()
		{
			const string originalContent = "Hello world!";
			var originalPath = _tempDirectoryPath.AddPath("original.txt");
			File.WriteAllText(originalPath, originalContent);

			var sevenZip = new SevenZip(silent: false);
			var zipPath = _tempDirectoryPath.AddPath("compressed.7z");
			sevenZip.Compress(originalPath, zipPath);
			File.Exists(zipPath).Should().BeTrue();

			var extractedDirectoryPath = _tempDirectoryPath.AddPath("extracted");
			Directory.CreateDirectory(extractedDirectoryPath);
			sevenZip.Extract(zipPath, extractedDirectoryPath);

			var extractedPath = extractedDirectoryPath.AddPath("original.txt");
			Directory.Exists(extractedDirectoryPath).Should().BeTrue();
			File.Exists(extractedPath).Should().BeTrue();

			var extractedContent = File.ReadAllText(extractedPath);
			extractedContent.Should().Be(originalContent);
		}

		[Test]
		public void Test_extract_excluding_path()
		{
			const string originalContent = "Hello world!";
			var originalDirPath = _tempDirectoryPath.AddPath("original");
			Directory.CreateDirectory(originalDirPath);
			var originalPath1 = originalDirPath.AddPath("to_be_extracted.txt");
			var originalPath2 = originalDirPath.AddPath("to_be_excluded.txt");
			File.WriteAllText(originalPath1, originalContent);
			File.WriteAllText(originalPath2, originalContent);

			var sevenZip = new SevenZip(silent: false);
			var zipPath = _tempDirectoryPath.AddPath("compressed.7z");
			sevenZip.Compress(originalDirPath, zipPath);
			File.Exists(zipPath).Should().BeTrue();

			var extractedDirectoryPath = _tempDirectoryPath.AddPath("extracted");
			var extractedDirPath = extractedDirectoryPath.AddPath("original");
			Directory.CreateDirectory(extractedDirectoryPath);
			sevenZip.Extract(zipPath, extractedDirectoryPath,
				excludedFiles: new[] { extractedDirPath.AddPath("to_be_excluded.txt") });

			Directory.Exists(extractedDirectoryPath).Should().BeTrue();
			Directory.Exists(extractedDirPath).Should().BeTrue();
			File.Exists(extractedDirPath.AddPath("to_be_extracted.txt")).Should().BeTrue();
			File.Exists(extractedDirPath.AddPath("to_be_excluded.txt")).Should().BeFalse();
		}

		private string _tempDirectoryPath;
	}
}
