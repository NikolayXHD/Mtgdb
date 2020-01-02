using System;
using System.IO;
using Mtgdb.Data;
using Ninject;

namespace Mtgdb.Util
{
	internal static class UtilProgram
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
				HelpDownloader.UpdateLocalHelp().Wait();
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
				var integration = _kernel.Get<ImageExport>();
				integration.SignFiles(directory, output, setCodes);
				return;
			}

			directory = args.GetParam("-export");
			if (directory != null)
			{
				bool silent = args.GetFlag("-silent");
				bool small = args.GetFlag("-small");
				bool zoomed = args.GetFlag("-zoomed");
				string setCodes = args.GetParam("-set");
				bool tokens = args.GetParam("-type") == "token";

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

				bool forceRemoveCorner = args.GetFlag("-force-remove-corner");


				if (Directory.Exists(directory))
					exportImages(small, zoomed, setCodes, directory, silent, smallSubdir, zoomedSubdir, forceRemoveCorner, tokens);
				else
					Console.WriteLine($"directory not found: {directory}");
			}
		}

		private static void exportImages(
			bool small, bool zoomed, string setCodes, string directory, bool silent, string smallSubdir, string zoomedSubdir,
			bool forceRemoveCorner, bool tokens)
		{
			var integration = _kernel.Get<ImageExport>();

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

			if (tokens)
				Console.Write("token ");
			else
				Console.Write("card ");

			Console.WriteLine($"images will be exported to {directory}");

			if (!silent)
			{
				Console.WriteLine("Press ENTER to continue");
				Console.ReadLine();
			}

			integration.Load(enabledImageGroups: Array.From("dev", "xlhq"));

			Console.WriteLine("== Exporting card images ==");

			foreach (string setCode in setCodes?.Split(';', ',', '|') ?? new string[] { null })
				integration.ExportCardImages(directory, small, zoomed, setCode, smallSubdir, zoomedSubdir, forceRemoveCorner, tokens);

			if (!silent)
			{
				Console.WriteLine("Export done. Press ENTER to exit");
				Console.ReadLine();
			}
		}

		private static void printUsage()
		{
			Console.WriteLine("Usage:");

			Console.WriteLine("Mtgdb.Util.exe -rename_artworks directory");
			Console.WriteLine("\t- rename artwork images moving artist and tags file attribute to name like cardname.[set soi,abc][artist John Doe].jpg");

			Console.WriteLine("Mtgdb.Util.exe -export directory [-set setcode1;setcode2;...] [-force-remove-corner] [-silent] -small");
			Console.WriteLine("\t- export small images to directory specified after -path");
			Console.WriteLine("Mtgdb.Util.exe -export directory [-set setcode1;setcode2;...] [-force-remove-corner] [-silent] -zoomed");
			Console.WriteLine("\t- export zoomed images to directory specified after -path");

			Console.WriteLine("Mtgdb.Util.exe -export directory [-set setcode1;setcode2;...] [-force-remove-corner] [-silent] -small subdirectory_small -zoomed subdirectory_zoomed");
			Console.WriteLine("\t- export small images to directory\\subdirectory_small and zoomed images to directory\\subdirectory_zoomed");

			Console.WriteLine("Mtgdb.Util.exe -sign directory_or_file [-output filelist_name] [-set setcode1;setcode2;...]");
			Console.WriteLine("\t- create a list of files with corresponding md5 hashes");

			Console.WriteLine("Mtgdb.Util.exe -update_help");
			Console.WriteLine("\t- update local help web pages from https://github.com/NikolayXHD/Mtgdb/wiki");

			Console.WriteLine("Press ENTER to exit");
			Console.ReadLine();
		}
	}
}
