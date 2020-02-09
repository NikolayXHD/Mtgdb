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

		public FileSignature AsRelativeTo(string dir, bool internPath)
		{
			string path = dir != null
				? Path.Substring(dir.Length + 1)
				: Path;
			return new FileSignature
			{
				Path = internPath ? string.Intern(path) : path,
				Md5Hash = Md5Hash
			};
		}
	}
}
