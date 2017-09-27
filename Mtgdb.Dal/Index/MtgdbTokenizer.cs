using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Contrib;

namespace Mtgdb.Dal.Index
{
	public sealed class MtgdbTokenizer : Tokenizer
	{
		public MtgdbTokenizer(TextReader inputReader)
			: base(inputReader)
		{
			init();
		}

		private void init()
		{
			_termAtt = AddAttribute<ITermAttribute>();
			_offsetAtt = AddAttribute<IOffsetAttribute>();
		}

		private int _offset, _bufferIndex, _dataLen;
		private const int MaxWordLen = 255;
		private const int IoBufferSize = 1024;
		private readonly char[] _buffer = new char[MaxWordLen];
		private readonly char[] _ioBuffer = new char[IoBufferSize];

		private int _length;
		private int _start;

		private ITermAttribute _termAtt;
		private IOffsetAttribute _offsetAtt;

		private void push(char c)
		{
			if (_length == 0)
				_start = _offset - 1; // start of token

			_buffer[_length++] = char.ToLower(c); // buffer it
		}

		private bool flush()
		{
			if (_length == 0)
				return false;

			_termAtt.SetTermBuffer(_buffer, 0, _length);
			_offsetAtt.SetOffset(CorrectOffset(_start), CorrectOffset(_start + _length));
			return true;
		}


		public override bool IncrementToken()
		{
			ClearAttributes();

			_length = 0;
			_start = _offset;


			while (true)
			{
				_offset++;

				if (_bufferIndex >= _dataLen)
				{
					_dataLen = input.Read(_ioBuffer, 0, _ioBuffer.Length);
					_bufferIndex = 0;
				}

				if (_dataLen == 0)
				{
					_offset--;
					return flush();
				}

				char c = _ioBuffer[_bufferIndex++];

				if (Char.IsLetterOrDigit(c) || WordCharsSet.Contains(c))
				{
					if (c.IsCj())
					{
						if (_length > 0)
						{
							_bufferIndex--;
							_offset--;
							return flush();
						}
						else
						{
							push(c);
							return flush();
						}
					}
					else
					{
						push(c);
						if (_length == MaxWordLen)
							return flush();
					}
				}
				else
				{
					if (_length > 0)
						return flush();
				}
			}
		}

		public override void End()
		{
			// set final offset
			int finalOffset = CorrectOffset(_offset);
			_offsetAtt.SetOffset(finalOffset, finalOffset);
		}

		public override void Reset()
		{
			base.Reset();
			_offset = _bufferIndex = _dataLen = 0;
		}

		public override void Reset(TextReader inputReader)
		{
			base.Reset(inputReader);
			Reset();
		}

		public static readonly char[] WordChars =
		{
			'_',
			'-',
			'−',
			'—',
			'+',
			'/',
			'{',
			'}',
			'½'
		};

		public static readonly HashSet<char> WordCharsSet = new HashSet<char>(WordChars);
	}
}
