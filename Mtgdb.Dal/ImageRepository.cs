using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shell32;

namespace Mtgdb.Dal
{
	public class ImageRepository
	{
		public ImageRepository(ImageLocationsConfig config)
		{
			_config = config;
		}

		public void LoadFiles(IEnumerable<string> enabledGroups = null)
		{
			var filesByDirCache = new Dictionary<string, IList<string>>(Str.Comparer);
			var enabledDirectories = _config.GetEnabledDirectories(enabledGroups);

			loadFilesSmall(enabledDirectories, filesByDirCache);
			loadFilesZoom(enabledDirectories, filesByDirCache);
			loadFilesArt(enabledDirectories, filesByDirCache);
		}

		public void LoadFilesSmall()
		{
			loadFilesSmall(_config.GetEnabledDirectories(), new Dictionary<string, IList<string>>(Str.Comparer));
		}

		public void LoadFilesZoom()
		{
			loadFilesZoom(_config.GetEnabledDirectories(), new Dictionary<string, IList<string>>(Str.Comparer));
		}

		public void LoadFilesArt()
		{
			loadFilesArt(_config.GetEnabledDirectories(), new Dictionary<string, IList<string>>(Str.Comparer));
		}

		private void loadFilesSmall(IList<DirectoryConfig> enabledDirectories, Dictionary<string, IList<string>> filesByDirCache)
		{
			var directories = getFolders(enabledDirectories, ImageType.Small);
			var files = new HashSet<string>(Str.Comparer);

			loadFiles(filesByDirCache, directories, files);

			_directories = directories;
			_files = files;
		}

		private void loadFilesZoom(IList<DirectoryConfig> enabledDirectories, Dictionary<string, IList<string>> filesByDirCache)
		{
			var directories = getFolders(enabledDirectories, ImageType.Zoom);
			var files = new HashSet<string>(Str.Comparer);

			loadFiles(filesByDirCache, directories, files);

			_directoriesZoom = directories;
			_filesZoom = files;
		}

		private void loadFilesArt(IList<DirectoryConfig> enabledDirectories, Dictionary<string, IList<string>> filesByDirCache)
		{
			var directories = getFolders(enabledDirectories, ImageType.Art);
			var files = new HashSet<string>(Str.Comparer);

			loadFiles(filesByDirCache, directories, files);

			_directoriesArt = directories;
			_filesArt = files;
		}



		private static DirectoryConfig[] getFolders(IList<DirectoryConfig> directoryConfigs, Func<DirectoryConfig, bool> filter)
		{
			return directoryConfigs
				.Where(c => filter(c) && Directory.Exists(c.Path))
				// use_dir_sorting_to_find_most_nested_root
				.OrderByDescending(c => c.Path.Length)
				.ToArray();
		}

		private static void loadFiles(Dictionary<string, IList<string>> filesByDirCache, IList<DirectoryConfig> directories, ICollection<string> files)
		{
			foreach (var directory in directories)
			{
				var excludes = directory.Exclude?.Split(';');

				foreach (string file in getDirectoryFiles(filesByDirCache, directory.Path))
				{
					if (excludes != null && excludes.Any(exclude => file.IndexOf(exclude, Str.Comparison) >= 0))
						continue;

					files.Add(file);
				}
			}
		}

		private static IEnumerable<string> getDirectoryFiles(Dictionary<string, IList<string>> filesByDirCache, string path)
		{
			if (filesByDirCache.TryGetValue(path, out var cache))
			{
				foreach (string file in cache)
					yield return file;
			}
			else
			{
				cache = new List<string>();

				foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
				{
					if (!_extensions.Contains(Path.GetExtension(file)))
						continue;

					cache.Add(file);
					yield return file;
				}

				filesByDirCache.Add(path, cache);
			}
		}



		public void LoadSmall()
		{
			if (!IsLoadingSmallFileComplete)
				throw new InvalidOperationException($"{nameof(LoadFilesSmall)} must be executed first");

			var models = new Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>>(Str.Comparer);

			load(models, _directories, _files);

			_modelsByNameBySetByVariant = models;
		}

		public void LoadZoom()
		{
			if (!IsLoadingZoomFileComplete)
				throw new InvalidOperationException($"{nameof(LoadFilesZoom)} must be executed first");

			var models = new Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>>(Str.Comparer);

			load(models, _directoriesZoom, _filesZoom);

			_modelsByNameBySetByVariantZoom = models;
		}

		public void LoadArt()
		{
			if (!IsLoadingArtFileComplete)
				throw new InvalidOperationException($"{nameof(LoadFilesArt)} must be executed first");

			var models = new Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>>(Str.Comparer);

			load(models, _directoriesArt, _filesArt, isArt: true);

			_modelsByNameBySetByVariantArt = models;
		}

		private static void load(
			Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> modelsByNameBySetByVariant,
			IList<DirectoryConfig> directories,
			IEnumerable<string> files,
			bool isArt = false)
		{
			Shell shl = null;
			foreach (var entryByDirectory in files.GroupBy(Path.GetDirectoryName))
			{
				// use_dir_sorting_to_find_most_nested_root
				var root = directories.First(_ => entryByDirectory.Key.StartsWith(_.Path));
				string customSetCode = root.Set;

				bool readAttributes = root.ReadMetadataFromAttributes == true;

				Folder dir = null;
				if (isArt && readAttributes)
				{
					shl = shl ?? new Shell();
					dir = shl.NameSpace(entryByDirectory.Key);
				}

				foreach (string file in entryByDirectory)
				{
					IList<string> authors = null;
					IList<string> setCodes = customSetCode?.Split(';').ToList();

					if (isArt)
					{
						string fileName = Path.GetFileName(file);
						getMetadataFromName(fileName, ref authors, ref setCodes);

						if (readAttributes)
							getMetadataFromAttributes(dir, fileName, ref authors, ref setCodes);
					}

					authors = notNullOrEmpty(authors);
					setCodes = notNullOrEmpty(setCodes);

					foreach (string author in authors)
						foreach (string setCode in setCodes)
						{
							var model = new ImageFile(file, root.Path, setCode, author, isArt);
							add(model, modelsByNameBySetByVariant);
						}
				}
			}
		}

		private static void getMetadataFromAttributes(Folder dir, string fileName, ref IList<string> authors, ref IList<string> keywords)
		{
			var item = dir.ParseName(fileName);
			string authorsValue = dir.GetDetailsOf(item, 20);
			string keywordsValue = dir.GetDetailsOf(item, 18);

			add(ref authors, authorsValue?.Split(';').Select(_ => _.Trim()).ToArray());
			add(ref keywords, keywordsValue?.Split(';').Select(_ => _.Trim()).ToArray());
		}

		private static void getMetadataFromName(string fileName, ref IList<string> artist, ref IList<string> set)
		{
			var parts = fileName.Split('.');

			foreach (string part in parts)
			{
				if (part.Length < 2)
					continue;

				if (part[0] != MetadataBegin || part[part.Length - 1] != MetadataEnd)
					continue;

				var subparts = part.Substring(1, part.Length - 2).Split(_metadataSeparator, StringSplitOptions.None);

				foreach (string subpart in subparts)
					if (!tryAdd(ref artist, "artist ", subpart))
						tryAdd(ref set, "set ", subpart);
			}
		}

		private static bool tryAdd(ref IList<string> valuesList, string prefix, string subpart)
		{
			if (!subpart.StartsWith(prefix, Str.Comparison))
				return false;

			var values = subpart.Substring(prefix.Length).Split(_metadataValueSeparator);
			add(ref valuesList, values);
			return true;
		}

		private static void add(ref IList<string> valuesList, string[] values)
		{
			if (values == null)
				return;

			valuesList = valuesList ?? new List<string>();

			foreach (string value in values)
				valuesList.Add(value);
		}

		private static IList<string> notNullOrEmpty(IList<string> value)
		{
			if (value == null || value.Count == 0)
				return new string[] { null };

			return value;
		}

		private static void add(
			ImageFile imageFile,
			Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> modelsByNameBySetByVariant)
		{
			lock (modelsByNameBySetByVariant)
			{
				int separator = imageFile.Name.IndexOfAny(Array.From('_', '»'));

				if (separator > 0 && separator < imageFile.Name.Length - 1)
				{
					add(imageFile, modelsByNameBySetByVariant, imageFile.Name.Substring(0, separator));
					add(imageFile, modelsByNameBySetByVariant, imageFile.Name.Substring(separator + 1));
				}
				else
					add(imageFile, modelsByNameBySetByVariant, imageFile.Name);
			}
		}

		private static void add(ImageFile imageFile, Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> modelsByNameBySetByVariant, string name)
		{
			name = string.Intern(name);

			if (!modelsByNameBySetByVariant.TryGetValue(name, out var modelsBySet))
			{
				modelsBySet = new Dictionary<string, Dictionary<int, ImageFile>>(Str.Comparer);
				modelsByNameBySetByVariant.Add(name, modelsBySet);
			}

			if (!modelsBySet.TryGetValue(imageFile.SetCode, out var modelsByImageVariant))
			{
				modelsByImageVariant = new Dictionary<int, ImageFile>();
				modelsBySet.Add(imageFile.SetCode, modelsByImageVariant);
			}

			// For each number select the representative with best quality 
			if (!modelsByImageVariant.TryGetValue(imageFile.VariantNumber, out var currentImageFile) || currentImageFile.Quality < imageFile.Quality)
				modelsByImageVariant[imageFile.VariantNumber] = imageFile;
		}


		public ImageModel GetSmallImage(Card card, Func<string, string, string> setCodePreference)
		{
			var result = getImage(card, setCodePreference, _modelsByNameBySetByVariant);

			if (result != null)
				return result;

			result = getImage(card, setCodePreference, _modelsByNameBySetByVariantZoom);
			return result;
		}

		public ImageModel GetImagePrint(Card card, Func<string, string, string> setCodePreference)
		{
			return
				getImage(card, setCodePreference, _modelsByNameBySetByVariantZoom)?.ImageFile.NonRotated() ??
				getImage(card, setCodePreference, _modelsByNameBySetByVariant)?.ImageFile.NonRotated();
		}

		private static ImageModel getImage(
			Card card,
			Func<string, string, string> setCodePreference,
			Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> modelsByNameBySetByVariant)
		{
			var model = getImage(modelsByNameBySetByVariant,
				setCodePreference,
				card.SetCode,
				card.ImageNameBase,
				card.ImageName,
				card.Artist);

			var result = model?.ApplyRotation(card, zoom: false);
			return result;
		}

		private static ImageFile getImage(
			Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> modelsByNameBySetByVariant,
			Func<string, string, string> setCodePreference,
			string set,
			string imageNameBase,
			string imageName,
			string artist)
		{
			lock (modelsByNameBySetByVariant)
			{
				if (!modelsByNameBySetByVariant.TryGetValue(imageNameBase, out var bySet))
					return null;

				if (set == null || !bySet.TryGetValue(set, out var byImageVariant))
				{
					byImageVariant = bySet
						.AtMax(setPriority2(artist))
						.ThenAtMax(setPriority3(setCodePreference, set))
						.Find()
						.Value;
				}

				var model = byImageVariant.Values
					.AtMax(cardPriority1(set, imageName))
					.ThenAtMax(cardPriority2(set, artist))
					.ThenAtMax(cardPriority3(artist))
					.ThenAtMax(cardPriority4(imageName))
					.ThenAtMin(cardUnpriority5())
					.Find();

				return model;
			}
		}

		public List<ImageModel> GetZooms(Card card, Func<string, string, string> setCodePreference)
		{
			List<ImageModel> result;

			if (IsLoadingZoomComplete)
			{
				result = getImageModels(card, setCodePreference, _modelsByNameBySetByVariantZoom);

				if (result != null)
					return result;
			}

			result = getImageModels(card, setCodePreference, _modelsByNameBySetByVariant);
			return result;
		}

		public List<ImageModel> GetArts(Card card, Func<string, string, string> setCodePreference)
		{
			if (!IsLoadingArtComplete)
				return null;

			var models = getImageModels(card, setCodePreference, _modelsByNameBySetByVariantArt);

			var distinctModels = models?
				.GroupBy(_ => _.ImageFile.FullPath)
				.Select(_ => _.First().ImageFile.NonRotated())
				.ToList();

			return distinctModels;
		}

		public IEnumerable<ImageFile> GetAllArts()
		{
			foreach (var bySet in _modelsByNameBySetByVariantArt.Values)
				foreach (var byVariant in bySet.Values)
					foreach (var model in byVariant.Values)
						yield return model;
		}

		public IEnumerable<ImageFile> GetAllZooms()
		{
			foreach (var bySet in _modelsByNameBySetByVariantZoom.Values)
				foreach (var byVariant in bySet.Values)
					foreach (var model in byVariant.Values)
						yield return model;
		}

		private static List<ImageModel> getImageModels(
			Card card,
			Func<string, string, string> setCodePreference,
			Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> modelsByNameBySetByVariant)
		{
			lock (modelsByNameBySetByVariant)
			{
				if (!modelsByNameBySetByVariant.TryGetValue(card.ImageNameBase, out var modelsBySet))
					return null;

				var entriesBySet = modelsBySet
					.OrderByDescending(setPriority1(card.SetCode))
					.ThenByDescending(setPriority2(card.Artist))
					.ThenByDescending(setPriority3(setCodePreference, card.SetCode))
					.ToList();

				var models = entriesBySet
					.SelectMany(bySet => orderCardsWithinSet(card, bySet.Value))
					.ToList();

				if (models.Count == 0)
					return null;

				return models.Select(m => m.ApplyRotation(card, zoom: true)).ToList();
			}
		}

		private static IEnumerable<ImageFile> orderCardsWithinSet(Card card, Dictionary<int, ImageFile> variants)
		{
			var result = variants.Select(byVariant => byVariant.Value)
				.OrderByDescending(cardPriority1(card.SetCode, card.ImageName))
				.ThenByDescending(cardPriority2(card.SetCode, card.Artist))
				.ThenByDescending(cardPriority3(card.Artist))
				.ThenByDescending(cardPriority4(card.ImageName))
				.ThenBy(cardUnpriority5())
				.ToList();

			return result;
		}



		/// <summary>
		/// First the matching set
		/// </summary>
		private static Func<KeyValuePair<string, Dictionary<int, ImageFile>>, bool> setPriority1(string set)
		{
			return bySet => Str.Equals(bySet.Key, set);
		}

		/// <summary>
		/// Then the sets that have a variant with matching artist
		/// </summary>
		private static Func<KeyValuePair<string, Dictionary<int, ImageFile>>, bool> setPriority2(string artist)
		{
			return bySet => artist != null && bySet.Value.Values.Any(_ => Str.Equals(_.Artist, artist));
		}

		/// <summary>
		/// Other sets go from new to old
		/// </summary>
		private static Func<KeyValuePair<string, Dictionary<int, ImageFile>>, string> setPriority3(Func<string, string, string> setCodePreference, string set)
		{
			return bySet => setCodePreference(set, bySet.Key);
		}

		/// <summary>
		/// In matching set let's show first the card with matching variant number
		/// </summary>
		private static Func<ImageFile, bool> cardPriority1(string set, string imageName)
		{
			return model =>
				Str.Equals(model.SetCode, set) &&
				Str.Equals(model.ImageName, imageName);
		}

		/// <summary>
		/// then the card with matching artist
		/// </summary>
		private static Func<ImageFile, bool> cardPriority2(string set, string artist)
		{
			return model =>
				Str.Equals(model.SetCode, set) &&
				Str.Equals(model.Artist, artist);
		}

		/// <summary>
		/// In other sets first the card with matching artist
		/// </summary>
		private static Func<ImageFile, bool> cardPriority3(string artist)
		{
			return model => Str.Equals(model.Artist, artist);
		}

		/// <summary>
		/// Then matching number
		/// </summary>
		private static Func<ImageFile, bool> cardPriority4(string imageName)
		{
			return model => Str.Equals(model.ImageName, imageName);
		}

		/// <summary>
		/// Others by variant number ascending
		
		/// </summary>
		/// <returns></returns>
		private static Func<ImageFile, int> cardUnpriority5()
		{
			return model => model.VariantNumber;
		}



		public ImageModel GetReplacementImage(ImageFile imageFile, Func<string, string, string> setCodePreference)
		{
			if (string.IsNullOrEmpty(imageFile.Name))
				return null;

			var result = getImage(_modelsByNameBySetByVariantZoom,
				setCodePreference,
				imageFile.SetCode,
				imageFile.Name,
				imageFile.ImageName,
				imageFile.Artist);

			if (result != null)
				return result.NonRotated();

			result = getImage(_modelsByNameBySetByVariant,
				setCodePreference,
				imageFile.SetCode,
				imageFile.Name,
				imageFile.ImageName,
				imageFile.Artist);

			return result.NonRotated();
		}


		public bool IsLoadingSmallComplete => _modelsByNameBySetByVariant != null;
		public bool IsLoadingZoomComplete => _modelsByNameBySetByVariantZoom != null;
		public bool IsLoadingArtComplete => _modelsByNameBySetByVariantArt != null;

		private bool IsLoadingSmallFileComplete => _files != null && _directories != null;
		private bool IsLoadingZoomFileComplete => _filesZoom != null && _directoriesZoom != null;
		private bool IsLoadingArtFileComplete => _filesArt != null && _directoriesArt != null;


		private HashSet<string> _files;
		private HashSet<string> _filesZoom;
		private HashSet<string> _filesArt;

		private IList<DirectoryConfig> _directories;
		private IList<DirectoryConfig> _directoriesZoom;
		private IList<DirectoryConfig> _directoriesArt;

		private Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> _modelsByNameBySetByVariant;
		private Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> _modelsByNameBySetByVariantZoom;
		private Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> _modelsByNameBySetByVariantArt;



		private static readonly HashSet<string> _extensions = new HashSet<string>(Str.Comparer) { ".jpg", ".png" };
		private static readonly string[] _metadataSeparator = { "][" };
		private const char MetadataBegin = '[';
		private const char MetadataEnd = ']';
		private static readonly char[] _metadataValueSeparator = { ',', ';' };



		private readonly ImageLocationsConfig _config;
	}
}