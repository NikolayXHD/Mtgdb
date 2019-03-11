using System.IO;

namespace Mtgdb.Data
{
	public class IndexVersion
	{
		public string IndexDirectory { get; }

		private readonly string _completionLabelFile;
		private readonly string _indexVersion;

		public bool IsUpToDate => File.Exists(_completionLabelFile) && File.ReadAllText(_completionLabelFile) == _indexVersion;

		public IndexVersion(string directory, string indexVersion)
		{
			IndexDirectory = Path.Combine(directory, indexVersion);
			_indexVersion = indexVersion;
			_completionLabelFile = IndexDirectory.AddPath("indexing.done");
		}

		public void CreateDirectory()
		{
			if (Directory.Exists(IndexDirectory))
				Directory.Delete(IndexDirectory, recursive: true);

			Directory.CreateDirectory(IndexDirectory);
		}

		public void SetIsUpToDate()
		{
			File.WriteAllText(_completionLabelFile, _indexVersion);
		}

		public void Invalidate()
		{
			var fileInfo = new FileInfo(_completionLabelFile);

			if (fileInfo.Exists)
				fileInfo.Delete();
		}
	}
}