using System.IO;

namespace Mtgdb.Dal.Index
{
	internal class IndexVersion
	{
		public string Directory { get; }

		private readonly string _completionLabelFile;
		private readonly string _indexVerision;

		public bool IsUpToDate => File.Exists(_completionLabelFile) && File.ReadAllText(_completionLabelFile) == _indexVerision;

		public IndexVersion(string directory, string indexVerision)
		{
			Directory = Path.Combine(directory, indexVerision);
			_indexVerision = indexVerision;
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
			File.WriteAllText(_completionLabelFile, _indexVerision);
		}

		public void Invalidate()
		{
			var fileInfo = new FileInfo(_completionLabelFile);
			
			if (fileInfo.Exists)
				fileInfo.Delete();
		}
	}
}