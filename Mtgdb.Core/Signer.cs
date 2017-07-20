using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Mtgdb
{
	public static class Signer
	{
		public const string SignaturesFile = "filelist.txt";

		public static IList<FileSignature> CreateSignatures(string path, string pattern = "*.*")
		{
			var files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
			var result = new List<FileSignature>(files.Length);

			using (var md5 = MD5.Create())
				for (int i = 0; i < files.Length; i++)
				{
					if (Str.Equals(files[i].LastPathSegment(), SignaturesFile))
						continue;

					result.Add(new FileSignature
					{
						Path = files[i].Substring(path.Length + 1),
						Md5Hash = getMd5Hash(md5, File.ReadAllBytes(files[i]))
					});
				}

			return result;
		}

		public static FileSignature CreateSignature(string path, bool useAbsolutePath = false)
		{
			using (var md5 = MD5.Create())
			{
				return new FileSignature
				{
					Path = useAbsolutePath ? path : Path.GetFileName(path),
					Md5Hash = getMd5Hash(md5, File.ReadAllBytes(path))
				};
			}
		}

		public static PatchFileMetadata[] CreatePatchMetadata(FileSignature[] package, FileSignature[] existing)
		{
			var packageHashByPath = package.ToDictionary(_ => _.Path, _ => _.Md5Hash, Str.Comparer);
			var existingHashByPath = existing.ToDictionary(_ => _.Path, _ => _.Md5Hash, Str.Comparer);

			var paths = packageHashByPath.Keys.Union(existingHashByPath.Keys, Str.Comparer).ToArray();

			var result = new PatchFileMetadata[paths.Length];

			for (int i = 0; i < paths.Length; i++)
			{
				var path = paths[i];

				PatchAction action;
				if (packageHashByPath.ContainsKey(path))
				{
					if (existingHashByPath.ContainsKey(path))
					{
						if (packageHashByPath[path] == existingHashByPath[path])
							action = PatchAction.None;
						else
							action = PatchAction.Replace;
					}
					else
					{
						action = PatchAction.Add;
					}
				}
				else
				{
					action = PatchAction.Remove;
				}

				result[i] = new PatchFileMetadata
				{
					Path = path,
					RequiredAction = action
				};
			}

			return result;
		}

		public static void WriteToFile(string targetFile, IEnumerable<FileSignature> signatures)
		{
			File.WriteAllLines(targetFile, signatures.Select(_ => _.Md5Hash + "\t" + _.Path));
		}

		public static FileSignature[] ReadFromFile(string targetFile)
		{
			if (!File.Exists(targetFile))
				return null;

			var result = File.ReadAllLines(targetFile)
				.Where(_ => !string.IsNullOrEmpty(_))
				.Select(_ => new FileSignature
				{
					Md5Hash = _.Substring(0, 32),
					Path = _.Substring(33)
				}).ToArray();

			return result;
		}

		private static string getMd5Hash(MD5 md5Hash, byte[] inputBytes)
		{
			byte[] data = md5Hash.ComputeHash(inputBytes);
			var sBuilder = new StringBuilder(32);

			for (int i = 0; i < data.Length; i++)
				sBuilder.Append(data[i].ToString("x2"));

			return sBuilder.ToString();
		}
	}
}