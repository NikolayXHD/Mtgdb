namespace Mtgdb.Data
{
	public interface IShell
	{
		IShellFolder GetFolder(FsPath path);
	}

	public interface IShellFolder
	{
		IShellFile GetFile(string name);
	}

	public interface IShellFile
	{
		string GetAuthors();
		string GetKeywords();
	}
}
