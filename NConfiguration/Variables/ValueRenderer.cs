using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Variables
{
	public class ValueRenderer
	{
		private readonly List<Token> _tokenList;

		public ValueRenderer(string sourceText)
		{
			_tokenList = parse(sourceText).ToList();
		}

		public string Render(IVariableStorage variableStorage)
		{
			if (_tokenList.Count == 1)
			{
				var token = _tokenList[0];
				return token.IsVariable ? variableStorage[token.Text] : token.Text;
			}

			var builder = new StringBuilder();
			foreach (var token in _tokenList)
				builder.Append(token.IsVariable ? variableStorage[token.Text] : token.Text);

			return builder.ToString();
		}

		private struct Token
		{
			public string Text;
			public bool IsVariable;
		}

		private IEnumerable<Token> parse(string sourceText)
		{
			if (string.IsNullOrEmpty(sourceText))
			{
				yield return new Token() { Text = sourceText, IsVariable = false };
				yield break;
			}

			var start = 0;

			while (true)
			{
				var beginPos = sourceText.IndexOf("${", start, StringComparison.Ordinal);

				if (beginPos == -1)
				{
					yield return new Token() { Text = sourceText.Substring(start), IsVariable = false };
					yield break;
				}

				if (beginPos > start)
				{
					yield return new Token() { Text = sourceText.Substring(start, beginPos - start), IsVariable = false };
				}

				var endPos = sourceText.IndexOf("}", beginPos + 2, StringComparison.Ordinal);

				if (endPos == -1)
					throw new FormatException("end of variable not found");

				yield return new Token() {Text = sourceText.Substring(beginPos + 2, endPos - beginPos - 2), IsVariable = true};

				if (endPos == sourceText.Length - 1)
					yield break;

				start = endPos + 1;
			}
		}
	}
}
