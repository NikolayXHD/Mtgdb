using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers.Classic;
using Mtgdb.Dal;
using Mtgdb.Index;
using NLog;

namespace Mtgdb.Controls
{
	public class DeckSearcher : LuceneSearcher<int, DeckModel>
	{
		[UsedImplicitly]
		public DeckSearcher(
			DeckListModel deckList,
			DeckSpellchecker spellchecker,
			DeckDocumentAdapter adapter)
			: base(spellchecker, adapter)
		{
			_deckList = deckList;
		}

		protected override IEnumerable<IEnumerable<Document>> GetDocumentGroupsToIndex() =>
			Sequence.From(_deckList.GetModels(_ui)
				.Select(Adapter.ToDocument));

		protected override Analyzer CreateAnalyzer() =>
			new MtgAnalyzer(Adapter);

		protected override QueryParser CreateQueryParser(string language, Analyzer analyzer) => 
			new DeckQueryParser((MtgAnalyzer) analyzer, (DeckDocumentAdapter) Adapter);

		public SearchResult<int> Search(string query, UiModel ui)
		{
			LoadIndexes(ui);

			lock (_sync)
				return Search(query, language: null);
		}

		public IntellisenseSuggest Suggest(TextInputState searchState, UiModel ui)
		{
			LoadIndexes(ui);

			lock (_sync)
				return ((DeckSpellchecker) Spellchecker).Suggest(language: null, input: searchState);
		}

		public IntellisenseSuggest CycleValue(TextInputState input, bool backward, UiModel ui)
		{
			LoadIndexes(ui);

			lock (_sync)
				return ((DeckSpellchecker) Spellchecker).CycleValue(null, input, backward);
		}

		public void ModelChanged() =>
			_modelUpdatedTime = DateTime.UtcNow;

		protected override void LoadIndex()
		{
			_log.Info($"Begin {nameof(LoadIndex)}");

			IsLoaded = false;
			base.LoadIndex();

			_log.Info($"End {nameof(LoadIndex)}");
		}

		public void LoadIndexes(UiModel ui)
		{
			lock (_sync)
			{
				var modelUpdatedTime = _modelUpdatedTime;

				if (_indexUpdatedTime == modelUpdatedTime)
					return;

				_ui = ui;
				((DeckSpellchecker) Spellchecker).SetUi(ui);

				LoadIndexes();

				_indexUpdatedTime = modelUpdatedTime;
			}
		}

		private UiModel _ui;

		private DateTime? _indexUpdatedTime;
		private DateTime _modelUpdatedTime = DateTime.UtcNow;

		private readonly DeckListModel _deckList;
		private readonly object _sync = new object();

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}