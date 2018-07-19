using System;
using System.Threading;
using Mtgdb.Index;

namespace Mtgdb.Ui
{
	public abstract class SuggestModel<TId, TObj>
	{
		protected SuggestModel(LuceneSearcher<TId, TObj> searcher)
		{
			Searcher = searcher;
			_suggestThread = new Thread(_ => suggestThread());
		}

		private void suggestLoopIteration()
		{
			if (!Searcher.Spellchecker.IsLoaded || TextInputStateCurrent == null || IsSuggestUpToDate())
				Thread.Sleep(100);
			else
				suggest();
		}

		private void suggest()
		{
			var searchState =_textInputState = TextInputStateCurrent;

			var suggest = Suggest(searchState);

			if (!IsSuggestUpToDate())
				return;

			var handled = Suggested?.Invoke(suggest, searchState);

			if (handled == false)
				_textInputState = null;
		}

		protected abstract IntellisenseSuggest Suggest(TextInputState state);

		protected virtual bool IsSuggestUpToDate()
		{
			if (_textInputState == null && TextInputStateCurrent == null)
				return true;

			if (_textInputState == null || TextInputStateCurrent == null)
				return false;

			return _textInputState.Equals(TextInputStateCurrent);
		}

		public void StartSuggestThread() =>
			_suggestThread.Start();

		public void AbortSuggestThread() =>
			_suggestThread.Abort();

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

		public TextInputState TextInputStateCurrent { get; set; }

		public event Func<IntellisenseSuggest, TextInputState, bool> Suggested;

		private TextInputState _textInputState;

		protected readonly LuceneSearcher<TId, TObj> Searcher;
		private readonly Thread _suggestThread;
	}
}