using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers.Classic;
using Mtgdb.Dal;
using Mtgdb.Index;

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
			Sequence.From(_deckList.GetModels(_ui).Select(Adapter.ToDocument));

		protected override Analyzer CreateAnalyzer() =>
			new MtgAnalyzer(Adapter);

		protected override QueryParser CreateQueryParser(string language, Analyzer analyzer) => 
			new DeckQueryParser((MtgAnalyzer) analyzer, (DeckDocumentAdapter) Adapter);

		public SearchResult<int> Search(string query, UiModel ui)
		{
			ensureIndexIsUpToDate(ui);

			lock (_sync)
				return Search(query, language: null);
		}

		public IntellisenseSuggest Suggest(TextInputState searchState, UiModel ui)
		{
			ensureIndexIsUpToDate(ui);

			lock (_sync)
				return Spellchecker.Suggest(language: null, input: searchState);
		}

		public DeckSpellchecker Spellchecker =>
			(DeckSpellchecker) LuceneSpellchecker;

		public IntellisenseSuggest CycleValue(TextInputState input, bool backward, UiModel ui)
		{
			ensureIndexIsUpToDate(ui);

			lock (_sync)
				return Spellchecker.CycleValue(null, input, backward);
		}

		public void ModelChanged() =>
			_modelUpdatedTime = DateTime.UtcNow;

		public override void LoadIndex()
		{
			IsLoaded = false;
			base.LoadIndex();
		}

		private void ensureIndexIsUpToDate(UiModel ui)
		{
			var modelUpdatedTime = _modelUpdatedTime;

			if (_indexUpdatedTime == modelUpdatedTime)
				return;

			lock (_sync)
			{
				_ui = ui;
				Spellchecker.SetUi(ui);

				LoadIndexes();
			}

			_indexUpdatedTime = modelUpdatedTime;
		}

		private UiModel _ui;

		private DateTime? _indexUpdatedTime;
		private DateTime _modelUpdatedTime = DateTime.UtcNow;

		private readonly DeckListModel _deckList;
		private readonly object _sync = new object();
	}
}