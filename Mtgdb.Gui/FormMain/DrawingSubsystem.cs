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
using System.Windows.Forms;
using Lucene.Net.Contrib;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Mtgdb.Gui.Resx;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Gui
{
	[Localizable(false)]
	public class DrawingSubsystem
	{
		public DrawingSubsystem(
			MtgLayoutView layoutViewCards,
			MtgLayoutView layoutViewDeck,
			DraggingSubsystem draggingSubsystem,
			SearchStringSubsystem searchStringSubsystem,
			DeckModel deckModel,
			QuickFilterFacade quickFilterFacade,
			LegalitySubsystem legalitySubsystem,
			ImageLoader imageLoader)
		{
			_layoutViewCards = layoutViewCards;
			_layoutViewDeck = layoutViewDeck;
			_draggingSubsystem = draggingSubsystem;
			_searchStringSubsystem = searchStringSubsystem;
			_deckModel = deckModel;
			_quickFilterFacade = quickFilterFacade;
			_legalitySubsystem = legalitySubsystem;
			_imageLoader = imageLoader;

			_layoutViewCards.RowDataLoaded += setHighlightMatches;
			_layoutViewCards.SetIconRecognizer(createIconRecognizer());

			_analyzer = new MtgAnalyzer();
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
				"E",
				"Q"
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
			rect.Offset((int) ((rect.Width - lineSize.Width) / 2f), 0);

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

			int cornerYSize = rect.Height / cornerShare;
			int cornerXSize = rect.Height / cornerShare;

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

			var font = new Font("Arial Black", 18.ByDpiHeight(), GraphicsUnit.Pixel);
			var textFormatFlags = new StringFormat(default(StringFormatFlags)).ToTextFormatFlags();

			var textSize = TextRenderer.MeasureText(
				e.Graphics,
				countText,
				font,
				new Size((int) (rect.Width * 1.5f), rect.Height),
				textFormatFlags);

			var targetRect = new Rectangle(
				(int) Math.Ceiling(rect.Left + 0.5f * (rect.Width - textSize.Width)),
				(int) Math.Ceiling(rect.Top + 0.5f * (rect.Height - textSize.Height)),
				textSize.Width,
				textSize.Height);

			targetRect.Inflate(1, 0);
			targetRect.Offset(2, 0);

			TextRenderer.DrawText(
				e.Graphics,
				countText,
				font,
				targetRect,
				Color.Black,
				textFormatFlags);
		}

		private Rectangle getSelectionRectangle(CustomDrawArgs e)
		{
			var size = new Size(80, 30).ByDpi();

			var rect = new Rectangle(
				e.Bounds.Left + (_imageLoader.CardSize.Width - size.Width) / 2,
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
				countText.AppendFormat(@" / {0}", collectionCount);

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

			foreach (var displayField in view.FieldNames)
			{
				if (displayField == nameof(Card.Image))
					continue;

				var displayText = _layoutViewCards.GetFieldText(rowHandle, displayField);

				var matches = new List<TextRange>();
				var contextMatches = new List<TextRange>();

				addFilterButtonMatches(matches, displayField, displayText);
				addSearchStringMatches(matches, contextMatches, displayField, displayText);
				addLegalityMatches(matches, displayField, displayText);

				var highlightRanges = getHighlightRanges(matches, contextMatches);
				_layoutViewCards.SetHighlightTextRanges(highlightRanges, rowHandle, displayField);
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

		private void addSearchStringMatches(List<TextRange> matches, List<TextRange> contextMatches, string displayField, string displayText)
		{
			var searchResult = _searchStringSubsystem.SearchResult;

			addTermMatches(matches, contextMatches, displayField, displayText, searchResult);
			addPhraseMatches(matches, displayField, displayText, searchResult);
		}

		private void addTermMatches(
			List<TextRange> matches,
			List<TextRange> contextMatches,
			string displayField,
			string displayText,
			SearchResult searchResult)
		{
			var highlightTerms = searchResult?.HighlightTerms;

			if (highlightTerms == null)
				return;

			foreach (var pair in highlightTerms.Where(term => isRelevantField(displayField, term.Key)))
			{
				var terms = pair.Value;
				var patternsSet = new Dictionary<string, (HashSet<string> TokenFields, Regex Pattern)>();
				var contextPatternsSet = new Dictionary<string, (HashSet<string> TokenFields, Regex Pattern)>();

				addTextualPatterns(searchResult.Query, terms, patternsSet, contextPatternsSet);
				addKeywordPatterns(searchResult.Query, terms, patternsSet);

				addMatches(displayText, patternsSet.Values, matches);
				addMatches(displayText, contextPatternsSet.Values, contextMatches);
			}
		}

		private void addTextualPatterns(
			string query,
			IReadOnlyList<Token> terms,
			Dictionary<string, (HashSet<string> TokenFields, Regex Pattern)> patternsSet,
			Dictionary<string, (HashSet<string> TokenFields, Regex Pattern)> contextPatternsSet)
		{
			var textualTokens = terms.Where(t =>
				t.Type.IsAny(TokenType.FieldValue | TokenType.AnyChar | TokenType.RegexBody) &&
				!Str.Equals(t.ParentField, nameof(KeywordDefinitions.Keywords)) &&
				!string.IsNullOrEmpty(t.Value));

			foreach (var token in textualTokens)
			{
				if (!getPattern(query, token, out string pattern, out var contextPatterns))
					continue;
				
				addPattern(token.ParentField, pattern, patternsSet);

				foreach (string contextPattern in contextPatterns)
					addPattern(token.ParentField, contextPattern, contextPatternsSet);
			}
		}

		private static void addKeywordPatterns(
			string query,
			IReadOnlyList<Token> terms,
			IDictionary<string, (HashSet<string> TokenFields, Regex Pattern)> patternsSet)
		{
			var keywordTerms = terms.Where(t => Str.Equals(t.ParentField, nameof(KeywordDefinitions.Keywords)));
			foreach (var keywordTerm in keywordTerms)
			{
				string termText = keywordTerm.GetPhraseText(query);

				if (!KeywordDefinitions.KeywordPatternsByValue.TryGetValue(termText, out var regex))
					continue;

				string pattern = regex.ToString();

				if (patternsSet.TryGetValue(pattern, out var tokenPattern))
				{
					tokenPattern.TokenFields.Add(keywordTerm.ParentField);
					continue;
				}

				tokenPattern = (new HashSet<string>(Str.Comparer) { keywordTerm.ParentField }, regex);
				patternsSet.Add(pattern, tokenPattern);
			}
		}

		private void addPhraseMatches(
			List<TextRange> matches,
			string displayField,
			string displayText,
			SearchResult searchResult)
		{
			var highlightPhrases = searchResult?.HighlightPhrases;

			if (highlightPhrases == null)
				return;

			var displayTextTokens = new Lazy<IReadOnlyList<(string Term, int Offset)>>(() =>
				_analyzer.GetTokens(displayField, displayText).ToReadOnlyList());

			var displayTextValues = new Lazy<IReadOnlyList<string>>(() =>
				displayTextTokens.Value.Select(_ => _.Term).ToReadOnlyList());

			foreach (var term in highlightPhrases.Where(term => isRelevantField(displayField, term.Key)))
			{
				foreach (var tokenValues in term.Value)
				{
					if (tokenValues.Count == 0)
						continue;

					if (tokenValues.Count == 1)
					{
						var mathesToAdd = displayTextTokens.Value
							.Where(token => Str.Comparer.Equals(token.Term, tokenValues[0]))
							.Select(token => new TextRange(token.Offset, token.Term.Length));

						matches.AddRange(mathesToAdd);
						continue;
					}

					var searcher = new KnutMorrisPrattSubstringSearch<string>(tokenValues, Str.Comparer);

					foreach (int index in searcher.FindAll(displayTextValues.Value))
					{
						var firstWordMatch = displayTextTokens.Value[index];
						var lastWordMatch = displayTextTokens.Value[index + tokenValues.Count - 1];

						var startIndex = firstWordMatch.Offset;
						var length = lastWordMatch.Offset + lastWordMatch.Term.Length - startIndex;

						matches.Add(new TextRange(startIndex, length));
					}
				}
			}
		}

		private static bool isRelevantField(string displayField, string queryField)
		{
			return string.IsNullOrEmpty(queryField) || Str.Equals(queryField, displayField);
		}

		private void addPattern(string termField, string pattern, IDictionary<string, (HashSet<string> TokenFields, Regex Pattern)> patternsSet)
		{
			if (pattern == null)
				return;

			if (patternsSet.TryGetValue(pattern, out var tokenPattern))
			{
				tokenPattern.TokenFields.Add(termField);
				return;
			}

			tokenPattern = (new HashSet<string>(Str.Comparer) { termField }, getRegex(pattern));
			patternsSet.Add(pattern, tokenPattern);
		}

		private Regex getRegex(string pattern)
		{
			if (_regexCache.TryGetValue(pattern, out var regex))
				return regex;
			
			regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
			_regexCache.Add(pattern, regex);

			return regex;
		}

		private void addMatches(string displayText, IEnumerable<(HashSet<string> TokenField, Regex Pattern)> tokenPatterns, List<TextRange> matches)
		{
			foreach (var (tokenFields, findRegex) in tokenPatterns)
				foreach (var tokenField in tokenFields)
					foreach (var token in _analyzer.GetTokens(tokenField, displayText))
					{
						var toAdd = findRegex.Matches(token.Term)
							.Cast<Match>()
							.Where(match => match.Success && match.Length != 0)
							.Select(match => new TextRange(match.Index + token.Offset, match.Length));

						matches.AddRange(toAdd);
					}
		}

		private bool getPattern(string query, Token token, out string result, out List<string> contextPatterns)
		{
			if (token.IsPhraseStart && !token.IsPhraseComplex)
			{
				var patternBuilder = new StringBuilder();
				appendFieldValuePattern(patternBuilder, token.ParentField, token.GetPhraseText(query));
				
				result = patternBuilder.ToString();
				contextPatterns = new List<string>();
				return true;
			}

			if (token.Type.IsAny(TokenType.RegexBody))
			{
				result = token.Value;
				contextPatterns = new List<string>();
				return isValidRegex(result);
			}

			if (token.ParentField.IsNumericField())
			{
				result = @"\$?" + Regex.Escape(token.Value);
				contextPatterns = new List<string>();
				return true;
			}

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

			return true;
		}

		private static bool isValidRegex(string result)
		{
			try
			{
				var regex = new Regex(result);
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		private string getPattern(List<Token> prefixTokens, List<Token> radixTokens, List<Token> suffixTokens)
		{
			string prefixPattern = getPattern(prefixTokens);
			string suffixPattern = getPattern(suffixTokens);
			string radixPattern = getPattern(radixTokens);

			string result = $"(?<=^{prefixPattern}){radixPattern}(?={suffixPattern}$)";
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
					pattern.Append(MtgAplhabet.CharPattern);
				else if (token.Type.IsAny(TokenType.AnyString))
					pattern.Append(MtgAplhabet.CharPattern + "*");
				else if (token.Type.IsAny(TokenType.FieldValue))
					appendFieldValuePattern(pattern, token.ParentField, token.Value);
			}

			return pattern.ToString();
		}

		private void appendFieldValuePattern(StringBuilder patternBuilder, string tokenField, string tokenValue)
		{
			var builder = new StringBuilder();

			foreach (var word in _analyzer.GetTokens(tokenField, StringEscaper.Unescape(tokenValue)))
				builder.Append(word.Term);

			var luceneUnescaped = builder.ToString();

			foreach (char c in luceneUnescaped)
			{
				var equivalents = MtgAplhabet.GetEquivalents(c).ToArray();

				if (equivalents.Length == 0)
					continue;
				if (equivalents.Length == 1)
					patternBuilder.Append(Regex.Escape(new string(equivalents[0], 1)));
				else
				{
					patternBuilder.Append("(");

					patternBuilder.Append(Regex.Escape(new string(equivalents[0], 1)));

					for (int i = 1; i < equivalents.Length; i++)
					{
						patternBuilder.Append("|");
						patternBuilder.Append(Regex.Escape(new string(equivalents[i], 1)));
					}

					patternBuilder.Append(")");
				}
			}
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



		private MtgLayoutView getView(object view)
		{
			if (_layoutViewCards.Wraps(view))
				return _layoutViewCards;

			if (_layoutViewDeck.Wraps(view))
				return _layoutViewDeck;

			throw new Exception(@"wrapper not found");
		}


		public UiModel Ui { get; set; }


		private readonly Dictionary<string, Regex> _regexCache = new Dictionary<string, Regex>();

		private readonly MtgLayoutView _layoutViewCards;
		private readonly MtgLayoutView _layoutViewDeck;
		private readonly DraggingSubsystem _draggingSubsystem;
		private readonly SearchStringSubsystem _searchStringSubsystem;
		private readonly DeckModel _deckModel;
		private readonly QuickFilterFacade _quickFilterFacade;
		private readonly LegalitySubsystem _legalitySubsystem;
		private readonly ImageLoader _imageLoader;
		private readonly MtgAnalyzer _analyzer;
	}
}