using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mtgdb.Dal;
using Ninject;

namespace Mtgdb.Util
{
	internal class UtilProgram
	{
		private static readonly IKernel _kernel = new StandardKernel();

		[LoaderOptimization(LoaderOptimization.MultiDomainHost)]
		public static void Main(string[] args)
		{
			ShadowCopy.RunMain(main, args);
		}

		private static void main(string[] args)
		{
			_kernel.Load<CoreModule>();
			_kernel.Load<DalModule>();
			_kernel.Load<UtilModule>();

			if (args.GetFlag("-notify"))
			{
				NewVersionNotifier.Notify();
				return;
			}

			if (args.GetFlag("-update_help"))
			{
				Console.Write("Updating local help...");
				HelpDownloader.UpdateLocalHelp();
				Console.WriteLine(" Done");
				return;
			}

			string directory = args.GetParam("-rename_artworks");
			if (directory != null)
			{
				ArtworkRenamer.RenameArtworks(directory);
				return;
			}

			directory = args.GetParam("-sign");
			if (directory != null)
			{
				string setCodes = args.GetParam("-set");
				var output = args.GetParam("-output") ?? AppDir.Root.AddPath("filelist.txt");
				sign(directory, output, setCodes);
				return;
			}

			directory = args.GetParam("-export");
			if (directory != null)
			{
				bool silent = args.GetFlag("-silent");
				bool small = args.GetFlag("-small");
				bool zoomed = args.GetFlag("-zoomed");
				string setCodes = args.GetParam("-set");

				string smallSubdir;
				string zoomedSubdir;

				if (small || zoomed)
				{
					smallSubdir = args.GetParam("-small");
					zoomedSubdir = args.GetParam("-zoomed");
				}
				else
				{
					printUsage();
					return;
				}


				if (Directory.Exists(directory))
					exportImages(small, zoomed, setCodes, directory, silent, smallSubdir, zoomedSubdir);
				else
					printUsage();

				return;
			}

			if (args.GetFlag("-forge"))
			{
				string setCode = args.GetParam("-set");
				replaceForgeImages(setCode);
			}
		}

		private static void exportImages(bool small, bool zoomed, string setCodes, string directory, bool silent, string smallSubdir, string zoomedSubdir)
		{
			var integration = _kernel.Get<ForgeIntegration>();

			if (small && !zoomed)
				Console.Write("Small ");
			else if (zoomed && !small)
				Console.Write("Zoomed ");
			else
				Console.Write("Small and Zoomed ");

			if (setCodes != null)
				Console.Write("set " + setCodes + " ");
			else
				Console.Write("all sets ");

			Console.WriteLine($"Mtgdb.Gui images will be exported to {directory}.");

			if (!silent)
			{
				Console.WriteLine("Press ENTER to continue");
				Console.ReadLine();
			}

			integration.Load(enabledImageGroups: Array.From("dev", "xlhq"));

			Console.WriteLine("== Exporting card images ==");

			foreach (string setCode in setCodes?.Split(';', ',', '|') ?? new string[] { null })
				integration.ExportCardImages(directory, small, zoomed, setCode, smallSubdir, zoomedSubdir);

			if (!silent)
			{
				Console.WriteLine("Export done. Press ENTER to exit");
				Console.ReadLine();
			}
		}

		private static void replaceForgeImages(string setCode)
		{
			var integration = _kernel.Get<ForgeIntegration>();

			Console.WriteLine($"Forge images for {setCode ?? "all sets"} at {integration.CardPicsPath} will be replaced.");
			Console.WriteLine($"Replaced images will be backed up to {integration.CardPicsBackupPath}.");
			Console.WriteLine(@"To change target directory edit etc\Mtgdb.Integration.Forge.xml and start this executable again.");
			Console.WriteLine("To begin replacing Forge images press ENTER.");

			Console.ReadLine();
			integration.Load();

			Console.WriteLine("== Start overriding Forge pictures ==");
			integration.OverrideForgePictures(setCode);

			Console.WriteLine("Replacing done. Press ENTER to exit");
			Console.ReadLine();
		}

		private static void printUsage()
		{
			Console.WriteLine("Usage:");

			Console.WriteLine("Mtgdb.Util.exe -rename_artworks directory");
			Console.WriteLine("\t- rename artwork images moving artist and tags file attribute to name like cardname.[set soi,abc][artist John Doe].jpg");

			Console.WriteLine("Mtgdb.Util.exe -forge [-set setcode]");
			Console.WriteLine("\t- replace images in Forge image directory");

			Console.WriteLine("Mtgdb.Util.exe -export directory [-set setcode1;setcode2;...] -small");
			Console.WriteLine("\t- export small images to directory specified after -path");
			Console.WriteLine("Mtgdb.Util.exe -export directory [-set setcode1;setcode2;...] -zoomed");
			Console.WriteLine("\t- export zoomed images to directory specified after -path");

			Console.WriteLine("Mtgdb.Util.exe -export directory [-set setcode1;setcode2;...] -small subdirectory_small -zoomed subdirectory_zoomed");
			Console.WriteLine("\t- export small images to directory\\subdirectory_small and zoomed images to directory\\subdirectory_zoomed");

			Console.WriteLine("Mtgdb.Util.exe -sign directory_or_file [-output filelist_name] [-set setcode1;setcode2;...]");
			Console.WriteLine("\t- create a list of files with corresponding md5 hashes");

			Console.WriteLine("Mtgdb.Util.exe -update_help");
			Console.WriteLine("\t- update local help web pages from https://github.com/NikolayXHD/Mtgdb/wiki");

			Console.WriteLine("Press ENTER to exit");
			Console.ReadLine();
		}

		private static void sign(string packagePath, string output, string setCodes)
		{
			string parentDir = output.Parent();
			if (!Directory.Exists(parentDir))
			{
				Console.WriteLine("Cannot create output file. Directory {0} does not exist", parentDir);
				return;
			}

			if (Directory.Exists(packagePath))
			{
				var sets = setCodes?.Split(';', ',', '|').ToHashSet(Str.Comparer);

				var prevSignatureByPath = sets != null && File.Exists(output)
					? Signer.ReadFromFile(output)
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