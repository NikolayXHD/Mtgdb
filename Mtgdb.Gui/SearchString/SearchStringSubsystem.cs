using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
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
		private readonly Form _parent;
		private readonly SuggestModel _suggestModel;
		private readonly RichTextBox _findEditor;
		private readonly Panel _panelSearchIcon;
		private readonly ListBox _listBoxSuggest;
		private readonly UiModel _uiModel;
		private readonly LuceneSearcher _searcher;

		private string _appliedText;
		private DateTime? _lastUserInput;
		private readonly Thread _idleInputMonitoringThread;

		public SearchResult SearchResult { get; private set; }
		private readonly BindingList<string> _dataSource = new BindingList<string>();

		private readonly SearchStringHighlighter _highligter;
		private string _currentText = string.Empty;
		
		public event Action TextApplied;
		public event Action TextChanged;
		
		public string AppliedText
		{
			get { return _appliedText; }
			set
			{
				_appliedText = value;
				_findEditor.Text = value;
			}
		}

		public SearchStringSubsystem(Form parent, SuggestModel suggestModel, RichTextBox findEditor, Panel panelSearchIcon, ListBox listBoxSuggest, UiModel uiModel, LuceneSearcher searcher)
		{
			_parent = parent;
			_suggestModel = suggestModel;

			_findEditor = findEditor;
			_panelSearchIcon = panelSearchIcon;
			//_findEditor.Properties.NullValuePrompt = Resources.FindEditor_Prompt;

			_listBoxSuggest = listBoxSuggest;
			_uiModel = uiModel;
			_searcher = searcher;

			_listBoxSuggest.Visible = false;
			_listBoxSuggest.Height = 0;

			_highligter = new SearchStringHighlighter(_findEditor);
			_highligter.Highlight();

			_listBoxSuggest.DataSource = _dataSource;
			_idleInputMonitoringThread = new Thread(idleInputMonitoringThread);
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
		}

		private static void findEditorMouseWheel(object sender, MouseEventArgs e)
		{
			SendKeys.Send(@"{Tab}");
		}

		private static int getHeight(string text)
		{
			int linesCount = 1;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '\n')
					linesCount++;
			}

			return 11*linesCount + 2;
		}

		public void StartThread()
		{
			_idleInputMonitoringThread.Start();
		}

		public void AbortThread()
		{
			_idleInputMonitoringThread.Abort();
		}

		private void languageChanged()
		{
			_suggestModel.LanguageCurrent = _uiModel.Language;
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

		private void suggested(IntellisenseSuggest intellisenseSuggest)
		{
			if (_parent.Visible)
				_findEditor.Invoke(delegate
				{
					updateSuggestListBox(intellisenseSuggest);
				});
		}

		private void updateSuggestListBox(IntellisenseSuggest intellisenseSuggest)
		{
			_listBoxSuggest.BeginUpdate();
			updateDataSource(intellisenseSuggest.Values);
			_listBoxSuggest.EndUpdate();

			if (intellisenseSuggest.Values.Length > 0)
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
			_suggestModel.SearhStateCurrent = new SearhStringState(_currentText, _findEditor.SelectionStart);
			_suggestModel.LanguageCurrent = _uiModel.Language;
		}

		private void parentKeyDown(object sender, KeyEventArgs e)
		{
			updateBackgroundColor();

			if (isSearchFocused())
				return;

			if (e.KeyCode == Keys.Enter && !_listBoxSuggest.Visible)
			{
				e.Handled = true;
				ApplyFind();
			}
		}


		private void findKeyDown(object sender, KeyEventArgs e)
		{
			updateBackgroundColor();
			_lastUserInput = DateTime.Now;
			
			if (e.KeyData == Keys.Down)
			{
				if (_listBoxSuggest.Visible)
				{
					if (_listBoxSuggest.SelectedIndex < _dataSource.Count - 1)
						_listBoxSuggest.SelectedIndex++;
				}
				else
					showSuggest();

				e.Handled = true;
			}
			else if (e.KeyData == Keys.Up)
			{
				if (_listBoxSuggest.Visible)
				{
					if (_listBoxSuggest.SelectedIndex > 0)
						_listBoxSuggest.SelectedIndex--;
					else
						closeSuggest();
				}

				e.Handled = true;
			}
			else if (e.KeyData == Keys.Escape)
			{
				if (_listBoxSuggest.Visible)
					closeSuggest();
				
				e.Handled = true;
			}
			else if (e.KeyData == Keys.Enter)
			{
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
			}
			else if (e.KeyData == (Keys.Control | Keys.Space))
			{
				showSuggest();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			else if (e.KeyData == (Keys.Control | Keys.V) || e.KeyData == (Keys.Shift | Keys.Insert))
			{
				if (Clipboard.ContainsText())
				{
					int selectionStart = _findEditor.SelectionStart;
					int suffixStart = selectionStart + _findEditor.SelectionLength;
					string text = _findEditor.Text;

					var builder = new StringBuilder();
					if (selectionStart >= 0)
						builder.Append(text.Substring(0, selectionStart));

					builder.Append(Clipboard.GetText().Replace(@"\r\n", @" ").Replace(@"\n", @" ").Replace(@"\r", @" "));

					int length = builder.Length;

					if (suffixStart < text.Length)
						builder.Append(text.Substring(suffixStart));


					_findEditor.Text = builder.ToString();
					_findEditor.SelectionStart = length;

					e.Handled = true;
					e.SuppressKeyPress = true;
				}
			}
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
				focusFind();
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
			focusFind();
			if (_listBoxSuggest.Visible)
				_listBoxSuggest.Visible = false;
		}

		private void selectSuggest()
		{
			var selectedSuggest = (string) _listBoxSuggest.SelectedItem;

			if (string.IsNullOrEmpty(selectedSuggest))
				return;

			applySuggestSelection(selectedSuggest);
		}

		private void applySuggestSelection(string selectedSuggest)
		{
			var token = _suggestModel.Token;
			var editParts = _suggestModel.SearhStateCurrent;

			int left = token?.Position ?? editParts.Caret;
			var length = token?.Value?.Length ?? 0;
			if (token?.Type.Is(TokenType.Field) == true)
				// Включим : в токен
				if (left + length < editParts.Text.Length)
					length++;

			string prefix = editParts.Text.Substring(0, left);
			string suffix = editParts.Text.Substring(left + length);

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

		private void focusFind()
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
				SearchResult = _searcher.Search(_currentText, _uiModel.Language);
			else
				SearchResult = null;
		}

		private bool isSearchFocused()
		{
			return _findEditor.ContainsFocus;
		}

		private void updateDataSource(string[] suggest)
		{
			_dataSource.Clear();
			foreach (var sugg in suggest)
				_dataSource.Add(sugg);

			if (_dataSource.Count == 0)
				closeSuggest();
		}
	}
}