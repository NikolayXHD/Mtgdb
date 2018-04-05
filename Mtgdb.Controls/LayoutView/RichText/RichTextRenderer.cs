using System.Collections.Generic;

namespace Mtgdb.Controls
{
	public static class RichTextRenderer
	{
		public static void Render(RichTextRenderContext renderContext, IconRecognizer iconRecognizer)
		{
			var textTokenPrinter = new RichTextLayout(renderContext, iconRecognizer);
			var readingContext = new RichTextTokenReader(renderContext.Text, renderContext.HighlightRanges);

			var currentWord = new List<RichTextToken>();
			bool aborted = false;
			while (readingContext.ReadToken())
			{
				var token = readingContext.Current;

				bool isCj = renderContext.Text[token.Index + token.Length - 1].IsCj();

				if (token.Type == RichTextTokenType.Word)
				{
					if (iconRecognizer == null)
						currentWord.Add(token);
					else
					{
						var tokens = iconRecognizer.Recognize(token, renderContext.Text);
						currentWord.AddRange(tokens);
					}

					if (isCj)
					{
						if (currentWord.Count > 0 && !textTokenPrinter.PrintWord(currentWord))
						{
							aborted = true;
							break;
						}

						currentWord.Clear();
					}
				}
				else if (token.Type == RichTextTokenType.Space)
				{
					if (currentWord.Count > 0 && !textTokenPrinter.PrintWord(currentWord))
					{
						aborted = true;
						break;
					}

					currentWord.Clear();

					if (!textTokenPrinter.PrintSpace(readingContext.Current))
					{
						aborted = true;
						break;
					}
				}
				else if (token.Type == RichTextTokenType.NewLine)
				{
					if (currentWord.Count > 0 && !textTokenPrinter.PrintWord(currentWord))
					{
						aborted = true;
						break;
					}

					currentWord.Clear();

					if (!textTokenPrinter.NewParagraph())
					{
						aborted = true;
						break;
					}
				}
			}

			if (!aborted && currentWord.Count > 0)
				textTokenPrinter.PrintWord(currentWord);

			textTokenPrinter.Flush();
		}
	}
}