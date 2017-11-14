using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Contrib;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Mtgdb.Gui.Resx;

namespace Mtgdb.Gui
{
	[Localizable(false)]
	public class DrawingSubsystem
	{
		private readonly LayoutView _layoutViewCards;
		private readonly LayoutView _layoutViewDeck;
		private readonly Font _font;
		private readonly DraggingSubsystem _draggingSubsystem;
		private readonly SearchStringSubsystem _searchStringSubsystem;
		private readonly DeckModel _deckModel;
		private readonly QuickFilterFacade _quickFilterFacade;
		private readonly LegalitySubsystem _legalitySubsystem;
		private readonly ImageCache _imageCache;
		
		public DrawingSubsystem(
			LayoutView layoutViewCards,
			LayoutView layoutViewDeck,
			Font font,
			DraggingSubsystem draggingSubsystem,
			SearchStringSubsystem searchStringSubsystem,
			DeckModel deckModel,
			QuickFilterFacade quickFilterFacade,
			LegalitySubsystem legalitySubsystem,
			ImageCache imageCache)
		{
			_layoutViewCards = layoutViewCards;
			_layoutViewDeck = layoutViewDeck;
			_font = font;
			_draggingSubsystem = draggingSubsystem;
			_searchStringSubsystem = searchStringSubsystem;
			_deckModel = deckModel;
			_quickFilterFacade = quickFilterFacade;
			_legalitySubsystem = legalitySubsystem;
			_imageCache = imageCache;

			_layoutViewCards.RowDataLoaded += setHighlightMatches;
			_layoutViewCards.SetIconRecognizer(createIconRecognizer());
		}

		private static IconRecognizer createIconRecognizer()
		{
			var mappings = ResourcesCost.ResourceManager
				.GetResourceSet(CultureInfo.CurrentCulture, false, true)
				.Cast<DictionaryEntry>()
				.Select(_ => new { symbols = getSymbol((string) _.Key), image = (Bitmap) _.Value });

			var imageByText = new Dictionary<string, Bitmap>(Str.Comparer);

			foreach (var mapping in mappings)
			{
				if (mapping.symbols == null || mapping.symbols.Length == 0)
					continue;

				foreach (string symbol in mapping.symbols)
					imageByText[symbol] = mapping.image;
			}

			var iconRecognizer = new IconRecognizer(imageByText);
			return iconRecognizer;
		}

		private static string[] getSymbol(string key)
		{
			string name = key.TrimStart('_');

			if (name == "05")
				return new[] { "0.5", ".5", "1/2", "½" };

			if (name == "i")
				return new[] { "∞", "oo" };

			if (name.Length == 1 || name.Length == 2 && name.All(char.IsDigit) || name == "100" || name == "1000000" || name == "chaos")
				return new[] { name };
			
			if (name.Length == 2)
				return new[] { $"{name[0]}/{name[1]}", $"{name[1]}/{name[0]}", $"{name[0]}{name[1]}", $"{name[1]}{name[0]}" };

			return null;
		}

		public void SetupDrawingCardEvent()
		{
			_layoutViewCards.CustomDrawField += drawCard;
			_layoutViewDeck.CustomDrawField += drawCard;
		}

		private void drawCard(object sender, CustomDrawArgs e)
		{
			var card = _draggingSubsystem.GetCard(getView(sender), e.RowHandle);

			if (card == null)
				return;

			if (e.FieldName != nameof(Card.Image))
				return;

			e.Handled = true;

			if (card.Image != null)
			{
				var bounds = card.Image.Size.FitIn(e.Bounds);
				e.Graphics.DrawImage(card.Image, bounds);
			}

			if (card == _deckModel.TouchedCard)
				drawSelection(e, Color.LightSkyBlue, Color.LightCyan, 236);
			else if (card.DeckCount == 0 && card.CollectionCount > 0)
				drawSelection(e, Color.Lavender, Color.White, 96);
			else if (card.DeckCount > 0)
				drawSelection(e, Color.Lavender, Color.White, 236);

			var legalityWarning = _legalitySubsystem.GetWarning(card);

			if (legalityWarning == Legality.Restricted && card.DeckCount <= 1 && sender == _layoutViewDeck)
				legalityWarning = null;

			if (!string.IsNullOrEmpty(legalityWarning))
				drawLegalityWarning(e, legalityWarning);

			drawCount(sender, e, card);
		}

		private void drawLegalityWarning(CustomDrawArgs e, string warning)
		{
			var rect = getWarningRectangle(e);
			const int size = 18;
			var font = new Font(FontFamily.GenericMonospace, size, FontStyle.Italic | FontStyle.Bold);
			var brush = new SolidBrush(Color.FromArgb(192, Color.OrangeRed));

			var lineSize = e.Graphics.MeasureString(warning, font);
			rect.Offset((int) ((rect.Width - lineSize.Width)/2f), 0);

			e.Graphics.DrawString(warning, font, brush, rect, StringFormat.GenericDefault);
		}

		private void drawSelection(CustomDrawArgs e, Color borderColor, Color foreColor, int opacity)
		{
			var rect = getSelectionRectangle(e);

			const int borderWidth = 2;
			const int cornerOffset = -1;
			var cornerShare = 4;

			rect.Inflate(new Size(-borderWidth, -borderWidth));

			int cornerYSize = rect.Height/cornerShare;
			int cornerXSize = rect.Height/cornerShare;

			var points = new[]
			{
				new Point(rect.Left - cornerOffset + cornerXSize, rect.Top - cornerOffset),
				new Point(rect.Right + cornerOffset - cornerXSize, rect.Top - cornerOffset),

				new Point(rect.Right + cornerOffset, rect.Top - cornerOffset + cornerYSize),
				new Point(rect.Right + cornerOffset, rect.Bottom + cornerOffset - cornerYSize),

				new Point(rect.Right + cornerOffset - cornerXSize, rect.Bottom + cornerOffset),
				new Point(rect.Left - cornerOffset + cornerXSize, rect.Bottom + cornerOffset),
					
				new Point(rect.Left - cornerOffset, rect.Bottom + cornerOffset - cornerYSize),
				new Point(rect.Left - cornerOffset, rect.Top - cornerOffset + cornerYSize)
			};

			rect.Inflate(-3, -7);

			var brush = new LinearGradientBrush(
				rect,
				Color.FromArgb(opacity, borderColor),
				Color.FromArgb(opacity, foreColor),
				LinearGradientMode.BackwardDiagonal);

			e.Graphics.FillClosedCurve(brush, points);
		}

		private void drawCount(object sender, CustomDrawArgs e, Card card)
		{
			var countText = getCountText(sender, card);
			if (string.IsNullOrEmpty(countText))
				return;

			var rect = getSelectionRectangle(e);

			var format = StringFormat.GenericTypographic;
			var size = new Size(12, 18).ByDpi();
			var font = new Font(_font.FontFamily, size.Height, FontStyle.Bold, GraphicsUnit.Pixel);
			var textSize = e.Graphics.MeasureString(countText, font, rect.Size, format);

			const int opacity = 224;

			var targetRect = new Rectangle(
				(int) Math.Floor(rect.Left + 0.5f * (rect.Width - textSize.Width)),
				(int) Math.Floor(rect.Top + 0.5f * (rect.Height - textSize.Height)),
				(int) Math.Ceiling(textSize.Width),
				(int) Math.Ceiling(textSize.Height));

			e.Graphics.DrawString(
				countText,
				font,
				new SolidBrush(Color.FromArgb(opacity, Color.Black)),
				targetRect,
				format);
		}

		private Rectangle getSelectionRectangle(CustomDrawArgs e)
		{
			var size = new Size(50, 30).ByDpi();

			var rect = new Rectangle(
				e.Bounds.Left + (_imageCache.CardSize.Width - size.Width)/2,
				e.Bounds.Bottom - size.Height,
				size.Width,
				size.Height);

			return rect;
		}

		private Rectangle getWarningRectangle(CustomDrawArgs e)
		{
			var stripSize = new Size(_imageCache.CardSize.Width, 30.ByDpiHeight());

			var rect = new Rectangle(
				e.Bounds.Left,
				(int) (e.Bounds.Bottom - 1.75f * stripSize.Height),
				stripSize.Width,
				stripSize.Height);

			return rect;
		}

		private string getCountText(object sender, Card card)
		{
			var countText = new StringBuilder();

			if (card.DeckCount > 0)
				countText.Append(card.DeckCount);

			if (card.CollectionCount > 0 && (_deckModel.Zone != Zone.SampleHand || !_layoutViewDeck.Wraps(sender)))
				countText.AppendFormat(@"/{0}", card.CollectionCount);

			string countTextStr = countText.ToString();
			return countTextStr;
		}

		private void setHighlightMatches(object sender, int rowHandle)
		{
			var view = getView(sender);
			var card = (Card) view.GetRow(rowHandle);

			if (card == null)
				return;

			foreach (var fieldName in view.FieldNames)
			{
				if (fieldName == nameof(Card.Image))
					continue;

				var text = _layoutViewCards.GetFieldText(rowHandle, fieldName);
				setMatches(rowHandle, text, fieldName);
			}
		}

		private void setMatches(int rowHandle, string text, string fieldName)
		{
			var matches = _quickFilterFacade.GetMatchedText(text, fieldName) ?? new List<TextRange>();
			var contextMatches = new List<TextRange>();
			
			addHighlightTerms(matches, contextMatches, text, fieldName);

			if (!string.IsNullOrEmpty(_legalitySubsystem.FilterFormat) && fieldName == nameof(Card.Rulings))
			{
				var legalityMatches = new Regex(_legalitySubsystem.FilterFormat).Matches(text)
					.OfType<Match>()
					.Where(_ => _.Success)
					.Select(TextRange.Copy);

				matches.AddRange(legalityMatches);
			}

			var highlightAreas = getHighlightRanges(matches, contextMatches);
			_layoutViewCards.SetHighlightTextRanges(highlightAreas, rowHandle, fieldName);
		}

		private static List<TextRange> getHighlightRanges(IList<TextRange> matches, IList<TextRange> contextMathes)
		{
			var result = new List<TextRange>();

			foreach (var match in contextMathes)
				match.IsContext = true;

			var orderedMatches = matches.Union(contextMathes)
				.OrderBy(_ => _.Index)
				.ThenByDescending(_ => _.Length);

			TextRange previousArea = null;
			int previousMatchEnd = 0;
			foreach (var m in orderedMatches)
			{
				var thisEnd = m.Index + m.Length;

				if (previousArea != null && m.Index < previousMatchEnd)
				{
					if (thisEnd > previousMatchEnd)
					{
						int newPreviousLength = m.Index - previousArea.Index;
						if (newPreviousLength > 0)
							previousArea.Length = newPreviousLength;
						else
							result.RemoveAt(result.Count - 1);

						previousArea = m;
						previousMatchEnd = thisEnd;
						result.Add(previousArea);
					}
				}
				else
				{
					previousArea = m;
					previousMatchEnd = thisEnd;
					result.Add(previousArea);
				}
			}

			return result;
		}

		private void addHighlightTerms(List<TextRange> matches, List<TextRange> contextMatches, string displayText, string fieldName)
		{
			if (_searchStringSubsystem.SearchResult?.HighlightTerms == null)
				return;

			foreach (var term in _searchStringSubsystem.SearchResult.HighlightTerms)
			{
				string displayField;
				if (string.IsNullOrEmpty(term.Key))
					displayField = null;
				else
					displayField = term.Key;

				if (!string.IsNullOrEmpty(displayField) && !Str.Equals(displayField, fieldName))
					continue;

				var patternsSet = new HashSet<string>();
				var contextPatternsSet = new HashSet<string>();

				var relevantTokens = term.Value.Where(token => 
					token.Type.Is(TokenType.FieldValue|TokenType.AnyChar|TokenType.RegexBody) &&
					!string.IsNullOrEmpty(token.Value));

				foreach (var token in relevantTokens)
				{
					string pattern;
					List<string> contextPatterns;
					getPattern(token, out pattern, out contextPatterns);

					if (pattern != null)
						patternsSet.Add(pattern);

					foreach (string contextPattern in contextPatterns)
						contextPatternsSet.Add(contextPattern);
				}

				addMatches(displayText, patternsSet, matches, fieldName);
				addMatches(displayText, contextPatternsSet, contextMatches, fieldName);
			}
		}

		private static void addMatches(string displayText, IEnumerable<string> patterns, List<TextRange> matches, string fieldName)
		{
			foreach (string pattern in patterns)
			{
				var findRegex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

				if (DocumentFactory.NotAnalyzedFields.Contains(fieldName))
				{
					var findMatches = findRegex.Matches(displayText)
						.OfType<Match>()
						.Where(_ => _.Success && _.Length > 0)
						.Select(TextRange.Copy);

					matches.AddRange(findMatches);
				}
				else
				{
					var analyzer = new MtgdbAnalyzer();
					var tokenStream = analyzer.GetTokenStream(fieldName, displayText);

					tokenStream.Reset();

					using (tokenStream)
						while (tokenStream.IncrementToken())
						{
							var term = tokenStream.GetAttribute<ICharTermAttribute>();
							var offset = tokenStream.GetAttribute<IOffsetAttribute>();

							var findMatches = findRegex.Matches(term.ToString())
								.OfType<Match>()
								.Where(_ => _.Success && _.Length > 0)
								.Select(_ => TextRange.Offset(offset.StartOffset, _));

							matches.AddRange(findMatches);
						}
				}
			}
		}

		private static void getPattern(Token token, out string result, out List<string> contextPatterns)
		{
			var prefixTokens = getPrefixTokens(token);
			var currentTokens = new List<Token> { token };
			var suffixTokens = getSuffixTokens(token);

			if (token.Type.Is(TokenType.FieldValue | TokenType.RegexBody))
				result = getPattern(prefixTokens, currentTokens, suffixTokens);
			else
				result = null;

			// создадим по 1 паттерну контекста для каждой группы следующих непрерывно wildcard токенов

			var tokenGroup = new List<Token>();

			tokenGroup.AddRange(prefixTokens);
			tokenGroup.Add(token);
			tokenGroup.AddRange(suffixTokens);

			contextPatterns = new List<string>();
			suffixTokens.Clear();
			suffixTokens.AddRange(tokenGroup);

			int i = 0;
			while (true)
			{
				i += suffixTokens.TakeWhile(_ => !_.Type.Is(TokenType.Wildcard)).Count();

				if (i == tokenGroup.Count)
					break;

				prefixTokens.Clear();
				prefixTokens.AddRange(
					tokenGroup.Take(i));

				currentTokens.Clear();
				currentTokens.AddRange(
					tokenGroup.Skip(i).TakeWhile(_ => _.Type.Is(TokenType.Wildcard)));

				i = prefixTokens.Count + currentTokens.Count;

				suffixTokens.Clear();
				suffixTokens.AddRange(
					tokenGroup.Skip(prefixTokens.Count + currentTokens.Count));

				contextPatterns.Add(getPattern(prefixTokens, currentTokens, suffixTokens));
			}
		}

		private static string getPattern(List<Token> prefixTokens, List<Token> radixTokens, List<Token> suffixTokens)
		{
			string prefixPattern = getPattern(prefixTokens);
			string suffixPattern = getPattern(suffixTokens);
			string radixPattern = getPattern(radixTokens);

			prefixPattern = "^" + prefixPattern;
			suffixPattern += "$";

			string result = $"(?<={prefixPattern}){radixPattern}(?={suffixPattern})";
			return result;
		}

		private static List<Token> getSuffixTokens(Token token)
		{
			var suffixTokens = new List<Token>();

			if (token.Type.Is(TokenType.RegexBody))
				return suffixTokens;

			while (true)
			{
				if (token.Next == null ||
					!token.Next.TouchesCaret(token.Position + token.Value.Length) ||
					token.Type.Is(TokenType.FieldValue) && token.Value[token.Value.Length - 1].IsCj() ||
					token.Next.Type.Is(TokenType.FieldValue) && token.Next.Value[0].IsCj())
					// Вплотную прилегающее к wildcard значение является его продолжением в отличие от случая, если между ними есть пробел,
					// тогда это уже другой термин
					break;
				if (token.Next.Type.Is(TokenType.Wildcard | TokenType.FieldValue))
					suffixTokens.Add(token.Next);
				else
					break;

				token = token.Next;
			}

			return suffixTokens;
		}

		private static List<Token> getPrefixTokens(Token token)
		{
			var prefixTokens = new List<Token>();

			if (token.Type.Is(TokenType.RegexBody))
				return prefixTokens;

			while (true)
			{
				if (token.Previous == null ||
					!token.Previous.TouchesCaret(token.Position) ||
					token.Type.Is(TokenType.FieldValue) && token.Value[0].IsCj() ||
					token.Previous.Type.Is(TokenType.FieldValue) && token.Previous.Value[token.Previous.Value.Length - 1].IsCj())
					// Вплотную прилегающее к wildcard значение является его продолжением в отличие от случая, если между ними есть пробел,
					// тогда это уже другой термин
					break;

				if (token.Previous.Type.Is(TokenType.Wildcard | TokenType.FieldValue))
					prefixTokens.Insert(0, token.Previous);
				else
					break;

				token = token.Previous;
			}

			return prefixTokens;
		}

		private static string getPattern(IEnumerable<Token> tokens)
		{
			var pattern = new StringBuilder();
			foreach (var token in tokens)
			{
				if (token.Type.Is(TokenType.AnyChar))
				{
					pattern.Append(MtgdbTokenizerPatterns.Word);
				}
				else if (token.Type.Is(TokenType.AnyString))
					pattern.Append(MtgdbTokenizerPatterns.Word + "*");
				else if (token.Type.Is(TokenType.FieldValue))
				{
					string luceneUnescaped = StringEscaper.Unescape(token.Value);

					foreach (char c in luceneUnescaped)
					{
						var equivalents = MtgdbTokenizerPatterns.GetEquivalents(c);
						pattern.Append("(");

						using (var enumerator = equivalents.GetEnumerator())
							if (enumerator.MoveNext())
							{
								pattern.Append(Regex.Escape(new string(enumerator.Current, 1)));

								while (enumerator.MoveNext())
								{
									pattern.Append("|");
									pattern.Append(Regex.Escape(new string(enumerator.Current, 1)));
								}
							}

						pattern.Append(")");
					}
				}
				else if (token.Type.Is(TokenType.RegexBody))
				{
					pattern.Append(token.Value);
				}
			}

			return pattern.ToString();
		}

		private LayoutView getView(object view)
		{
			if (_layoutViewCards.Wraps(view))
				return _layoutViewCards;

			if (_layoutViewDeck.Wraps(view))
				return _layoutViewDeck;

			throw new Exception(@"wrapper not found");
		}
	}
}