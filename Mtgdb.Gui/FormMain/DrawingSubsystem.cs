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
		public DrawingSubsystem(
			LayoutView layoutViewCards,
			LayoutView layoutViewDeck,
			Font font,
			DraggingSubsystem draggingSubsystem,
			SearchStringSubsystem searchStringSubsystem,
			DeckModel deckModel,
			QuickFilterFacade quickFilterFacade,
			LegalitySubsystem legalitySubsystem,
			ImageLoader imageLoader)
		{
			_layoutViewCards = layoutViewCards;
			_layoutViewDeck = layoutViewDeck;
			_font = font;
			_draggingSubsystem = draggingSubsystem;
			_searchStringSubsystem = searchStringSubsystem;
			_deckModel = deckModel;
			_quickFilterFacade = quickFilterFacade;
			_legalitySubsystem = legalitySubsystem;
			_imageLoader = imageLoader;

			_layoutViewCards.RowDataLoaded += setHighlightMatches;
			_layoutViewCards.SetIconRecognizer(createIconRecognizer());
			_mtgdbAnalyzer = new MtgdbAnalyzer();
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

			var nonShadowedIcons = new HashSet<string>(Str.Comparer)
			{
				"E", "Q"
			};

			var iconRecognizer = new IconRecognizer(imageByText, nonShadowedIcons);
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

			var image = card.Image(Ui);

			if (image != null)
			{
				var bounds = image.Size.FitIn(e.Bounds);
				e.Graphics.DrawImage(image, bounds);
			}

			if (card == _deckModel.TouchedCard)
				drawSelection(e, Color.LightSkyBlue, Color.LightCyan, 236);
			else
			{
				int deckCount = card.DeckCount(Ui);
				int collectionCount = card.CollectionCount(Ui);

				if (deckCount == 0 && collectionCount > 0)
					drawSelection(e, Color.Lavender, Color.White, 96);
				else if (deckCount > 0)
					drawSelection(e, Color.Lavender, Color.White, 236);
			}

			drawLegalityWarning(e, sender, card);

			drawCountWarning(e, card);

			drawCount(sender, e, card);
		}

		private void drawLegalityWarning(CustomDrawArgs e, object sender, Card card)
		{
			var legalityWarning = _legalitySubsystem.GetWarning(card);

			int deckCount = card.DeckCount(Ui);

			if (legalityWarning == Legality.Restricted && deckCount <= 1 && sender == _layoutViewDeck)
				legalityWarning = null;

			if (string.IsNullOrEmpty(legalityWarning))
				return;

			var rect = getLegalityWarningRectangle(e);
			const int size = 18;
			var font = new Font(FontFamily.GenericMonospace, size, FontStyle.Italic | FontStyle.Bold);
			var brush = new SolidBrush(Color.FromArgb(192, Color.OrangeRed));

			var lineSize = e.Graphics.MeasureString(legalityWarning, font);
			rect.Offset((int) ((rect.Width - lineSize.Width)/2f), 0);

			e.Graphics.DrawString(legalityWarning, font, brush, rect, StringFormat.GenericDefault);
		}

		private void drawCountWarning(CustomDrawArgs e, Card card)
		{
			int countInMain = _deckModel.MainDeck.GetCount(card.Id);
			int countInSideboard = _deckModel.SideDeck.GetCount(card.Id);

			if (countInMain == 0 || countInSideboard == 0)
				// the excessive count is not due to main + side sum
				// therefore it is obvious, warning is not neseccary
				return;

			var totalCount = countInMain + countInSideboard;

			Color color;
			int maxCount;

			if (totalCount > card.MaxCountInDeck())
			{
				maxCount = card.MaxCountInDeck();
				color = Color.Crimson;
			}
			else
			{
				int collectionCount = card.CollectionCount(Ui);

				if (totalCount > collectionCount && collectionCount > 0)
				{
					maxCount = collectionCount;
					color = Color.Blue;
				}
				else
					return;
			}

			string warning;
			if (countInMain == 0)
				warning = $"{countInSideboard}/{maxCount}";
			else if (countInSideboard == 0)
				warning = $"{countInMain}/{maxCount}";
			else
				warning = $"{countInMain}+{countInSideboard}/{maxCount}";

			var rect = getCountWarningRectangle(e);

			const int size = 16;
			var font = new Font(FontFamily.GenericMonospace, size, FontStyle.Italic | FontStyle.Bold);
			
			var brush = new SolidBrush(Color.FromArgb(224, color));
			var lineSize = e.Graphics.MeasureString(warning, font);
			rect.Offset((int) ((rect.Width - lineSize.Width) / 2f), 0);

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
				e.Bounds.Left + (_imageLoader.CardSize.Width - size.Width)/2,
				e.Bounds.Bottom - size.Height,
				size.Width,
				size.Height);

			return rect;
		}

		private Rectangle getLegalityWarningRectangle(CustomDrawArgs e)
		{
			var stripSize = new Size(_imageLoader.CardSize.Width, 20.ByDpiHeight());

			var rect = new Rectangle(
				e.Bounds.Left,
				(int) (e.Bounds.Bottom - 2.625f * stripSize.Height),
				stripSize.Width,
				stripSize.Height);

			return rect;
		}

		private Rectangle getCountWarningRectangle(CustomDrawArgs e)
		{
			var result = getLegalityWarningRectangle(e);
			result.Offset(new Point(0, -result.Height));
			return result;
		}

		private string getCountText(object sender, Card card)
		{
			var countText = new StringBuilder();

			int deckCount = card.DeckCount(Ui);

			if (deckCount > 0)
				countText.Append(deckCount);

			int collectionCount = card.CollectionCount(Ui);

			if (collectionCount > 0 && (_deckModel.Zone != Zone.SampleHand || !_layoutViewDeck.Wraps(sender)))
				countText.AppendFormat(@"/{0}", collectionCount);

			string countTextStr = countText.ToString();
			return countTextStr;
		}

		private void setHighlightMatches(object sender, int rowHandle)
		{
			var view = getView(sender);

			if (!view.TextualFieldsVisible)
				return;

			var card = (Card) view.GetRow(rowHandle);

			if (card == null)
				return;

			foreach (var fieldName in view.FieldNames)
			{
				if (fieldName == nameof(Card.Image))
					continue;

				var text = _layoutViewCards.GetFieldText(rowHandle, fieldName);

				var matches = new List<TextRange>();
				var contextMatches = new List<TextRange>();

				addFilterButtonMatches(matches, fieldName, text);
				addSearchStringMatches(matches, contextMatches, fieldName, text);
				addLegalityMatches(matches, fieldName, text);

				var highlightRanges = getHighlightRanges(matches, contextMatches);
				_layoutViewCards.SetHighlightTextRanges(highlightRanges, rowHandle, fieldName);
			}
		}

		private void addFilterButtonMatches(List<TextRange> matches, string fieldName, string text)
		{
			var quickFilterMatches = _quickFilterFacade.GetMatches(text, fieldName);

			if (quickFilterMatches == null)
				return;

			matches.AddRange(quickFilterMatches);
		}

		private void addLegalityMatches(List<TextRange> matches, string fieldName, string text)
		{
			if (string.IsNullOrEmpty(_legalitySubsystem.FilterFormat))
				return;

			if (fieldName != nameof(Card.Rulings))
				return;

			var legalityMatches = new Regex(_legalitySubsystem.FilterFormat).Matches(text)
				.OfType<Match>()
				.Where(_ => _.Success)
				.Select(TextRange.Copy);

			matches.AddRange(legalityMatches);
		}

		private void addSearchStringMatches(List<TextRange> matches, List<TextRange> contextMatches, string fieldName, string displayText)
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

				var patternsSet = new Dictionary<string, Regex>();
				var contextPatternsSet = new Dictionary<string, Regex>();

				var relevantTokens = term.Value.Where(token => 
					token.Type.IsAny(TokenType.FieldValue|TokenType.AnyChar|TokenType.RegexBody) &&
					!string.IsNullOrEmpty(token.Value));

				foreach (var token in relevantTokens)
				{
					getPattern(token, out string pattern, out var contextPatterns);

					addPattern(pattern, patternsSet);

					foreach (string contextPattern in contextPatterns)
						addPattern(contextPattern, contextPatternsSet);
				}

				addMatches(displayText, patternsSet.Values, matches, fieldName);
				addMatches(displayText, contextPatternsSet.Values, contextMatches, fieldName);
			}
		}

		private void addPattern(string pattern, Dictionary<string, Regex> patternsSet)
		{
			if (pattern == null)
				return;

			if (patternsSet.TryGetValue(pattern, out var regex))
				return;
			
			if (!_regexCache.TryGetValue(pattern, out regex))
			{
				regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
				_regexCache.Add(pattern, regex);
			}

			patternsSet.Add(pattern, regex);
		}

		private void addMatches(string displayText, IEnumerable<Regex> patterns, List<TextRange> matches, string fieldName)
		{
			foreach (Regex findRegex in patterns)
			{
				if (DocumentFactory.NotAnalyzedFields.Contains(fieldName))
				{
					var toAdd = findRegex
						.Matches(displayText)
						.Cast<Match>()
						.Where(match => match.Success && match.Length != 0)
						.Select(TextRange.Copy);

					matches.AddRange(toAdd);
				}
				else
				{
					var tokenStream = _mtgdbAnalyzer.GetTokenStream(fieldName, displayText);

					tokenStream.Reset();

					using (tokenStream)
						while (tokenStream.IncrementToken())
						{
							var term = tokenStream.GetAttribute<ICharTermAttribute>().ToString();
							var offset = tokenStream.GetAttribute<IOffsetAttribute>().StartOffset;

							var toAdd = findRegex.Matches(term)
								.Cast<Match>()
								.Where(match => match.Success && match.Length != 0)
								.Select(match => TextRange.Offset(offset, match));

							matches.AddRange(toAdd);
						}
				}
			}
		}

		private void getPattern(Token token, out string result, out List<string> contextPatterns)
		{
			var prefixTokens = getPrefixTokens(token);
			var currentTokens = new List<Token> { token };
			var suffixTokens = getSuffixTokens(token);

			if (token.Type.IsAny(TokenType.FieldValue | TokenType.RegexBody))
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
				i += suffixTokens.TakeWhile(_ => !_.Type.IsAny(TokenType.Wildcard)).Count();

				if (i == tokenGroup.Count)
					break;

				prefixTokens.Clear();
				prefixTokens.AddRange(
					tokenGroup.Take(i));

				currentTokens.Clear();
				currentTokens.AddRange(
					tokenGroup.Skip(i).TakeWhile(_ => _.Type.IsAny(TokenType.Wildcard)));

				i = prefixTokens.Count + currentTokens.Count;

				suffixTokens.Clear();
				suffixTokens.AddRange(
					tokenGroup.Skip(prefixTokens.Count + currentTokens.Count));

				contextPatterns.Add(getPattern(prefixTokens, currentTokens, suffixTokens));
			}
		}

		private string getPattern(List<Token> prefixTokens, List<Token> radixTokens, List<Token> suffixTokens)
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

			if (token.Type.IsAny(TokenType.RegexBody))
				return suffixTokens;

			while (true)
			{
				if (token.Next == null ||
					!token.Next.TouchesCaret(token.Position + token.Value.Length) ||
					token.Type.IsAny(TokenType.FieldValue) && token.Value[token.Value.Length - 1].IsCj() ||
					token.Next.Type.IsAny(TokenType.FieldValue) && token.Next.Value[0].IsCj())
					// Вплотную прилегающее к wildcard значение является его продолжением в отличие от случая, если между ними есть пробел,
					// тогда это уже другой термин
					break;
				if (token.Next.Type.IsAny(TokenType.Wildcard | TokenType.FieldValue))
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

			if (token.Type.IsAny(TokenType.RegexBody))
				return prefixTokens;

			while (true)
			{
				if (token.Previous == null ||
					!token.Previous.TouchesCaret(token.Position) ||
					token.Type.IsAny(TokenType.FieldValue) && token.Value[0].IsCj() ||
					token.Previous.Type.IsAny(TokenType.FieldValue) && token.Previous.Value[token.Previous.Value.Length - 1].IsCj())
					// Вплотную прилегающее к wildcard значение является его продолжением в отличие от случая, если между ними есть пробел,
					// тогда это уже другой термин
					break;

				if (token.Previous.Type.IsAny(TokenType.Wildcard | TokenType.FieldValue))
					prefixTokens.Insert(0, token.Previous);
				else
					break;

				token = token.Previous;
			}

			return prefixTokens;
		}

		private string getPattern(IEnumerable<Token> tokens)
		{
			var pattern = new StringBuilder();
			foreach (var token in tokens)
			{
				if (token.Type.IsAny(TokenType.AnyChar))
					pattern.Append(MtgdbTokenizerPatterns.CharPattern);
				else if (token.Type.IsAny(TokenType.AnyString))
					pattern.Append(MtgdbTokenizerPatterns.CharPattern + "*");
				else if (token.Type.IsAny(TokenType.FieldValue))
				{
					string luceneUnescaped = StringEscaper.Unescape(token.Value);

					if (!DocumentFactory.NotAnalyzedFields.Contains(token.ParentField))
					{
						var builder = new StringBuilder();
						var tokenStream = _mtgdbAnalyzer.GetTokenStream(token.ParentField, luceneUnescaped);

						tokenStream.Reset();

						using (tokenStream)
							while (tokenStream.IncrementToken())
							{
								var term = tokenStream.GetAttribute<ICharTermAttribute>().ToString();
								builder.Append(term);
							}

						luceneUnescaped = builder.ToString();
					}

					foreach (char c in luceneUnescaped)
					{
						var equivalents = MtgdbTokenizerPatterns.GetEquivalents(c).ToArray();


						if (equivalents.Length == 0)
							continue;
						if (equivalents.Length == 1)
							pattern.Append(Regex.Escape(new string(equivalents[0], 1)));
						else
						{
							pattern.Append("(");

							pattern.Append(Regex.Escape(new string(equivalents[0], 1)));

							for (int i = 1; i < equivalents.Length; i++)
							{
								pattern.Append("|");
								pattern.Append(Regex.Escape(new string(equivalents[i], 1)));
							}

							pattern.Append(")");
						}
					}
				}
				else if (token.Type.IsAny(TokenType.RegexBody))
				{
					pattern.Append(token.Value);
				}
			}

			return pattern.ToString();
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



		private LayoutView getView(object view)
		{
			if (_layoutViewCards.Wraps(view))
				return _layoutViewCards;

			if (_layoutViewDeck.Wraps(view))
				return _layoutViewDeck;

			throw new Exception(@"wrapper not found");
		}


		public UiModel Ui { get; set; }


		private readonly Dictionary<string, Regex> _regexCache = new Dictionary<string, Regex>();

		private readonly LayoutView _layoutViewCards;
		private readonly LayoutView _layoutViewDeck;
		private readonly Font _font;
		private readonly DraggingSubsystem _draggingSubsystem;
		private readonly SearchStringSubsystem _searchStringSubsystem;
		private readonly DeckModel _deckModel;
		private readonly QuickFilterFacade _quickFilterFacade;
		private readonly LegalitySubsystem _legalitySubsystem;
		private readonly ImageLoader _imageLoader;
		private readonly MtgdbAnalyzer _mtgdbAnalyzer;
	}
}