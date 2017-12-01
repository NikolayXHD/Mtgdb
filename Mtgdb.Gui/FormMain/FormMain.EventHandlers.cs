﻿using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Properties;

namespace Mtgdb.Gui
{
	partial class FormMain
	{
		private void formLoad(object sender, EventArgs e)
		{
			setupCheckButtonImages();
			updateExcludeManaAbility();
			updateExcludeManaCost();
			updateShowProhibited();
			updateShowSampleHandButtons();

			setupTooltips();
			subscribeToEvents();
			startThreads();

			historyUpdateGlobals();

			historyApply(_historyModel.Current);

			if (_requiredDeck != null)
			{
				_requiredDeck = null;
				historyUpdate();
			}

			_deckSerializationSubsystem.LastFile = _historyModel.Current.DeckFile;
		}

		private void applicationExit(object sender, EventArgs e)
		{
			new Thread(_ => _historyModel.Save()).Start();
			stopThreads();
			unsubscribeFromEvents();
		}

		private void formClosing(object sender, FormClosingEventArgs e)
		{
			stopThreads();
			unsubscribeFromEvents();
		}



		private void cardRepoSetAdded()
		{
			if (_cardRepo.SetsByCode.Count % 3 == 0)
				this.Invoke(delegate
				{
					updateFormStatus();
					Application.DoEvents();
				});
		}

		private void imageLoadingComplete()
		{
			this.Invoke(delegate
			{
				updateShowSampleHandButtons();

				beginRestoreSettings();

				_deckModel.LoadDeck(_cardRepo);
				_sortSubsystem.Invalidate();

				endRestoreSettings();

				RunRefilterTask();
			});
		}

		private void localizationLoadingComplete()
		{
			this.Invoke(delegate
			{
				beginRestoreSettings();
				_sortSubsystem.Invalidate();
				endRestoreSettings();

				RunRefilterTask();
			});
		}

		private void luceneSearcherLoaded()
		{
			this.Invoke(delegate
			{
				beginRestoreSettings();
				_searchStringSubsystem.ApplyFind();
				endRestoreSettings();

				RunRefilterTask();
			});
		}

		private void luceneSearcherDisposed()
		{
			this.Invoke(updateFormStatus);
		}

		private void luceneSearcherIndexingProgress()
		{
			this.Invoke(updateFormStatus);
		}



		private void resetExcludeManaAbility(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
				_buttonExcludeManaAbility.Checked = false;
		}

		private void resetExcludeManaCost(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
				_buttonExcludeManaCost.Checked = true;
		}



		private void showDuplicatesCheckedChanged(object sender, EventArgs e)
		{
			if (restoringSettings())
				return;

			resetTouchedCard();
			RunRefilterTask();
			historyUpdate();
		}

		private void showProhibitChanged(object sender, EventArgs e)
		{
			updateShowProhibited();

			if (restoringSettings())
				return;

			historyUpdate();
		}

		private void excludeManaCostChanged(object sender, EventArgs e)
		{
			updateExcludeManaCost();

			if (restoringSettings())
				return;

			historyUpdate();
		}

		private void excludeManaAbilityChanged(object sender, EventArgs e)
		{
			updateExcludeManaAbility();

			if (restoringSettings())
				return;

			historyUpdate();
		}



		private void gridScrolled(LayoutView sender)
		{
			if (restoringSettings())
				return;

			if (sender == _viewCards)
			{
				_imagePreloadingSubsystem.Reset();

				if (_cardRepo.IsLoadingComplete)
					_historyModel.Current.SearchResultScroll = _viewCards.VisibleRecordIndex;
			}

			updateFormStatus();
		}



		private void quickFiltersChanged(object sender, EventArgs e)
		{
			if (restoringSettings())
				return;

			if (!isFilterGroupEnabled(FilterGroupButtons))
				setFilterManagerState(FilterGroupButtons, FilterValueState.Required);

			updateTerms();

			resetTouchedCard();
			RunRefilterTask();
			historyUpdate();
		}

		private void quickFilterManagerChanged(object sender, EventArgs e)
		{
			if (restoringSettings())
				return;

			resetTouchedCard();
			RunRefilterTask();
			historyUpdate();
		}



		private void deckZoneChanged(TabHeaderControl sender, int selected)
		{
			beginRestoreSettings();

			_deckModel.SetZone(DeckZone, _cardRepo);

			endRestoreSettings();

			updateFormStatus();
			updateShowSampleHandButtons();
		}


		private void deckChanged(bool listChanged, bool countChanged, Card card, bool touchedChanged, Zone? zone)
		{
			updateViewCards(listChanged, card, FilterGroupDeck, touchedChanged);
			updateViewDeck(listChanged, countChanged, card, touchedChanged);

			if (restoringSettings())
				return;

			if (zone != Zone.SampleHand && (countChanged || listChanged))
				historyUpdate();

			updateFormStatus();
		}

		private void collectionChanged(bool listChanged, bool countChanged, Card card, bool touchedChanged, Zone? zone)
		{
			updateViewCards(listChanged, card, FilterGroupCollection, touchedChanged);

			updateViewDeck(
				listChanged:false,
				countChanged: true,
				card: card,
				touchedChanged: touchedChanged);
			
			if (restoringSettings())
				return;

			if (countChanged || listChanged)
				historyUpdate();

			updateFormStatus();
		}



		private void updateViewCards(bool listChanged, Card card, int filterGroup, bool touchedChanged)
		{
			if (touchedChanged || listChanged && isFilterGroupEnabled(filterGroup))
			{
				if (restoringSettings())
					return;

				if (touchedChanged && _deckModel.TouchedCard != null)
				{
					Action onFinished = () =>
						_scrollSubsystem.EnsureCardVisibility(_deckModel.TouchedCard, _viewCards);

					RunRefilterTask(onFinished);
				}
				else
				{
					RunRefilterTask();
				}
			}
			else
			{
				if (card == null)
					_viewCards.Invalidate();
				else
					_viewCards.InvalidateCard(card);
			}
		}

		private void updateViewDeck(bool listChanged, bool countChanged, Card card, bool touchedChanged)
		{
			if (listChanged)
				_viewDeck.RefreshData();

			if (touchedChanged && _deckModel.TouchedCard != null)
				_scrollSubsystem.EnsureCardVisibility(_deckModel.TouchedCard, _viewDeck);

			if (listChanged)
				_viewDeck.Invalidate();
			else if (touchedChanged || countChanged)
				_viewDeck.Invalidate();
			else if (card != null)
				_viewDeck.InvalidateCard(card);
		}



		private void loadDeck(Deck deck)
		{
			_historyModel.DeckFile = deck.File;
			_historyModel.DeckName = deck.Name;

			_deckModel.SetDeck(deck, _cardRepo);
			_deckModel.Shuffle();
		}

		private void loadCollection(Deck collection)
		{
			_collectionModel.LoadCollection(collection, append: false);
		}



		private void deckZoneHover(object sender, EventArgs e)
		{
			if (!IsDraggingCard)
				return;

			var hoveredIndex = _tabHeadersDeck.HoveredIndex;

			if (hoveredIndex < 0 || hoveredIndex == _tabHeadersDeck.SelectedIndex || hoveredIndex >= _tabHeadersDeck.Count)
				return;

			_tabHeadersDeck.SelectedIndex = hoveredIndex;

			var card = DraggedCard;
			StopDragging();
			dragCard(card);
		}

		private void deckZoneDrag(object sender, DragEventArgs e)
		{
			var location = _tabHeadersDeck.PointToClient(new Point(e.X, e.Y));

			int hoveredIndex;
			bool hoveredClose;
			_tabHeadersDeck.GetTabIndex(location, out hoveredIndex, out hoveredClose);

			if (hoveredIndex < 0 || hoveredIndex == _tabHeadersDeck.SelectedIndex || hoveredIndex >= _tabHeadersDeck.Count)
				return;

			_tabHeadersDeck.SelectedIndex = hoveredIndex;
		}



		private void legalityFilterChanged()
		{
			if (restoringSettings())
				return;

			if (!isFilterGroupEnabled(FilterGroupLegality))
				setFilterManagerState(FilterGroupLegality, FilterValueState.Required);

			RunRefilterTask();
			historyUpdate();
			_viewCards.Focus();
		}

		private void languageChanged()
		{
			if (restoringSettings())
				return;

			if (_sortSubsystem.IsLanguageDependent || isFilterGroupEnabled(FilterGroupFind) && isSearchStringApplied())
			{
				if (_sortSubsystem.IsLanguageDependent)
					_sortSubsystem.Invalidate();

				if (isFilterGroupEnabled(FilterGroupFind) && isSearchStringApplied())
					_searchStringSubsystem.ApplyFind();

				RunRefilterTask();
			}
			else
			{
				_viewCards.RefreshData();
				_viewCards.Invalidate();
			}

			historyUpdate();
		}

		private void searchStringApplied()
		{
			if (restoringSettings())
				return;

			if (!isFilterGroupEnabled(FilterGroupFind))
				setFilterManagerState(FilterGroupFind, FilterValueState.Required);

			resetTouchedCard();

			RunRefilterTask();
			historyUpdate();
		}

		private void searchStringChanged()
		{
			updateFormStatus();
		}

		private void sortChanged()
		{
			if (restoringSettings())
				return;

			RunRefilterTask();
			historyUpdate();
		}



		private void setupCheckButtonImages()
		{
			const float sat = 2f;

			float sampleHandOpacity = 0.6f;

			_buttonSubsystem.SetupButton(_buttonSampleHandNew,
				new ButtonImages(
					null,
					Resources.hand_48.SetOpacity(sampleHandOpacity),
					null,
					Resources.hand_48,
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonSampleHandDraw,
				new ButtonImages(
					null,
					Resources.draw_48.SetOpacity(sampleHandOpacity),
					null,
					Resources.draw_48,
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonSampleHandMulligan,
				new ButtonImages(
					null,
					Resources.mulligan_48.SetOpacity(sampleHandOpacity),
					null,
					Resources.mulligan_48,
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonShowDuplicates,
				new ButtonImages(
					Resources.clone_48.TransformColors(saturation: 0f),
					Resources.clone_48,
					Resources.clone_48.TransformColors(saturation: 0.9f),
					Resources.clone_48.TransformColors(saturation: 2.5f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonShowProhibit,
				new ButtonImages(
					Resources.exclude_hidden_24,
					Resources.exclude_shown_24,
					Resources.exclude_hidden_24.TransformColors(sat, 1.2f),
					Resources.exclude_shown_24.TransformColors(sat, 1.2f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonExcludeManaAbility,
				new ButtonImages(
					Resources.include_plus_24,
					Resources.exclude_minus_24,
					Resources.include_plus_24.TransformColors(sat, 1f),
					Resources.exclude_minus_24.TransformColors(sat, 1.2f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonExcludeManaCost,
				new ButtonImages(
					Resources.include_plus_24,
					Resources.exclude_minus_24,
					Resources.include_plus_24.TransformColors(sat, 1f),
					Resources.exclude_minus_24.TransformColors(sat, 1.2f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonHideDeck,
				new ButtonImages(
					Resources.shown_40,
					Resources.hidden_40,
					Resources.shown_40.TransformColors(brightness: 0.1f),
					Resources.hidden_40.TransformColors(brightness: 0.1f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonHidePartialCards,
				new ButtonImages(
					Resources.partial_card_enabled_40,
					Resources.partial_card_disabled_40,
					Resources.partial_card_enabled_40.TransformColors(brightness: 0.1f),
					Resources.partial_card_disabled_40.TransformColors(brightness: 0.1f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonHideText,
				new ButtonImages(
					Resources.text_enabled_40,
					Resources.text_disabled_40,
					Resources.text_enabled_40.TransformColors(brightness: 0.1f),
					Resources.text_disabled_40.TransformColors(brightness: 0.1f),
					areImagesDoubleSized: true));
		}



		private static void deckDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (files.Length < 10)
					e.Effect = DragDropEffects.Copy;
			}
			else if (e.Data.GetFormats().Contains(DataFormats.Text))
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		public bool IsSearchFocused() => _searchStringSubsystem.IsSearchFocused();



		public void PasteDeck(bool append)
		{
			if (!_cardRepo.IsImageLoadingComplete)
				return;

			var text = Clipboard.GetText();
			if (string.IsNullOrWhiteSpace(text))
				return;

			pasteDeckFromText(text, append);
		}

		public void PasteCollection(bool append)
		{
			if (!_cardRepo.IsImageLoadingComplete)
				return;

			var text = Clipboard.GetText();
			if (string.IsNullOrWhiteSpace(text))
				return;

			pasteCollectionFromText(text, append);
		}

		public void CopyCollection()
		{
			var deck = Deck.Create(
				_collectionModel.CountById?.ToDictionary(),
				_collectionModel.CountById?.Keys.OrderBy(_ => _cardRepo.CardsById[_].NameEn).ToList(),
				null,
				null);

			var serialized = _deckSerializationSubsystem.SaveSerialized("*.txt", deck);
			Clipboard.SetText(serialized);
		}

		public void CopyDeck()
		{
			Deck deck;

			switch (DeckZone)
			{
				case Zone.SampleHand:
					deck = Deck.Create(
						_deckModel.SampleHand.CountById.ToDictionary(),
						_deckModel.SampleHand.CardsIds.ToList(),
						null,
						null);
					break;
				case Zone.Side:
					deck = Deck.Create(
						_deckModel.SideDeck.CountById.ToDictionary(),
						_deckModel.SideDeck.CardsIds.ToList(),
						null,
						null);
					break;
				case Zone.Main:
					deck = Deck.Create(
						_deckModel.MainDeck.CountById.ToDictionary(),
						_deckModel.MainDeck.CardsIds.ToList(),
						_deckModel.SideDeck.CountById.ToDictionary(),
						_deckModel.SideDeck.CardsIds.ToList());
					break;
				default:
					return;
			}

			var serialized = _deckSerializationSubsystem.SaveSerialized("*.txt", deck);
			Clipboard.SetText(serialized);
		}

		private void pasteDeckFromText(string text, bool append)
		{
			var deck = _deckSerializationSubsystem.LoadSerialized("*.txt", text);

			if (deck.Error != null)
				MessageBox.Show(deck.Error);
			else
				_deckModel.Paste(deck, append, _cardRepo);
		}

		private void hideSampleHand()
		{
			if (DeckZone == Zone.SampleHand)
				DeckZone = Zone.Main;
		}

		private void pasteCollectionFromText(string text, bool append)
		{
			var deck = _deckSerializationSubsystem.LoadSerialized("*.txt", text);

			if (deck.Error != null)
				MessageBox.Show(deck.Error);
			else
				_collectionModel.LoadCollection(deck, append);
		}

		private void deckDragDropped(object sender, DragEventArgs e)
		{
			if (_cardRepo.IsLoadingComplete)
			{
				dragDropped(e.Data);
				return;
			}

			_cardRepo.LoadingComplete += () => { this.Invoke(delegate { dragDropped(e.Data); }); };

			MessageBox.Show(this,
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

				var decks = files.Select(f => _deckSerializationSubsystem.LoadFile(f))
					.ToArray();

				var failedDecks = decks.Where(d => d.Error != null).ToArray();
				var loadedDecks = decks.Where(d => d.Error == null).ToArray();

				if (failedDecks.Length > 0)
				{
					var message = string.Join(Str.Endl,
						failedDecks.Select(f => $"{f.File}{Str.Endl}{f.Error}{Str.Endl}"));

					MessageBox.Show(message);
				}

				if (loadedDecks.Length > 0)
					deckLoaded(loadedDecks[0]);

				for (int i = 1; i < loadedDecks.Length; i++)
				{
					var deck = loadedDecks[i];
					_uiModel.Form.NewTab(form => ((FormMain) form)._requiredDeck = deck);
				}
			}
			else if (dragData.GetFormats().Contains(DataFormats.Text))
			{
				string text = (string) dragData.GetData(DataFormats.Text, autoConvert: true);

				if (ModifierKeys == Keys.Alt)
					pasteCollectionFromText(text, append: false);
				else if (ModifierKeys == (Keys.Alt | Keys.Shift))
					pasteCollectionFromText(text, append: true);
				else if (ModifierKeys == Keys.None)
					pasteDeckFromText(text, append: false);
				else if (ModifierKeys == Keys.Shift)
					pasteDeckFromText(text, append: true);
			}
		}



		private void sampleHandNew(object sender, EventArgs e)
		{
			if (!_cardRepo.IsImageLoadingComplete)
				return;

			_deckModel.NewSampleHand(_cardRepo);
		}

		private void sampleHandMulligan(object sender, EventArgs e)
		{
			if (!_cardRepo.IsImageLoadingComplete)
				return;

			_deckModel.Mulligan(_cardRepo);
		}

		private void sampleHandDraw(object sender, EventArgs e)
		{
			if (!_cardRepo.IsImageLoadingComplete)
				return;

			_deckModel.Draw(_cardRepo);
		}
	}
}
