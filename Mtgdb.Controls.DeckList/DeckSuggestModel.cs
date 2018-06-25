using System;
using System.Threading;
using JetBrains.Annotations;
using Mtgdb.Dal;
using Mtgdb.Index;

namespace Mtgdb.Controls
{
	public class DeckSuggestModel
	{
		[UsedImplicitly]
		public DeckSuggestModel(DeckSearcher searcher)
		{
			_searcher = searcher;
			_suggestThread = new Thread(_ => suggestThread());
		}

		private void suggestLoopIteration()
		{
			if (TextInputStateCurrent == null || isSuggestUpToDate())
				Thread.Sleep(100);
			else
				suggest();
		}

		private void suggest()
		{
			var searchState =_textInputState = TextInputStateCurrent;

			var suggest = _searcher.Suggest(searchState, Ui);

			if (isSuggestUpToDate())
			{
				var handled = Suggested?.Invoke(suggest, searchState);

				if (handled == false)
					_textInputState = null;
			}
		}

		private bool isSuggestUpToDate()
		{
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

		public TextInputState TextInputStateCurrent { get; set; }
		public UiModel Ui { get; set; }

		public event Func<IntellisenseSuggest, TextInputState, bool> Suggested;

		private TextInputState _textInputState;

		private readonly DeckSearcher _searcher;
		private readonly Thread _suggestThread;
	}
}