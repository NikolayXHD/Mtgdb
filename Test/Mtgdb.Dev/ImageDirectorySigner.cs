using System;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Dev
{
	public class ImageDirectorySigner
	{
		public void SignFiles(FsPath packagePath, FsPath output, string setCodes)
		{
			FsPath parentDir = output.Parent();
			parentDir.CreateDirectory();

			if (packagePath.IsDirectory())
			{
				var sets = setCodes?.Split(';', ',', '|').ToHashSet(Str.Comparer);

				var prevSignatureByPath = sets != null && output.IsFile()
					? Signer.ReadFromFile(output, internPath: false)
						.Where(_ => !sets.Contains(_.Path.Parent().Value))
						.ToDictionary(_ => _.Path)
					: new Dictionary<FsPath, FileSignature>();

				var signatures = Signer.CreateSignatures(packagePath, precalculated: prevSignatureByPath);
				Signer.WriteToFile(output, signatures);
			}
			else if (packagePath.IsFile())
			{
				var metadata = Signer.CreateSignature(packagePath);
				Signer.WriteToFile(output, Sequence.Array(metadata));
			}
			else
			{
				Console.WriteLine("Specified path {0} does not exist", packagePath);
			}
		}
	}
}
