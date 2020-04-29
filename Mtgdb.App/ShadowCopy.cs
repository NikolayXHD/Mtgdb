using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
			FsPath binSignaturesFile = AppDir.BinVersion.Join(Signer.SignaturesFile);
			FsPath binShadowCopySignaturesFile = AppDir.BinShadowCopy.Join(Signer.SignaturesFile);

			IList<FileSignature> signatures = Signer.ReadFromFile(binSignaturesFile, internPath: false);
			var copiedSignatures = Signer.ReadFromFile(binShadowCopySignaturesFile, internPath: false);

			if (signatures == null || copiedSignatures == null || !equal(signatures, copiedSignatures))
			{
				if (AppDir.BinShadowCopy.IsDirectory())
					AppDir.BinShadowCopy.DeleteDirectory(recursive: true);

				AppDir.BinShadowCopy.CreateDirectory();

				if (signatures == null)
				{
					signatures = Signer.CreateSignatures(AppDir.BinVersion);
					Signer.WriteToFile(binSignaturesFile, signatures);
				}

				AppDir.BinVersion.CopyDirectoryTo(AppDir.BinShadowCopy, overwrite: true);
			}

			string arguments = escape(args);
			FsPath exe = AppDir.BinShadowCopy.Join(AppDir.Executable.Basename());
			Process.Start(exe.Value, arguments);
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
			string dirName = AppDir.BinVersion.Basename();

			return dirName.IndexOf("debug", Str.Comparison) >= 0 ||
			       dirName.IndexOf("release", Str.Comparison) >= 0 ||
			       AppDir.BinVersion == AppDir.BinShadowCopy;
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
