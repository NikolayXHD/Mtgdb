using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Data;

namespace Mtgdb.Util
{
	internal static class ArtworkRenamer
	{
		private static readonly Regex _propertyRegex = new Regex(@"\.?\[(?<prop>[^\]]+)\]\.?");
		private static readonly char[] _valueSeparator = { ',', ';' };

		public static void RenameArtworks(FsPath directory)
		{
			Console.WriteLine("Artwork images at {0} will be renamed.", directory);
			Console.WriteLine("Press ENTER to continue");
			Console.ReadLine();

			var imageRepository = new ImageRepository(
				new ImageLocationsConfig
				{
					Directories = Array.From(
						new DirectoryConfig
						{
							Path = directory,
							Art = true,
							ReadMetadataFromAttributes = true,
							Zoom = "False"
						})
				});

			Console.WriteLine("Reading metadata from attributes...");

			imageRepository.LoadFiles();
			imageRepository.LoadArt();

			Console.WriteLine("Renaming...");

			var images = imageRepository.GetAllArts()
				.GroupBy(_ => _.FullPath)
				.ToDictionary(
					gr => gr.Key,
					gr => (
						artists: gr.Select(_ => _.Artist).Where(a => !string.IsNullOrWhiteSpace(a)).ToList(),
						sets: gr.Where(_ => _.SetCodeIsFromAttribute && !string.IsNullOrWhiteSpace(_.SetCode)).Select(_ => _.SetCode).ToList()
					));

			foreach ((FsPath path, (List<string> artistsList, List<string> setsList)) in images)
			{
				string original = path.Basename();
				string renamed = Rename(original, artistsList, setsList);

				if (Str.Equals(renamed, original))
					continue;

				Console.WriteLine("\trenaming {0} to {1}", original, renamed);
				FsPath renamedFullPath = path.Parent().Join(renamed);
				if (renamedFullPath.IsFile())
				{
					var originalSignature = Signer.CreateSignature(path);
					var renamedSignature = Signer.CreateSignature(renamedFullPath);
					if (Str.Equals(originalSignature.Md5Hash, renamedSignature.Md5Hash))
					{
						Console.WriteLine("Deleting {0} as identical to renamed {1}", path, renamedFullPath);
						path.DeleteFile();
					}
					else
						Console.WriteLine("FILE ALREADY EXISTS: {0}", renamedFullPath);
				}
				else
					path.MoveFileTo(renamedFullPath);
			}

			Console.WriteLine("Press ENTER to exit");
			Console.ReadLine();
		}

		internal static string Rename(string original, List<string> artistsList = null, List<string> setsList = null)
		{
			artistsList ??= new List<string>();
			setsList ??= new List<string>();

			var renamedBuilder = new StringBuilder();
			var unknownList = new List<string>();

			int dotIndex = original.LastIndexOf('.');
			if (dotIndex < 0)
				dotIndex = original.Length;

			string name = original.Substring(0, dotIndex);
			var extension = original.Substring(dotIndex);

			var replaced = _propertyRegex.Replace(name, match =>
			{
				string propertyValue = match.Groups["prop"].Value;
				if (propertyValue.StartsWith("artist "))
					artistsList.AddRange(propertyValue.Substring("artist ".Length).Split(_valueSeparator));
				else if (propertyValue.StartsWith("set "))
					setsList.AddRange(propertyValue.Substring("set ".Length).Split(_valueSeparator));
				else
					unknownList.Add(propertyValue);
				return string.Empty;
			});

			renamedBuilder.Append(replaced);

			var artists = artistsList
				.Where(_ => !string.IsNullOrWhiteSpace(_))
				.Distinct(Str.Comparer)
				.OrderBy(Str.Comparer)
				.ToList();

			var propertiesList = new List<string>(unknownList.Count + 2);
			propertiesList.AddRange(unknownList);

			if (artists.Count > 0)
				propertiesList.Add("artist " + string.Join(",", artists));

			var sets = setsList
				.Where(_ => !string.IsNullOrWhiteSpace(_))
				.Distinct(Str.Comparer)
				.OrderBy(Str.Comparer)
				.ToList();

			if (sets.Count > 0)
				propertiesList.Add("set " + string.Join(",", sets));

			propertiesList = propertiesList.Distinct(Str.Comparer).ToList();

			if (propertiesList.Count > 0)
			{
				renamedBuilder.Append('.');
				foreach (string property in propertiesList)
				{
					renamedBuilder
						.Append('[')
						.Append(property)
						.Append(']');
				}
			}

			renamedBuilder.Append(extension);
			return renamedBuilder.ToString();
		}
	}
}
