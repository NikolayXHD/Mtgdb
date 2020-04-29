namespace Mtgdb
{
	public class FileSignature
	{
		public FsPath Path { get; set; }
		public string Md5Hash { get; set; }

		public FileSignature AsRelativeTo(FsPath dir, bool internPath) =>
			new FileSignature
			{
				Path = Path.RelativeTo(dir).Intern(internPath),
				Md5Hash = Md5Hash
			};
	}
}
