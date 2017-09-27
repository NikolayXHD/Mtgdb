using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mtgdb.Dal;

namespace Mtgdb.Util
{
	internal static class ArtworkRenamer
	{
		private static readonly Regex _propertyRegex = new Regex(@"\[[^\]]+\]", RegexOptions.Compiled);
		private static readonly char[] _valueSeparator = { ',', ';' };

		public static void RenameArtworks(string directory)
		{
			Console.WriteLine("Artwork images at {0} will be renamed.", directory);
			Console.WriteLine("Press ENTER to continue");
			Console.ReadLine();

			var imageRepository = new ImageRepository(new ImageLocationsConfig
			{
				Directories = new[]
				{
					new DirectoryConfig
					{
						Path = directory,
						Art = true,
						ReadMetadataFromAttributes = true,
						Zoom = "False"
					}
				}
			});

			Console.WriteLine("Reading metadata from attributes...");

			imageRepository.LoadFiles();
			imageRepository.LoadArt();

			Console.WriteLine("Renaming...");

			var images = imageRepository.GetAllImagesArt()
				.GroupBy(_ => _.FullPath)
				.ToDictionary(
					gr => gr.Key,
					gr => new
					{
						artists = gr.Select(_ => _.Artist).Where(a => !string.IsNullOrWhiteSpace(a)).ToList(),
						sets = gr.Where(_ => _.SetCodeIsFromAttribute && !string.IsNullOrWhiteSpace(_.SetCode)).Select(_ => _.SetCode).ToList()
					});

			foreach (var entryByPath in images)
			{
				var original = entryByPath.Key.LastPathSegment();
				var dir = entryByPath.Key.Parent();
				var renamedBuilder = new StringBuilder();
				var parts = original.Split('.');

				var attribute = entryByPath.Value;

				var propertiesList = new List<string>();

				renamedBuilder.Append(parts[0]);

				for (int i = 1; i < parts.Length - 1; i++)
				{
					var part = parts[i];

					if (!part.Contains('['))
					{
						renamedBuilder.Append('.');
						renamedBuilder.Append(part);
						continue;
					}

					var propertyMatches = _propertyRegex.Matches(part);
					foreach (Match propertyMatch in propertyMatches)
					{
						string matchValue = propertyMatch.Value;
						string propertyValue = matchValue.Substring(1, matchValue.Length - 2);
						
						if (propertyValue.StartsWith("artist "))
							attribute.artists.AddRange(propertyValue.Substring("artist ".Length).Split(_valueSeparator));
						else if (propertyValue.StartsWith("set "))
							attribute.sets.AddRange(propertyValue.Substring("set ".Length).Split(_valueSeparator));
						else
							propertiesList.Add(propertyValue);
					}
				}

				var artists = attribute.artists
					.Where(_ => !string.IsNullOrWhiteSpace(_))
					.Distinct(Str.Comparer)
					.OrderBy(_=>_)
					.ToList();

				if (artists.Count > 0)
					propertiesList.Add("artist " + string.Join(",", artists));

				var sets = attribute.sets
					.Where(_=>!string.IsNullOrWhiteSpace(_))
					.Distinct(Str.Comparer)
					.OrderBy(_ => _)
					.ToList();

				if (sets.Count > 0)
					propertiesList.Add("set " + string.Join(",", sets));

				propertiesList = propertiesList.Distinct(Str.Comparer).ToList();

				if (propertiesList.Count > 0)
				{
					renamedBuilder.Append('.');
					foreach (string property in propertiesList)
					{
						renamedBuilder.Append('[');
						renamedBuilder.Append(property);
						renamedBuilder.Append(']');
					}
				}

				renamedBuilder.Append('.');
				renamedBuilder.Append(parts[parts.Length - 1]);

				var renamed = renamedBuilder.ToString();
				if (!Str.Equals(renamed, original))
				{
					Console.WriteLine("\trenaming {0} to {1}", original, renamed);
					string renamedFullPath = Path.Combine(dir, renamed);
					if (File.Exists(renamedFullPath))
					{
						var originalSignature = Signer.CreateSignature(entryByPath.Key);
						var renamedSignature = Signer.CreateSignature(renamedFullPath);
						if (Str.Equals(originalSignature.Md5Hash, renamedSignature.Md5Hash))
						{
							Console.WriteLine("Deleting {0} as identical to renamed {1}", entryByPath.Key, renamedFullPath);
							File.Delete(entryByPath.Key);
						}
						else
							Console.WriteLine("FILE ALREADY EXISTS: {0}", renamedFullPath);
					}
					else
						File.Move(entryByPath.Key, renamedFullPath);
				}
			}

			Console.WriteLine("Press ENTER to exit");
			Console.ReadLine();
		}
	}
}