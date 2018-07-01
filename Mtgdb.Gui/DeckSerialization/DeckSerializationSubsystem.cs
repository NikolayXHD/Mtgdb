using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class DeckSerializationSubsystem
	{
		[UsedImplicitly]
		public DeckSerializationSubsystem(
			CardRepository cardRepository,
			ForgeSetRepository forgeSetRepo)
		{
			_formatters = new IDeckFormatter[]
			{
				new JsonDeckFormatter(),
				new ForgeDeckFormatter(cardRepository, forgeSetRepo),
				new MagarenaDeckFormatter(cardRepository),
				new DeckedBuilderDeckFormatter(cardRepository),
				new XMageDeckFormatter(cardRepository),
				new MtgoDeckFormatter(cardRepository)
			};

			string anyFormatFilter = $"Any {{type}}|{string.Join(@";", _formatters.Where(_ => _.SupportsImport).Select(f => f.FileNamePattern).Distinct())}";

			_loadFilter = string.Join(@"|", Sequence.From(anyFormatFilter).Concat(
				_formatters.Where(_ => _.SupportsImport).Select(f => $"{f.Description}|{f.FileNamePattern}")));

			_saveFilter = string.Join(@"|",
				_formatters.Where(_ => _.SupportsExport).Select(f => $"{f.Description}|{f.FileNamePattern}"));

			Directory.CreateDirectory(AppDir.Save);
		}



		public Deck SaveDeck(Deck deck)
		{
			return save(deck, "deck");
		}

		public Deck SaveCollection(Deck collection)
		{
			return save(collection, "collection");
		}

		private Deck save(Deck deck, string fileType)
		{
			var fileToSave = selectFileToSave(deck.Name, fileType);

			if (fileToSave == null)
				return null;

			deck.File = fileToSave.File;
			if (deck.Name == null)
				deck.Name = Path.GetFileNameWithoutExtension(fileToSave.File);

			var name = Path.GetFileNameWithoutExtension(fileToSave.File);
			var formatter = _formatters[fileToSave.FormatIndex];

			string serialized = formatter.ExportDeck(name, deck);

			try
			{
				if (formatter.UseBom)
				{
					var preamble = Encoding.UTF8.GetPreamble();
					var content = Encoding.UTF8.GetBytes(serialized);
					var bytes = preamble.Concat(content).ToArray();

					File.WriteAllBytes(fileToSave.File, bytes);
				}
				else
				{
					File.WriteAllText(fileToSave.File, serialized);
				}
			}
			catch (IOException ex)
			{
				deck.Error = ex.Message;
			}

			return deck;
		}

		public Deck LoadCollection()
		{
			var file = selectFilesToOpen("collection", allowMultiple: false).SingleOrDefault();
			return file?.Invoke(Deserialize);
		}

		public Deck Deserialize(string file)
		{
			State.LastLoadedFile = file;

			Deck deck = Deck.Create();
			deck.File = file;

			int maxLen = 0x8000000; // 128 MB
			long length = new FileInfo(file).Length;
			if (length > maxLen)
			{
				deck.Error = $"File size {length} bytes exceeds maximum of {maxLen} bytes";
				return deck;
			}

			string serialized;

			try
			{
				serialized = File.ReadAllText(file);
			}
			catch (IOException ex)
			{
				deck.Error = ex.Message;
				return deck;
			}
			
			var format = @"*" + Path.GetExtension(file);

			var formatter = getFormatter(format, serialized);

			if (formatter == null)
			{
				deck.Error = "Deck format is not supported";
				return deck;
			}

			deck = LoadSerialized(format, serialized);

			deck.File = file;

			if (deck.Name == null)
				deck.Name = Path.GetFileNameWithoutExtension(file);

			return deck;
		}

		private IDeckFormatter getFormatter(string format, string serialized)
		{
			var formatter = _formatters.FirstOrDefault(f =>
				Str.Equals(f.FileNamePattern, format) &&
				f.ValidateFormat(serialized)
			);

			return formatter;
		}

		public string SaveSerialized(string format, Deck deck)
		{
			var formatter = _formatters.First(f => Str.Equals(f.FileNamePattern, format) && f.SupportsExport);
			var result = formatter.ExportDeck(deck.Name, deck);
			return result;
		}

		public Deck LoadSerialized(string format, string serialized)
		{
			var formatter = getFormatter(format, serialized);

			Deck deck;
			if (formatter == null)
			{
				deck = Deck.Create();

				deck.Error = "Deck format is not supported";

				var hint = _formatters.Where(f => Str.Equals(f.FileNamePattern, format) && f.SupportsImport)
					.Select(f => f.FormatHint)
					.FirstOrDefault();

				if (hint != null)
					deck.Error += Str.Endl + hint;

				return deck;
			}

			deck = formatter.ImportDeck(serialized);
			return deck;
		}

		private string LastSavedDirectory => getDir(State.LastSavedFile);

		private string LastLoadedDirectory => getDir(State.LastLoadedFile);



		private string LastSavedExtension => Path.GetExtension(State.LastSavedFile);

		private string LastLoadedExtension => Path.GetExtension(State.LastLoadedFile);



		private int SaveFilterIndex => getIndex(LastSavedExtension, _saveFilter);

		private int LoadFilterIndex => getIndex(LastLoadedExtension, _loadFilter);

		private DeckFile selectFileToSave(string name, string fileType)
		{
			var dlg = new SaveFileDialog
			{
				InitialDirectory = LastSavedDirectory ?? new DirectoryInfo(AppDir.Save).FullName,
				Filter = _saveFilter.Replace("{type}", fileType),
				AddExtension = true,
				FilterIndex = SaveFilterIndex,
				Title = @"Select a file to save " + fileType,
				FileName =  name.Non(string.Empty)
			};

			if (dlg.ShowDialog() != DialogResult.OK)
				return null;

			State.LastSavedFile = dlg.FileName;
			return new DeckFile(dlg.FileName, dlg.FilterIndex - 1);
		}

		public string[] SelectDeckFiles() =>
			selectFilesToOpen("deck", allowMultiple: true);

		private string[] selectFilesToOpen(string fileType, bool allowMultiple)
		{
			var dlg = new OpenFileDialog
			{
				InitialDirectory = LastLoadedDirectory ?? new DirectoryInfo(AppDir.Save).FullName,
				Filter = _loadFilter.Replace("{type}", fileType),
				AddExtension = true,
				FilterIndex = LoadFilterIndex,
				Title = $"Select {(allowMultiple ? "one or more" : "a")} file to load " + fileType,
				Multiselect = allowMultiple
			};

			if (dlg.ShowDialog() != DialogResult.OK)
				return null;

			return dlg.FileNames;
		}



		private static string getDir(string lastFile)
		{
			if (lastFile == null)
				return null;

			var result = Path.GetDirectoryName(lastFile);

			if (!Directory.Exists(result))
				return null;

			return result;
		}

		private static int getIndex(string extension, string filter)
		{
			if (extension == null)
				return 0;

			string filterPart = @"*" + extension.ToLower(Str.Culture);
			var filterParts = filter.Split('|').Select(_=>_.ToLower(Str.Culture)).ToArray();

			var index = filterParts.IndexOf(filterPart);

			return 1 + index / 2;
		}


		public string GetShortDisplayName(string deckName)
		{
			if (deckName == null)
				return NoDeck;

			const int maxLength = 30;

			if (deckName.Length > maxLength)
				return $"…{deckName.Substring(deckName.Length - maxLength)}";

			return deckName;
		}


		private readonly string _loadFilter;
		private readonly string _saveFilter;

		private readonly IDeckFormatter[] _formatters;
		public FileDialogState State { get; } = new FileDialogState();

		/// <summary>
		/// Used instead of empty string or null to avoid weird .NET call to RecreateHandle()
		/// when setting form text to null or empty
		/// </summary>
		public const string NoDeck = " ";
	}
}