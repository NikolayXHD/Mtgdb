using System.Collections.Generic;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class IconRecognizer
	{
		public IconRecognizer(Dictionary<string, Bitmap> imageByText, HashSet<string> nonShadowedIcons)
		{
			_imageByText = imageByText;
			_nonShadowedIcons = nonShadowedIcons;
		}

		public IList<RichTextToken> Recognize(RichTextToken originalToken, string text)
		{
			var result = new List<RichTextToken>();

			int iconStart = -1;
			int iconEnd = originalToken.Index - 1;

			for (int i = originalToken.Index; i <= originalToken.Index + originalToken.Length; i++)
			{
				bool tokenEnded = i == originalToken.Index + originalToken.Length;

				char c;
				if (tokenEnded)
					c = default;
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
							new RichTextToken(originalToken, index, length)
							{
								IconName = null
							});

						iconEnd = index + length - 1;
					}
				}
				else if (iconStart >= 0 && c == '}')
				{
					iconEnd = i;
					int length = i + 1 - iconStart;

					var token = new RichTextToken(originalToken, iconStart, length);

					string symbol = text.Substring(iconStart + 1, length - 2);

					if (_imageByText.TryGetValue(symbol, out var icon))
					{
						token.IconName = symbol;
						token.IconNeedsShadow = icon.Width == icon.Height && !_nonShadowedIcons.Contains(symbol);
					}

					result.Add(token);

					iconStart = -1;
				}
			}

			return result;
		}

		public Bitmap GetIcon(string name, int maxHeight)
		{
			if (!_imageByText.TryGetValue(name, out var icon))
				return null;

			var scaledSize = icon.Size.FitIn(new Size(int.MaxValue, maxHeight));

			if (!_iconsByTextByHeight.TryGetValue(name, out var iconsBySize))
			{
				iconsBySize = new Dictionary<int, Bitmap>();
				_iconsByTextByHeight.Add(name, iconsBySize);
			}

			if (iconsBySize.TryGetValue(scaledSize.Height, out var scaledIcon))
				return scaledIcon;

			scaledIcon = icon.FitIn(scaledSize).ApplyColorScheme();
			iconsBySize.Add(scaledSize.Height, scaledIcon);

			return scaledIcon;
		}

		private readonly Dictionary<string, Bitmap> _imageByText;
		private readonly HashSet<string> _nonShadowedIcons;

		private readonly Dictionary<string, Dictionary<int, Bitmap>> _iconsByTextByHeight =
			new Dictionary<string, Dictionary<int, Bitmap>>();
	}
}
