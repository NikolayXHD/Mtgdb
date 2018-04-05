using System;
using System.Collections.Generic;

namespace Mtgdb.Controls
{
	internal class RichTextTokenReader
	{
		private readonly IList<TextRange> _matches;
		private readonly string _text;
		private readonly int _len;
		private int _m;
		private int _i;
		private bool _matchStarted;
		private bool _matchIsContext;

		public RichTextTokenReader(string text, IList<TextRange> matches)
		{
			_text = text;
			_i = 0;
			_len = text.Length;

			_matches = matches;
			_m = 0;
		}
		
		public RichTextToken Current;

		public bool ReadToken()
		{
			if (_i == _len)
				return false;

			if (_i == 0)
				updateMatchState();

			if (_text[_i] == ' ')
			{
				readSpace();
				return true;
			}

			if (_text[_i] == '\r' || _text[_i] == '\n')
			{
				readNewLine();
				return true;
			}

			readWord();
			return true;
		}

		private void readWord()
		{
			int start = _i;

			bool matchStartedOriginal = _matchStarted;
			bool matchIsContextOriginal = _matchIsContext;

			while (true)
			{
				_i++;
				updateMatchState();

				if (_i == _len || _text[_i] == ' ' || _text[_i] == '\r' || _text[_i] == '\n' || _matchStarted != matchStartedOriginal || _matchIsContext != matchIsContextOriginal || _text[_i - 1].IsCjk())
				{
					Current = new RichTextToken
					{
						Index = start,
						Length = _i - start,
						Type = RichTextTokenType.Word,
						IsHighlighted = matchStartedOriginal,
						IsContext = matchIsContextOriginal
					};

					break;
				}
			}
		}

		private void readNewLine()
		{
			if (_text[_i] == '\r')
			{
				if (_i + 1 >= _len || _text[_i + 1] != '\n')
					throw new Exception(@"invalid line end");

				Current = new RichTextToken
				{
					Index = _i,
					Length = 2,
					Type = RichTextTokenType.NewLine,
					IsHighlighted = _matchStarted,
					IsContext = _matchIsContext
				};

				_i++;
				updateMatchState();

				_i++;
				updateMatchState();
			}
			else if (_text[_i] == '\n')
			{
				Current = new RichTextToken
				{
					Index = _i,
					Length = 1,
					Type = RichTextTokenType.NewLine,
					IsHighlighted = _matchStarted,
					IsContext = _matchIsContext
				};

				_i++;
				updateMatchState();
			}
			else
				throw new Exception(@"invalid line end");
		}

		private void readSpace()
		{
			Current = new RichTextToken
			{
				Index = _i,
				Length = 1,
				Type = RichTextTokenType.Space,
				IsHighlighted = _matchStarted,
				IsContext = _matchIsContext
			};

			_i++;
			updateMatchState();
		}

		private void updateMatchState()
		{
			if (_matches == null || _matches.Count == 0)
				return;

			if (!_matchStarted)
			{
				_matchStarted = _m < _matches.Count && _i == _matches[_m].Index;
				_matchIsContext = _matchStarted && _matches[_m].IsContext;
				return;
			}

			_matchStarted &= _i != _matches[_m].Index + _matches[_m].Length;

			if (!_matchStarted)
			{
				_m++;
				_matchStarted = _m < _matches.Count && _i == _matches[_m].Index;
				_matchIsContext = _matchStarted && _matches[_m].IsContext;
			}
		}
	}
}