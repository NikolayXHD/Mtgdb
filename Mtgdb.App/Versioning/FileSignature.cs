namespace Mtgdb
{
	public class FileSignature
	{
		public string Path { get; set; }
		public string Md5Hash { get; set; }

		public bool IsRelativeTo(string dir)
		{
			if (dir == null)
				return true;

			bool result = Path.StartsWith(dir + System.IO.Path.DirectorySeparatorChar, Str.Comparison);
			return result;
		}

		public FileSignature AsRelativeTo(string dir)
		{
			return new FileSignature
			{
				Path = dir != null
					? Path.Substring(dir.Length + 1)
					: Path,
				Md5Hash = Md5Hash
			};
		}
	}
}