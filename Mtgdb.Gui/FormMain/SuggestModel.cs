using System;
using System.Threading;
using JetBrains.Annotations;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;

namespace Mtgdb.Gui
{
	public class SuggestModel
	{
		[UsedImplicitly]
		public SuggestModel(LuceneSearcher searcher)
		{
			_spellchecker = searcher.Spellchecker;
			_suggestThread = new Thread(_ => suggestThread());
		}

		private void suggestLoopIteration()
		{
			if (_spellchecker == null || SearchInputStateCurrent == null || isSuggestUpToDate())
				Thread.Sleep(100);
			else
				suggest();
		}

		private void suggest()
		{
			var searchState =_searchInputState = SearchInputStateCurrent;
			_language = Ui.LanguageController.Language;

			var suggest = _spellchecker.Suggest(_language, searchState.Text, searchState.Caret);

			if (isSuggestUpToDate())
				Suggested?.Invoke(suggest, searchState);
		}

		private bool isSuggestUpToDate()
		{
			if (_language != Ui.LanguageController.Language)
				return false;

			if (_searchInputState == null && SearchInputStateCurrent == null)
				return true;

			if (_searchInputState == null || SearchInputStateCurrent == null)
				return false;

			return _searchInputState.Equals(SearchInputStateCurrent);
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

		private readonly LuceneSpellchecker _spellchecker;

		private SearchInputState _searchInputState;
		public SearchInputState SearchInputStateCurrent { get; set; }

		private string _language;

		public event Action<IntellisenseSuggest, SearchInputState> Suggested;
		
		private readonly Thread _suggestThread;
	}
}