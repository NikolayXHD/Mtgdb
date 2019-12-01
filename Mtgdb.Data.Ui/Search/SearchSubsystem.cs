using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lucene.Net.Contrib;
using Mtgdb.Controls;
using Mtgdb.Data;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Ui
{
	public abstract class SearchSubsystem<TId, TObj> : ISearchSubsystem
	{
		protected SearchSubsystem(
			Control parent,
			SearchBar searchBar,
			UiConfigRepository uiConfigRepository,
			LuceneSearcher<TId, TObj> searcher,
			IDocumentAdapter<TId, TObj> adapter,
			params LayoutViewControl[] views)
		{
			_parent = parent;
			_searchBar = searchBar;
			_uiConfigRepository = uiConfigRepository;

			Searcher = searcher;
			_adapter = adapter;
			_views = views;

			_highlighter = new SearchStringHighlighter(_searchBar.Input);
			_highlighter.Highlight();
		}

		public void SubscribeToEvents()
		{
			_searchBar.Input.KeyDown += findKeyDown;
			_searchBar.Input.KeyUp += findKeyUp;

			_searchBar.Input.TextChanged += findTextChanged;
			_searchBar.Input.SelectionChanged += findSelectionChanged;
			_searchBar.Input.LocationChanged += findLocationChanged;
			_searchBar.Input.LostFocus += findLostFocus;

			_searchBar.MenuItemPressed += suggestPressed;
			_searchBar.MenuItemKeyUp += suggestKeyUp;

			foreach (var view in _views)
				view.SearchClicked += gridSearchClicked;

			ColorSchemeController.SystemColorsChanging += systemColorsChanged;

			_searchBar.Input.MouseUp += handleMouseClick;
			_searchBar.MouseUp += handleMouseClick;
		}

		private void handleMouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
				ResetText();
		}

		public bool ResetText()
		{
			if (string.IsNullOrEmpty(AppliedText))
				return false;

			AppliedText = string.Empty;
			Apply();
			return true;
		}

		public void UnsubscribeFromEvents()
		{
			_searchBar.Input.KeyDown -= findKeyDown;
			_searchBar.Input.KeyUp -= findKeyUp;
			_searchBar.Input.TextChanged -= findTextChanged;
			_searchBar.Input.SelectionChanged -= findSelectionChanged;
			_searchBar.Input.LocationChanged -= findLocationChanged;
			_searchBar.Input.LostFocus -= findLostFocus;

			_searchBar.MenuItemPressed -= suggestPressed;
			_searchBar.MenuItemKeyUp -= suggestKeyUp;

			foreach (var view in _views)
				view.SearchClicked -= gridSearchClicked;

			ColorSchemeController.SystemColorsChanging -= systemColorsChanged;

			_searchBar.Input.MouseUp -= handleMouseClick;
			_searchBar.MouseUp -= handleMouseClick;
		}

		public void SubscribeSuggestModelEvents()
		{
			SuggestModel.Suggested += suggested;
		}

		public void UnsubscribeSuggestModelEvents()
		{
			SuggestModel.Suggested -= suggested;
		}

		private void systemColorsChanged()
		{
			_searchBar.Input.TouchColorProperties();
			updateBackColor();
			updateForeColor();
			_highlighter.Highlight();
		}

		protected abstract string GetLanguage();

		public void StartThread()
		{
			if (_cts != null && !_cts.IsCancellationRequested)
				throw new InvalidOperationException("Already started");

			var cts = new CancellationTokenSource();
			TaskEx.Run(async () =>
			{
				const int delay = 1000;

				while (!cts.IsCancellationRequested)
				{
					updateBackColor();

					int deltaMs;
					bool isAutoApplyPending =
						_uiConfigRepository.Config.AutoApplySearchBar &&
						!_searchBar.IsPopupOpen &&
						_lastUserInput.HasValue &&
						!IsApplied;

					if (!isAutoApplyPending)
						deltaMs = delay;
					else
						deltaMs = delay - (int) (DateTime.Now - _lastUserInput.Value).TotalMilliseconds;

					if (deltaMs > 0)
						await TaskEx.Delay(deltaMs + 100);
					else
						_parent.Invoke(Apply);
				}
			});

			_cts = cts;
		}

		public void AbortThread() =>
			_cts?.Cancel();

		private void updateBackColor()
		{
			_parent.Invoke(delegate
			{
				var color = getBackColor();
				_searchBar.BackColor = color;
			});
		}

		private Color getBackColor()
		{
			if (SearchResult?.ParseErrorMessage != null)
			{
				var result = SystemColors.GradientInactiveCaption.TransformHsv(
					h: _ => _ + Color.LightSteelBlue.RotationTo(Color.LavenderBlush));

				return result;
			}

			if (!IsApplied || SearchResult?.IndexNotBuilt == true)
				return SystemColors.Control;

			return SystemColors.Window;
		}

		public bool IsApplied =>
			_currentText == _appliedText;


		private bool suggested(IntellisenseSuggest suggest, TextInputState source)
		{
			if (!_parent.Visible)
				return false;

			return _parent.Invoke(delegate { updateSuggestListBox(suggest, source); });
		}

		private void updateSuggestListBox(IntellisenseSuggest suggest, TextInputState source)
		{
			var values = suggest.Values.ToList();
			for (int i = 0; i < values.Count; i++)
				values[i] = values[i].Replace(Environment.NewLine, " ");

			lock (_syncSuggest)
			{
				_suggestSource = source;
				_suggestTypes = suggest.Types;
				_suggestToken = suggest.Token;

				_searchBar.SetMenuValues(values);
			}

			if (values.Count == 0)
				continueEditingAfterSuggest();
			else
				updateSuggestLocation();
		}

		private void showSuggest()
		{
			if (_searchBar.MenuItems.Count == 0)
				return;

			updateSuggestLocation();
			_searchBar.OpenPopup();
		}

		private void updateSuggestLocation()
		{
			int editedWordIndex = _suggestToken?.Position ?? _searchBar.Input.SelectionStart;
			var caretPosition =  _searchBar.Input.GetPositionFromCharIndex(editedWordIndex);

			var caretPositionAtForm = _searchBar.Input.PointToScreen(caretPosition);
			var bottomPositionAtForm = _searchBar.PointToScreen(new Point(0, _searchBar.Height));

			_searchBar.CustomMenuLocation = new Point(caretPositionAtForm.X + 4, bottomPositionAtForm.Y);
		}


		private void findSelectionChanged(object sender, EventArgs e) =>
			UpdateSuggestInput();

		private void findTextChanged(object sender, EventArgs e)
		{
			_currentText = _searchBar.Input.Text;

			UpdateSuggestInput();
			TextChanged?.Invoke();
		}

		private void findLocationChanged(object sender, EventArgs e)
		{
			updateSuggestLocation();
		}

		private void findLostFocus(object sender, EventArgs e)
		{
			if (!_searchBar.MenuControl.ContainsFocus)
				closeSuggest();
		}



		public void UpdateSuggestInput()
		{
			if (SuggestModel != null)
				SuggestModel.TextInputStateCurrent = getSearchInputState();
		}

		private TextInputState getSearchInputState() =>
			new TextInputState(
				_currentText,
				_searchBar.Input.SelectionStart,
				_searchBar.Input.SelectionLength);

		public void FocusSearch()
		{
			if (IsSearchFocused())
				return;

			_searchBar.Input.SelectionStart = _searchBar.Input.TextLength;
			focusSearch();
		}

		private void findKeyDown(object sender, KeyEventArgs e)
		{
			updateBackColor();
			_lastUserInput = DateTime.Now;

			switch (e.KeyData)
			{
				case Keys.Down:
					if (_searchBar.IsPopupOpen)
					{
						if (_searchBar.SelectedIndex < _searchBar.MenuItems.Count - 1)
							_searchBar.SelectedIndex++;
					}
					else
						showSuggest();

					e.Handled = true;
					break;

				case Keys.Up:
					if (_searchBar.IsPopupOpen)
					{
						if (_searchBar.SelectedIndex > 0)
							_searchBar.SelectedIndex--;
						else
							continueEditingAfterSuggest();
					}

					e.Handled = true;
					break;

				case Keys.Control | Keys.Down:
				case Keys.Alt | Keys.Down:
					cycleValue(backward: false);
					e.Handled = true;
					break;

				case Keys.Control | Keys.Up:
				case Keys.Alt | Keys.Up:
					cycleValue(backward: true);
					e.Handled = true;
					break;

				case Keys.Escape:
					if (_searchBar.IsPopupOpen)
						continueEditingAfterSuggest();
					else if (_appliedText != _searchBar.Input.Text)
					{
						var appliedText = _appliedText;
						ApplyDirtyText();

						setFindText(appliedText, appliedText.Length);
						Apply();
					}

					e.Handled = true;
					break;

				case Keys.Enter:
					if (!_searchBar.IsPopupOpen)
					{
						Apply();
					}
					else
					{
						selectSuggest();
						continueEditingAfterSuggest();
					}

					e.Handled = true;
					break;

				case Keys.Control | Keys.Space:
					showSuggest();
					e.Handled = true;
					e.SuppressKeyPress = true;
					break;

				case Keys.Control | Keys.V:
				case Keys.Shift | Keys.Insert:
					pasteFromClipboard();

					e.Handled = true;
					e.SuppressKeyPress = true;
					break;

				case Keys.PageDown:
				case Keys.PageUp:
					e.Handled = true;
					e.SuppressKeyPress = true;
					break;

				// prevent beep sound on attempt to move caret outside text bounds
				case Keys.Right:
				case Keys.End:
				case Keys.Control | Keys.Right:
				case Keys.Control | Keys.End:
					if (_searchBar.Input.SelectionStart == _searchBar.Input.TextLength)
					{
						e.Handled = true;
						e.SuppressKeyPress = true;
					}

					break;

				case Keys.Left:
				case Keys.Home:
				case Keys.Control | Keys.Left:
				case Keys.Control | Keys.Home:
					if (_searchBar.Input.SelectionStart == 0)
					{
						e.Handled = true;
						e.SuppressKeyPress = true;
					}

					break;
			}
		}

		public void ApplyDirtyText()
		{
			if (_appliedText != _searchBar.Input.Text)
				Apply();
		}

		private void cycleValue(bool backward)
		{
			if (!Searcher.Spellchecker.IsLoaded)
				return;

			var currentState = SuggestModel.TextInputStateCurrent;
			var cycleSuggest = CycleValue(currentState, backward);

			if (cycleSuggest == null)
				return;

			var type = cycleSuggest.Types[0];

			string value = type.IsAny(TokenType.FieldValue)
				? getValueExpression(cycleSuggest.Values[0])
				: cycleSuggest.Values[0];

			pasteText(value, type, currentState, cycleSuggest.Token, positionCaretToNextValue: false);
			Apply();
		}

		protected abstract IntellisenseSuggest CycleValue(TextInputState currentState, bool backward);

		private void pasteFromClipboard()
		{
			string text;

			try
			{
				text = Clipboard.GetText();
			}
			catch (ExternalException)
			{
				return;
			}

			if (string.IsNullOrEmpty(text))
				return;

			var preProcessedText = removeExtraWhitespaces(text);

			var source = getSearchInputState();
			var token = new MtgTolerantTokenizer(source.Text).GetTokenForArbitraryInsertion(source.Caret);
			pasteText(preProcessedText, TokenType.None, source, token, positionCaretToNextValue: false);
		}

		private void pasteText(string value, TokenType type, TextInputState source, Token token, bool positionCaretToNextValue)
		{
			int left, length;
			Token start, end;

			bool isValuePhrase = value.StartsWith("\"") && value.EndsWith("\"");

			if (token == null || source.SelectionLength > 0 || type == TokenType.None)
			{
				(start, end) = (null, null);
				(left, length) = (source.Caret, source.SelectionLength);
			}
			else
			{
				if (type == TokenType.FieldValue && token.IsPhrase && !_adapter.IsSuggestAnalyzedIn(token.ParentField, GetLanguage()) || isValuePhrase)
					(start, end) = (token.PhraseOpen, token.PhraseClose);
				else
					(start, end) = (token, token);

				(left, length) = (start.Position, end.Position + end.Value.Length - start.Position);
			}

			bool appendSpace = positionCaretToNextValue && end?.Next?.Type.IsAny(TokenType.AnyClose) != true;
			bool prependSpace = positionCaretToNextValue && start?.Previous?.Type.IsAny(TokenType.AnyOpen) == false;

			string prefix = source.Text.Substring(0, left);
			string suffix = source.Text.Substring(left + length);

			if (prependSpace)
			{
				prefix = prefix.TrimEnd();
				value = " " + value;
			}

			if (appendSpace)
			{
				suffix = suffix.TrimStart();
				value += " ";
			}

			if (type.IsAny(TokenType.Field) && suffix.StartsWith(":"))
				suffix = suffix.Substring(1);

			var replacement = prefix + value + suffix;
			int caret = prefix.Length + value.Length;

			if (!positionCaretToNextValue && isValuePhrase && type != TokenType.None)
				caret--;

			setFindText(replacement, caret);
			updateBackColor();

			_highlighter.Highlight();
		}

		private static string removeExtraWhitespaces(string text) =>
			RegexUtil.WhitespacePattern.Replace(text, " ");



		public virtual string GetFieldValueQuery(string fieldName, string fieldValue)
		{
			string valueExpression = getValueExpression(fieldValue);

			if (string.IsNullOrEmpty(valueExpression))
				return $"-{fieldName}: *";

			var builder = new StringBuilder();

			builder.Append(fieldName);
			builder.Append(": ");
			builder.Append(valueExpression);

			return builder.ToString();
		}

		private static string getValueExpression(string value)
		{
			string escaped = StringEscaper.Escape(value);

			if (!value.Any(char.IsWhiteSpace))
				return escaped;

			return $"\"{escaped}\"";
		}

		private void findKeyUp(object sender, KeyEventArgs e)
		{
			_lastUserInput = DateTime.Now;
			updateBackColor();
			UpdateSuggestInput();
		}



		private void suggestKeyUp(object sender, KeyEventArgs e)
		{
			_lastUserInput = DateTime.Now;

			if (e.KeyData == Keys.Up && _searchBar.SelectedIndex == 0)
				focusSearch();
			else if (e.KeyData == Keys.Escape)
				continueEditingAfterSuggest();
		}

		private void suggestPressed(object sender, MenuItemEventArgs e)
		{
			selectSuggest();
			continueEditingAfterSuggest();
		}


		private void continueEditingAfterSuggest()
		{
			_searchBar.ClosePopup();
			focusSearch();
		}

		private void closeSuggest()
		{
			if (_searchBar.IsPopupOpen)
				_searchBar.ClosePopup();
		}

		private void selectSuggest()
		{
			TextInputState source;
			TokenType type;
			Token token;

			int selectedIndex = _searchBar.SelectedIndex;
			if (selectedIndex < 0)
				return;

			string value = _searchBar.MenuValues[selectedIndex];
			if (string.IsNullOrEmpty(value))
				return;

			lock (_syncSuggest)
			{
				source = _suggestSource;

				if (!source.Equals(SuggestModel.TextInputStateCurrent))
					return;

				type = _suggestTypes[selectedIndex];
				token = _suggestToken;
			}

			if (type.IsAny(TokenType.FieldValue))
				value = getValueExpression(value);

			pasteText(value, type, source, token, positionCaretToNextValue: true);
		}

		private void setFindText(string text, int caret)
		{
			_searchBar.SuspendLayout();
			_searchBar.Input.Visible = false;

			_searchBar.Input.Text = text;
			_searchBar.Input.SelectionStart = caret;
			_searchBar.Input.SelectionLength = 0;

			_searchBar.Input.Visible = true;
			_searchBar.Input.ResumeLayout(false);
		}

		private void focusSearch()
		{
			int originalSelectionStart = _searchBar.Input.SelectionStart;
			int originalSelectionLength = _searchBar.Input.SelectionLength;

			_searchBar.Input.Focus();
			_searchBar.Input.SelectionStart = originalSelectionStart;
			_searchBar.Input.SelectionLength = originalSelectionLength;
		}

		public void Apply()
		{
			_appliedText = _currentText;

			updateSearchResult();
			updateBackColor();
			updateForeColor();

			_highlighter.Highlight();

			TextApplied?.Invoke();
		}

		private void updateForeColor()
		{
			if (SearchResult?.ParseErrorMessage == null)
				_searchBar.Input.ResetForeColor();
			else
				_searchBar.Input.ForeColor = SystemColors.HotTrack.TransformHsv(
					h: _ => _ + Color.Blue.RotationTo(Color.DarkRed));
		}

		private void updateSearchResult() =>
			SearchResult = string.IsNullOrWhiteSpace(_currentText)
				? null
				: Search(_currentText);

		protected abstract SearchResult<TId> Search(string query);

		public bool IsSearchFocused() =>
			_searchBar.ContainsFocus || _searchBar.MenuControl.ContainsFocus;

		private void gridSearchClicked(object view, SearchArgs searchArgs)
		{
			var query = GetFieldValueQuery(searchArgs.FieldName, searchArgs.FieldValue);
			var source = getSearchInputState();

			int queryStartIndex = source.Text.IndexOf(query, Str.Comparison);
			if (queryStartIndex >= 0)
				removeQueryFromInput(queryStartIndex, query, source);
			else
			{
				var token = new MtgTolerantTokenizer(source.Text).GetTokenForTermInsertion(source.Caret);
				pasteText(query, TokenType.Field, source, token, positionCaretToNextValue: true);
			}

			Apply();
		}



		private void removeQueryFromInput(int queryStartIndex, string query, TextInputState source)
		{
			string sourceText = source.Text;
			int caret = source.Caret;

			var (modifiedText, movedCaret) = removeSubstring(sourceText, caret, queryStartIndex, query.Length);
			(modifiedText, movedCaret) = trimStart(modifiedText, movedCaret);
			(modifiedText, movedCaret) = removeDuplicateWhitespaces(modifiedText, movedCaret);

			setFindText(modifiedText, movedCaret);
		}

		private static (string modifiedText, int movedCaret) removeSubstring(string sourceText, int caret, int queryStartIndex, int length)
		{
			int queryEndIndex = queryStartIndex + length;
			int movedCaret;

			if (caret >= queryEndIndex)
				movedCaret = caret - length;
			else if (caret > queryStartIndex)
				movedCaret = queryStartIndex;
			else
				movedCaret = caret;

			string modifiedText = sourceText.Substring(0, queryStartIndex) + sourceText.Substring(queryEndIndex);
			return (modifiedText, movedCaret);
		}

		private static (string modifiedText, int movedCaret) trimStart(string modifiedText, int movedCaret)
		{
			var trimmedStart = modifiedText.TrimStart();
			var deltaLength = modifiedText.Length - trimmedStart.Length;
			modifiedText = trimmedStart;
			movedCaret = Math.Max(0, movedCaret - deltaLength);
			return (modifiedText, movedCaret);
		}

		private static (string modifiedText, int movedCaret) removeDuplicateWhitespaces(string modifiedText, int movedCaret)
		{
			while (true)
			{
				int duplicateWhitespaceIndex = modifiedText.IndexOf("  ", Str.Comparison);

				if (duplicateWhitespaceIndex < 0)
					break;

				if (movedCaret > duplicateWhitespaceIndex)
					movedCaret--;

				modifiedText = modifiedText.Substring(0, duplicateWhitespaceIndex) + modifiedText.Substring(duplicateWhitespaceIndex + 1);
			}

			return (modifiedText, movedCaret);
		}



		public string AppliedText
		{
			get => _appliedText;
			set
			{
				_appliedText = value;
				_searchBar.Input.Text = value;
			}
		}

		public event Action TextApplied;
		public event Action TextChanged;

		private SuggestModel<TId, TObj> _suggestModel;
		public SuggestModel<TId, TObj> SuggestModel
		{
			get => _suggestModel;
			set
			{
				if (_suggestModel == value)
					return;

				_suggestModel = value;
				UpdateSuggestInput();
			}
		}

		public SearchResult<TId> SearchResult { get; private set; }

		ISearchResultBase ISearchSubsystem.SearchResult =>
			SearchResult;

		private string _appliedText;
		private DateTime? _lastUserInput;

		private string _currentText = string.Empty;

		private TextInputState _suggestSource;
		private IReadOnlyList<TokenType> _suggestTypes = Enumerable.Empty<TokenType>().ToReadOnlyList();
		private Token _suggestToken;

		private readonly Control _parent;
		private readonly SearchBar _searchBar;

		protected readonly LuceneSearcher<TId, TObj> Searcher;
		private readonly IDocumentAdapter<TId, TObj> _adapter;
		private readonly UiConfigRepository _uiConfigRepository;
		private readonly LayoutViewControl[] _views;
		private readonly SearchStringHighlighter _highlighter;

		private CancellationTokenSource _cts;
		private readonly object _syncSuggest = new object();
	}
}
