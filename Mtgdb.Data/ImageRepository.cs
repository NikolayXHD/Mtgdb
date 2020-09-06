using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Ninject;

namespace Mtgdb.Data
{
	public class ImageRepository
	{
		public ImageRepository(
			ImageLocationsConfig config,
			[Optional] IShell shell)
		{
			_config = config;
			_shell = shell;
		}

		public void LoadFiles(IEnumerable<string> enabledGroups = null)
		{
			var filesByDirCache = new Dictionary<FsPath, IList<FsPath>>();
			var enabledDirectories = _config.GetEnabledDirectories(enabledGroups);

			loadFilesSmall(enabledDirectories, filesByDirCache);
			loadFilesZoom(enabledDirectories, filesByDirCache);
			loadFilesArt(enabledDirectories, filesByDirCache);
		}

		public void LoadFilesSmall()
		{
			loadFilesSmall(_config.GetEnabledDirectories(), new Dictionary<FsPath, IList<FsPath>>());
		}

		public void LoadFilesZoom()
		{
			loadFilesZoom(_config.GetEnabledDirectories(), new Dictionary<FsPath, IList<FsPath>>());
		}

		public void LoadFilesArt()
		{
			loadFilesArt(_config.GetEnabledDirectories(), new Dictionary<FsPath, IList<FsPath>>());
		}

		private void loadFilesSmall(IList<DirectoryConfig> enabledDirectories, Dictionary<FsPath, IList<FsPath>> filesByDirCache)
		{
			var directories = getFolders(enabledDirectories, ImageType.Small);
			var files = new HashSet<FsPath>();

			loadFiles(filesByDirCache, directories, files);

			_directories = directories;
			_files = files;

			IsLoadingSmallFileComplete.Signal();
		}

		private void loadFilesZoom(IList<DirectoryConfig> enabledDirectories, Dictionary<FsPath, IList<FsPath>> filesByDirCache)
		{
			var directories = getFolders(enabledDirectories, ImageType.Zoom);
			var files = new HashSet<FsPath>();

			loadFiles(filesByDirCache, directories, files);

			_directoriesZoom = directories;
			_filesZoom = files;

			IsLoadingZoomFileComplete.Signal();
		}

		private void loadFilesArt(IList<DirectoryConfig> enabledDirectories, Dictionary<FsPath, IList<FsPath>> filesByDirCache)
		{
			var directories = getFolders(enabledDirectories, ImageType.Art);
			var files = new HashSet<FsPath>();

			loadFiles(filesByDirCache, directories, files);

			_directoriesArt = directories;
			_filesArt = files;

			IsLoadingArtFileComplete.Signal();
		}



		private static DirectoryConfig[] getFolders(IList<DirectoryConfig> directoryConfigs, Func<DirectoryConfig, bool> filter)
		{
			return directoryConfigs
				.Where(c => filter(c) && c.Path.IsDirectory())
				// use_dir_sorting_to_find_most_nested_root
				.OrderByDescending(c => c.Path.Value.Length)
				.ToArray();
		}

		private static void loadFiles(Dictionary<FsPath, IList<FsPath>> filesByDirCache, IList<DirectoryConfig> directories, ICollection<FsPath> files)
		{
			foreach (var directory in directories)
			{
				if (!directory.Path.HasValue())
					continue;

				var excludes = directory.Exclude?.Split(Array.From(';'), StringSplitOptions.RemoveEmptyEntries);
				foreach (FsPath file in getDirectoryFiles(filesByDirCache, directory.Path))
				{
					if (excludes != null && excludes.Any(exclude => file.Value.IndexOf(exclude, Str.Comparison) >= 0))
						continue;

					files.Add(file);
				}
			}
		}

		private static IEnumerable<FsPath> getDirectoryFiles(Dictionary<FsPath, IList<FsPath>> filesByDirCache, FsPath path)
		{
			if (filesByDirCache.TryGetValue(path, out var cache))
			{
				foreach (FsPath file in cache)
					yield return file;
			}
			else
			{
				cache = new List<FsPath>();

				foreach (FsPath file in path.EnumerateFiles(option: SearchOption.AllDirectories))
				{
					if (!_extensions.Contains(file.Extension()))
						continue;

					cache.Add(file);
					yield return file;
				}

				filesByDirCache.Add(path, cache);
			}
		}



		public void LoadSmall()
		{
			if (!IsLoadingSmallFileComplete.Signaled)
				throw new InvalidOperationException($"{nameof(LoadFilesSmall)} must be executed first");

			var models = new Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>>(Str.Comparer);

			load(models, _directories, _files);

			_modelsByNameBySetByVariant = models;

			IsLoadingSmallComplete.Signal();
		}

		public void LoadZoom()
		{
			if (!IsLoadingZoomFileComplete.Signaled)
				throw new InvalidOperationException($"{nameof(LoadFilesZoom)} must be executed first");

			var models = new Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>>(Str.Comparer);

			load(models, _directoriesZoom, _filesZoom);

			_modelsByNameBySetByVariantZoom = models;

			IsLoadingZoomComplete.Signal();
		}

		public void LoadArt()
		{
			if (!IsLoadingArtFileComplete.Signaled)
				throw new InvalidOperationException($"{nameof(LoadFilesArt)} must be executed first");

			var models = new Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>>(Str.Comparer);

			load(models, _directoriesArt, _filesArt, isArt: true);

			_modelsByNameBySetByVariantArt = models;

			IsLoadingArtComplete.Signal();
		}

		private void load(
			Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> modelsByNameBySetByVariant,
			IList<DirectoryConfig> directories,
			IEnumerable<FsPath> files,
			bool isArt = false)
		{
			foreach (var entryByDirectory in files.GroupBy(_=>_.Parent()))
			{
				// use_dir_sorting_to_find_most_nested_root
				var root = directories.First(_ => _.Path.IsParentOrEqualOf(entryByDirectory.Key));
				string customSetCode = root.Set;
				int? customPriority = root.Priority;

				bool readAttributes = root.ReadMetadataFromAttributes == true && _shell != null;

				IShellFolder dir = null;
				if (isArt && readAttributes)
				{
					dir = _shell.GetFolder(entryByDirectory.Key);
				}

				foreach (FsPath file in entryByDirectory)
				{
					IList<string> authors = null;
					IList<string> setCodes = customSetCode?.Split(';').ToList();

					if (isArt)
					{
						string fileName = file.Basename();
						GetMetadataFromName(fileName, ref authors, ref setCodes);

						if (readAttributes)
							getMetadataFromAttributes(dir, fileName, ref authors, ref setCodes);
					}

					authors = notNullOrEmpty(authors);
					setCodes = notNullOrEmpty(setCodes);

					foreach (string author in authors)
						foreach (string setCode in setCodes)
						{
							var model = new ImageFile(file, root.Path, setCode, author, isArt, customPriority);
							add(model, modelsByNameBySetByVariant);
						}
				}
			}
		}

		private static void getMetadataFromAttributes(IShellFolder dir, string fileName, ref IList<string> authors, ref IList<string> keywords)
		{
			var shellFile = dir.GetFile(fileName);
			string authorsValue = shellFile.GetAuthors();
			string keywordsValue = shellFile.GetKeywords();

			add(ref authors, authorsValue?.Split(';').Select(_ => _.Trim()).ToArray());
			add(ref keywords, keywordsValue?.Split(';').Select(_ => _.Trim()).ToArray());
		}

		internal static void GetMetadataFromName(string fileName, ref IList<string> artist, ref IList<string> set)
		{
			foreach (Match match in _metadataRegex.Matches(fileName))
			{
				switch (match.Groups["field"].Value)
				{
					case "artist":
						add(match, ref artist);
						break;
					case "set":
						add(match, ref set);
						break;
					default:
						continue;
				}
			}

			static void add(Match match, ref IList<string> list)
			{
				foreach (Capture capture in match.Groups["name"].Captures)
					(list ??= new List<string>()).Add(capture.Value);
			}
		}

		private static void add(ref IList<string> valuesList, string[] values)
		{
			if (values == null)
				return;

			valuesList ??= new List<string>();

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
			if (!modelsByImageVariant.TryGetValue(imageFile.VariantNumber, out var currentImageFile) || currentImageFile.Priority < imageFile.Priority)
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
			var model = getImage(
				modelsByNameBySetByVariant,
				setCodePreference,
				card.SetCode,
				card.ImageNameBase,
				card.ImageName,
				card.Artist,
				card.IsToken);

			var result = model?.ApplyRotation(card, zoom: false);
			return result;
		}

		private static ImageFile getImage(
			Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> modelsByNameBySetByVariant,
			Func<string, string, string> setCodePreference,
			string set,
			string imageNameBase,
			string imageName,
			string artist,
			bool isToken)
		{
			lock (modelsByNameBySetByVariant)
			{
				if (!modelsByNameBySetByVariant.TryGetValue(imageNameBase, out var bySet))
					return null;

				if (set == null || !bySet.TryGetValue(set, out var byImageVariant))
				{
					byImageVariant = bySet
						.AtMax(setPriority2(isToken))
						.ThenAtMax(setPriority3(artist))
						.ThenAtMax(setPriority4(setCodePreference, set))
						.Find()
						.Value;
				}

				var model = byImageVariant.Values
					.AtMax(cardPriority1(set, isToken))
					.ThenAtMax(cardPriority2(set, imageName))
					.ThenAtMax(cardPriority3(set, artist))
					.ThenAtMax(cardPriority4(isToken))
					.ThenAtMax(cardPriority5(artist))
					.ThenAtMax(cardPriority6(imageName))
					.ThenAtMin(cardUnpriority7())
					.Find();

				return model;
			}
		}

		public IReadOnlyList<ImageModel> GetZooms(Card card, Func<string, string, string> setCodePreference)
		{
			IReadOnlyList<ImageModel> result;

			if (IsLoadingZoomComplete.Signaled)
			{
				result = getImageModels(card, setCodePreference, _modelsByNameBySetByVariantZoom);

				if (result != null)
					return result;
			}

			result = getImageModels(card, setCodePreference, _modelsByNameBySetByVariant);
			return result;
		}

		public IReadOnlyList<ImageModel> GetArts(Card card, Func<string, string, string> setCodePreference)
		{
			if (!IsLoadingArtComplete.Signaled)
				return null;

			var models = getImageModels(card, setCodePreference, _modelsByNameBySetByVariantArt);

			var distinctModels = models?.GroupBy(_ => _.ImageFile.FullPath)
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

		private static IReadOnlyList<ImageModel> getImageModels(
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
					.ThenByDescending(setPriority2(card.IsToken))
					.ThenByDescending(setPriority3(card.Artist))
					.ThenByDescending(setPriority4(setCodePreference, card.SetCode))
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
				.OrderByDescending(cardPriority1(card.SetCode, card.IsToken))
				.ThenByDescending(cardPriority2(card.SetCode, card.ImageName))
				.ThenByDescending(cardPriority3(card.SetCode, card.Artist))
				.ThenByDescending(cardPriority4(card.IsToken))
				.ThenByDescending(cardPriority5(card.Artist))
				.ThenByDescending(cardPriority6(card.ImageName))
				.ThenBy(cardUnpriority7())
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
		/// Then sets that have the card with matching IsToken flag
		/// </summary>
		private static Func<KeyValuePair<string, Dictionary<int, ImageFile>>, bool> setPriority2(bool isToken)
		{
			return bySet => bySet.Value.Values.Any(_ => _.IsToken == isToken);
		}

		/// <summary>
		/// Then the sets that have a variant with matching artist
		/// </summary>
		private static Func<KeyValuePair<string, Dictionary<int, ImageFile>>, bool> setPriority3(string artist)
		{
			return bySet => artist != null && bySet.Value.Values.Any(_ => Str.Equals(_.Artist, artist));
		}

		/// <summary>
		/// Other sets go from new to old
		/// </summary>
		private static Func<KeyValuePair<string, Dictionary<int, ImageFile>>, string> setPriority4(Func<string, string, string> setCodePreference, string set)
		{
			return bySet => setCodePreference(set, bySet.Key);
		}

		/// <summary>
		/// In matching set let's show first the card with matching variant number
		/// </summary>
		private static Func<ImageFile, bool> cardPriority2(string set, string imageName)
		{
			return model =>
				Str.Equals(model.SetCode, set) &&
				Str.Equals(model.ImageName, imageName);
		}

		/// <summary>
		/// In matching set let's show first the card with matching IsToken flag
		/// </summary>
		private static Func<ImageFile, bool> cardPriority1(string set, bool isToken)
		{
			return model =>
				Str.Equals(model.SetCode, set) &&
				model.IsToken == isToken;
		}

		/// <summary>
		/// then the card with matching artist
		/// </summary>
		private static Func<ImageFile, bool> cardPriority3(string set, string artist)
		{
			return model =>
				Str.Equals(model.SetCode, set) &&
				Str.Equals(model.Artist, artist);
		}

		/// <summary>
		/// In other sets first the card with matching IsToken flag
		/// </summary>
		private static Func<ImageFile, bool> cardPriority4(bool isToken)
		{
			return model => model.IsToken == isToken;
		}

		/// <summary>
		/// Then matching artist
		/// </summary>
		private static Func<ImageFile, bool> cardPriority5(string artist)
		{
			return model => Str.Equals(model.Artist, artist);
		}

		/// <summary>
		/// Then matching number
		/// </summary>
		private static Func<ImageFile, bool> cardPriority6(string imageName)
		{
			return model => Str.Equals(model.ImageName, imageName);
		}

		/// <summary>
		/// Others by variant number ascending
		/// </summary>
		/// <returns></returns>
		private static Func<ImageFile, int> cardUnpriority7()
		{
			return model => model.VariantNumber;
		}



		public AsyncSignal IsLoadingSmallComplete { get; } = new AsyncSignal(multiple: true);
		public AsyncSignal IsLoadingZoomComplete { get; } = new AsyncSignal(multiple: true);
		public AsyncSignal IsLoadingArtComplete { get; } = new AsyncSignal(multiple: true);

		private AsyncSignal IsLoadingSmallFileComplete { get; } = new AsyncSignal(multiple: true);
		private AsyncSignal IsLoadingZoomFileComplete { get; } = new AsyncSignal(multiple: true);
		private AsyncSignal IsLoadingArtFileComplete { get; } = new AsyncSignal(multiple: true);


		private HashSet<FsPath> _files;
		private HashSet<FsPath> _filesZoom;
		private HashSet<FsPath> _filesArt;

		private IList<DirectoryConfig> _directories;
		private IList<DirectoryConfig> _directoriesZoom;
		private IList<DirectoryConfig> _directoriesArt;

		private Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> _modelsByNameBySetByVariant;
		private Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> _modelsByNameBySetByVariantZoom;
		private Dictionary<string, Dictionary<string, Dictionary<int, ImageFile>>> _modelsByNameBySetByVariantArt;



		private static readonly HashSet<string> _extensions = new HashSet<string>(Str.Comparer) { ".jpg", ".png" };
		private static readonly Regex _metadataRegex = new Regex(
			@"\[(?<field>set|artist) (?:(?<name>[^\];,]+)?[;,]?)+\]",
			RegexOptions.IgnoreCase);

		private readonly ImageLocationsConfig _config;
		private readonly IShell _shell;
	}
}
