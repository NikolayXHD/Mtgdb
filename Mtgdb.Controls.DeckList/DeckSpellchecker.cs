using System.Collections.Generic;
using JetBrains.Annotations;
using Lucene.Net.Index;
using Mtgdb.Dal;
using Mtgdb.Index;
using NLog;

namespace Mtgdb.Controls
{
	public class DeckSpellchecker : LuceneSpellchecker<int, DeckModel>
	{
		[UsedImplicitly]
		public DeckSpellchecker(DeckListModel deckList, DeckDocumentAdapter adapter)
			: base(adapter)
		{
			_deckList = deckList;
		}

		public void SetUi(UiModel ui) =>
			_ui = ui;

		protected override IEnumerable<DeckModel> GetObjectsToIndex() =>
			_deckList.GetModels(_ui);

		public override void LoadIndex(DirectoryReader indexReader)
		{
			_log.Info($"Begin {nameof(LoadIndex)}");

			IsLoaded = false;
			ValuesCache.Clear();
			base.LoadIndex(indexReader);

			_log.Info($"End {nameof(LoadIndex)}");
		}

		private readonly DeckListModel _deckList;
		private UiModel _ui;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}