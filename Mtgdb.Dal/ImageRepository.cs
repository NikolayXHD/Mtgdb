using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinqLib.Sequence;
using Shell32;

namespace Mtgdb.Dal
{
	public class ImageRepository
	{
		private readonly Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> _modelsByNameBySetByVariant =
			new Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>>(Str.Comparer);

		private readonly Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> _modelsByNameBySetByVariantZoom =
			new Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>>(Str.Comparer);

		private readonly Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> _modelsByNameBySetByVariantArt =
			new Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>>(Str.Comparer);

		public bool IsLoadingComplete { get; private set; }
		public bool IsLoadingZoomComplete { get; private set; }
		public bool IsLoadingArtComplete { get; private set; }

		public bool IsFileLoadingComplete { get; private set; }
		
		public event Action LoadingComplete;

		private readonly List<string> _files = new List<string>();
		private readonly List<string> _filesZoom = new List<string>();
		private readonly List<string> _filesArt = new List<string>();

		private readonly IList<string> _directories;
		private readonly IList<string> _directoriesZoom;
		private readonly IList<string> _directoriesArt;
		private readonly HashSet<string> _directoriesArtAttributes;

		private static readonly string[] _extensions = { "*.jpg", "*.png" };

		private static readonly string[] _metadataSeparator = { "][" };
		private const char MetadataBegin = '[';
		private const char MetadataEnd = ']';
		private static readonly char[] _metadataValueSeparator = { ',', ';' };

		public ImageRepository(ImageLocationsConfig config)
		{
			_directories = getFolders(config.Directories.Where(_ => _.Zoom != true && _.Art != true));
			_directoriesZoom = getFolders(config.Directories.Where(_ => _.Zoom == true));
			_directoriesArt = getFolders(config.Directories.Where(_ => _.Art == true));

			_directoriesArtAttributes = new HashSet<string>(
				getFolders(config.Directories.Where(_ => _.Art == true && _.ReadMetadataFromAttributes == true)));
		}

		private static string[] getFolders(IEnumerable<DirectoryConfig> directoryConfigs)
		{
			return directoryConfigs
				.Select(_ => _.Path)
				.Select(AppDir.GetRootPath)
				.Where(Directory.Exists)
				// связь с другим местом use_dir_sorting_to_find_most_nested_root
				.OrderByDescending(_ => _.Length)
				.ToArray();
		}

		public void LoadFiles()
		{
			loadFiles(_directories, _files);
			loadFiles(_directoriesZoom, _filesZoom);
			loadFiles(_directoriesArt, _filesArt);
			IsFileLoadingComplete = true;
		}

		private static void loadFiles(IList<string> directories, ICollection<string> files)
		{
			foreach (var directory in directories)
				foreach (string extension in _extensions)
					foreach (string file in Directory.EnumerateFiles(directory, extension, SearchOption.AllDirectories))
						files.Add(file);
		}

		public void Load()
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

		private void load(
			Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> modelsByNameBySetByVariant,
			IList<string> directories,
			IEnumerable<string> files,
			bool isArt = false)
		{
			Shell shl = null;
			foreach (var entryByDirectory in files.GroupBy(Path.GetDirectoryName))
			{
				// связь с другим местом use_dir_sorting_to_find_most_nested_root
				string root = directories.First(_ => entryByDirectory.Key.StartsWith(_));
				bool readAttributes = _directoriesArtAttributes.Contains(root);

				Folder dir = null;
				if (isArt && readAttributes)
				{
					shl = shl ?? new Shell();
					dir = shl.NameSpace(entryByDirectory.Key);
				}

				foreach (string file in entryByDirectory)
				{
					IList<string> authors = null;
					IList<string> keywords = null;
					if (isArt)
					{
						string fileName = Path.GetFileName(file);
						getMetadataFromName(fileName, ref authors, ref keywords);

						if (readAttributes)
							getMetadataFromAttributes(dir, fileName, ref authors, ref keywords);
					}

					authors = notNullOrEmpty(authors);
					keywords = notNullOrEmpty(keywords);

					foreach (string author in authors)
						foreach (string setCode in keywords)
						{
							var model = new ImageModel(file, root, setCode, author, isArt);

							if (model.IsCrop)
								continue;

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


		public IEnumerable<ImageModel> GetAllImageModels()
		{
			lock (_modelsByNameBySetByVariant)
				foreach (var bySet in _modelsByNameBySetByVariant.Values)
					foreach (var byVariant in bySet.Values)
						foreach (var model in byVariant.Values)
							yield return model;
		}


		public ImageModel GetImageSmall(Card card, Func<string, string> setCodePreference)
		{
			return getImage(card, setCodePreference, _modelsByNameBySetByVariant);
		}

		public ImageModel GetImagePrint(Card card, Func<string, string> setCodePreference)
		{
			return
				getImage(card, setCodePreference, _modelsByNameBySetByVariantZoom) ??
				getImage(card, setCodePreference, _modelsByNameBySetByVariant);
		}

		private static ImageModel getImage(
			Card card,
			Func<string, string> setCodePreference,
			Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> modelsByNameBySetByVariant)
		{
			lock (modelsByNameBySetByVariant)
			{
				string imageName = card.ImageName;

				Dictionary<string, Dictionary<int, ImageModel>> bySet;
				if (!modelsByNameBySetByVariant.TryGetValue(card.ImageNameBase, out bySet))
					return null;

				Dictionary<int, ImageModel> byImageVariant;
				if (card.SetCode == null || !bySet.TryGetValue(card.SetCode, out byImageVariant))
					byImageVariant = bySet.ElementAtMax(pair => setCodePreference.Invoke(pair.Key)).Value;

				var model = byImageVariant.ElementAtMax(pair => getMatchRank(pair.Value, imageName)).Value;
				return model;
			}
		}

		private static int getMatchRank(ImageModel model, string imageName)
		{
			bool imageNameIsMatching = Str.Equals(model.ImageName, imageName);
			int variantNumber = model.VariantNumber;
			var quality = model.Quality;

			return -variantNumber + 100*quality + 10000*Convert.ToInt32(imageNameIsMatching);
		}

		public List<ImageModel> GetImagesZoom(Card card, Func<string, string> setCodePreference)
		{
			var models = getSpecificModels(card,
				setCodePreference,
				IsLoadingZoomComplete,
				_modelsByNameBySetByVariantZoom,
				_modelsByNameBySetByVariant);

			return models;
		}

		public List<ImageModel> GetImagesArt(Card card, Func<string, string> setCodePreference)
		{
			var models = getImageModels(card, setCodePreference, _modelsByNameBySetByVariantArt);
			var distinctModels = models?.GroupBy(_ => _.FullPath).Select(_ => _.First()).ToList();
			return distinctModels;
		}

		public IEnumerable<ImageModel> GetAllImagesArt()
		{
			foreach (var bySet in _modelsByNameBySetByVariantArt.Values)
				foreach (var byVariant in bySet.Values)
					foreach (var model in byVariant.Values)
						yield return model;
		}

		private static List<ImageModel> getSpecificModels(
			Card card,
			Func<string, string> setCodePreference,
			bool isLoadingSpecificComplete,
			Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> modelsSpecific,
			Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> modelsFallback)
		{
			List<ImageModel> models;

			if (isLoadingSpecificComplete)
			{
				models =
					getImageModels(card, setCodePreference, modelsSpecific) ??
					getImageModels(card, setCodePreference, modelsFallback);
			}
			else
				models = getImageModels(card, setCodePreference, modelsFallback);

			return models;
		}

		private static List<ImageModel> getImageModels(
			Card card,
			Func<string, string> setCodePreference,
			Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> modelsByNameBySetByVariant)
		{
			lock (modelsByNameBySetByVariant)
			{
				Dictionary<string, Dictionary<int, ImageModel>> modelsBySet;
				if (!modelsByNameBySetByVariant.TryGetValue(card.ImageNameBase, out modelsBySet))
					return null;

				var models = modelsBySet
					// Сначала совпадающий сет
					.OrderByDescending(bySet => Str.Equals(bySet.Key, card.SetCode))
					// Далее сеты, в которых есть вариант с совпадающим артистом
					.ThenByDescending(bySet => card.Artist != null && bySet.Value.Values.Any(_ => Str.Equals(_.Artist, card.Artist)))
					// Остальные сеты пойдут от новых к старым
					.ThenByDescending(bySet => setCodePreference(bySet.Key))
					.SelectMany(bySet =>
						bySet.Value.Select(byVariant => byVariant.Value)
							// В совпадающем сете покажем 
							// * сначала карту с совпадающим номером варианта
							.OrderByDescending(model =>
								Str.Equals(model.SetCode, card.SetCode) &&
								Str.Equals(model.ImageName, card.ImageName))
							// * далее карту с совпадающим артистом
							.ThenByDescending(model =>
								Str.Equals(model.SetCode, card.SetCode) &&
								Str.Equals(model.Artist, card.Artist))
							// В остальных сетах 
							// * сначала карту с совпадающим артистом
							.ThenByDescending(model => Str.Equals(model.Artist, card.Artist))
							// * прочие по возрастанию номера варианта
							.ThenBy(model => model.VariantNumber))
					.ToList();

				if (models.Count > 0)
					return models;

				return null;
			}
		}


		public ImageModel GetReplacementImage(ImageModel model, Func<string, string> setCodePreference)
		{
			if (string.IsNullOrEmpty(model.Name))
				return null;

			var result =
				getReplacementImage(model, setCodePreference, _modelsByNameBySetByVariantZoom) ??
				getReplacementImage(model, setCodePreference, _modelsByNameBySetByVariant);

			return result;
		}

		private static ImageModel getReplacementImage(
			ImageModel modelOriginal,
			Func<string, string> setCodePreference,
			Dictionary<string, Dictionary<string, Dictionary<int, ImageModel>>> modelsByNameBySetByVariant)
		{
			string setCode = modelOriginal.SetCode;
			string name = modelOriginal.Name;
			int variant = modelOriginal.VariantNumber;

			lock (modelsByNameBySetByVariant)
			{
				Dictionary<string, Dictionary<int, ImageModel>> bySet;
				if (!modelsByNameBySetByVariant.TryGetValue(name, out bySet))
					return null;

				Dictionary<int, ImageModel> byImageVariant;
				if (setCode == null || !bySet.TryGetValue(setCode, out byImageVariant))
					byImageVariant = bySet.ElementAtMax(pair => setCodePreference.Invoke(pair.Key)).Value;

				ImageModel model;
				if (!byImageVariant.TryGetValue(variant, out model))
					model = byImageVariant.ElementAtMax(pair => -pair.Key + pair.Value.Quality*1000).Value;

				return model;
			}
		}
	}
}