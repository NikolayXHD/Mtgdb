using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mtgdb
{
	public static class FsPathExt
	{
		public static FileInfo File(this FsPath path) =>
			new FileInfo(path.Value);

		public static DirectoryInfo Directory(this FsPath path) =>
			new DirectoryInfo(path.Value);

		public static bool IsFile(this FsPath path) =>
			System.IO.File.Exists(path.Value);

		public static bool IsDirectory(this FsPath path) =>
			System.IO.Directory.Exists(path.Value);

		public static FsPath ExpandEnvironmentVariables(this FsPath path) =>
			new FsPath(Environment.ExpandEnvironmentVariables(path.Value));

		public static FsPath ChangeDirectory(this FsPath path, FsPath directory) =>
			directory.Join(path.Basename());

		public static FsPath ChangeDirectory(this FsPath path, FsPath original, FsPath replacement)
		{
			FsPath relative = path.RelativeTo(original);
			if (relative == FsPath.None)
				return original;
			return replacement.Join(relative);
		}

		public static FsPath WithName(this FsPath path, Func<string, string> transformation)
		{
			var tail = path.Basename();
			string transformed = transformation(tail);
			if (FsPath.Comparer.Equals(tail, transformed))
				return path;

			return path.Parent().Join(transformed);
		}

		public static string Extension(this FsPath path) =>
			Path.GetExtension(path.Value);

		public static bool IsParentOf(this FsPath path, FsPath other)
		{
			return
				other.Value != null && other.Value.Length > path.Value.Length + 1
				&& other.Value[path.Value.Length] == FsPath.Separator
				&& other.Value.StartsWith(path.Value, FsPath.Comparison);
		}

		public static bool IsParentOrEqualOf(this FsPath path, FsPath other)
		{
			return
				other.Value != null && other.Value.Length >= path.Value.Length
				&& (other.Value.Length == path.Value.Length || other.Value[path.Value.Length] == FsPath.Separator)
				&& other.Value.StartsWith(path.Value, FsPath.Comparison);
		}

		public static DirectoryInfo CreateDirectory(this FsPath path) =>
			System.IO.Directory.CreateDirectory(path.Value);

		public static FileStream OpenFile(this FsPath path, FileMode mode) =>
			System.IO.File.Open(path.Value, mode);

		public static FileStream OpenFile(this FsPath path, FileMode mode, FileAccess access) =>
			System.IO.File.Open(path.Value, mode, access);

		public static FileStream OpenFile(this FsPath path, FileMode mode, FileAccess access, FileShare share) =>
			System.IO.File.Open(path.Value, mode, access, share);

		public static StreamReader OpenText(this FsPath path) =>
			System.IO.File.OpenText(path.Value);

		public static StreamWriter CreateText(this FsPath path) =>
			System.IO.File.CreateText(path.Value);


		public static IEnumerable<FsPath> EnumerateFiles(
			this FsPath path, string pattern = "*", SearchOption option = SearchOption.TopDirectoryOnly) =>
			System.IO.Directory.EnumerateFiles(path.Value, pattern, option).Select(_ => new FsPath(_));

		public static IEnumerable<FsPath> EnumerateDirectories(
			this FsPath path, string pattern = "*", SearchOption option = SearchOption.TopDirectoryOnly) =>
			System.IO.Directory.EnumerateDirectories(path.Value, pattern, option).Select(_ => new FsPath(_));

		public static IEnumerable<FsPath> EnumerateChildren(
			this FsPath path, string pattern = "*", SearchOption option = SearchOption.TopDirectoryOnly) =>
			System.IO.Directory.EnumerateFileSystemEntries(path.Value, pattern, option).Select(_ => new FsPath(_));


		public static void DeleteDirectory(this FsPath path, bool recursive = false) =>
			System.IO.Directory.Delete(path.Value, recursive);

		public static void DeleteEmptyDirectory(this FsPath path)
		{
			if (path.EnumerateChildren().Any())
				return;

			path.DeleteDirectory();
		}

		public static void EnsureEmptyDirectory(this FsPath path)
		{
			if (!path.IsDirectory())
			{
				path.CreateDirectory();
				return;
			}

			foreach (var subdirInfo in path.EnumerateDirectories().ToArray())
				subdirInfo.DeleteDirectory(recursive: true);

			foreach (var fileInfo in path.EnumerateFiles().ToArray())
				fileInfo.DeleteFile();
		}

		public static void DeleteFile(this FsPath path) =>
			System.IO.File.Delete(path.Value);

		public static void MoveDirectoryTo(this FsPath path, FsPath other) =>
			System.IO.Directory.Move(path.Value, other.Value);

		public static void MoveFileTo(this FsPath path, FsPath other) =>
			System.IO.File.Move(path.Value, other.Value);

		public static void CopyFileTo(this FsPath path, FsPath other, bool overwrite = false) =>
			System.IO.File.Copy(path.Value, other.Value, overwrite);

		public static void CopyDirectoryTo(this FsPath path, FsPath other, bool overwrite)
		{
			foreach (FsPath dirPath in path.EnumerateDirectories("*", SearchOption.AllDirectories).ToArray())
				dirPath.ChangeDirectory(path, other).CreateDirectory();

			foreach (FsPath newPath in path.EnumerateFiles("*", SearchOption.AllDirectories))
				newPath.CopyFileTo(newPath.ChangeDirectory(path, other), overwrite);
		}


		public static byte[] ReadAllBytes(this FsPath path) =>
			System.IO.File.ReadAllBytes(path.Value);

		public static string[] ReadAllLines(this FsPath path) =>
			System.IO.File.ReadAllLines(path.Value);

		public static string ReadAllText(this FsPath path, Encoding encoding = null) =>
			System.IO.File.ReadAllText(path.Value, encoding ?? Encoding.UTF8);

		public static void WriteAllText(this FsPath path, string text) =>
			System.IO.File.WriteAllText(path.Value, text);

		public static void WriteAllBytes(this FsPath path, byte[] bytes) =>
			System.IO.File.WriteAllBytes(path.Value, bytes);

		public static void WriteAllLines(this FsPath path, IEnumerable<string> lines) =>
			System.IO.File.WriteAllLines(path.Value, lines);


		public static bool HasValue(this FsPath path) =>
			!string.IsNullOrEmpty(path.Value);

		public static bool HasValue(this FsPath? path) =>
			path.HasValue && !string.IsNullOrEmpty(path.Value.Value);

		public static FsPath Or(this FsPath path, FsPath other) =>
			string.IsNullOrEmpty(path.Value)
				? other
				: path;

		public static FsPath Or(this FsPath? path, FsPath other) =>
			!path.HasValue || string.IsNullOrEmpty(path.Value.Value)
				? other
				: path.Value;

		public static FsPath Or(this FsPath path, string other) =>
			string.IsNullOrEmpty(path.Value)
				? new FsPath(other)
				: path;

		public static FsPath Or(this FsPath? path, string other) =>
			!path.HasValue || string.IsNullOrEmpty(path.Value.Value)
				? new FsPath(other)
				: path.Value;

		public static FsPath OrEmpty(this FsPath? path) =>
			path?.Value == null
				? FsPath.Empty
				: path.Value;

		public static FsPath OrEmpty(this FsPath path) =>
			path.Value == null
				? FsPath.Empty
				: path;

		public static FsPath OrNone(this FsPath? path) =>
			path ?? FsPath.None;
	}
}
