﻿using System;
using Mtgdb.Data;
using Mtgdb.Dev;
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

			string directory = args.GetParam("-sign");
			if (directory != null)
			{
				string setCodes = args.GetParam("-set");
				string output = args.GetParam("-output") ?? AppDir.Root.Join("filelist.txt").Value;
				_kernel.Get<ImageDirectorySigner>().SignFiles(new FsPath(directory), new FsPath(output), setCodes);
				return;
			}

			printUsage();
		}

		private static void printUsage()
		{
			Console.WriteLine("Usage:");

			Console.WriteLine("Mtgdb.Util.exe -sign directory_or_file [-output filelist_name] [-set setcode1;setcode2;...]");
			Console.WriteLine("\t- create a list of files with corresponding md5 hashes");

			Console.WriteLine("Press ENTER to exit");
			Console.ReadLine();
		}
	}
}
