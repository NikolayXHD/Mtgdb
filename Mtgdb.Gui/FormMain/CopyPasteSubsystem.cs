using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Ui;
using NLog;

namespace Mtgdb.Gui
{
	public class CopyPasteSubsystem
	{
		public CopyPasteSubsystem(
			CardRepository cardRepo,
			DeckSerializationSubsystem serialization,
			CollectionEditorModel collection,
			DeckEditorModel deckEditor,
			FormMain targetForm,
			DeckListControl deckListControl,
			params Control[] targetControls)
		{
			_cardRepo = cardRepo;
			_serialization = serialization;
			_collection = collection;
			_deckEditor = deckEditor;
			_targetForm = targetForm;
			_deckListControl = deckListControl;
			_targetControls = targetControls;
		}

		public void SubscribeToEvents()
		{
			foreach (var control in _targetControls)
			{
				control.AllowDrop = true;
				control.DragEnter += deckDragEnter;
				control.DragDrop += deckDragDropped;
			}
		}

		public void UnsubscribeFromEvents()
		{
			foreach (var control in _targetControls)
			{
				control.DragEnter -= deckDragEnter;
				control.DragDrop -= deckDragDropped;
			}
		}

		private static void deckDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetFormats().Contains(DataFormats.Text))
				e.Effect = DragDropEffects.Copy;
		}

		private void deckDragDropped(object sender, DragEventArgs e)
		{
			if (_cardRepo.IsLoadingComplete)
			{
				dragDropped(e.Data);
				return;
			}

			_cardRepo.LoadingComplete += () => { _targetForm.Invoke(delegate { dragDropped(e.Data); }); };

			MessageBox.Show(_targetForm,
				"Mtgdb.Gui is loading cards.\r\n" +
				"When completed, the deck(s) will be opened.",
				"Opening deck(s) delayed",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);
		}

		private void dragDropped(IDataObject dragData)
		{
			if (dragData.GetDataPresent(DataFormats.FileDrop))
			{
				var files = (string[]) dragData.GetData(DataFormats.FileDrop);
				PasteDecks(files);
			}
			else if (dragData.GetFormats().Contains(DataFormats.Text))
			{
				string text = (string) dragData.GetData(DataFormats.Text, autoConvert: true);

				if (Control.ModifierKeys == Keys.Alt)
					pasteCollectionFromText(text, append: false);
				else if (Control.ModifierKeys == (Keys.Alt | Keys.Shift))
					pasteCollectionFromText(text, append: true);
				else if (Control.ModifierKeys == Keys.None)
					pasteDeckFromText(text, append: false);
				else if (Control.ModifierKeys == Keys.Shift)
					pasteDeckFromText(text, append: true);
			}
		}

		public void PasteDecks(string[] files)
		{
			bool tryGetFile(int i, out (string File, string Dir) file)
			{
				lock (_sync)
				{
					if (i == _filesToLoad.Count)
					{
						_filesToLoad.Clear();
						_filesToLoadDistinct.Clear();

						file = (null, null);
						return false;
					}

					file = _filesToLoad[i];
				}

				return true;
			}

			bool wereAddedToCurrentLoadingProcess(string[] toAdd)
			{
				lock (_sync)
				{
					bool inProgress = _filesToLoad.Count > 0;

					foreach (string path in toAdd)
					{
						IEnumerable<(string File, string Dir)> actualFiles;

						if (File.Exists(path))
							actualFiles = Sequence.From<(string File, string Dir)>((path, null));
						else if (Directory.Exists(path))
							actualFiles = _serialization.ImportedFilePatterns
								.SelectMany(pattern => Directory.GetFiles(path, pattern))
								.Select(file => (file, path));
						else
							actualFiles = Empty<(string File, string Dir)>.Sequence;

						foreach (var file in actualFiles)
						{
							if (_filesToLoadDistinct.Add(file))
								_filesToLoad.Add(file);
						}
					}

					if (inProgress)
						_deckListControl.ContinueLoadingDecks(_filesToLoad.Count);
					else
						_deckListControl.BeginLoadingDecks(_filesToLoad.Count);

					return inProgress;
				}
			}

			if (wereAddedToCurrentLoadingProcess(files))
				return;

			ThreadPool.QueueUserWorkItem(_ =>
			{
				var failedDecks = new List<Deck>();

				bool openedFirst = false;

				int i = 0;

				while (true)
				{
					if (_abort)
						break;

					if (!tryGetFile(i, out var file))
						break;

					var deck = _serialization.Deserialize(file.File, file.Dir);

					if (deck.Error != null)
						failedDecks.Add(deck);
					else
					{
						if (!openedFirst)
						{
							openedFirst = true;
							_targetForm.Invoke(delegate { _targetForm.LoadDeck(deck); });
						}

						_deckListControl.AddDeck(deck);
					}

					i++;
				}

				_targetForm.Invoke(delegate
				{
					_deckListControl.EndLoadingDecks();

					if (failedDecks.Count == 0)
						return;

					var message = string.Join(Str.Endl,
						failedDecks.Select(f => $"{f.File}{Str.Endl}{f.Error}{Str.Endl}"));

					_log.Error($"Errors loading decks: {message}");
					MessageBox.Show($@"Failed to load {failedDecks.Count} of {i} decks. See \logs\error.log for details");
				});
			});
		}

		public void Abort() =>
			_abort = true;

		private void pasteCollectionFromText(string text, bool append)
		{
			var deck = _serialization.LoadSerialized("*.txt", text);

			if (deck.Error != null)
				MessageBox.Show(deck.Error);
			else
				_collection.LoadCollection(deck, append);
		}

		private void pasteDeckFromText(string text, bool append)
		{
			var deck = _serialization.LoadSerialized("*.txt", text);

			if (deck.Error != null)
				MessageBox.Show(deck.Error);
			else
				_deckEditor.Paste(deck, append, _cardRepo);
		}

		public void PasteDeck(bool append)
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			var text = Clipboard.GetText();
			if (string.IsNullOrWhiteSpace(text))
				return;

			pasteDeckFromText(text, append);
		}

		public void PasteCollection(bool append)
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			var text = Clipboard.GetText();
			if (string.IsNullOrWhiteSpace(text))
				return;

			pasteCollectionFromText(text, append);
		}

		public void CopyCollection()
		{
			var deck = Deck.Create(
				_collection.CountById?.ToDictionary(),
				_collection.CountById?.Keys.OrderBy(_ => _cardRepo.CardsById[_].NameEn).ToList(),
				null,
				null);

			var serialized = _serialization.SaveSerialized("*.txt", deck);
			Clipboard.SetText(serialized);
		}

		public void CopyDeck()
		{
			Deck deck;

			switch (_targetForm.DeckZone)
			{
				case Zone.Main:
				case null when _targetForm.IsDeckListSelected:
					deck = GetDeckCopy();
					break;
				case Zone.Side:
					deck = copySideDeck();
					break;
				case Zone.SampleHand:
					deck = copySampleHand();
					break;
				default:
					return;
			}

			var serialized = _serialization.SaveSerialized("*.txt", deck);
			Clipboard.SetText(serialized);
		}

		private Deck copySampleHand() =>
			Deck.Create(
				_deckEditor.SampleHand.CountById.ToDictionary(),
				_deckEditor.SampleHand.CardsIds.ToList(),
				null,
				null);

		private Deck copySideDeck() =>
			Deck.Create(
				_deckEditor.SideDeck.CountById.ToDictionary(),
				_deckEditor.SideDeck.CardsIds.ToList(),
				null,
				null);

		public Deck GetDeckCopy()
		{
			var result = Deck.Create(
				_deckEditor.MainDeck.CountById.ToDictionary(),
				_deckEditor.MainDeck.CardsIds.ToList(),
				_deckEditor.SideDeck.CountById.ToDictionary(),
				_deckEditor.SideDeck.CardsIds.ToList());

			result.Name = _deckEditor.DeckName;
			result.File = _deckEditor.DeckFile;

			return result;
		}

		public void LoadDeck(Deck deck)
		{
			_deckEditor.DeckFile = deck.File;
			_deckEditor.DeckName = deck.Name;

			_deckEditor.SetDeck(deck, _cardRepo);
			_deckEditor.Shuffle();
		}


		private bool _abort;

		private readonly HashSet<(string File, string Dir)> _filesToLoadDistinct = new HashSet<(string, string)>();
		private readonly List<(string File, string Dir)> _filesToLoad = new List<(string, string)>();

		private readonly object _sync = new object();

		private readonly CardRepository _cardRepo;
		private readonly DeckSerializationSubsystem _serialization;
		private readonly CollectionEditorModel _collection;
		private readonly DeckEditorModel _deckEditor;
		private readonly FormMain _targetForm;
		private readonly DeckListControl _deckListControl;
		private readonly Control[] _targetControls;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}