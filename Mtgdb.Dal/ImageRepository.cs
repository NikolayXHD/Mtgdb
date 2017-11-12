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
			foreach (var directoryConfig in config.Directories)
				directoryConfig.Path = AppDir.GetRootPath(directoryConfig.Path);

			_directories = getFolders(config.Directories, ImageType.Small);
			_directoriesZoom = getFolders(config.Directories, ImageType.Zoom);
			_directoriesArt = getFolders(config.Directories, ImageType.Art);
		}

		private static DirectoryConfig[] getFolders(IList<DirectoryConfig> directoryConfigs, Func<DirectoryConfig, bool> filter)
		{
			return directoryConfigs
				.Where(c=> filter(c) && Directory.Exists(c.Path))
				// связь с другим местом use_dir_sorting_to_find_most_nested_root
				.OrderByDescending(c => c.Path.Length)
				.ToArray();
		}

		public void LoadFiles()
		{
			loadFiles(_directories, _files);
			loadFiles(_directoriesZoom, _filesZoom);
			loadFiles(_directoriesArt, _filesArt);
			IsFileLoadingComplete = true;
		}

		private static void loadFiles(IList<DirectoryConfig> directories, ICollection<string> files)
		{
			foreach (var directory in directories)
			{
				var excludes = directory.Exclude?.Split(';');

				foreach (string extension in _extensions)
					foreach (string file in Directory.EnumerateFiles(directory.Path, extension, SearchOption.AllDirectories))
					{
						if (excludes != null && excludes.Any(exclude => file.IndexOf(exclude, Str.Comparison) >= 0))
							continue;

						files.Add(file);
					}
			}
		}

		public void LoadSmall()
		{
			load(_modelsByNameBySetByVariant, _directories, _files);

			IsLoadingComplete = true;
			LoadingComplete?.Invoke();
		}

		public void LoadZoom()
		{
			load(_modelsByNameBySetByVariantZoom, _directoriesZoom, _filesZoom);
			IsLoadingZoomComplete = true;
		}

		public void LoadArt()
		{
			load(_modelsByNameBySetByVariantArt, _directoriesArt, _filesArt, isArt: true);
			IsLoadingArtComplete = true;
		}

		private static void load(
			Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> modelsByNameBySetByVariant,
			IList<DirectoryConfig> directories,
			IEnumerable<string> files,
			bool isArt = false)
		{
			Shell shl = null;
			foreach (var entryByDirectory in files.GroupBy(Path.GetDirectoryName))
			{
				// связь с другим местом use_dir_sorting_to_find_most_nested_root
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
							var model = new ImageModel(file, root.Path, setCode, author, isArt);
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
			ImageModel model,
			Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> modelsByNameBySetByVariant)
		{
			lock (modelsByNameBySetByVariant)
			{
				int separator = model.Name.IndexOfAny(new[] { '_', '»' });

				if (separator > 0 && separator < model.Name.Length - 1)
				{
					add(model, modelsByNameBySetByVariant, model.Name.Substring(0, separator));
					add(model, modelsByNameBySetByVariant, model.Name.Substring(separator + 1));
				}
				else
					add(model, modelsByNameBySetByVariant, model.Name);
			}
		}

		private static void add(ImageModel model, Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> modelsByNameBySetByVariant, string name)
		{
			if (model.IsToken)
				return;

			name = string.Intern(name);

			Dictionary<string, Dictionary<int, ImageModel>> modelsBySet;
			if (!modelsByNameBySetByVariant.TryGetValue(name, out modelsBySet))
			{
				modelsBySet = new Dictionary<string, Dictionary<int, ImageModel>>(Str.Comparer);
				modelsByNameBySetByVariant.Add(name, modelsBySet);
			}

			Dictionary<int, ImageModel> modelsByImageVariant;
			if (!modelsBySet.TryGetValue(model.SetCode, out modelsByImageVariant))
			{
				modelsByImageVariant = new Dictionary<int, ImageModel>();
				modelsBySet.Add(model.SetCode, modelsByImageVariant);
			}

			ImageModel currentModel;

			// Для каждого номера варианта выберем представителя с наилучшим качеством
			if (!modelsByImageVariant.TryGetValue(model.VariantNumber, out currentModel) || currentModel.Quality < model.Quality)
				modelsByImageVariant[model.VariantNumber] = model;
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
				getImage(card, setCodePreference, _modelsByNameBySetByVariantZoom) ??
				getImage(card, setCodePreference, _modelsByNameBySetByVariant);
		}

		private static ImageModel getImage(
			Card card,
			Func<string, string, string> setCodePreference,
			Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> modelsByNameBySetByVariant)
		{
			var model = getImage(modelsByNameBySetByVariant,
				setCodePreference,
				card.SetCode,
				card.ImageNameBase,
				card.ImageName,
				card.Artist);

			if (model == null)
				return null;

			bool isAftermath =
				Str.Equals(card.Layout, "aftermath") &&
				card.Names?.Count == 2 &&
				Str.Equals(card.NameEn, card.Names[1]);

			if (isAftermath)
				model = model.Rotate();

			return model;
		}

		private static ImageModel getImage(Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> modelsByNameBySetByVariant, Func<string, string, string> setCodePreference, string set, string imageNameBase, string imageName, string artist)
		{
			lock (modelsByNameBySetByVariant)
			{
				Dictionary<string, Dictionary<int, ImageModel>> bySet;
				if (!modelsByNameBySetByVariant.TryGetValue(imageNameBase, out bySet))
					return null;

				Dictionary<int, ImageModel> byImageVariant;

				if (set == null || !bySet.TryGetValue(set, out byImageVariant))
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
			var models = getImageModels(card, setCodePreference, _modelsByNameBySetByVariantArt);
			var distinctModels = models?.GroupBy(_ => _.FullPath).Select(_ => _.First()).ToList();
			return distinctModels;
		}

		public IEnumerable<ImageModel> GetAllArts()
		{
			foreach (var bySet in _modelsByNameBySetByVariantArt.Values)
				foreach (var byVariant in bySet.Values)
					foreach (var model in byVariant.Values)
						yield return model;
		}

		public IEnumerable<ImageModel> GetAllZooms()
		{
			foreach (var bySet in _modelsByNameBySetByVariantZoom.Values)
				foreach (var byVariant in bySet.Values)
					foreach (var model in byVariant.Values)
						yield return model;
		}

		private static List<ImageModel> getImageModels(
			Card card,
			Func<string, string, string> setCodePreference,
			Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> modelsByNameBySetByVariant)
		{
			lock (modelsByNameBySetByVariant)
			{
				Dictionary<string, Dictionary<int, ImageModel>> modelsBySet;
				if (!modelsByNameBySetByVariant.TryGetValue(card.ImageNameBase, out modelsBySet))
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

				bool isAftermath =
					Str.Equals(card.Layout, "aftermath") &&
					card.Names?.Count == 2 &&
					Str.Equals(card.NameEn, card.Names[1]);

				if (isAftermath)
					return models.Select(m => m.Rotate()).ToList();

				return models;
			}
		}

		private static IEnumerable<ImageModel> orderCardsWithinSet(Card card, Dictionary<int, ImageModel> variants)
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
		/// Сначала совпадающий сет
		/// </summary>
		private static Func<KeyValuePair<string, Dictionary<int, ImageModel>>, bool> setPriority1(string set)
		{
			return bySet => Str.Equals(bySet.Key, set);
		}

		/// <summary>
		/// Далее сеты, в которых есть вариант с совпадающим артистом
		/// </summary>
		private static Func<KeyValuePair<string, Dictionary<int, ImageModel>>, bool> setPriority2(string artist)
		{
			return bySet => artist != null && bySet.Value.Values.Any(_ => Str.Equals(_.Artist, artist));
		}

		/// <summary>
		/// Остальные сеты пойдут от новых к старым
		/// </summary>
		private static Func<KeyValuePair<string, Dictionary<int, ImageModel>>, string> setPriority3(Func<string, string, string> setCodePreference, string set)
		{
			return bySet => setCodePreference(set, bySet.Key);
		}

		/// <summary>
		/// В совпадающем сете покажем сначала карту с совпадающим номером варианта
		/// </summary>
		private static Func<ImageModel, bool> cardPriority1(string set, string imageName)
		{
			return model =>
				Str.Equals(model.SetCode, set) &&
				Str.Equals(model.ImageName, imageName);
		}

		/// <summary>
		/// далее карту с совпадающим артистом
		/// </summary>
		private static Func<ImageModel, bool> cardPriority2(string set, string artist)
		{
			return model =>
				Str.Equals(model.SetCode, set) &&
				Str.Equals(model.Artist, artist);
		}

		/// <summary>
		/// В остальных сетах сначала карту с совпадающим артистом
		/// </summary>
		private static Func<ImageModel, bool> cardPriority3(string artist)
		{
			return model => Str.Equals(model.Artist, artist);
		}

		/// <summary>
		/// Далее совпадающим номером
		/// </summary>
		private static Func<ImageModel, bool> cardPriority4(string imageName)
		{
			return model => Str.Equals(model.ImageName, imageName);
		}

		/// <summary>
		/// прочие по возрастанию номера варианта
		/// </summary>
		/// <returns></returns>
		private static Func<ImageModel, int> cardUnpriority5()
		{
			return model => model.VariantNumber;
		}



		public ImageModel GetReplacementImage(ImageModel model, Func<string, string, string> setCodePreference)
		{
			if (string.IsNullOrEmpty(model.Name))
				return null;

			var result = getImage(_modelsByNameBySetByVariantZoom,
					setCodePreference,
					model.SetCode,
					model.Name,
					model.ImageName,
					model.Artist);

			if (result != null)
				return result;

			result = getImage(_modelsByNameBySetByVariant,
				setCodePreference,
				model.SetCode,
				model.Name,
				model.ImageName,
				model.Artist);

			return result;
		}



		public bool IsLoadingComplete { get; private set; }
		public bool IsLoadingZoomComplete { get; private set; }
		public bool IsLoadingArtComplete { get; private set; }

		public bool IsFileLoadingComplete { get; private set; }

		public event Action LoadingComplete;


		private readonly HashSet<string> _files = new HashSet<string>(Str.Comparer);
		private readonly HashSet<string> _filesZoom = new HashSet<string>(Str.Comparer);
		private readonly HashSet<string> _filesArt = new HashSet<string>(Str.Comparer);

		private readonly IList<DirectoryConfig> _directories;
		private readonly IList<DirectoryConfig> _directoriesZoom;
		private readonly IList<DirectoryConfig> _directoriesArt;

		private static readonly string[] _extensions = { "*.jpg", "*.png" };

		private static readonly string[] _metadataSeparator = { "][" };
		private const char MetadataBegin = '[';
		private const char MetadataEnd = ']';
		private static readonly char[] _metadataValueSeparator = { ',', ';' };


		private readonly Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> _modelsByNameBySetByVariant =
			new Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>>(Str.Comparer);

		private readonly Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> _modelsByNameBySetByVariantZoom =
			new Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>>(Str.Comparer);

		private readonly Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> _modelsByNameBySetByVariantArt =
			new Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>>(Str.Comparer);
	}
}