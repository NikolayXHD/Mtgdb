using System.IO;

namespace Mtgdb.Data
{
	public class IndexVersion
	{
		public string IndexDirectory { get; }

		private readonly string _completionLabelFile;
		private readonly string _root;
		private readonly string _indexVersion;

		public bool IsUpToDate => File.Exists(_completionLabelFile) && File.ReadAllText(_completionLabelFile) == _indexVersion;

		public IndexVersion(string root, string indexVersion)
		{
			IndexDirectory = root.AddPath(indexVersion);
			_root = root;
			_indexVersion = indexVersion;
			_completionLabelFile = IndexDirectory.AddPath("indexing.done");
		}

		public void RemoveObsoleteIndexes()
		{
			var rootDir = new DirectoryInfo(_root);
			if (rootDir.Exists)
				foreach (var subdir in rootDir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
					if (!Str.Equals(subdir.FullName, IndexDirectory))
						subdir.Delete(recursive: true);
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
