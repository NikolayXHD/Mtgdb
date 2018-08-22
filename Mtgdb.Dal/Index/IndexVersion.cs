using System.IO;

namespace Mtgdb.Dal.Index
{
	public class IndexVersion
	{
		public string Directory { get; }

		private readonly string _completionLabelFile;
		private readonly string _indexVersion;

		public bool IsUpToDate => File.Exists(_completionLabelFile) && File.ReadAllText(_completionLabelFile) == _indexVersion;

		public IndexVersion(string directory, string indexVersion)
		{
			Directory = Path.Combine(directory, indexVersion);
			_indexVersion = indexVersion;
			_completionLabelFile = Directory.AddPath("indexing.done");
		}

		public void CreateDirectory()
		{
			if (System.IO.Directory.Exists(Directory))
				System.IO.Directory.Delete(Directory, recursive: true);

			System.IO.Directory.CreateDirectory(Directory);
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