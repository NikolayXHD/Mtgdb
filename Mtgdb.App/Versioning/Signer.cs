using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Mtgdb
{
	public static class Signer
	{
		public static readonly FsPath SignaturesFile = new FsPath("filelist.txt");

		public static IList<FileSignature> CreateSignatures(
			FsPath path,
			string pattern = "*",
			Dictionary<FsPath, FileSignature> precalculated = null)
		{
			var files = path.EnumerateFiles(pattern, SearchOption.AllDirectories);
			var result = new List<FileSignature>();

			using var md5 = MD5.Create();
			foreach (FsPath file in files)
			{
				if (Str.Equals(file.Basename(), SignaturesFile.Value))
					continue;

				var relativePath = file.RelativeTo(path);

				if (precalculated == null || !precalculated.TryGetValue(relativePath, out var signature))
				{
					signature = new FileSignature
					{
						Path = relativePath,
						Md5Hash = getMd5Hash(md5, file.ReadAllBytes())
					};
				}

				result.Add(signature);
			}

			return result;
		}

		public static FileSignature CreateSignature(FsPath path, bool useAbsolutePath = false)
		{
			using (var md5 = MD5.Create())
				return new FileSignature
				{
					Path = useAbsolutePath ? path : path.Base(),
					Md5Hash = getMd5Hash(md5, path.ReadAllBytes())
				};
		}

		public static void WriteToFile(FsPath targetFile, IEnumerable<FileSignature> signatures)
		{
			targetFile.WriteAllLines(signatures.Select(_ => _.Md5Hash + "\t" + _.Path.Value));
		}

		public static FileSignature[] ReadFromFile(FsPath targetFile, bool internPath)
		{
			if (!targetFile.IsFile())
				return null;

			var lines = targetFile.ReadAllLines()
				.Where(_ => !string.IsNullOrEmpty(_));

			if (Path.DirectorySeparatorChar != '\\')
				lines = lines.Select(_ => _.Replace('\\', Path.DirectorySeparatorChar));

			var result = lines
				.Select(_ => new FileSignature
				{
					Md5Hash = _.Substring(0, 32),
					Path = FsPathPersistence.Deserialize(_.Substring(33)).Intern(internPath)
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
