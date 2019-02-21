using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Mtgdb
{
	[Localizable(false)]
	public static class ShadowCopy
	{
		private static readonly Regex _invalidChar = new Regex("[\x00\x0a\x0d]");
		private static readonly Regex _needsQuotes = new Regex(@"\s|""");
		private static readonly Regex _escapeQuote = new Regex(@"(\\*)(""|$)");

		private static void startShadowCopy(string[] args)
		{
			string binSignaturesFile = AppDir.BinVersion.AddPath(Signer.SignaturesFile);
			string binShadowCopySignaturesFile = AppDir.BinShadowCopy.AddPath(Signer.SignaturesFile);

			IList<FileSignature> signatures = Signer.ReadFromFile(binSignaturesFile);
			var copiedSignatures = Signer.ReadFromFile(binShadowCopySignaturesFile);

			if (signatures == null || copiedSignatures == null || !equal(signatures, copiedSignatures))
			{
				if (Directory.Exists(AppDir.BinShadowCopy))
					Directory.Delete(AppDir.BinShadowCopy, true);

				Directory.CreateDirectory(AppDir.BinShadowCopy);

				if (signatures == null)
				{
					signatures = Signer.CreateSignatures(AppDir.BinVersion);
					Signer.WriteToFile(binSignaturesFile, signatures);
				}
				
				copyRecursively(AppDir.BinVersion, AppDir.BinShadowCopy);
			}

			string arguments = escape(args);
			string exe = AppDir.BinShadowCopy.AddPath(AppDir.Executable.LastPathSegment());
			Process.Start(exe, arguments);
		}

		private static bool equal(IList<FileSignature> val1, IList<FileSignature> val2)
		{
			if (val1.Count != val2.Count)
				return false;

			for (int i = 0; i < val1.Count; i++)
			{
				if (val1[i].Path != val2[i].Path)
					return false;

				if (val1[i].Md5Hash != val2[i].Md5Hash)
					return false;
			}

			return true;
		}

		private static string escape(string[] args)
		{
			if (args == null)
				return null;

			StringBuilder arguments = new StringBuilder();
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == null)
					throw new ArgumentNullException("args[" + i + "]");

				if (_invalidChar.IsMatch(args[i]))
					throw new ArgumentOutOfRangeException("args[" + i + "]");

				if (args[i] == string.Empty)
					arguments.Append("\"\"");
				else if (!_needsQuotes.IsMatch(args[i]))
					arguments.Append(args[i]);
				else
				{
					arguments.Append('"');

					arguments.Append(_escapeQuote.Replace(args[i],
						m =>
							m.Groups[1].Value + m.Groups[1].Value +
							(m.Groups[2].Value == "\"" ? "\\\"" : "")
						));

					arguments.Append('"');
				}

				if (i + 1 < args.Length)
					arguments.Append(' ');
			}

			return arguments.ToString();
		}

		private static bool isShadowCopied()
		{
			string dirName = AppDir.BinVersion.LastPathSegment();

			return dirName.IndexOf("debug", Str.Comparison) >= 0 ||
			       dirName.IndexOf("release", Str.Comparison) >= 0 ||
			       AppDir.BinVersion == AppDir.BinShadowCopy;
		}

		private static void copyRecursively(string sourcePath, string targetPath)
		{
			foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
				Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

			foreach (string newPath in Directory.GetFiles(sourcePath,
				"*.*",
				SearchOption.AllDirectories))
				File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
		}

		public static void RunMain(Action<string[]> actualMain, string[] args)
		{
			if (isShadowCopied())
			{
				actualMain(args);
				return;
			}

			startShadowCopy(args);
		}
	}
}