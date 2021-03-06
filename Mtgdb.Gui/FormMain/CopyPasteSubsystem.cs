﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Data;
using Mtgdb.Ui;
using NLog;

namespace Mtgdb.Gui
{
	public class CopyPasteSubsystem : IComponent
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
			_ctsLifetime = new CancellationTokenSource();
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
			if (_cardRepo.IsLoadingComplete.Signaled)
			{
				dragDropped(e.Data);
				return;
			}

			MessageBox.Show(_targetForm,
				"Mtgdb.Gui is loading cards.\r\n" +
				"When completed, the deck(s) will be opened.",
				"Opening deck(s) delayed",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);

			_ctsLifetime.Token
				.When(_cardRepo.IsLoadingComplete)
				.Run(() => _targetForm.Invoke(delegate { dragDropped(e.Data); }));
		}

		private void dragDropped(IDataObject dragData)
		{
			if (dragData.GetDataPresent(DataFormats.FileDrop))
			{
				var files = (string[]) dragData.GetData(DataFormats.FileDrop);
				PasteDecks(files.Select(_ => new FsPath(_)).ToArray());
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

		public void PasteDecks(FsPath[] files)
		{
			bool tryGetFile(int i, out (FsPath File, FsPath Dir) file)
			{
				lock (_sync)
				{
					if (i == _filesToLoad.Count)
					{
						_filesToLoad.Clear();
						_filesToLoadDistinct.Clear();

						file = (FsPath.Empty, FsPath.Empty);
						return false;
					}

					file = _filesToLoad[i];
				}

				return true;
			}

			(bool LoadingInProgress, bool Added) addFilesToLoad(FsPath[] toAdd)
			{
				lock (_sync)
				{
					bool inProgress = _filesToLoad.Count > 0;

					foreach (FsPath path in toAdd)
					{
						IEnumerable<(FsPath File, FsPath Dir)> actualFiles;

						if (path.IsFile())
							actualFiles = Sequence.From<(FsPath File, FsPath Dir)>((path, FsPath.Empty));
						else if (path.IsDirectory())
							actualFiles = _serialization.ImportedFilePatterns
								.SelectMany(pattern => path.EnumerateFiles(pattern, SearchOption.AllDirectories))
								.Select(file => (file, file.Parent()));
						else
							actualFiles = Empty<(FsPath File, FsPath Dir)>.Sequence;

						foreach (var file in actualFiles)
						{
							if (_filesToLoadDistinct.Add(file))
								_filesToLoad.Add(file);
						}
					}

					if (_filesToLoad.Count > 0)
					{
						if (inProgress)
							_deckListControl.ContinueLoadingDecks(_filesToLoad.Count);
						else
							_deckListControl.BeginLoadingDecks(_filesToLoad.Count);
					}

					return (inProgress, _filesToLoad.Count > 0);
				}
			}

			var (loadingInProgress, added) = addFilesToLoad(files);

			if (loadingInProgress || !added)
				return;

			_ctsLifetime.Token.Run(token =>
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

					string message = string.Join(Str.Endl,
						failedDecks.Select(f => $"{f.File}{Str.Endl}{f.Error}{Str.Endl}"));

					_log.Error($"Errors loading decks: {message}");
					MessageBox.Show($@"Failed to load {failedDecks.Count} of {i} decks. See {new FsPath("logs", "error.log")} for details");
				});
			});
		}

		public void Abort() =>
			_abort = true;

		private Deck pasteCollectionFromText(string text, bool append)
		{
			var deck = _serialization.LoadSerialized("*.txt", text, exact: true);

			if (deck.Error != null)
			{
				MessageBox.Show(deck.Error);
				return null;
			}

			_collection.LoadCollection(deck, append);
			return deck;
		}

		private Deck pasteDeckFromText(string text, bool append)
		{
			var deck = _serialization.LoadSerialized("*.txt", text, exact: true);

			if (deck.Error != null)
			{
				MessageBox.Show(deck.Error);
				return null;
			}

			var deckToPaste = getPasteOperations();
			return _deckEditor.Paste(deckToPaste, append, _cardRepo);

			Deck getPasteOperations()
			{
				DeckZone mainDeck;
				DeckZone sideDeck;
				DeckZone maybeDeck;
				DeckZone sampleHand;

				switch (_targetForm.DeckZone)
				{
					case Zone.Main:
					case null when _targetForm.IsDeckListSelected:
						mainDeck = deck.MainDeck;
						sideDeck = deck.Sideboard.Order.Count > 0
							? deck.Sideboard
							: null;
						maybeDeck = deck.Maybeboard.Order.Count > 0
							? deck.Maybeboard
							: null;
						sampleHand = null;
						break;

					case Zone.Side:
						mainDeck = null;
						sideDeck = deck.MainDeck;
						maybeDeck = null;
						sampleHand = null;
						break;

					case Zone.Maybe:
						mainDeck = null;
						sideDeck = null;
						maybeDeck = deck.MainDeck;
						sampleHand = null;
						break;

					case Zone.SampleHand:
						mainDeck = null;
						sideDeck = null;
						maybeDeck = null;
						sampleHand = deck.MainDeck;
						break;

					default:
						return null;
				}

				return Deck.Create(
					mainDeck?.Count,
					mainDeck?.Order,
					sideDeck?.Count,
					sideDeck?.Order,
					maybeDeck?.Count,
					maybeDeck?.Order,
					sampleHand?.Count,
					sampleHand?.Order);
			}
		}

		public Deck PasteDeck(bool append)
		{
			if (!_cardRepo.IsLoadingComplete.Signaled)
				return null;

			var text = Clipboard.GetText();
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return pasteDeckFromText(text, append);
		}

		public Deck PasteCollection(bool append)
		{
			if (!_cardRepo.IsLoadingComplete.Signaled)
				return null;

			var text = Clipboard.GetText();
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return pasteCollectionFromText(text, append);
		}

		public Deck CopyCollection(IDeckFormatter formatter)
		{
			var deck = Deck.Create(
				_collection.CountById?.ToDictionary(),
				_collection.CountById?.Keys.OrderBy(_ => _cardRepo.CardsById[_].NameEn).ToList(),
				null,
				null,
				null,
				null,
				null,
				null);

			var serialized = _serialization.SaveSerialized(deck, exact: true, formatter);
			if (serialized.TryCopyToClipboard())
				return deck;

			return null;
		}

		public Deck CopyDeck(IDeckFormatter formatter)
		{
			Deck deck;

			switch (_targetForm.DeckZone)
			{
				case Zone.Main:
				case null when _targetForm.IsDeckListSelected:
					deck = _deckEditor.Snapshot();
					break;

				case Zone.Side:
					deck = copySideDeck();
					break;

				case Zone.Maybe:
					deck = copyMaybeDeck();
					break;

				case Zone.SampleHand:
					deck = copySampleHand();
					break;

				default:
					return null;
			}

			var serialized = _serialization.SaveSerialized(deck, exact: true, formatter);
			if (serialized.TryCopyToClipboard())
				return deck;

			return null;
		}

		public Deck CopyCards(ICollection<Card> cards, IDeckFormatter formatter)
		{
			var deck = Deck.Create(
				cards.ToDictionary(_=>_.Id, _ => 1),
				cards.Select(_=>_.Id).ToList(),
				null,
				null,
				null,
				null,
				null,
				null);

			var serialized = _serialization.SaveSerialized(deck, exact: true, formatter);
			if (serialized.TryCopyToClipboard())
				return deck;

			return null;
		}

		private Deck copySampleHand() =>
			Deck.Create(
				_deckEditor.SampleHand.CountById.ToDictionary(),
				_deckEditor.SampleHand.CardsIds.ToList(),
				null,
				null,
				null,
				null,
				null,
				null);

		private Deck copySideDeck() =>
			Deck.Create(
				_deckEditor.SideDeck.CountById.ToDictionary(),
				_deckEditor.SideDeck.CardsIds.ToList(),
				null,
				null,
				null,
				null,
				null,
				null);

		private Deck copyMaybeDeck() =>
			Deck.Create(
				_deckEditor.MaybeDeck.CountById.ToDictionary(),
				_deckEditor.MaybeDeck.CardsIds.ToList(),
				null,
				null,
				null,
				null,
				null,
				null);

		public void LoadDeck(Deck deck)
		{
			_deckEditor.DeckFile = deck.File;
			_deckEditor.DeckName = deck.Name;

			_deckEditor.SetDeck(deck, _cardRepo);
			_deckEditor.Shuffle();
		}



		public ISite Site { get; set; }
		public event EventHandler Disposed;

		public void Dispose()
		{
			_ctsLifetime.Cancel();
			Disposed?.Invoke(this, EventArgs.Empty);
		}

		private bool _abort;

		private readonly HashSet<(FsPath File, FsPath Dir)> _filesToLoadDistinct = new HashSet<(FsPath, FsPath)>();
		private readonly List<(FsPath File, FsPath Dir)> _filesToLoad = new List<(FsPath, FsPath)>();

		private readonly object _sync = new object();

		private readonly CardRepository _cardRepo;
		private readonly DeckSerializationSubsystem _serialization;
		private readonly CollectionEditorModel _collection;
		private readonly DeckEditorModel _deckEditor;
		private readonly FormMain _targetForm;
		private readonly DeckListControl _deckListControl;
		private readonly CancellationTokenSource _ctsLifetime;
		private readonly Control[] _targetControls;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
