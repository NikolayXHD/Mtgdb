using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mtgdb.Util
{
	public class ImageDirectorySigner
	{
		public void SignFiles(string packagePath, string output, string setCodes)
		{
			string parentDir = output.Parent();
			Directory.CreateDirectory(parentDir);

			if (Directory.Exists(packagePath))
			{
				var sets = setCodes?.Split(';', ',', '|').ToHashSet(Str.Comparer);

				var prevSignatureByPath = sets != null && File.Exists(output)
					? Signer.ReadFromFile(output, internPath: false)
						.Where(_ => !sets.Contains(Path.GetDirectoryName(_.Path)))
						.ToDictionary(_ => _.Path)
					: new Dictionary<string, FileSignature>();

				var signatures = Signer.CreateSignatures(packagePath, precalculated: prevSignatureByPath);
				Signer.WriteToFile(output, signatures);
			}
			else if (File.Exists(packagePath))
			{
				var metadata = Signer.CreateSignature(packagePath);
				Signer.WriteToFile(output, Array.From(metadata));
			}
			else
			{
				Console.WriteLine("Specified path {0} does not exist", packagePath);
			}
		}
	}
}
