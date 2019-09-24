using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Controls
{
	public static class RichTextRenderer
	{
		public static void Render(RichTextRenderContext renderContext, IconRecognizer iconRecognizer)
		{
			using (var textTokenPrinter = new RichTextLayout(renderContext, iconRecognizer))
			{
				var readingContext = new RichTextTokenReader(renderContext.Text, renderContext.HighlightRanges);

				var currentWord = new List<RichTextToken>();
				bool aborted = false;
				while (readingContext.ReadToken())
				{
					var token = readingContext.Current;

					bool isCj = renderContext.Text[token.Index + token.Length - 1].IsCj();

					if (token.Type == RichTextTokenType.Word)
					{
						var subtokens = getSubtokens(token, renderContext, iconRecognizer);
						currentWord.AddRange(subtokens);

						if (isCj)
						{
							bool canContinue = textTokenPrinter.PrintWord(currentWord);

							if (currentWord.Count > 0 && !canContinue)
							{
								aborted = true;
								break;
							}

							currentWord.Clear();
						}
					}
					else if (token.Type == RichTextTokenType.Space)
					{
						bool canContinue = textTokenPrinter.PrintWord(currentWord);

						if (currentWord.Count > 0 && !canContinue)
						{
							aborted = true;
							break;
						}

						currentWord.Clear();

						canContinue = textTokenPrinter.PrintSpace(token);

						if (!canContinue)
						{
							aborted = true;
							break;
						}
					}
					else if (token.Type == RichTextTokenType.NewLine)
					{
						bool canContinue = textTokenPrinter.PrintWord(currentWord);

						if (currentWord.Count > 0 && !canContinue)
						{
							aborted = true;
							break;
						}

						currentWord.Clear();
						canContinue = textTokenPrinter.NewParagraph();

						if (!canContinue)
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

		private static IEnumerable<RichTextToken> getSubtokens(
			RichTextToken originalToken,
			RichTextRenderContext renderContext,
			IconRecognizer iconRecognizer)
		{
			IEnumerable<RichTextToken> tokens;

			if (iconRecognizer == null)
				tokens = Sequence.From(originalToken);
			else
				tokens = iconRecognizer.Recognize(originalToken, renderContext.Text);

			foreach (var token in tokens)
			{
				if (token.IconName != null || token.Length == 1)
					yield return token;
				else
				{
					var splitPositions = Enumerable.Range(token.Index + 1, token.Length)
						.Where(i =>
							i == token.Index + token.Length ||
							!char.IsLetterOrDigit(renderContext.Text[i]) ||
							!char.IsLetterOrDigit(renderContext.Text[i - 1]));

					int position = token.Index;
					foreach (int i in splitPositions)
					{
						var subtoken = new RichTextToken(token, position, i - position);
						yield return subtoken;

						position = i;
					}
				}
			}
		}
	}
}