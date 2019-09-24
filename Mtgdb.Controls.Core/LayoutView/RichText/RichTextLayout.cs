using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	internal class RichTextLayout : IDisposable
	{
		public RichTextLayout(RichTextRenderContext renderContext, IconRecognizer iconRecognizer)
		{
			_renderContext = renderContext;
			_iconRecognizer = iconRecognizer;

			_highlightBrush = new SolidBrush(renderContext.HighlightColor);
			_highlightContextBrush = new SolidBrush(renderContext.HighlightContextColor);
			_selectionBrush = new SolidBrush(Color.FromArgb(_renderContext.SelectionAlpha,
				_renderContext.SelectionBackColor));

			_pen = new Pen(renderContext.HighlightBorderColor, renderContext.HighlightBorderWidth);

			_x = _renderContext.Rect.Left;
			_y = _renderContext.Rect.Top;

			var lineSize = getLineSize(@" ");
			_lineHeight = lineSize.Height;
			_spaceWidth = lineSize.Width;

			_iconShadowOffset = new SizeF(-0.7f, 1f).ByDpi().ToPointF();
			_caretPen = new Pen(_renderContext.ForeColor, 1f);
			_shadowBrush = new SolidBrush(Color.FromArgb(0x50, 0x50, 0x50));
		}

		public bool PrintWord(List<RichTextToken> word)
		{
			if (_y + _lineHeight * HeightPart > _renderContext.Rect.Bottom)
				return false;

			var size = getLineSize(_renderContext.Text, word);
			if (_x + size.Width >= _renderContext.Rect.Right && _x > _renderContext.Rect.Left &&
				!newLine())
			{
				printWord(word);
				return false;
			}

			printWord(word);
			return true;
		}

		private void printWord(List<RichTextToken> word)
		{
			foreach (var token in word)
			{
				var location = new PointF(_x, _y);
				var width = getTokenWidth(_renderContext.Text, token);
				var overflow = _x + width >= _renderContext.Rect.Right;
				RectangleF targetRect;
				if (overflow)
					targetRect = new RectangleF(location,
						new SizeF(_renderContext.Rect.Right - _x, _lineHeight));
				else
					targetRect = new RectangleF(location, new SizeF(width, _lineHeight));

				var printBatch = new RenderBatch(token.IsHighlighted);

				if (token.IsHighlighted)
					printHighlight(targetRect, printBatch, token.IsContext);

				string tokenText = _renderContext.Text.Substring(token.Index, token.Length);

				if (token.IconName == null)
				{
					if (overflow)
						while (tokenText.Length > 0 && getLineSize(tokenText).Width > targetRect.Width)
							tokenText = tokenText.Substring(0, tokenText.Length - 1);

					if (tokenText.Length > 0)
					{
						printBatch.Add(
							targetRect,
							(rect, hb, he) =>
							{
								_renderContext.TextSelection.PrintedTokens.Add((token, rect));

								var foreColor = _renderContext.ForeColor;

								if (printSelection(rect, token))
									foreColor = _renderContext.SelectionForeColor;

								var textRect = rect;
								textRect.Inflate(1, 0);
								textRect.Offset(1, 0);

								_renderContext.Graphics.DrawText(tokenText, _renderContext.Font, textRect.Round(),
									foreColor);
								printCaret(rect, token);
							});
					}
				}
				else
				{
					if (token.IconNeedsShadow)
					{
						printBatch.Add(targetRect,
							(rect, hb, he) =>
							{
								_renderContext.TextSelection.PrintedTokens.Add((token, rect));

								var icon = _iconRecognizer.GetIcon(token.IconName, _lineHeight.Round() - 1);
								var iconRect = new RectangleF(rect.Location.Round(), icon.Size);

								printSelection(rect, token);

								var shadowOffset = _iconShadowOffset.MultiplyBy(_lineHeight / 16f);

								var shadowRect = iconRect;
								shadowRect.Offset(shadowOffset);
								_renderContext.Graphics.FillEllipse(_shadowBrush, shadowRect);

								_renderContext.Graphics.DrawImage(icon, iconRect);
								printCaret(rect, token);
							});
					}
					else
					{
						printBatch.Add(targetRect,
							(rect, hb, he) =>
							{
								_renderContext.TextSelection.PrintedTokens.Add((token, rect));

								var icon = _iconRecognizer.GetIcon(token.IconName, _lineHeight.Round());
								var iconRect = new RectangleF(rect.Location.Round(), icon.Size);

								printSelection(rect, token);
								_renderContext.Graphics.DrawImage(icon, iconRect);
								printCaret(rect, token);
							});
					}
				}

				_lineQueue.Add(printBatch);

				_x += targetRect.Width;

				if (overflow)
					break;
			}
		}

		private void printHighlight(RectangleF targetRect, RenderBatch batch, bool isContext)
		{
			batch.Add(targetRect,
				(rectF, hb, he) =>
				{
					var rect = rectF.Round();

					var fillRect = (RectangleF) rect;
					fillRect.Offset(-0.5f, -0.5f);

					var brush = isContext ? _highlightContextBrush : _highlightBrush;
					_renderContext.Graphics.FillRectangle(brush, fillRect);

					int left = rect.Left - 1;
					int top = rect.Top - 1;
					int bottom = rect.Bottom;
					int right = rect.Right;

					if (hb)
						_renderContext.Graphics.DrawLine(_pen,
							left,
							top,
							left,
							bottom);

					if (he)
						_renderContext.Graphics.DrawLine(_pen,
							right,
							top,
							right,
							bottom);

					_renderContext.Graphics.DrawLine(_pen,
						left,
						top,
						right,
						top);

					_renderContext.Graphics.DrawLine(_pen,
						left,
						bottom,
						right,
						bottom);
				});
		}


		public bool PrintSpace(RichTextToken space)
		{
			if (_y + _lineHeight * HeightPart < _renderContext.Rect.Bottom)
			{
				if (_x + _spaceWidth >= _renderContext.Rect.Right)
					return newLine();

				var targetRect = new RectangleF(_x, _y, _spaceWidth, _lineHeight);

				var printBatch = new RenderBatch(space.IsHighlighted);

				if (space.IsHighlighted)
					printHighlight(targetRect, printBatch, space.IsContext);

				printBatch.Add(targetRect, (rect, hb, he) =>
				{
					_renderContext.TextSelection.PrintedTokens.Add((space, rect));
					printSelection(rect, space);
					printCaret(rect, space);
				});

				_lineQueue.Add(printBatch);

				_x += _spaceWidth;
				return true;
			}

			return false;
		}

		public bool NewParagraph()
		{
			Flush();

			_y += _lineHeight * 4f / 3f;
			_x = _renderContext.Rect.Left;

			return _y + _lineHeight * HeightPart < _renderContext.Rect.Bottom;
		}

		private bool newLine()
		{
			_y += _lineHeight;
			if (_y + _lineHeight * HeightPart < _renderContext.Rect.Bottom)
			{
				Flush();
				_x = _renderContext.Rect.Left;
				return true;
			}

			_y -= _lineHeight;
			return false;
		}


		private SizeF getLineSize(string text, IList<RichTextToken> word)
		{
			float width = word.Sum(token => getTokenWidth(text, token));
			return new SizeF(width, _lineHeight);
		}

		private float getTokenWidth(string text, RichTextToken richTextToken)
		{
			float delta;
			if (richTextToken.IconName == null)
				delta = getLineSize(text.Substring(richTextToken.Index, richTextToken.Length)).Width;
			else
				delta = _iconRecognizer.GetIcon(richTextToken.IconName, (int) Math.Round(_lineHeight))
					.Width;

			return delta;
		}

		private SizeF getLineSize(string tokenText) =>
			_renderContext.Graphics.MeasureText(tokenText, _renderContext.Font);

		private bool printSelection(RectangleF tokenRect, RichTextToken token)
		{
			var textSelection = _renderContext.TextSelection;

			if (!_renderContext.Selecting && textSelection.IsEmpty)
				return false;

			int tokenRight = token.Index + token.Length;

			bool isSelected =
				textSelection.Start < tokenRight &&
				textSelection.Start + textSelection.Length > token.Index;

			if (textSelection.IsEmpty)
			{
				var (isStart, isRightToLeft) = isRectSelectionStart(tokenRect);

				if (!isStart)
					return false;

				if (isRightToLeft)
				{
					textSelection.Begin = tokenRight;
					textSelection.End = token.Index;
				}
				else
				{
					textSelection.Begin = token.Index;
					textSelection.End = tokenRight;
				}
			}
			else if (!isSelected)
			{
				if (!isRectSelectionContinuation(tokenRect))
					return false;

				if (textSelection.IsRightToLeft)
					textSelection.Begin = tokenRight;
				else
					textSelection.End = tokenRight;
			}

			RectangleF selectionRect = tokenRect.Round();
			selectionRect.Offset(-0.5f, -0.5f);

			_renderContext.Graphics.FillRectangle(_selectionBrush, selectionRect);

			return true;
		}

		private void printCaret(RectangleF rect, RichTextToken token)
		{
			if (_renderContext.Selecting)
				return;

			if (!_renderContext.TextSelection.IsCaretVisible)
				return;

			var roundedRect = rect.Round();

			int x;
			if (token.Right == _renderContext.TextSelection.End)
				x = roundedRect.Right - 1;
			else if (token.Index == _renderContext.TextSelection.End)
				x = roundedRect.Left - 1;
			else
				return;

			float top = roundedRect.Top;
			var bottom = roundedRect.Bottom - 1f;

			_renderContext.Graphics.DrawLine(_caretPen, x, top, x, bottom);
		}

		private (bool IsStart, bool IsRightToLeft) isRectSelectionStart(RectangleF tokenRect)
		{
			var (selectionStartX, selectionEndX) = getSelectionDelimitersForLine(tokenRect);
			bool isStart = tokenRect.Right > selectionStartX && tokenRect.Left <= selectionEndX;

			bool isMultiline = selectionStartX == int.MinValue || selectionEndX == int.MaxValue;

			bool isRightToLeft =
				isMultiline && _renderContext.SelectionStart.Y > _renderContext.SelectionEnd.Y ||
				!isMultiline && _renderContext.SelectionStart.X > _renderContext.SelectionEnd.X;

			return (isStart, isRightToLeft);
		}

		private bool isRectSelectionContinuation(RectangleF rectangle)
		{
			return rectangle.Left <= getSelectionDelimitersForLine(rectangle).SelectionEndX;
		}

		private (int SelectionStartX, int SelectionEndX) getSelectionDelimitersForLine(
			RectangleF tokenFromLine)
		{
			float lineTop = tokenFromLine.Top;
			float lineBottom = tokenFromLine.Bottom - 1;

			bool selectionStartsBeforeLine =
				_renderContext.SelectionStart.Y < lineTop ||
				_renderContext.SelectionEnd.Y < lineTop;

			bool selectionStartsWithinLine =
				((float) _renderContext.SelectionStart.Y).IsWithin(lineTop, lineBottom);
			bool selectionEndsWithinLine =
				((float) _renderContext.SelectionEnd.Y).IsWithin(lineTop, lineBottom);

			bool selectionContinuesNextLine =
				_renderContext.SelectionStart.Y > lineBottom ||
				_renderContext.SelectionEnd.Y > lineBottom;

			int selectionEndX;
			if (selectionContinuesNextLine)
				selectionEndX = int.MaxValue;
			else if (selectionStartsWithinLine && selectionEndsWithinLine)
				selectionEndX = Math.Max(_renderContext.SelectionStart.X, _renderContext.SelectionEnd.X);
			else if (selectionStartsWithinLine)
				selectionEndX = _renderContext.SelectionStart.X;
			else if (selectionEndsWithinLine)
				selectionEndX = _renderContext.SelectionEnd.X;
			else
				selectionEndX = int.MinValue;

			int selectionStartX;
			if (selectionStartsBeforeLine)
				selectionStartX = int.MinValue;
			else if (selectionStartsWithinLine && selectionEndsWithinLine)
				selectionStartX = Math.Min(_renderContext.SelectionStart.X, _renderContext.SelectionEnd.X);
			else if (selectionStartsWithinLine)
				selectionStartX = _renderContext.SelectionStart.X;
			else if (selectionEndsWithinLine)
				selectionStartX = _renderContext.SelectionEnd.X;
			else
				selectionStartX = int.MaxValue;

			return (selectionStartX, selectionEndX);
		}


		public void Flush()
		{
			float offsetX;
			if (_renderContext.HorizAlignment == HorizontalAlignment.Right)
				offsetX = _renderContext.Rect.Right - _x;
			else if (_renderContext.HorizAlignment == HorizontalAlignment.Center)
				offsetX = 0.5f * (_renderContext.Rect.Right - _x);
			else
				offsetX = 0f;

			var queue = _lineQueue;

			for (int i = 0; i < queue.Count; i++)
			{
				queue[i].Offset(offsetX, 0);

				bool highlightBegin = queue[i].IsHighlighted && (i == 0 || !queue[i - 1].IsHighlighted);
				bool highlightEnd = queue[i].IsHighlighted &&
					(i == queue.Count - 1 || !queue[i + 1].IsHighlighted);

				queue[i].Invoke(highlightBegin, highlightEnd);
			}

			queue.Clear();
		}

		public void Dispose()
		{
			_highlightBrush.Dispose();
			_highlightContextBrush.Dispose();
			_pen.Dispose();
			_shadowBrush.Dispose();
			_selectionBrush.Dispose();
			_caretPen.Dispose();
		}


		private float _x;
		private float _y;
		private const float HeightPart = 0.8f;
		private readonly float _lineHeight;
		private readonly float _spaceWidth;
		private readonly RichTextRenderContext _renderContext;
		private readonly RenderBatchQueue _lineQueue = new RenderBatchQueue();
		private readonly IconRecognizer _iconRecognizer;
		private readonly PointF _iconShadowOffset;

		private readonly Brush _highlightBrush;
		private readonly Brush _highlightContextBrush;
		private readonly Pen _pen;
		private readonly SolidBrush _shadowBrush;
		private readonly SolidBrush _selectionBrush;
		private readonly Pen _caretPen;
	}
}
