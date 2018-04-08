using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Lucene.Net.Contrib;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Mtgdb.Controls;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Gui
{
	public class SearchStringSubsystem
	{
		public SearchStringSubsystem(
			Form parent,
			RichTextBox findEditor,
			Panel panelSearchIcon,
			ListBox listBoxSuggest,
			LuceneSearcher searcher,
			MtgLayoutView viewCards,
			MtgLayoutView viewDeck)
		{
			_parent = parent;

			_findEditor = findEditor;
			_panelSearchIcon = panelSearchIcon;

			_listBoxSuggest = listBoxSuggest;

			_searcher = searcher;
			_viewCards = viewCards;
			_viewDeck = viewDeck;

			_listBoxSuggest.Visible = false;
			_listBoxSuggest.Height = 0;

			_highligter = new SearchStringHighlighter(_findEditor);
			_highligter.Highlight();

			_listBoxSuggest.DataSource = _suggestValues;
			_analyzer = new MtgAnalyzer();
		}

		public void SubscribeToEvents()
		{
			_findEditor.KeyDown += findKeyDown;
			_findEditor.KeyUp += findKeyUp;

			_findEditor.TextChanged += findTextChanged;
			_findEditor.SelectionChanged += findSelectionChanged;
			_findEditor.LocationChanged += findLocationChanged;

			_listBoxSuggest.Click += suggestClick;
			_listBoxSuggest.KeyUp += suggestKeyUp;

			_parent.KeyDown += parentKeyDown;
			_viewCards.SearchClicked += gridSearchClicked;
			_viewDeck.SearchClicked += gridSearchClicked;
		}

		public void UnsubscribeFromEvents()
		{
			_findEditor.KeyDown -= findKeyDown;
			_findEditor.KeyUp -= findKeyUp;
			_findEditor.TextChanged -= findTextChanged;
			_findEditor.SelectionChanged -= findSelectionChanged;
			_findEditor.LocationChanged -= findLocationChanged;

			_listBoxSuggest.Click -= suggestClick;
			_listBoxSuggest.KeyUp -= suggestKeyUp;

			_parent.KeyDown -= parentKeyDown;
			_viewCards.SearchClicked -= gridSearchClicked;
			_viewDeck.SearchClicked -= gridSearchClicked;
		}

		public void SubscribeSuggestModelEvents()
		{
			SuggestModel.Suggested += suggested;
		}

		public void UnsubscribeSuggestModelEvents()
		{
			SuggestModel.Suggested -= suggested;
		}



		private string getLanguage()
		{
			return Ui.LanguageController.Language;
		}

		private int getHeight(string text)
		{
			var fontSize = _findEditor.Font.SizeInPixels();
			int linesCount = 1 + text.Count(_ => _ == '\n');
			return (int) (fontSize * 1.1f * linesCount);
		}

		public void StartThread()
		{
			if (_idleInputMonitoringThread?.ThreadState == ThreadState.Running)
				throw new InvalidOperationException("Already started");

			_idleInputMonitoringThread = new Thread(idleInputMonitoringThread);
			_idleInputMonitoringThread.Start();
		}

		public void AbortThread()
		{
			_idleInputMonitoringThread.Abort();
		}

		private void idleInputMonitoringThread()
		{
			const int delay = 1000;

			try
			{
				while (true)
				{
					updateBackColor();

					int deltaMs;
					if (!_lastUserInput.HasValue || _listBoxSuggest.Visible || _currentText == _appliedText)
						deltaMs = delay;
					else
						deltaMs = delay - (int) (DateTime.Now - _lastUserInput.Value).TotalMilliseconds;

					if (deltaMs > 0)
						Thread.Sleep(deltaMs + 100);
					else
						_findEditor.Invoke(Apply);
				}
			}
			catch (ThreadAbortException)
			{
			}
		}

		private void updateBackColor()
		{
			_findEditor.Invoke(delegate
			{
				var color = getBackColor();

				if (_findEditor.BackColor != color)
					_findEditor.BackColor =
						_findEditor.Parent.BackColor =
							_panelSearchIcon.BackColor = color;
			});
		}

		private Color getBackColor()
		{
			Color requiredColor;

			if (SearchResult?.ParseErrorMessage != null)
				requiredColor = Color.LavenderBlush;
			else if (_currentText != _appliedText || SearchResult?.IndexNotBuilt == true)
				requiredColor = Color.FromArgb(0xF0, 0xF0, 0xF0);
			else
				requiredColor = _panelSearchIcon.BackColor = Color.White;
			return requiredColor;
		}



		private void suggested(IntellisenseSuggest suggest, SearchInputState source)
		{
			if (!_parent.Visible)
				return;

			_parent.Invoke(delegate { updateSuggestListBox(suggest, source); });
		}

		private void updateSuggestListBox(IntellisenseSuggest suggest, SearchInputState source)
		{
			lock (_syncSuggest)
			{
				_suggestSource = source;
				_suggestTypes = suggest.Types;
				_suggestToken = suggest.Token;

				_listBoxSuggest.BeginUpdate();

				_suggestValues.Clear();
				foreach (string sugg in suggest.Values)
					_suggestValues.Add(sugg);

				_listBoxSuggest.EndUpdate();
			}

			if (_suggestValues.Count == 0)
			{
				_listBoxSuggest.Height = 0;
				closeSuggest();
			}
			else
			{
				_listBoxSuggest.Height = 2 + suggest.Values.Sum(getHeight);
				updateSuggestLocation();
			}
		}

		private void showSuggest()
		{
			if (_suggestValues.Count == 0)
				return;

			updateSuggestLocation();
			_listBoxSuggest.Visible = true;
		}

		private void updateSuggestLocation()
		{
			int editedWordIndex = _suggestToken?.Position ?? _findEditor.SelectionStart;
			var caretPosition = _findEditor.GetPositionFromCharIndex(editedWordIndex);
			var caretPositionAtForm = _parent.PointToClient(_findEditor, caretPosition);

			var bottomPositionAtForm = _parent.PointToClient(_panelSearchIcon, new Point(0, _panelSearchIcon.Height));
			_listBoxSuggest.Location = new Point(caretPositionAtForm.X, bottomPositionAtForm.Y);
		}


		private void findSelectionChanged(object sender, EventArgs e)
		{
			if (_highligter.HighlightingInProgress)
				return;

			UpdateSuggestInput();
		}

		private void findTextChanged(object sender, EventArgs e)
		{
			if (_highligter.HighlightingInProgress)
				return;

			_currentText = _findEditor.Text;

			UpdateSuggestInput();
			_highligter.Highlight();
			TextChanged?.Invoke();
		}

		private void findLocationChanged(object sender, EventArgs e)
		{
			updateSuggestLocation();
		}

		public void UpdateSuggestInput()
		{
			SuggestModel.SearchInputStateCurrent = getSearchInputState();
		}

		private SearchInputState getSearchInputState() => new SearchInputState(
			_currentText,
			_findEditor.SelectionStart,
			_findEditor.SelectionLength);

		private void parentKeyDown(object sender, KeyEventArgs e)
		{
			updateBackColor();
		}

		public void FocusSearch()
		{
			if (IsSearchFocused())
				return;

			_findEditor.SelectionStart = _findEditor.TextLength;
			focusSearch();
		}

		private void findKeyDown(object sender, KeyEventArgs e)
		{
			updateBackColor();
			_lastUserInput = DateTime.Now;

			switch (e.KeyData)
			{
				case Keys.Down:
					if (_listBoxSuggest.Visible)
					{
						if (_listBoxSuggest.SelectedIndex < _suggestValues.Count - 1)
							_listBoxSuggest.SelectedIndex++;
					}
					else
						showSuggest();

					e.Handled = true;
					break;

				case Keys.Up:
					if (_listBoxSuggest.Visible)
					{
						if (_listBoxSuggest.SelectedIndex > 0)
							_listBoxSuggest.SelectedIndex--;
						else
							closeSuggest();
					}

					e.Handled = true;
					break;

				case Keys.Escape:
					if (_listBoxSuggest.Visible)
						closeSuggest();

					e.Handled = true;
					break;

				case Keys.Enter:
					if (!_listBoxSuggest.Visible)
					{
						Apply();
					}
					else
					{
						selectSuggest();
						closeSuggest();
					}

					e.Handled = true;
					break;

				case Keys.Control | Keys.Space:
					showSuggest();
					e.Handled = true;
					e.SuppressKeyPress = true;
					break;

				case Keys.Control | Keys.X:
				case Keys.Shift | Keys.Delete:
					if (!string.IsNullOrEmpty(_findEditor.SelectedText))
					{
						Clipboard.SetText(SearchStringMark + _findEditor.SelectedText);

						var prefix = _findEditor.Text.Substring(0, _findEditor.SelectionStart);
						int suffixStart = _findEditor.SelectionStart + _findEditor.SelectionLength;
						var suffix = suffixStart < _findEditor.Text.Length
							? _findEditor.Text.Substring(suffixStart)
							: string.Empty;

						setFindText(prefix + suffix, prefix.Length);
					}

					e.Handled = true;
					e.SuppressKeyPress = true;
					break;

				case Keys.Control | Keys.C:
				case Keys.Control | Keys.Insert:
					if (!string.IsNullOrEmpty(_findEditor.SelectedText))
						Clipboard.SetText(SearchStringMark + _findEditor.SelectedText);

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
					if (_findEditor.SelectionStart == _findEditor.TextLength)
					{
						e.Handled = true;
						e.SuppressKeyPress = true;
					}

					break;

				case Keys.Shift | Keys.Right:
				case Keys.Shift | Keys.End:
				case Keys.Shift | Keys.Control | Keys.Right:
				case Keys.Shift | Keys.Control | Keys.End:
					if (_findEditor.SelectionStart + _findEditor.SelectionLength == _findEditor.TextLength)
					{
						e.Handled = true;
						e.SuppressKeyPress = true;
					}

					break;

				case Keys.Left:
				case Keys.Home:
				case Keys.Control | Keys.Left:
				case Keys.Control | Keys.Home:
				case Keys.Shift | Keys.Left:
				case Keys.Shift | Keys.Home:
				case Keys.Shift | Keys.Control | Keys.Left:
				case Keys.Shift | Keys.Control | Keys.Home:
					if (_findEditor.SelectionStart == 0)
					{
						e.Handled = true;
						e.SuppressKeyPress = true;
					}

					break;
			}
		}



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

			if (string.IsNullOrEmpty(text))
				throw new ArgumentException();

			text = text.Trim();

			var (preProcessedText, type) = classifyPastedText(text);

			var source = getSearchInputState();
			var token = EditedTokenLocator.GetTokenForArbitraryInsertion(source.Text, source.Caret);
			pasteText(preProcessedText, type, source, token);
		}

		private (string PreProcessedText, TokenType Type) classifyPastedText(string text)
		{
			if (text.StartsWith(SearchStringMark))
				return (preProcessTextCopiedFromSearchInput(text), TokenType.None);

			var (fieldName, fieldValue) = parseCopiedFromTooltip(text);

			if (fieldName != null)
				return (GetFieldValueQuery(fieldName, fieldValue), TokenType.None);

			return (preProcessArbitraryPastedText(text), TokenType.FieldValue);
		}

		private static string preProcessTextCopiedFromSearchInput(string text)
		{
			return _endLineRegex.Replace(text.Substring(SearchStringMark.Length), " ");
		}

		private static string preProcessArbitraryPastedText(string text)
		{
			string[] postfixes =
			{
				".jpg",
				".xlhq",
				".full"
			};

			foreach (var postfix in postfixes)
				if (text.EndsWith(postfix, Str.Comparison))
					text = text.Substring(0, text.Length - postfix.Length);

			return text;
		}

		private static (string FieldName, string FieldValue) parseCopiedFromTooltip(string text)
		{
			int endOfLine = text.IndexOf('\n');

			if (endOfLine < 0)
				return (null, null);

			string fieldName = text.Substring(0, endOfLine).Trim();

			if (!DocumentFactory.UserFields.Contains(fieldName))
				return (null, null);

			string fieldValue = text.Substring(endOfLine);

			return (fieldName, fieldValue);
		}

		private void pasteText(string value, TokenType type, SearchInputState source, Token token)
		{
			int left;
			int length;

			if (token != null && source.SelectionLength == 0)
			{
				left = token.Position;
				length = token.Value.Length;
			}
			else
			{
				left = source.Caret;
				length = source.SelectionLength;
			}

			string prefix = source.Text.Substring(0, left).Trim();
			string suffix = source.Text.Substring(left + length).Trim();

			if (type.IsAny(TokenType.Field))
			{
				if (suffix.StartsWith(":"))
					suffix = suffix.Substring(1);
			}
			else if (type.IsAny(TokenType.FieldValue))
			{
				bool isInsidePhrase = token?.IsPhrase == true || token?.Type.IsAny(TokenType.OpenQuote) == true;

				lock (_syncAnalyzer)
					value = _analyzer.GetValueExpression(token?.ParentField, value);

				if (isInsidePhrase && value.StartsWith("\"") && value.EndsWith("\""))
					value = value.Substring(1, value.Length - 2);
			}

			if (token?.Previous != null && !token.Previous.Type.IsAny(TokenType.AnyOpen))
				value = " " + value;

			if (token?.Next == null || !token.Next.Type.IsAny(TokenType.AnyClose))
				value += " ";

			var replacement = prefix + value + suffix;
			int caret = left + value.Length;

			setFindText(replacement, caret: caret);
			updateBackColor();
		}



		public string GetFieldValueQuery(string fieldName, string fieldValue, bool useAndOperator = false)
		{
			if (fieldName == nameof(Card.Image))
				fieldName = MtgQueryParser.Like;

			string valueExpression;

			lock (_syncAnalyzer)
				valueExpression = _analyzer.GetValueExpression(fieldName, fieldValue);

			if (string.IsNullOrEmpty(valueExpression))
				return $"-{fieldName}:*";

			var builder = new StringBuilder();

			if (useAndOperator)
				builder.Append('+');

			builder.Append(fieldName);
			builder.Append(':');

			builder.Append(valueExpression);

			return builder.ToString();
		}

		private void findKeyUp(object sender, KeyEventArgs e)
		{
			updateBackColor();
			_lastUserInput = DateTime.Now;
			UpdateSuggestInput();
		}



		private void suggestKeyUp(object sender, KeyEventArgs e)
		{
			_lastUserInput = DateTime.Now;
			if (e.KeyCode == Keys.Enter)
			{
				selectSuggest();
				closeSuggest();
			}
			else if (e.KeyCode == Keys.Up && _listBoxSuggest.SelectedIndex == 0)
				focusSearch();
			else if (e.KeyCode == Keys.Escape)
				closeSuggest();
		}

		private void suggestClick(object sender, EventArgs e)
		{
			selectSuggest();
			closeSuggest();
		}


		private void closeSuggest()
		{
			if (_listBoxSuggest.Visible)
			{
				_listBoxSuggest.Visible = false;
				focusSearch();
			}
		}

		private void selectSuggest()
		{
			SearchInputState source;
			string value;
			TokenType type;
			Token token;

			lock (_syncSuggest)
			{
				source = _suggestSource;

				if (!source.Equals(SuggestModel.SearchInputStateCurrent))
					return;

				int selectedIndex = _listBoxSuggest.SelectedIndex;

				if (selectedIndex < 0)
					return;

				value = _suggestValues[selectedIndex];
				type = _suggestTypes[selectedIndex];
				token = _suggestToken;
			}

			if (string.IsNullOrEmpty(value))
				return;

			pasteText(value, type, source, token);
		}

		private void setFindText(string text, int caret)
		{
			_findEditor.Text = text;
			_findEditor.SelectionStart = caret;
			_findEditor.SelectionLength = 0;
		}

		private void focusSearch()
		{
			int originalSelectionStart = _findEditor.SelectionStart;
			int originalSelectionLength = _findEditor.SelectionLength;

			_findEditor.Focus();
			_findEditor.SelectionStart = originalSelectionStart;
			_findEditor.SelectionLength = originalSelectionLength;
		}

		public void Apply()
		{
			_appliedText = _currentText;

			updateBackColor();
			updateSearchResult();

			if (SearchResult?.ParseErrorMessage == null)
				_findEditor.ResetForeColor();
			else
				_findEditor.ForeColor = Color.DarkRed;

			TextApplied?.Invoke();
		}

		private void updateSearchResult()
		{
			if (!string.IsNullOrWhiteSpace(_currentText))
				SearchResult = _searcher.Search(_currentText, getLanguage());
			else
				SearchResult = null;
		}

		public bool IsSearchFocused()
		{
			return _findEditor.ContainsFocus;
		}

		private void gridSearchClicked(object view, SearchArgs searchArgs)
		{
			var query = GetFieldValueQuery(searchArgs.FieldName, searchArgs.FieldValue, searchArgs.UseAndOperator);
			var source = getSearchInputState();

			int queryStartIndex = source.Text.IndexOf(query, Str.Comparison);
			if (queryStartIndex >= 0)
				removeQueryFromInput(queryStartIndex, query, source);
			else
			{
				var token = EditedTokenLocator.GetTokenForTermInsertion(source.Text, source.Caret);
				pasteText(query, TokenType.None, source, token);
			}

			Apply();
		}



		private void removeQueryFromInput(int queryStartIndex, string query, SearchInputState source)
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
				_findEditor.Text = value;
			}
		}

		public event Action TextApplied;
		public event Action TextChanged;

		private const string SearchStringMark = "search: ";
		private static readonly Regex _endLineRegex = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled | RegexOptions.Singleline);

		public SuggestModel SuggestModel { get; set; }

		public UiModel Ui { get; set; }

		public SearchResult SearchResult { get; private set; }

		private string _appliedText;
		private DateTime? _lastUserInput;
		private Thread _idleInputMonitoringThread;

		private string _currentText = string.Empty;

		private SearchInputState _suggestSource;
		private readonly BindingList<string> _suggestValues = new BindingList<string>();
		private IReadOnlyList<TokenType> _suggestTypes = Enumerable.Empty<TokenType>().ToReadOnlyList();
		private Token _suggestToken;

		private readonly Form _parent;
		private readonly RichTextBox _findEditor;
		private readonly Panel _panelSearchIcon;
		private readonly ListBox _listBoxSuggest;
		private readonly LuceneSearcher _searcher;
		private readonly MtgLayoutView _viewCards;
		private readonly MtgLayoutView _viewDeck;
		private readonly SearchStringHighlighter _highligter;
		private readonly MtgAnalyzer _analyzer;

		private readonly object _syncSuggest = new object();
		private readonly object _syncAnalyzer = new object();
	}
}