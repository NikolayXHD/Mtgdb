using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	internal class RichTextLayout
	{
		public RichTextLayout(RichTextRenderContext renderContext, IconRecognizer iconRecognizer)
		{
			_renderContext = renderContext;
			_iconRecognizer = iconRecognizer;

			_brush = new SolidBrush(renderContext.HighlightColor);
			_contextBrush = new SolidBrush(renderContext.HighlightContextColor);
			_pen = new Pen(renderContext.HighlightBorderColor, renderContext.HighlightBorderWidth);

			_x = _renderContext.Rect.Left;
			_y = _renderContext.Rect.Top;

			var lineSize = getLineSize(@" ");
			_lineHeight = lineSize.Height;
			_spaceWidth = lineSize.Width;

			_iconShadowOffset = new SizeF(-0.7f, 1f).ByDpi().ToPointF();
		}

		public bool PrintWord(List<RichTextToken> word)
		{
			if (_y + _lineHeight * HeightPart > _renderContext.Rect.Bottom)
				return false;

			var size = getLineSize(_renderContext.Text, word);
			if (_x + size.Width >= _renderContext.Rect.Right && _x > _renderContext.Rect.Left && !newLine())
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
					targetRect = new RectangleF(location, new SizeF(_renderContext.Rect.Right - _x, _lineHeight));
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
								rect = new RectangleF(rect.Location, new SizeF(rect.Width + 2, rect.Height));
								var rectangle = toRectangle(rect);

								var foreColor = _renderContext.ForeColor;

								if (printSelection(rect, token))
									foreColor = _renderContext.SelectionForeColor;

								TextRenderer.DrawText(
									_renderContext.Graphics,
									tokenText,
									_renderContext.Font,
									rectangle,
									foreColor,
									_renderContext.StringFormat.ToTextFormatFlags());
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
								var icon = _iconRecognizer.GetIcon(token.IconName, _lineHeight.Round() - 1);
								var iconRect = new RectangleF(rect.Location.Round(), icon.Size);

								printSelection(rect, token);

								var shadowOffset = _iconShadowOffset.MultiplyBy(_lineHeight / 16f);

								var shadowRect = iconRect;
								shadowRect.Offset(shadowOffset);
								_renderContext.Graphics.FillEllipse(_shadowBrush, shadowRect);

								_renderContext.Graphics.DrawImage(icon, iconRect);
							});
					}
					else
					{
						printBatch.Add(targetRect,
							(rect, hb, he) =>
							{
								var icon = _iconRecognizer.GetIcon(token.IconName, _lineHeight.Round());
								var iconRect = new RectangleF(targetRect.Location.Round(), icon.Size);

								printSelection(rect, token);
								_renderContext.Graphics.DrawImage(icon, iconRect);
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
					var rect = toRectangle(new RectangleF(rectF.Location, new SizeF(rectF.Width - _spaceWidth + 1, rectF.Height)));

					rect = new Rectangle(new Point(rect.X - 1, rect.Y - 1), new Size(rect.Width + 2, rect.Height + 1));

					var brush = isContext ? _contextBrush : _brush;
					_renderContext.Graphics.FillRectangle(brush, rect);

					if (hb)
						_renderContext.Graphics.DrawLine(_pen,
							rect.Left,
							rect.Top,
							rect.Left,
							rect.Bottom);

					if (he)
						_renderContext.Graphics.DrawLine(_pen,
							rect.Right,
							rect.Top,
							rect.Right,
							rect.Bottom);

					_renderContext.Graphics.DrawLine(_pen,
						rect.Left,
						rect.Top,
						rect.Right,
						rect.Top);

					_renderContext.Graphics.DrawLine(_pen,
						rect.Left,
						rect.Bottom,
						rect.Right,
						rect.Bottom);
				});
		}

		private static Rectangle toRectangle(RectangleF rect)
		{
			return new Rectangle(strongRound(rect.Left), strongRound(rect.Top), strongRound(rect.Width), strongRound(rect.Height));
		}

		private static int strongRound(double value)
		{
			return Math.Sign(value) * (int) (Math.Abs(value) + 0.5);
		}



		public bool PrintSpace(RichTextToken space)
		{
			if (_y + _lineHeight * HeightPart < _renderContext.Rect.Bottom)
			{
				if (_x + _spaceWidth >= _renderContext.Rect.Right)
					return newLine();

				var targetRect = new RectangleF(_x, _y, _spaceWidth, _lineHeight);

				var printBatch = new RenderBatch(space.IsHighlighted);

				printBatch.Add(targetRect, (rect, hb, he) => printSelection(rect, space));

				if (space.IsHighlighted)
					printHighlight(targetRect, printBatch, space.IsContext);

				_lineQueue.Add(printBatch);

				_x += _spaceWidth;
				return true;
			}

			return false;
		}

		public bool NewParagraph()
		{
			Flush();

			_y += _lineHeight * 4 / 3;
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
				delta = _iconRecognizer.GetIcon(richTextToken.IconName, (int) Math.Round(_lineHeight)).Width;

			return delta;
		}

		private SizeF getLineSize(string tokenText)
		{
			var textFormatFlags = _renderContext.StringFormat.ToTextFormatFlags();

			var area = _renderContext.Rect.Size.ToSize();

			var size = TextRenderer.MeasureText(
				_renderContext.Graphics,
				tokenText,
				_renderContext.Font,
				new Size((int) (area.Width * 1.2f), area.Height),
				textFormatFlags);

			return size;
		}



		private bool printSelection(RectangleF rectangle, RichTextToken token)
		{
			if (!_renderContext.RectSelected)
				return false;

			if (_renderContext.SelectionStartIndex < 0)
			{
				if (!isSelectionStart(rectangle))
					return false;

				_renderContext.SelectionStartIndex = token.Index;
				_renderContext.SelectionLength = token.Length;
			}
			else
			{
				if (!isSelectionContinuation(rectangle))
					return false;

				_renderContext.SelectionLength = token.Index + token.Length - _renderContext.SelectionStartIndex;
			}

			rectangle = toRectangle(rectangle);
			rectangle.Offset(-0.5f, -0.5f);

			_renderContext.Graphics.FillRectangle(new SolidBrush(_renderContext.SelectionBackColor), rectangle);

			return true;
		}

		private bool isSelectionStart(RectangleF tokenFromLine)
		{
			if (_renderContext.SelectionIsAll)
				return true;

			var (selectionStartX, selectionEndX) = getSelectionDelimitersForLine(tokenFromLine);

			return tokenFromLine.Right - 1 >= selectionStartX && tokenFromLine.Left <= selectionEndX;
		}

		private bool isSelectionContinuation(RectangleF rectangle)
		{
			return _renderContext.SelectionIsAll || rectangle.Left <= getSelectionDelimitersForLine(rectangle).SelectionEndX;
		}

		private (int SelectionStartX, int SelectionEndX) getSelectionDelimitersForLine(RectangleF tokenFromLine)
		{
			float lineTop = tokenFromLine.Top;
			float lineBottom = tokenFromLine.Bottom - 1;

			bool selectionStartsBeforeLine =
				_renderContext.SelectionStart.Y < lineTop ||
				_renderContext.SelectionEnd.Y < lineTop;

			bool selectionStartsWithinLine = ((float) _renderContext.SelectionStart.Y).IsWithin(lineTop, lineBottom);
			bool selectionEndsWithinLine = ((float) _renderContext.SelectionEnd.Y).IsWithin(lineTop, lineBottom);

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
				bool highlightEnd = queue[i].IsHighlighted && (i == queue.Count - 1 || !queue[i + 1].IsHighlighted);

				queue[i].Invoke(highlightBegin, highlightEnd);
			}

			queue.Clear();
		}



		private const float HeightPart = 0.8f;

		private float _x;
		private float _y;
		private readonly float _lineHeight;
		private readonly float _spaceWidth;

		private readonly Brush _brush;
		private readonly Brush _contextBrush;
		private readonly Pen _pen;

		private readonly RenderBatchQueue _lineQueue = new RenderBatchQueue();
		private readonly RichTextRenderContext _renderContext;
		private readonly IconRecognizer _iconRecognizer;
		private readonly SolidBrush _shadowBrush = new SolidBrush(Color.FromArgb(0x50, 0x50, 0x50));
		private readonly PointF _iconShadowOffset;
	}
}