using System.Collections.Generic;
using Mtgdb.Index;

namespace Mtgdb.Controls
{
	public class SearchResultHiglightSubsystem
	{
		public SearchResultHiglightSubsystem(
			LayoutViewControl view,
			DeckSearchSubsystem searchSubsystem,
			DeckDocumentAdapter adapter)
		{
			_view = view;
			var keywordHighlighter = new DeckKeywordHighlighter();
			_highlightSubsystem = new SearchResultHighlighter(searchSubsystem, adapter, keywordHighlighter);
		}

		public void SubscribeToEvents() =>
			_view.RowDataLoaded += rowDataLoaded;

		private void rowDataLoaded(object sender, int rowHandle)
		{
			var card = (DeckModel) _view.FindRow(rowHandle);

			if (card == null)
				return;

			foreach (var displayField in _view.FieldNames)
			{
				var displayText = _view.GetText(rowHandle, displayField);
				var matches = new List<TextRange>();
				var contextMatches = new List<TextRange>();

				_highlightSubsystem.AddSearchStringMatches(matches, contextMatches, displayField, displayText);

				var highlightRanges = _highlightSubsystem.GetHighlightRanges(matches, contextMatches);
				_view.SetHighlightTextRanges(highlightRanges, rowHandle, displayField);
			}
		}

		private readonly LayoutViewControl _view;
		private readonly SearchResultHighlighter _highlightSubsystem;
	}
}