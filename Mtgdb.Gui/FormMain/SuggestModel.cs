using System;
using System.Threading;
using Lucene.Net.Contrib;
using Mtgdb.Dal.Index;

namespace Mtgdb.Gui
{
	public class SuggestModel
	{
		private const int SuggestCount = 20;

		private readonly LuceneSpellchecker _spellchecker;

		private SearhStringState _searhState;
		public SearhStringState SearhStateCurrent { get; set; }

		private string _language;
		public string LanguageCurrent;

		public event Action<IntellisenseSuggest> Suggested;
		
		private readonly Thread _suggestThread;

		public Token Token { get; private set; }

		public SuggestModel(LuceneSearcher searcher)
		{
			_spellchecker = searcher.Spellchecker;
			_suggestThread = new Thread(_ => suggestThread());
		}

		private void suggestLoopIteration()
		{
			if (_spellchecker == null || isSuggestUpToDate())
				Thread.Sleep(100);
			else
				suggest();
		}

		private void suggest()
		{
			_searhState = SearhStateCurrent;
			_language = LanguageCurrent;

			var suggest = _spellchecker.Suggest(_searhState.Text, _searhState.Caret, _language, SuggestCount);
			Token = suggest.Token;

			if (isSuggestUpToDate())
				Suggested?.Invoke(suggest);
		}

		private bool isSuggestUpToDate()
		{
			if (_language != LanguageCurrent)
				return false;

			if (_searhState == null && SearhStateCurrent == null)
				return true;

			if (_searhState == null || SearhStateCurrent == null)
				return false;

			if (_searhState.Text != SearhStateCurrent.Text)
				return false;
			
			if (_searhState.Caret != SearhStateCurrent.Caret)
				return false;

			return true;
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
	}
}