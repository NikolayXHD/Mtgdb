using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;

namespace Mtgdb.Dal.Index
{
	public sealed class MtgTokenizer : Tokenizer
	{
		public MtgTokenizer(TextReader inputReader)
			: base(inputReader)
		{
			_termAtt = AddAttribute<ICharTermAttribute>();
			_offsetAtt = AddAttribute<IOffsetAttribute>();
		}

		private void push(char c)
		{
			if (_length == 0)
				_start = _offset - 1; // start of token

			_buffer[_length++] = c; // buffer it
		}

		private bool flush()
		{
			if (_length == 0)
				return false;

			_termAtt.CopyBuffer(_buffer, 0, _length);
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
					_dataLen = m_input.Read(_ioBuffer, 0, _ioBuffer.Length);
					_bufferIndex = 0;
				}

				if (_dataLen <= 0)
				{
					_offset--;
					return flush();
				}

				char c = _ioBuffer[_bufferIndex++];

				if (c == '}')
					return terminateToken(c);
				else if (c == '{')
				{
					if (_length > 0)
						return terminatePreviousToken();
					else
					{
						push(c);
						if (_length == MaxWordLen)
							return flush();
					}
				}
				else if (char.IsLetterOrDigit(c) || MtgAplhabet.WordCharsSet.Contains(c) || MtgAplhabet.SingletoneWordChars.Contains(c))
				{
					if (c.IsCj() || MtgAplhabet.SingletoneWordChars.Contains(c))
					{
						if (_length > 0)
							return terminatePreviousToken();
						else
							return terminateToken(c);
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

		private bool terminatePreviousToken()
		{
			_bufferIndex--;
			_offset--;
			return flush();
		}

		private bool terminateToken(char c)
		{
			push(c);
			return flush();
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



		private int _offset, _bufferIndex, _dataLen;
		private const int MaxWordLen = 255;
		private const int IoBufferSize = 1024;
		private readonly char[] _buffer = new char[MaxWordLen];
		private readonly char[] _ioBuffer = new char[IoBufferSize];

		private int _length;
		private int _start;

		private readonly ICharTermAttribute _termAtt;
		private readonly IOffsetAttribute _offsetAtt;
	}
}