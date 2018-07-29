using System;
using System.Threading;
using System.Threading.Tasks;
using Mtgdb.Index;

namespace Mtgdb.Ui
{
	public abstract class SuggestModel<TId, TObj>
	{
		protected SuggestModel(LuceneSearcher<TId, TObj> searcher)
		{
			Searcher = searcher;
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

		public void StartSuggestThread()
		{
			if (_cts != null && !_cts.IsCancellationRequested)
				throw new InvalidOperationException("Already started");

			var cts = new CancellationTokenSource();

			TaskEx.Run(async () =>
			{
				while (!cts.IsCancellationRequested)
				{
					if (!Searcher.Spellchecker.IsLoaded || TextInputStateCurrent == null || IsSuggestUpToDate())
						await TaskEx.Delay(100);
					else
						suggest();
				}
			});

			_cts = cts;
		}

		public void AbortSuggestThread() =>
			_cts?.Cancel();



		public TextInputState TextInputStateCurrent { get; set; }

		public event Func<IntellisenseSuggest, TextInputState, bool> Suggested;

		private TextInputState _textInputState;

		protected readonly LuceneSearcher<TId, TObj> Searcher;

		private CancellationTokenSource _cts;
	}
}