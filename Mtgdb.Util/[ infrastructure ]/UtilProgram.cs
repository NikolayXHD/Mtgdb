﻿using System;
using System.IO;
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


			string directory = args.GetParam("-rename_artworks");
			if (directory != null)
			{
				ArtworkRenamer.RenameArtworks(directory);
				return;
			}

			directory = args.GetParam("-sign");
			if (directory != null)
			{
				var output = args.GetParam("-output") ?? Path.Combine(AppDir.Root, "filelist.txt");
				sign(directory, output);
				return;
			}

			directory = args.GetParam("-export");
			if (directory != null)
			{
				bool silent = args.GetFlag("-silent");
				bool small = args.GetFlag("-small");
				bool zoomed = args.GetFlag("-zoomed");
				string setCode = args.GetParam("-set");

				string smallSubdir = null;
				string zoomedSubdir = null;

				if (small && zoomed)
				{
					smallSubdir = args.GetParam("-small");
					zoomedSubdir = args.GetParam("-zoomed");
				}
				else if (!small && !zoomed)
				{
					printUsage();
					return;
				}


				if (Directory.Exists(directory))
					exportImages(small, zoomed, setCode, directory, silent, smallSubdir, zoomedSubdir);
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

		

		private static void exportImages(bool small, bool zoomed, string setCode, string directory, bool silent, string smallSubdir, string zoomedSubdir)
		{
			var integration = _kernel.Get<ForgeIntegration>();

			if (small && !zoomed)
				Console.Write("Small ");
			else if (zoomed && !small)
				Console.Write("Zoomed ");
			else
				Console.Write("Small and Zoomed ");

			if (setCode != null)
				Console.Write("set " + setCode + " ");
			else
				Console.Write("all sets ");

			Console.WriteLine($"Mtgdb.Gui images will be exported to {directory}.");

			if (!silent)
			{
				Console.WriteLine("Press ENTER to continue");
				Console.ReadLine();
			}
			
			integration.Load();

			Console.WriteLine("== Exporting card images ==");
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

			Console.WriteLine($"Forge images at {integration.CardPicsPath} will be replaced.");
			Console.WriteLine($"Replaced images will be backed up to {integration.CardPicsBackupPath}.");
			Console.WriteLine(@"To change target directory edit etc\Mtgdb.Integration.Forge.xml and start this executable again.");
			Console.WriteLine("To begin replacing Forge images press ENTER.");

			Console.ReadLine();
			integration.Load();

			Console.WriteLine("== Start overridng Forge pictures ==");
			integration.OverrideForgePictures(setCode);

			Console.WriteLine("Replacing done. Press ENTER to exit");
			Console.ReadLine();
		}

		private static void printUsage()
		{
			Console.WriteLine("Usage:");

			Console.WriteLine("Mtgdb.Util.exe -rename_artworks directory");
			Console.WriteLine("\t- rename artwork images moving artist and tags file attribute to name like cardname.[set soi,abc][artist Jonh Doe].jpg");

			Console.WriteLine("Mtgdb.Util.exe -forge [-set setcode]");
			Console.WriteLine("\t- replace images in Forge image directory");

			Console.WriteLine("Mtgdb.Util.exe -export directory [-set setcode] -small");
			Console.WriteLine("\t- export small images to directory specified after -path");
			Console.WriteLine("Mtgdb.Util.exe -export directory [-set setcode] -zoomed");
			Console.WriteLine("\t- export zoomed images to directory specified after -path");

			Console.WriteLine("Mtgdb.Util.exe -export directory [-set setcode] -small subdirectory_small -zoomed subdirectory_zoomed");
			Console.WriteLine("\t- export small images to directory\\subdirectory_small and zoomed images to directory\\subdirectory_zoomed");

			Console.WriteLine("Mtgdb.Util.exe -sign directory_or_file [-output filelist_name]");
			Console.WriteLine("\t- create a list of files with corresponding md5 hashes");
			Console.WriteLine("Press ENTER to exit");
			Console.ReadLine();
		}

		private static void sign(string packagePath, string output)
		{
			string parentDir = output.Parent();
			if (!Directory.Exists(parentDir))
			{
				Console.WriteLine("Cannot create output file. Directory {0} does not exist", parentDir);
				return;
			}

			if (Directory.Exists(packagePath))
			{
				var signatures = Signer.CreateSignatures(packagePath);
				Signer.WriteToFile(output, signatures);
			}
			else if (File.Exists(packagePath))
			{
				var metadata = Signer.CreateSignature(packagePath);
				Signer.WriteToFile(output, new[] { metadata });
			}
			else
			{
				Console.WriteLine("Specified path {0} does not exist", packagePath);
			}
		}
	}
}