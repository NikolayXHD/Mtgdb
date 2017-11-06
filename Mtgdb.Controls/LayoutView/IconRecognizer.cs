using System.Collections.Generic;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class IconRecognizer
	{
		private readonly Dictionary<string, Bitmap> _imageByText;

		private readonly Dictionary<string, Dictionary<int, Bitmap>> _iconsByTextByHeight =
			new Dictionary<string, Dictionary<int, Bitmap>>();

		public IconRecognizer(Dictionary<string, Bitmap> imageByText)
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
								IconName = null,
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

					var textToken = new RichTextToken
					{
						Index = iconStart,
						Length = length,
						Type = richTextToken.Type,
						IsHighlighted = richTextToken.IsHighlighted,
						IsContext = richTextToken.IsContext
					};

					string symbol = text.Substring(iconStart + 1, length - 2);

					if (_imageByText.ContainsKey(symbol))
						textToken.IconName = symbol;

					result.Add(textToken);

					iconStart = -1;
				}
			}

			return result;
		}

		public Bitmap GetIcon(string name, int maxHeight)
		{
			Bitmap icon;
			if (!_imageByText.TryGetValue(name, out icon))
				return null;

			var scaledSize = icon.Size.FitIn(new Size(int.MaxValue, maxHeight));

			if (scaledSize == icon.Size)
				return icon;

			Dictionary<int, Bitmap> iconsBySize;
			if (!_iconsByTextByHeight.TryGetValue(name, out iconsBySize))
			{
				iconsBySize = new Dictionary<int, Bitmap>();
				_iconsByTextByHeight.Add(name, iconsBySize);
			}

			Bitmap scaledIcon;
			if (!iconsBySize.TryGetValue(scaledSize.Height, out scaledIcon))
			{
				scaledIcon = icon.FitIn(scaledSize);
				iconsBySize.Add(scaledSize.Height, scaledIcon);
			}

			return scaledIcon;
		}
	}
}