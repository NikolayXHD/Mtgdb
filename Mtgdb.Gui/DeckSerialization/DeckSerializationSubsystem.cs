using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Dal;
using Mtgdb.Ui;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Gui
{
	public class DeckSerializationSubsystem
	{
		[UsedImplicitly]
		public DeckSerializationSubsystem(
			CardRepository cardRepository,
			ForgeSetRepository forgeSetRepo)
		{
			MtgArenaFormatter = new MtgArenaFormatter(cardRepository);
			MtgoFormatter = new MtgoDeckFormatter(cardRepository);

			_formatters = new[]
			{
				new JsonDeckFormatter(),
				new ForgeDeckFormatter(cardRepository, forgeSetRepo),
				new MagarenaDeckFormatter(cardRepository),
				new DeckedBuilderDeckFormatter(cardRepository),
				new XMageDeckFormatter(cardRepository),
				MtgArenaFormatter,
				// must be after MtgArenaFormatter for correct format resolution
				MtgoFormatter
			};

			_loadFormatters = _formatters.Where(_ => _.SupportsImport && _.SupportsFile).ToArray();
			_saveFormatters = _formatters.Where(_ => _.SupportsExport && _.SupportsFile).ToArray();

			ImportedFilePatterns = _loadFormatters
				.Where(_ => _.SupportsImport)
				.Select(f => f.FileNamePattern)
				.Distinct()
				.ToReadOnlyList();

			_loadFilter = string.Join(@"|", Sequence.From($"Any {{type}}|{string.Join(@";", ImportedFilePatterns)}").Concat(
				_loadFormatters.Select(f => $"{f.Description}|{f.FileNamePattern}")));

			_saveFilter = string.Join(@"|",
				_saveFormatters.Select(f => $"{f.Description}|{f.FileNamePattern}"));

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

		public Deck SaveCollectionBackup(Deck collection)
		{
			return save(collection, "collection");
		}

		private Deck save(Deck deck, string fileType)
		{
			var fileToSave = selectFileToSave(deck.Name, fileType);

			if (fileToSave == null)
				return null;

			return save(deck, fileToSave);
		}

		private Deck save(Deck deck, DeckFile file)
		{
			deck.File = file.File;
			if (deck.Name == null)
				deck.Name = Path.GetFileNameWithoutExtension(file.File);

			var name = Path.GetFileNameWithoutExtension(file.File);
			var formatter = _saveFormatters[file.FormatIndex];

			string serialized = formatter.ExportDeck(name, deck);

			try
			{
				if (formatter.UseBom)
				{
					var preamble = Encoding.UTF8.GetPreamble();
					var content = Encoding.UTF8.GetBytes(serialized);
					var bytes = preamble.Concat(content).ToArray();

					File.WriteAllBytes(file.File, bytes);
				}
				else
				{
					File.WriteAllText(file.File, serialized);
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
			return file?.Invoke0(f => Deserialize(f, null));
		}

		public Deck Deserialize(string file, string dir)
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
			{
				string getNestedFileName() =>
					(Path.GetFileName(dir) + Path.DirectorySeparatorChar + file.Substring(dir.Length + 1 /*separator*/))
					.Replace(new string(Path.DirectorySeparatorChar, 1), Environment.NewLine);

				var extension = Path.GetExtension(file);

				string nameBase = dir == null
					? Path.GetFileName(file)
					: getNestedFileName();

				deck.Name = nameBase.Substring(0, nameBase.Length - extension.Length);
			}

			return deck;
		}

		private IDeckFormatter getFormatter(string format, string serialized)
		{
			var formatter = _formatters.FirstOrDefault(f =>
				Str.Equals(f.FileNamePattern, format) &&
				f.
					ValidateFormat(serialized)
			);

			return formatter;
		}

		public string SaveSerialized(Deck deck, IDeckFormatter formatter = null, string format = null)
		{
			if (formatter == null && format == null)
				throw new ArgumentException($"either {nameof(format)} or {nameof(formatter)} must be specified");

			formatter = formatter ?? _formatters.First(f => Str.Equals(f.FileNamePattern, format) && f.SupportsExport);
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

				var hint = _loadFormatters.Where(f => Str.Equals(f.FileNamePattern, format))
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
			if (string.IsNullOrEmpty(deckName))
				return NoDeck;

			const int maxLength = 30;
			const int maxLines = 2;

			var lines = deckName.Split(Array.From('\r', '\n'), StringSplitOptions.RemoveEmptyEntries);
			var builder = new StringBuilder();

			for (int i = Math.Max(0, lines.Length - maxLines); i < lines.Length; i++)
			{
				var line = lines[i];

				if (line.Length <= maxLength)
					builder.AppendLine(line);
				else
					builder.AppendLine($"…{line.Substring(line.Length - maxLength)}");
			}

			return builder.ToString();
		}


		private readonly string _loadFilter;
		private readonly string _saveFilter;

		private readonly IDeckFormatter[] _formatters;
		private readonly IList<IDeckFormatter> _loadFormatters;
		private readonly IList<IDeckFormatter> _saveFormatters;
		public IReadOnlyList<string> ImportedFilePatterns { get; }
		public FileDialogState State { get; } = new FileDialogState();

		/// <summary>
		/// Used instead of empty string or null to avoid weird .NET call to RecreateHandle()
		/// when setting form text to null or empty
		/// </summary>
		public const string NoDeck = " ";

		public IDeckFormatter MtgArenaFormatter { get; }
		public IDeckFormatter MtgoFormatter { get; }
	}
}