using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Lucene.Net.Contrib
{
	/// <summary>
	/// Extracts escaped sequences
	/// </summary>
	public class StringEscaper : IEnumerator<EscapedChar>
	{
		public static readonly HashSet<char> SpecialChars = new HashSet<char>
		{
			'&', '|', '+', '-', '!', '(', ')', '{', '}', '[', ']', '^' , '\"', '~', '*', '?', ':', '\\', '/'
		};

		public static readonly HashSet<char> TwoSymbolOperators = new HashSet<char>
		{
			'&', '|'
		};

		public StringEscaper(string query)
		{
			_query = query;
		}
		
		public bool MoveNext()
		{
			if (Position >= _query.Length)
			{
				Substring = null;
				return false;
			}

			int s = getSpacesCount();
			if (s > 0)
			{
				string result = new string(_query[Position], s);
				Position += s;
				Substring = result;
				return true;
			}

			if (_query[Position] != EscapeCharacter)
			{
				string result = new string(_query[Position], 1);
				Position++;
				Substring = result;
				return true;
			}

			Position++;
			if (Position < _query.Length)
			{
				var result = _query.Substring(Position - 1, length: 2);
				Position++;
				Substring = result;
				return true;
			}

			Substring = _escapeString;
			return true;
		}

		private int getSpacesCount()
		{
			for (int s = 0; Position + s < _query.Length; s++)
				if (!char.IsWhiteSpace(_query[Position + s]))
					return s;

			return _query.Length - Position;
		}



		object IEnumerator.Current => Substring;

		public void Reset()
		{
			Position = 0;
			Substring = null;
		}

		public void Dispose()
		{
		}

		public EscapedChar Current => new EscapedChar(Substring, Position);

		private int Position { get; set; }
		private string Substring { get; set; }

		private readonly string _query;
		private const char EscapeCharacter = '\\';
		private static readonly string _escapeString = new string(EscapeCharacter, count: 1);

		public static string Unescape(string value)
		{
			var resultBuilder = new StringBuilder();

			var escaper = new StringEscaper(value);
			while (escaper.MoveNext())
			{
				var current = escaper.Current;

				resultBuilder.Append(current.Value.Length == 1 
					? current.Value 
					: current.Value.Substring(1));
			}

			return resultBuilder.ToString();
		}

		public static string Escape(string value)
		{
			var resultBuilder = new StringBuilder();

			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				if (SpecialChars.Contains(c))
					resultBuilder.Append(EscapeCharacter);

				resultBuilder.Append(c);
			}

			return resultBuilder.ToString();
		}
	}
}