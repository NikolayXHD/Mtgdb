using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class DeckSerializationSubsystem
	{
		public DeckSerializationSubsystem(
			CardRepository cardRepository,
			ForgeSetRepository forgeSetRepo)
		{
			_formatters = new IDeckFormatter[]
			{
				new JsonDeckFormatter(),
				new ForgeDeckFormatter(cardRepository, forgeSetRepo),
				new MagarenaDeckFormatter(cardRepository),
				new XMageDeckFormatter(cardRepository),
				new MtgoDeckFormatter(cardRepository)
			};

			string anyFormatFilter = $"Any {{type}}|{string.Join(@";", _formatters.Where(_ => _.SupportsImport).Select(f => f.FileNamePattern).Distinct())}";

			_loadFilter = string.Join(@"|", Enumerable.Repeat(anyFormatFilter, 1).Concat(
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

		public Deck LoadDeck()
		{
			return load("deck");
		}

		public Deck LoadCollection()
		{
			return load("collection");
		}

		private Deck load(string fileType)
		{
			var fileToOpen = selectFileToOpen(fileType);

			if (fileToOpen == null)
				return null;

			var deck = Load(fileToOpen);
			return deck;
		}

		public Deck Load(string file)
		{
			LastLoadedFile = file;

			Deck deck = Deck.Create();
			deck.File = file;

			int maxLen = 0x8000000; // 128 MB
			if (new FileInfo(file).Length > maxLen)
			{
				deck.Error = "Deck file is too large";
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

			var formatter = _formatters.FirstOrDefault(f =>
				Str.Equals(f.FileNamePattern, format) &&
				f.ValidateFormat(serialized)
			);

			if (formatter == null)
			{
				deck.Error = "Deck format is not supported";
				return deck;
			}

			deck = formatter.ImportDeck(serialized);
			deck.File = file;

			if (deck.Name == null)
				deck.Name = Path.GetFileNameWithoutExtension(file);

			return deck;
		}


		public string LastFile { get; set; }

		private string LastSavedFile
		{
			get { return _lastSavedFile ?? LastFile; }

			set
			{
				_lastSavedFile = value;
				LastFile = value;
			}
		}

		private string LastLoadedFile
		{
			get { return _lastLoadedFile ?? LastFile; }

			set
			{
				_lastLoadedFile = value;
				LastFile = value;
			}
		}

		private string LastSavedDirectory => getDir(LastSavedFile);

		private string LastLoadedDirectory => getDir(LastLoadedFile);



		private string LastSavedExtension => Path.GetExtension(LastSavedFile);

		private string LastLoadedExtension => Path.GetExtension(_lastLoadedFile);



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
				FileName =  name.NonEmpty()
			};

			if (dlg.ShowDialog() != DialogResult.OK)
				return null;

			LastSavedFile = dlg.FileName;
			return new DeckFile(dlg.FileName, dlg.FilterIndex - 1);
		}

		private string selectFileToOpen(string fileType)
		{
			var dlg = new OpenFileDialog
			{
				InitialDirectory = LastLoadedDirectory ?? new DirectoryInfo(AppDir.Save).FullName,
				Filter = _loadFilter.Replace("{type}", fileType),
				AddExtension = true,
				FilterIndex = LoadFilterIndex,
				Title = @"Select a file to load " + fileType
			};

			if (dlg.ShowDialog() != DialogResult.OK)
				return null;

			return dlg.FileName;
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

			string filterPart = @"*" + extension.ToLowerInvariant();
			var filterParts = filter.Split('|').Select(_=>_.ToLowerInvariant()).ToArray();

			var index = Array.IndexOf(filterParts, filterPart);

			return 1 + index / 2;
		}



		private readonly string _loadFilter;
		private readonly string _saveFilter;

		private string _lastSavedFile;
		private string _lastLoadedFile;

		private readonly IDeckFormatter[] _formatters;
	}
}