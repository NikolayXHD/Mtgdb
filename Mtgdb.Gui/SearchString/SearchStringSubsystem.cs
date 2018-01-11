﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Lucene.Net.Contrib;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	public class SearchStringSubsystem
	{
		public SearchStringSubsystem(Form parent, SuggestModel suggestModel, RichTextBox findEditor, Panel panelSearchIcon, ListBox listBoxSuggest, UiModel uiModel, LuceneSearcher searcher, LayoutView viewCards)
		{
			_parent = parent;
			_suggestModel = suggestModel;

			_findEditor = findEditor;
			_panelSearchIcon = panelSearchIcon;

			_listBoxSuggest = listBoxSuggest;
			_uiModel = uiModel;
			_searcher = searcher;
			_viewCards = viewCards;

			_listBoxSuggest.Visible = false;
			_listBoxSuggest.Height = 0;

			_highligter = new SearchStringHighlighter(_findEditor);
			_highligter.Highlight();

			_listBoxSuggest.DataSource = _dataSource;
		}

		public void SubscribeToEvents()
		{
			_findEditor.KeyDown += findKeyDown;
			_findEditor.KeyUp += findKeyUp;
			
			_findEditor.TextChanged += findTextChanged;
			_findEditor.LocationChanged += findLocationChanged;

			_findEditor.MouseWheel += findEditorMouseWheel;

			_listBoxSuggest.Click += suggestClick;
			_listBoxSuggest.KeyUp += suggestKeyUp;

			_suggestModel.Suggested += suggested;
			
			_parent.KeyDown += parentKeyDown;
			_uiModel.Form.LanguageChanged += languageChanged;
			_viewCards.SearchClicked += gridSearchClicked;
		}

		public void UnsubscribeFromEvents()
		{
			_findEditor.KeyDown -= findKeyDown;
			_findEditor.KeyUp -= findKeyUp;
			_findEditor.TextChanged -= findTextChanged;
			_findEditor.LocationChanged -= findLocationChanged;
			_findEditor.MouseWheel -= findEditorMouseWheel;

			_listBoxSuggest.Click -= suggestClick;
			_listBoxSuggest.KeyUp -= suggestKeyUp;

			_suggestModel.Suggested -= suggested;
			
			_parent.KeyDown -= parentKeyDown;
			_uiModel.Form.LanguageChanged -= languageChanged;
			_viewCards.SearchClicked -= gridSearchClicked;
		}

		private static void findEditorMouseWheel(object sender, MouseEventArgs e)
		{
			SendKeys.Send(@"{Tab}");
		}

		private int getHeight(string text)
		{
			var fontSize = _findEditor.Font.SizeInPixels();
			int linesCount = 1 + text.Count(_ => _ == '\n');
			return (int) (fontSize * 1.1f * linesCount);
		}

		public void StartThread()
		{
			_idleInputMonitoringThread = new Thread(idleInputMonitoringThread);
			_idleInputMonitoringThread.Start();
		}

		public void AbortThread()
		{
			_idleInputMonitoringThread.Abort();
		}

		private void languageChanged()
		{
			_suggestModel.LanguageCurrent = _uiModel.Form.Language;
		}

		private void idleInputMonitoringThread()
		{
			const int delay = 1000;

			try
			{
				while (true)
				{
					updateBackgroundColor();

					int deltaMs;
					if (!_lastUserInput.HasValue || _listBoxSuggest.Visible || _currentText == _appliedText)
						deltaMs = delay;
					else
						deltaMs = delay - (int) (DateTime.Now - _lastUserInput.Value).TotalMilliseconds;

					if (deltaMs > 0)
						Thread.Sleep(deltaMs + 100);
					else
						_findEditor.Invoke(ApplyFind);
				}
			}
			catch (ThreadAbortException)
			{
			}
		}

		private void updateBackgroundColor()
		{
			_findEditor.Invoke(delegate
			{
				Color requiredColor;

				if (SearchResult?.ParseErrorMessage != null)
					requiredColor = Color.LavenderBlush;
				else if (_currentText != _appliedText || SearchResult?.IndexNotBuilt == true)
					requiredColor = Color.FromArgb(0xF0, 0xF0, 0xF0);
				else
					requiredColor = _panelSearchIcon.BackColor = Color.White;

				if (_findEditor.BackColor != requiredColor)
				{
					_findEditor.BackColor = 
					_findEditor.Parent.BackColor = 
					_panelSearchIcon.BackColor = requiredColor;

					_highligter.Highlight();
				}
			});
		}

		private void suggested(IntellisenseSuggest intellisenseSuggest, SearchStringState searchStringState)
		{
			if (_parent.Visible)
				_findEditor.Invoke(delegate
				{
					_suggestSource = searchStringState;
					updateSuggestListBox(intellisenseSuggest);
				});
		}

		private void updateSuggestListBox(IntellisenseSuggest intellisenseSuggest)
		{
			_listBoxSuggest.BeginUpdate();
			updateDataSource(intellisenseSuggest.Values);
			_listBoxSuggest.EndUpdate();

			if (intellisenseSuggest.Values.Count > 0)
				_listBoxSuggest.Height = 2 + intellisenseSuggest.Values.Sum(getHeight);
			else
				_listBoxSuggest.Height = 0;

			updateSuggestLocation();
		}

		private void showSuggest()
		{
			if (_dataSource.Count == 0)
				return;

			updateSuggestLocation();
			_listBoxSuggest.Visible = true;
		}

		private void updateSuggestLocation()
		{
			int editedWordIndex = _suggestModel.Token?.Position ?? _findEditor.SelectionStart;
			var caretPosition = _findEditor.GetPositionFromCharIndex(editedWordIndex);
			var caretPositionAtForm = _parent.PointToClient(_findEditor, caretPosition);

			var bottomPositionAtForm = _parent.PointToClient(_panelSearchIcon, new Point(0, _panelSearchIcon.Height));
			_listBoxSuggest.Location = new Point(caretPositionAtForm.X, bottomPositionAtForm.Y);
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
			_suggestModel.SearchStateCurrent = new SearchStringState(_currentText, _findEditor.SelectionStart);
			_suggestModel.LanguageCurrent = _uiModel.Form.Language;
		}

		private void parentKeyDown(object sender, KeyEventArgs e)
		{
			updateBackgroundColor();
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
			updateBackgroundColor();
			_lastUserInput = DateTime.Now;
			
			switch (e.KeyData)
			{
				case Keys.Down:
					if (_listBoxSuggest.Visible)
					{
						if (_listBoxSuggest.SelectedIndex < _dataSource.Count - 1)
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
						ApplyFind();
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
					if (Clipboard.ContainsText())
					{
						string searchQuery = clipboardTextToQuery(Clipboard.GetText());
						pasteSearchQuery(searchQuery);
					}

					e.Handled = true;
					e.SuppressKeyPress = true;
					break;
				case Keys.Control | Keys.Shift | Keys.Right:
				{
					break;
				}
				case Keys.Control | Keys.Shift | Keys.Left:
				{
					break;
				}
			}
		}

		private void pasteSearchQuery(string searchQuery)
		{
			int selectionStart = _findEditor.SelectionStart;
			int suffixStart = selectionStart + _findEditor.SelectionLength;
			string text = _findEditor.Text;

			var builder = new StringBuilder();
			if (selectionStart >= 0)
			{
				string prefix = text.Substring(0, selectionStart);
				builder.Append(prefix);
			}

			builder.Append(searchQuery);

			int length = builder.Length;

			if (suffixStart < text.Length)
			{
				string suffix = text.Substring(suffixStart);
				builder.Append(suffix);
			}

			_findEditor.Text = builder.ToString();
			_findEditor.SelectionStart = length;
		}

		private string clipboardTextToQuery(string text)
		{
			if (text.StartsWith(SearchStringMark))
				return _endLineRegex.Replace(text.Substring(SearchStringMark.Length), " ");

			string[] postfixes = 
			{
				".jpg",
				".xlhq",
				".full"
			};

			foreach (var postfix in postfixes)
				if (text.EndsWith(postfix, Str.Comparison))
					text = text.Substring(0, text.Length - postfix.Length);

			int endOfLine = text.IndexOf('\n');

			if (endOfLine >= 0)
			{
				string fieldName = text.Substring(0, endOfLine).Trim();
				
				if (DocumentFactory.UserFields.Contains(fieldName))
				{
					string fieldValue = text.Substring(endOfLine);
					return GetFieldValueQuery(fieldName, fieldValue);
				}
			}

			return getValueQuery(text);
		}

		public string GetFieldValueQuery(string fieldName, string fieldValue)
		{
			return fieldName + ": " + getValueQuery(fieldValue);
		}

		private static string getValueQuery(string text)
		{
			var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			if (lines.Length == 0)
				return string.Empty;

			var builder = new StringBuilder();

			if (lines.Length == 1 && lines[0].IndexOf(' ') < 0)
				builder.Append(StringEscaper.Escape(lines[0]));
			else if (lines.Length >= 1)
			{
				builder.Append('"');

				for (int i = 0; i < lines.Length; i++)
				{
					if (i > 0)
						builder.Append(' ');

					builder.Append(StringEscaper.Escape(lines[i]));
				}

				builder.Append('"');
			}

			return builder.ToString();
		}

		private void findKeyUp(object sender, KeyEventArgs e)
		{
			updateBackgroundColor();
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
			focusSearch();
			if (_listBoxSuggest.Visible)
				_listBoxSuggest.Visible = false;
		}

		private void selectSuggest()
		{
			var searchStringState = _suggestSource;

			if (!searchStringState.Equals(_suggestModel.SearchStateCurrent))
				return;

			var selectedSuggest = (string)_listBoxSuggest.SelectedItem;

			if (string.IsNullOrEmpty(selectedSuggest))
				return;

			selectedSuggest = StringEscaper.Escape(selectedSuggest);
			applySuggestSelection(selectedSuggest, searchStringState);
		}

		private void applySuggestSelection(string selectedSuggest, SearchStringState suggestSource)
		{
			var token = _suggestModel.Token;

			int left = token?.Position ?? suggestSource.Caret;
			var length = token?.Value?.Length ?? 0;
			if (token?.Type.Is(TokenType.Field) == true)
				// Включим : в токен
				if (left + length < suggestSource.Text.Length)
					length++;

			string prefix = suggestSource.Text.Substring(0, left);
			string suffix = suggestSource.Text.Substring(left + length);

			string rightTerminator;
			if (token?.Type.Is(TokenType.Field) == true)
				rightTerminator = @":";
			else
				rightTerminator = @" ";
			
			if (!suffix.StartsWith(rightTerminator) && !selectedSuggest.EndsWith(rightTerminator))
				selectedSuggest += rightTerminator;

			string leftTerminator;
			if (token?.Previous?.IsConnectedToCaret(token.Position) == true || token?.Previous?.Type.Is(TokenType.CloseQuote | TokenType.Close) == true)
				leftTerminator = @" ";
			else
				leftTerminator = string.Empty;

			if (!string.IsNullOrEmpty(prefix) && !string.IsNullOrEmpty(leftTerminator) && !prefix.StartsWith(leftTerminator))
				selectedSuggest = leftTerminator + selectedSuggest;

			var replacement = prefix + selectedSuggest + suffix;
				
			setFindText(replacement, left + selectedSuggest.Length);
			updateBackgroundColor();
		}

		private void setFindText(string text, int editedRight)
		{
			_findEditor.Text = text;
			_findEditor.SelectionStart = editedRight;
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

		public void ApplyFind()
		{
			_appliedText = _currentText;

			updateBackgroundColor();
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
				SearchResult = _searcher.Search(_currentText, _uiModel.Form.Language);
			else
				SearchResult = null;
		}

		public bool IsSearchFocused()
		{
			return _findEditor.ContainsFocus;
		}

		private void updateDataSource(IList<string> suggest)
		{
			_dataSource.Clear();
			foreach (var sugg in suggest)
				_dataSource.Add(sugg);

			if (_dataSource.Count == 0)
				closeSuggest();
		}

		private void gridSearchClicked(object view, SearchArgs searchArgs)
		{
			string query = GetFieldValueQuery(searchArgs.FieldName, searchArgs.FieldValue);

			if (searchArgs.UseAndOperator)
				query = "+" + query;

			if (_findEditor.TextLength > 0)
			{
				var caret = _findEditor.SelectionStart;

				bool leftIsBoundary = caret == 0 || caret > 0 && char.IsWhiteSpace(_findEditor.Text[caret - 1]);
				bool isRightBoundary = caret == _findEditor.TextLength || caret < _findEditor.TextLength && char.IsWhiteSpace(_findEditor.Text[caret]);

				if (!leftIsBoundary)
					query = " " + query;

				if (!isRightBoundary)
					query += " ";
			}

			pasteSearchQuery(query);
			ApplyFind();
		}


		public string AppliedText
		{
			get { return _appliedText; }
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

		private readonly Form _parent;
		private readonly SuggestModel _suggestModel;
		private readonly RichTextBox _findEditor;
		private readonly Panel _panelSearchIcon;
		private readonly ListBox _listBoxSuggest;
		private readonly UiModel _uiModel;
		private readonly LuceneSearcher _searcher;
		private readonly LayoutView _viewCards;

		private string _appliedText;
		private DateTime? _lastUserInput;
		private Thread _idleInputMonitoringThread;

		public SearchResult SearchResult { get; private set; }
		private readonly BindingList<string> _dataSource = new BindingList<string>();

		private readonly SearchStringHighlighter _highligter;
		private string _currentText = string.Empty;
		private SearchStringState _suggestSource;
	}
}