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
			_tempDirectoryPath = new FsPath(Path.GetTempPath()).Join(Path.GetRandomFileName());
			_tempDirectoryPath.CreateDirectory();
		}

		[TearDown]
		public void Teardown()
		{
			if (_tempDirectoryPath.IsDirectory())
				_tempDirectoryPath.DeleteDirectory(recursive: true);
		}

		[Test]
		public void Test_compress_to_extract_roundtrip()
		{
			const string originalContent = "Hello world!";
			var originalPath = _tempDirectoryPath.Join("original.txt");
			originalPath.WriteAllText(originalContent);

			var sevenZip = new SevenZip(silent: false);
			var zipPath = _tempDirectoryPath.Join("compressed.7z");
			sevenZip.Compress(originalPath, zipPath);
			zipPath.IsFile().Should().BeTrue();

			var extractedDirectoryPath = _tempDirectoryPath.Join("extracted");
			extractedDirectoryPath.CreateDirectory();
			sevenZip.Extract(zipPath, extractedDirectoryPath);

			var extractedPath = extractedDirectoryPath.Join("original.txt");
			extractedDirectoryPath.IsDirectory().Should().BeTrue();
			extractedPath.IsFile().Should().BeTrue();

			var extractedContent = extractedPath.ReadAllText();
			extractedContent.Should().Be(originalContent);
		}

		[Test]
		public void Test_extract_excluding_path()
		{
			const string originalContent = "Hello world!";
			var originalDirPath = _tempDirectoryPath.Join("original");
			originalDirPath.CreateDirectory();
			var originalPath1 = originalDirPath.Join("to_be_extracted.txt");
			var originalPath2 = originalDirPath.Join("to_be_excluded.txt");
			originalPath1.WriteAllText(originalContent);
			originalPath2.WriteAllText(originalContent);

			var sevenZip = new SevenZip(silent: false);
			var zipPath = _tempDirectoryPath.Join("compressed.7z");
			sevenZip.Compress(originalDirPath, zipPath);
			zipPath.IsFile().Should().BeTrue();

			var extractedDirectoryPath = _tempDirectoryPath.Join("extracted");
			var extractedDirPath = extractedDirectoryPath.Join("original");
			extractedDirectoryPath.CreateDirectory();
			sevenZip.Extract(zipPath, extractedDirectoryPath,
				excludedFiles: new[] { extractedDirPath.Join("to_be_excluded.txt") });

			extractedDirectoryPath.IsDirectory().Should().BeTrue();
			extractedDirPath.IsDirectory().Should().BeTrue();
			extractedDirPath.Join("to_be_extracted.txt").IsFile().Should().BeTrue();
			extractedDirPath.Join("to_be_excluded.txt").IsFile().Should().BeFalse();
		}

		private FsPath _tempDirectoryPath;
	}
}
