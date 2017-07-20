using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace NConfiguration.Json.Parsing
{
	internal sealed class CharEnumerator
	{
		private string _text;
		private int _current = -1;

		public CharEnumerator(string text)
		{
			_text = text;
		}

		public char Current
		{
			get
			{
				return _text[_current];
			}
		}

		public string Tail
		{
			get
			{
				return _text.Substring(_current);
			}
		}

		public bool MoveNext()
		{
			if (_current == -1)
			{
				_current = 0;
				return true;
			}
			
			if (_current == _text.Length - 1)
				return false;

			_current++;
			return true;
		}

		private static Regex _number = new Regex(@"-?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		internal void ExpectedRead(char ch, string tokenName)
		{
			if (!MoveNext())
				throw new FormatException(string.Format("unexpected end in the reading of {0}", tokenName));
			if (Current != ch)
				throw new FormatException(string.Format("unexpected symbol '{0}' in the reading of {1}", Current, tokenName));
		}

		internal bool MoveTo(Char end)
		{
			while (MoveNext())
			{
				var cur = Current;
				if (Char.IsWhiteSpace(cur))
					continue;
				if (cur == end)
					return true;
				throw new FormatException(string.Format("unexpected symbol '{0}'", cur));
			}

			return false;
		}

		internal bool MoveToNoWhite()
		{
			while (MoveNext())
			{
				if (!Char.IsWhiteSpace(Current))
					return true;
			}

			return false;
		}

		internal bool MoveTo(params Char[] ends)
		{
			int n = ends.Length;

			while (MoveNext())
			{
				var cur = Current;
				if (Char.IsWhiteSpace(cur))
					continue;

				for (int i = 0; i < n; i++)
					if (cur == ends[i])
						return true;

				throw new FormatException(string.Format("unexpected symbol '{0}'", cur));
			}

			return false;
		}

		internal JString ReadString()
		{
			StringBuilder text = new StringBuilder();

			while (true)
			{
				if (!MoveNext())
					throw new FormatException("unexpected end in the reading of string");

				switch (Current)
				{
					case '\"':
						return new JString(text.ToString());
					case '\\':
						text.Append(readEscapeSymbol());
						break;
					case '\b':
					case '\f':
					case '\r':
					case '\n':
					case '\t':
						throw new FormatException("unexpected control character in the reading of string");
					default:
						text.Append(Current);
						break;
				}
			}
		}

		private char readEscapeSymbol()
		{
			if (!MoveNext())
				throw new FormatException("unexpected end in the reading of string");

			switch (Current)
			{
				case '\"':
					return '\"';
				case '\\':
					return '\\';
				case '/':
					return '/';
				case 'b':
					return '\b';
				case 'f':
					return '\f';
				case 'r':
					return '\r';
				case 'n':
					return '\n';
				case 't':
					return '\t';
				case 'u':
					return readUnicodeSymbol();
				default:
					throw new FormatException(string.Format("unexpected escape symbol '{0}'", Current));
			}
		}

		private char readUnicodeSymbol()
		{
			StringBuilder text = new StringBuilder(4);

			for (int i = 0; i < 4; i++)
			{
				if (!MoveNext())
					throw new FormatException("unexpected end in the reading of string");
				if (!IsHexDigit(Current))
					throw new FormatException(string.Format("unexpected symbol '{0}' in hex digit in unicode symbol", Current));
				text.Append(Current);
			}

			var num = UInt16.Parse(text.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			return Convert.ToChar(num);
		}

		/// <summary>
		/// Is a character 0-9 a-f A-F ?
		/// </summary>
		public static bool IsHexDigit(char c)
		{
			if ('0' <= c && c <= '9') return true;
			if ('a' <= c && c <= 'f') return true;
			if ('A' <= c && c <= 'F') return true;
			return false;
		}

		internal JArray ReadArray()
		{
			var result = new JArray();

			if (!MoveToNoWhite())
				throw new FormatException("unexpected end in the reading of array");

			if (Current == ']')
				return result;

			result.Items.Add(ReadValue(false));

			while (true)
			{
				if (!MoveToNoWhite())
					throw new FormatException("unexpected end in the reading of array");

				if (Current == ']')
					return result;

				if (Current != ',')
					throw new FormatException(string.Format("unexpected symbol '{0}' in the reading of array", Current));

				if (!MoveToNoWhite())
					throw new FormatException("unexpected end in the reading of array");

				result.Items.Add(ReadValue(false));
			}
		}

		internal JBoolean ReadBoolean()
		{
			if (Current == 't')
			{
				ExpectedRead('r', "boolean");
				ExpectedRead('u', "boolean");
				ExpectedRead('e', "boolean");

				return new JBoolean(true);
			}
			else
			{
				ExpectedRead('a', "boolean");
				ExpectedRead('l', "boolean");
				ExpectedRead('s', "boolean");
				ExpectedRead('e', "boolean");

				return new JBoolean(false);
			}
		}

		internal JNull ReadNull()
		{
			ExpectedRead('u', "null");
			ExpectedRead('l', "null");
			ExpectedRead('l', "null");

			return JNull.Instance;
		}

		internal JNumber ReadNumber()
		{
			var m = _number.Match(_text, _current);
			if (!m.Success)
				throw new FormatException("invalid format of number");

			var result = m.Value;

			_current += result.Length - 1;

			return new JNumber(result);
		}

		internal JObject ReadObject()
		{
			var result = new JObject();

			if (!MoveTo('\"', '}'))
				throw new FormatException("unexpected end in the reading of object");

			if (Current == '}')
				return result;

			while (true)
			{
				var propName = ReadString();

				if (!MoveTo(':'))
					throw new FormatException("unexpected end in the reading of object");

				var propValue = ReadValue(true);

				result.Properties.Add(new KeyValuePair<string, JValue>(propName.Value, propValue));

				if (!MoveTo(',', '}'))
					throw new FormatException("unexpected end in the reading of object");

				if (Current == '}')
					return result;

				if (!MoveTo('\"'))
					throw new FormatException("unexpected end in the reading of object");
			}
		}

		internal JValue ReadValue(bool move)
		{
			if (move && !MoveNext())
				throw new FormatException("unexpected end in the reading of value");

			while (true)
			{
				if (Char.IsWhiteSpace(Current))
				{
					if (!MoveNext())
						throw new FormatException("unexpected end in the reading of value");
					continue;
				}

				switch (Current)
				{
					case 't':
					case 'f':
						return ReadBoolean();

					case 'n':
						return ReadNull();

					case '{':
						return ReadObject();

					case '\"':
						return ReadString();

					case '[':
						return ReadArray();

					case '-':
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						return ReadNumber();

					default:
						throw new FormatException(string.Format("unexpected symbol '{0}' in the reading of value", Current));
				}
			}
		}
	}
}
