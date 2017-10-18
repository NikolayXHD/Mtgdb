using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	internal class RichTextLayout
	{
		private const float HeightPart = 0.8f;

		private float _x;
		private float _y;
		private readonly float _lineHeight;
		private readonly float _spaceWidth;

		private readonly Brush _brush;
		private readonly Brush _contextBrush;
		private readonly Brush _backgroundBrush;
		private readonly Pen _pen;

		private readonly RenderBatchQueue _lineQueue = new RenderBatchQueue();
		private readonly RichTextRenderContext _renderContext;
		private readonly SolidBrush _shadowBrush = new SolidBrush(Color.FromArgb(0x50, 0x50, 0x50));

		public RichTextLayout(RichTextRenderContext renderContext)
		{
			_renderContext = renderContext;
			
			_brush = new SolidBrush(renderContext.HighlightColor);
			_contextBrush = new SolidBrush(renderContext.HighlightContextColor);
			_pen = new Pen(renderContext.HighlightBorderColor, renderContext.HighlightBorderWidth);
			_backgroundBrush = new SolidBrush(renderContext.BackgroundColor);

			_x = _renderContext.Rect.Left;
			_y = _renderContext.Rect.Top;

			var lineSize = getLineSize(@" ");
			_lineHeight = lineSize.Height;
			_spaceWidth = lineSize.Width;
		}

		public bool PrintWord(List<RichTextToken> word)
		{
			if (_y + _lineHeight*HeightPart > _renderContext.Rect.Bottom)
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
					printSelection(targetRect, printBatch, token.IsContext);

				string tokenText = _renderContext.Text.Substring(token.Index, token.Length);

				if (token.Icon == null)
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

							TextRenderer.DrawText(
								_renderContext.Graphics,
								tokenText,
								_renderContext.Font,
								toRectangle(rect),
								_renderContext.ForeColor,
								_renderContext.StringFormat.ToTextFormatFlags());
						});
					}
				}
				else
				{
					var iconRect = new RectangleF(
						new PointF(_x, _y),
						token.Icon.Size.FitIn(new SizeF(float.MaxValue, _lineHeight)));
					
					if (!StringComparer.InvariantCultureIgnoreCase.Equals(tokenText, @"{E}") &&
						!StringComparer.InvariantCultureIgnoreCase.Equals(tokenText, @"{Q}") &&
						!StringComparer.InvariantCultureIgnoreCase.Equals(tokenText, @"{CHAOS}"))
					{
						iconRect.Inflate(-0.5f, -0.5f);
						iconRect.Offset(-0.35f, 0.5f);
						printBatch.Add(iconRect, (rect, hb, he) => _renderContext.Graphics.FillEllipse(_shadowBrush, rect));

						iconRect.Offset(0.7f, -1f);

						printBatch.Add(iconRect,
							(rect, hb, he) =>
							{
								rect.Inflate(-0.5f, -0.5f);
								_renderContext.Graphics.FillEllipse(_backgroundBrush, rect);
							});

						printBatch.Add(iconRect, (rect, hb, he) => _renderContext.Graphics.DrawImage(token.Icon, rect));
					}
					else
					{
						printBatch.Add(iconRect, (rect, hb, he) => _renderContext.Graphics.DrawImage(token.Icon, rect));
					}
				}

				_lineQueue.Add(printBatch);

				_x += targetRect.Width;

				if (overflow)
					break;
			}
		}

		private static int strongRound(double value)
		{
			return Math.Sign(value) * (int)(Math.Abs(value) + 0.5);
		}

		private static Rectangle toRectangle(RectangleF rect)
		{
			return new Rectangle(strongRound(rect.Left), strongRound(rect.Top), strongRound(rect.Width), strongRound(rect.Height));
		}

		private void printSelection(RectangleF targetRect, RenderBatch batch, bool isContext)
		{
			batch.Add(targetRect,
				(rectF, hb, he) =>
				{
					var rect = toRectangle(new RectangleF(rectF.Location, new SizeF(rectF.Width - _spaceWidth + 1, rectF.Height)));
					rect.Inflate(1, 1);

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

		public bool PrintSpace(RichTextToken space)
		{
			if (_y + _lineHeight*HeightPart < _renderContext.Rect.Bottom)
			{
				if (_x + _spaceWidth >= _renderContext.Rect.Right)
					return newLine();

				if (space.IsHighlighted)
				{
					var printBatch = new RenderBatch(true);
					printSelection(new RectangleF(_x, _y, _spaceWidth, _lineHeight), printBatch, space.IsContext);
					_lineQueue.Add(printBatch);
				}
				else
				{
					var printBatch = new RenderBatch(false);
					_lineQueue.Add(printBatch);
				}

				_x += _spaceWidth;
				return true;
			}

			return false;
		}

		private bool newLine()
		{
			_y += _lineHeight;
			if (_y + _lineHeight*HeightPart < _renderContext.Rect.Bottom)
			{
				Flush();
				_x = _renderContext.Rect.Left;
				return true;
			}

			_y -= _lineHeight;
			return false;
		}

		public bool NewParagraph()
		{
			Flush();

			_y += _lineHeight*4/3;
			_x = _renderContext.Rect.Left;

			return _y + _lineHeight*HeightPart < _renderContext.Rect.Bottom;
		}

		private SizeF getLineSize(string text, IList<RichTextToken> word)
		{
			float width = word.Sum(token => getTokenWidth(text, token));
			return new SizeF(width, _lineHeight);
		}

		private float getTokenWidth(string text, RichTextToken richTextToken)
		{
			float delta;
			if (richTextToken.Icon == null)
				delta = getLineSize(text.Substring(richTextToken.Index, richTextToken.Length)).Width;
			else
				delta = richTextToken.Icon.Size.FitIn(new SizeF(float.MaxValue, _lineHeight)).Width;

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
				new Size((int) (area.Width*1.2f), area.Height),
				textFormatFlags);

			return size;
		}

		

		public void Flush()
		{
			float offsetX;
			if (_renderContext.HorizAlignment == HorizontalAlignment.Right)
				offsetX = _renderContext.Rect.Right - _x;
			else if (_renderContext.HorizAlignment == HorizontalAlignment.Center)
				offsetX = 0.5f*(_renderContext.Rect.Right - _x);
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
	}
}