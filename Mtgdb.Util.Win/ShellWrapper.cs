using Mtgdb.Data;
using Shell32;

namespace Mtgdb.Util
{
	internal class ShellWrapper: IShell
	{
		public IShellFolder GetFolder(FsPath path)
		{
			return new ShellFolder(Shell.NameSpace(path));
		}

		private Shell Shell => _shell ??= new Shell();
		private Shell _shell;
	}

	public class ShellFolder: IShellFolder
	{
		public ShellFolder(Folder folder)
		{
			Folder = folder;
		}

		public IShellFile GetFile(string name) =>
			new ShellFile(Folder, Folder.ParseName(name));

		private Folder Folder { get; }
	}

	public class ShellFile : IShellFile
	{
		public ShellFile(Folder folder, FolderItem file)
		{
			Folder = folder;
			File = file;
		}

		public string GetAuthors() =>
			Folder.GetDetailsOf(File, 20);

		public string GetKeywords() =>
			Folder.GetDetailsOf(File, 18);

		private readonly Folder Folder;
		private readonly FolderItem File;
	}
}
