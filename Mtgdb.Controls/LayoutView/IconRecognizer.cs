using System.Collections.Generic;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class IconRecognizer
	{
		private readonly Dictionary<string, Image> _imageByText;

		public IconRecognizer(Dictionary<string, Image> imageByText)
		{
			_imageByText = imageByText;
		}

		public IList<RichTextToken> Recognize(RichTextToken richTextToken, string text)
		{
			var result = new List<RichTextToken>();

			int iconStart = -1;
			int iconEnd = richTextToken.Index - 1;

			for (int i = richTextToken.Index; i <= richTextToken.Index + richTextToken.Length; i++)
			{
				bool tokenEnded = i == richTextToken.Index + richTextToken.Length;

				char c;
				if (tokenEnded)
					c = default(char);
				else
					c = text[i];

				if (iconStart < 0 && c == '{' || tokenEnded)
				{
					iconStart = i;
					int index = iconEnd + 1;
					int length = i - index;
					if (length > 0)
					{
						result.Add(
							new RichTextToken
							{
								Icon = null,
								Index = index,
								Length = length,
								Type = richTextToken.Type,
								IsHighlighted = richTextToken.IsHighlighted,
								IsContext = richTextToken.IsContext
							});

						iconEnd = index + length - 1;
					}
				}
				else if (iconStart >= 0 && c == '}')
				{
					iconEnd = i;
					int length = i + 1 - iconStart;

					string symbol = text.Substring(iconStart + 1, length -2);

					Image icon;
					_imageByText.TryGetValue(symbol, out icon);

					result.Add(
						new RichTextToken
						{
							Icon = icon,
							Index = iconStart,
							Length = length,
							Type = richTextToken.Type,
							IsHighlighted = richTextToken.IsHighlighted,
							IsContext = richTextToken.IsContext
						});

					iconStart = -1;
				}
			}

			return result;
		}
	}
}