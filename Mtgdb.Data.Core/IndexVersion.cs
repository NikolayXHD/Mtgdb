namespace Mtgdb.Data
{
	public class IndexVersion
	{
		public FsPath IndexDirectory { get; }

		private readonly FsPath _completionLabelFile;
		private readonly FsPath _root;
		private readonly string _indexVersion;

		public bool IsUpToDate => _completionLabelFile.IsFile() && _completionLabelFile.ReadAllText() == _indexVersion;

		public IndexVersion(FsPath root, string indexVersion)
		{
			IndexDirectory = root.Join(indexVersion);
			_root = root;
			_indexVersion = indexVersion;
			_completionLabelFile = IndexDirectory.Join("indexing.done");
		}

		public void RemoveObsoleteIndexes()
		{
			if (!_root.IsDirectory())
				return;

			foreach (var subdir in _root.EnumerateDirectories())
				if (subdir != IndexDirectory)
					subdir.DeleteDirectory(recursive: true);
		}

		public void CreateDirectory()
		{
			if (IndexDirectory.IsDirectory())
				IndexDirectory.DeleteDirectory(recursive: true);

			IndexDirectory.CreateDirectory();
		}

		public void SetIsUpToDate()
		{
			_completionLabelFile.WriteAllText(_indexVersion);
		}

		public void Invalidate()
		{
			if (_completionLabelFile.IsFile())
				_completionLabelFile.DeleteFile();
		}
	}
}
