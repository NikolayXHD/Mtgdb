using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Ini.Parsing
{
	internal class ParseContext
	{
		private char _beginComment = ';';
		private char _beginSection = '[';
		private char _endSection = ']';
		private char _keyValueSeparator = '=';
		private char _newLine = '\n';
		private char _carriageReturn = '\r';
		private char _textQuote = '\"';

		private LinkedList<Section> _sections = new LinkedList<Section>();

		private StringBuilder _tokenValue = new StringBuilder();
		private string _curentKey = null;
		private ParseState _state = ParseState.BeginLine;

		public ParseContext()
		{
			_sections.AddLast(new Section(string.Empty));
		}

		public void ParseSource(IEnumerable<char> chars)
		{
			if (_state != ParseState.BeginLine)
				throw new NotImplementedException("unexpected state: " + _state.ToString());

			foreach (char ch in chars)
				_state = appendChar(ch);

			endChars();
		}

		private void endChars()
		{
			switch (_state)
			{
				case ParseState.BeginLine:
				case ParseState.EmptyLine:
				case ParseState.Comment:
				case ParseState.SectionEnd:
				case ParseState.EndQuotedValue:
					return;

				case ParseState.SectionName:
					throw new FormatException("unexpected end in section name");

				case ParseState.KeyName:
				case ParseState.EndKeyName:
					throw new FormatException("unexpected end line in key name");

				case ParseState.BeginValue:
				case ParseState.SimpleValue:
				case ParseState.EscValue:
					endValue();
					return;

				case ParseState.QuotedValue:
					throw new FormatException("unexpected end line in quoted value");

				default:
					throw new NotImplementedException("unexpected state: " + _state.ToString());
			}
		}

		private ParseState appendChar(char ch)
		{
			//Console.WriteLine("{0} <{1}>", _state, ch);
			switch (_state)
			{
				case ParseState.BeginLine:
					return processBeginLine(ch);

				case ParseState.EmptyLine:
					return processEmptyLine(ch);

				case ParseState.Comment:
					return processComment(ch);

				case ParseState.SectionName:
					return processSectionName(ch);

				case ParseState.SectionEnd:
					return processSectionEnd(ch);

				case ParseState.KeyName:
					return processKeyName(ch);

				case ParseState.EndKeyName:
					return processEndKeyName(ch);

				case ParseState.BeginValue:
					return processBeginValue(ch);

				case ParseState.SimpleValue:
					return processSimpleValue(ch);

				case ParseState.QuotedValue:
					return processQuotedValue(ch);

				case ParseState.EscValue:
					return processEscValue(ch);

				case ParseState.EndQuotedValue:
					return processEndQuotedValue(ch);

				default:
					throw new NotImplementedException("unexpected state: " + _state.ToString());
			}
		}

		private ParseState processEndQuotedValue(char ch)
		{
			if(ch == _beginComment)
				return ParseState.Comment;

			if(ch == _newLine || ch == _carriageReturn)
				return ParseState.BeginLine;

			if(Char.IsWhiteSpace(ch))
				return ParseState.EndQuotedValue;

			throw new FormatException(string.Format("unexpected char '{0}' after '{1}'", ch, _textQuote));
		}

		private ParseState processEscValue(char ch)
		{
			if(ch == _textQuote)
			{
				_tokenValue.Append(ch);
				return ParseState.QuotedValue;
			}

			if(ch == _beginComment)
			{
				endValue();
				return ParseState.Comment;
			}

			if(ch == _newLine || ch == _carriageReturn)
			{
				endValue();
				return ParseState.BeginLine;
			}

			if(Char.IsWhiteSpace(ch))
			{
				endValue();
				return ParseState.EndQuotedValue;
			}

			throw new FormatException(string.Format("unexpected char '{0}' after '{1}'", ch, _textQuote));
		}

		private ParseState processQuotedValue(char ch)
		{
			if(ch == _textQuote)
				return ParseState.EscValue;

			// *
			_tokenValue.Append(ch);
			return ParseState.QuotedValue;
		}

		private ParseState processSimpleValue(char ch)
		{
			if (ch == _beginComment)
			{
				endValue();
				return ParseState.Comment;
			}

			if (ch == _newLine || ch == _carriageReturn)
			{
				endValue();
				return ParseState.BeginLine;
			}

			// *
			_tokenValue.Append(ch);
			return ParseState.SimpleValue;
		}

		private ParseState processBeginValue(char ch)
		{
			if (Char.IsWhiteSpace(ch))
				return ParseState.BeginValue;

			if (ch == _newLine || ch == _carriageReturn)
			{
				endValue();
				return ParseState.BeginLine;
			}

			if (ch == _textQuote)
				return ParseState.QuotedValue;

			// *
			_tokenValue.Append(ch);
			return ParseState.SimpleValue;
		}

		private void endValue()
		{
			var value = _tokenValue.ToString();
			_tokenValue.Clear();

			_sections.Last.Value.Pairs.Add(new KeyValuePair<string,string>(_curentKey, value));
		}

		private ParseState processEndKeyName(char ch)
		{
			if (ch == _newLine || ch == _carriageReturn)
				throw new FormatException("unexpected end line in key name");

			if (Char.IsWhiteSpace(ch))
				return ParseState.EndKeyName;

			if (ch == _keyValueSeparator)
				return ParseState.BeginValue;

			throw new FormatException(string.Format("unexpected char '{0}' before '{1}'", ch, _keyValueSeparator));
		}

		private ParseState processKeyName(char ch)
		{
			if (ch == _newLine || ch == _carriageReturn)
				throw new FormatException("unexpected end line in key name");

			if (Char.IsWhiteSpace(ch))
			{
				keyNameEnd();
				return ParseState.EndKeyName;
			}

			if (ch == _keyValueSeparator)
			{
				keyNameEnd();
				return ParseState.BeginValue;
			}

			// *
			_tokenValue.Append(ch);
			return ParseState.KeyName;
		}

		private void keyNameEnd()
		{
			_curentKey = _tokenValue.ToString();
			_tokenValue.Clear();
		}

		private ParseState processSectionEnd(char ch)
		{
			if (ch == _newLine || ch == _carriageReturn)
				return ParseState.BeginLine;

			if (ch == _beginComment)
				return ParseState.Comment;

			if (Char.IsWhiteSpace(ch))
				return ParseState.SectionEnd;

			throw new FormatException(string.Format("non white char '{0}' after section name", ch));
		}

		private ParseState processSectionName(char ch)
		{
			if (ch == _newLine || ch == _carriageReturn)
				throw new FormatException("unexpected end line in section name");


			if (ch == _endSection)
			{
				endSectionName();
				return ParseState.SectionEnd;
			}

			_tokenValue.Append(ch);
			return ParseState.SectionName;
		}

		private void endSectionName()
		{
			var sectionName = _tokenValue.ToString();
			_sections.AddLast(new Section(sectionName));
			_tokenValue.Clear();
		}

		private ParseState processComment(char ch)
		{
			if (ch == _newLine)
				return ParseState.BeginLine;

			return ParseState.Comment;
		}

		private ParseState processEmptyLine(char ch)
		{
			if (ch == _newLine)
				return ParseState.BeginLine;

			if (Char.IsWhiteSpace(ch))
				return ParseState.EmptyLine;

			throw new FormatException(string.Format("non white char '{0}' in empty line", ch));
		}

		private ParseState processBeginLine(char ch)
		{
			if (ch == _newLine)
				return ParseState.BeginLine;

			if (ch == _carriageReturn)
				return ParseState.BeginLine;

			if (Char.IsWhiteSpace(ch))
				return ParseState.EmptyLine;

			if (ch == _beginComment)
				return ParseState.Comment;

			if (ch == _beginSection)
				return ParseState.SectionName;

			// *
			_tokenValue.Append(ch);
			return ParseState.KeyName;
		}

		public IEnumerable<Section> Sections
		{
			get
			{
				if (_sections.First.Value.Name == string.Empty &&
					_sections.First.Value.Pairs.Count == 0)
					return _sections.Skip(1);

				return _sections;
			}
		}
	}
}
