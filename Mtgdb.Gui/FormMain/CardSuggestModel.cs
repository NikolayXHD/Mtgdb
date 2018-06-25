using System;
using System.Threading;
using JetBrains.Annotations;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Mtgdb.Index;

namespace Mtgdb.Gui
{
	public class CardSuggestModel
	{
		[UsedImplicitly]
		public CardSuggestModel(CardSearcher searcher)
		{
			_spellchecker = searcher.Spellchecker;
			_suggestThread = new Thread(_ => suggestThread());
		}

		private void suggestLoopIteration()
		{
			if (_spellchecker == null || TextInputStateCurrent == null || isSuggestUpToDate())
				Thread.Sleep(100);
			else
				suggest();
		}

		private void suggest()
		{
			var searchState =_textInputState = TextInputStateCurrent;
			_language = Language;

			var suggest = _spellchecker.Suggest(_language, searchState);

			if (isSuggestUpToDate())
				Suggested?.Invoke(suggest, searchState);
		}

		public string Language => Ui.LanguageController.Language;

		private bool isSuggestUpToDate()
		{
			if (_language != Ui.LanguageController.Language)
				return false;

			if (_textInputState == null && TextInputStateCurrent == null)
				return true;

			if (_textInputState == null || TextInputStateCurrent == null)
				return false;

			return _textInputState.Equals(TextInputStateCurrent);
		}

		public void StartSuggestThread()
		{
			_suggestThread.Start();
		}

		public void AbortSuggestThread()
		{
			_suggestThread.Abort();
		}

		private void suggestThread()
		{
			try
			{
				while (true)
					suggestLoopIteration();
			}
			catch (ThreadAbortException)
			{
			}
		}

		public UiModel Ui { get; set; }

		private readonly CardSpellchecker _spellchecker;

		private TextInputState _textInputState;
		public TextInputState TextInputStateCurrent { get; set; }

		private string _language;

		public event Action<IntellisenseSuggest, TextInputState> Suggested;
		
		private readonly Thread _suggestThread;
	}
}