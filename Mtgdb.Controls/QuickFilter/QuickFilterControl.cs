using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class QuickFilterControl : UserControl
	{
		public QuickFilterControl()
		{
			init();
			
			ImageSize = new Size(20, 20);
			PropertiesCount = 5;
			_selectionColor = Color.Transparent;
			_selectionBorderColor = Color.Transparent;
			_prohibitedColor = Color.Transparent;

			_opacityEnabled = 0.80f;
			_opacityToEnable = 0.65f;
			_opacityToDisable = 0.35f;
			_opacityDisabled = 0.20f;

			CostNeutralValues = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

			Paint += controlPaint;
			MouseClick += controlClick;
			MouseEnter += mouseEnter;
			MouseLeave += mouseLeave;
			MouseMove += mouseMove;
		}


		private void makeProhibitedValuesIgnored()
		{
			if (_states == null)
				return;

			bool stateChanged = false;

			for (int i = 0; i < _states.Length; i++)
			{
				stateChanged = _states[i] == FilterValueState.Prohibited;
				if (stateChanged)
					_states[i] = FilterValueState.Ignored;
			}

			if (stateChanged)
				StateChanged?.Invoke(this, null);
		}

		private void createDerivedImages()
		{
			if (_propertyImages == null)
				return;

			_images =
				transformImages(0.00f, 0.00f, 1.00f, Opacity1Enabled, _propertyImages);

			_imagesToEnable =
				transformImages(0.05f, 0.00f, 0.95f, Opacity2ToEnable, _propertyImages);

			_imagesToDisable =
				transformImages(0.20f, 0.00f, 0.80f, Opacity3ToDisable, _propertyImages);

			_imagesDisabled =
				transformImages(0.25f, 0.00f, 0.75f, Opacity4Disabled, _propertyImages);
		}

		private static Image[] transformImages(
			float whiteFactor,
			float greyFactor,
			float colorFactor,
			float opacity,
			Image[] imageCollection)
		{
			if (imageCollection == null || imageCollection.Length == 0)
				return null;

			var imagesTransformed = new Image[imageCollection.Length];

			for (int i = 0; i < imagesTransformed.Length; i++)
			{
				var copy = new Bitmap(imageCollection[i]);

				makeGreyscale(copy, colorFactor, whiteFactor, greyFactor, opacity);

				imagesTransformed[i] = copy;
			}

			return imagesTransformed;
		}



		private static void makeGreyscale(Bitmap bmp, float colorFactor, float whiteFactor, float grayFactor, float opacity)
		{
			// Specify a pixel format.
			const PixelFormat pxf = PixelFormat.Format32bppArgb;

			// Lock the bitmap's bits.
			var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
			var bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

			// Get the address of the first line.
			var ptr = bmpData.Scan0;

			// Declare an array to hold the bytes of the bitmap. 
			// int numBytes = bmp.Width * bmp.Height * 3; 
			int numBytes = bmpData.Stride*bmp.Height;
			var rgbValues = new byte[numBytes];

			// Copy the RGB values into the array.
			Marshal.Copy(ptr, rgbValues, 0, numBytes);

			float white = Byte.MaxValue*whiteFactor;
			var transparent = (byte) (Byte.MaxValue*opacity);
			for (int counter = 0; counter < rgbValues.Length; counter += 4)
			{
				var min = white + grayFactor*(rgbValues[counter] + rgbValues[counter + 1] + rgbValues[counter + 2])/3f;

				rgbValues[counter] = (byte) (min + rgbValues[counter]*colorFactor);
				rgbValues[counter + 1] = (byte) (min + rgbValues[counter + 1]*colorFactor);
				rgbValues[counter + 2] = (byte) (min + rgbValues[counter + 2]*colorFactor);
				rgbValues[counter + 3] = Math.Min(transparent, rgbValues[counter + 3]);
			}

			// Copy the RGB values back to the bitmap
			Marshal.Copy(rgbValues, 0, ptr, numBytes);

			// Unlock the bits.
			bmp.UnlockBits(bmpData);
		}



		private void setButtonState(StateClick click, FilterValueState[] states)
		{
			FilterValueState[] filterValueStates = states;
			if ((int) filterValueStates[click.ButtonIndex]%2 != (int) click.ClickedState%2)
				applyCheckLogic(click, states);
			else if (EnableRequiringSome && filterValueStates[click.ButtonIndex] == FilterValueState.Ignored && click.ClickedState == FilterValueState.Ignored)
				applyCheckLogic(click, states);
			else
				applyUncheckLogic(click, states);
		}

		private void applyUncheckLogic(StateClick click, FilterValueState[] states)
		{
			switch (click.ClickedState)
			{
				case FilterValueState.RequiredSome:
				case FilterValueState.Required:

					states[click.ButtonIndex] = FilterValueState.Ignored;
					break;

				case FilterValueState.Ignored:

					if (EnableCostBehavior && !isCostNeutral(click.ButtonIndex) && click.MouseButton == MouseButtons.Left)
						applyUncheckCostLogic(click, states);
					else if (EnableRequiringSome)
						states[click.ButtonIndex] = FilterValueState.Ignored;
					else
						states[click.ButtonIndex] = FilterValueState.Prohibited;
					break;

				case FilterValueState.Prohibited:

					states[click.ButtonIndex] = FilterValueState.Ignored;
					break;
			}
		}

		private bool isCostNeutral(int buttonIndex)
		{
			string value = Properties[buttonIndex];
			return CostNeutralValues.Contains(value);
		}

		private void applyUncheckCostLogic(StateClick click, FilterValueState[] states)
		{
			bool allAllowed = true;
			
			var i = click.ButtonIndex;
			for (int j = 0; j < states.Length; j++)
			{
				if (i == j || isCostNeutral(j))
					continue;

				allAllowed &= states[j] == FilterValueState.Ignored;
			}

			if (allAllowed)
			{
				// Оставить разрешёнными только кликнутое требование и его более слабую версию
				// Например, {W} и {W/P}
				for (int j = 0; j < states.Length; j++)
				{
					if (i == j || isCostNeutral(j) || isWeakerRequirement(i, j))
						continue;

					states[j] = FilterValueState.Prohibited;
				}
			}
			else
				states[i] = FilterValueState.Prohibited;
		}

		private bool isWeakerRequirement(int i, int j)
		{
			if (Properties == null || Properties[i] == null || Properties[j] == null)
				return false;

			return Properties[j].Trim('{', '}').Contains(Properties[i].Trim('{', '}'));
		}

		private void applyCheckLogic(StateClick click, FilterValueState[] states)
		{
			var i = click.ButtonIndex;

			if (EnableMutuallyExclusive && click.ClickedState == FilterValueState.Required)
				for (int j = 0; j < states.Length; j++)
					if (j != i && states[j] == FilterValueState.Required)
						states[j] = FilterValueState.Ignored;

			if (EnableCostBehavior && click.MouseButton == MouseButtons.Left &&
				states[i] == FilterValueState.Prohibited && click.ClickedState == FilterValueState.Ignored)
			{
				for (int j = 0; j < states.Length; j++)
					if ((i == j || isWeakerRequirement(i, j)) && states[j] == FilterValueState.Prohibited)
						states[j] = FilterValueState.Ignored;
			}

			if (EnableRequiringSome && states[i] != FilterValueState.RequiredSome && click.ClickedState == FilterValueState.Ignored)
				states[i] = FilterValueState.RequiredSome;
			else
				states[i] = click.ClickedState;
		}



		private void paintRequireButton(PaintEventArgs e, FilterValueState state, FilterValueState statePreview, int i)
		{
			paintButton(e, state, statePreview, FilterValueState.Required, ImageRequired ?? _imagesDisabled[i], i);
		}

		private void paintAllowButton(PaintEventArgs e, FilterValueState state, FilterValueState statePreview, int i)
		{
			FilterValueState filterValueState =
				EnableRequiringSome
					? FilterValueState.RequiredSome
					: FilterValueState.Ignored;

			paintButton(e, state, statePreview, filterValueState, _imagesDisabled[i], i);
		}

		private void paintProhibitButton(PaintEventArgs e, FilterValueState state, FilterValueState statePreview, int i)
		{
			if (HideProhibit)
				return;


			//if (ImageProhibit != null && (state == ButtonState.Prohibited || statePreview == ButtonState.Prohibited && _showPreview))
			//	e.Graphics.DrawImage(ImageProhibit, rectangle);

			paintButton(e, state, statePreview, FilterValueState.Prohibited, _imagesDisabled[i], i);
		}

		private void paintButton(
			PaintEventArgs e,
			FilterValueState state,
			FilterValueState statePreview,
			FilterValueState filterValueState,
			Image imageDisabled,
			int i)
		{
			if (imageDisabled == null)
				return;

			var rectangle = getPaintingRectangle(i, filterValueState);

			bool enabled = state == filterValueState;
			bool enabledPreview = statePreview == filterValueState;

			if (enabled)
			{
				if (enabledPreview || !_showPreview)
					e.Graphics.DrawImage(_images[i], rectangle);
				else
					e.Graphics.DrawImage(_imagesToDisable[i], rectangle);
			}
			else /* !enabled */
			{
				if (!enabledPreview || !_showPreview)
					e.Graphics.DrawImage(imageDisabled, rectangle);
				else
					e.Graphics.DrawImage(_imagesToEnable[i], rectangle);
			}
		}



		private void updateSize()
		{
			Size = MinimumSize = getSize();
		}

		private Size getSize()
		{
			int cellWidth = _imageSize.Width + Spacing.Width;
			int cellHeight = _imageSize.Height + Spacing.Height;

			int statesCount;

			if (HideProhibit)
				statesCount = 2;
			else
				statesCount = 3;

			int width = PropertiesCount*cellWidth + Spacing.Width;
			int height = statesCount*cellHeight + Spacing.Height;

			if (IsVertical)
				return new Size(height, width);

			return new Size(width, height);
		}

		private Rectangle getPaintingRectangle(int i, FilterValueState state)
		{
			int imageWidth = ImageSize.Width;
			var imageHeight = ImageSize.Height;

			int width = imageWidth + Spacing.Width;
			int height = imageHeight + Spacing.Height;

			// чтобы Allowed был 0 Prohibited сделан -1
			const int maxStateIndex = 1;
			// таким образом RequireSome == Allowed для вычисления координат
			int stateIndex = (int) state%2;
			int shift = maxStateIndex - stateIndex;

			int x = i*width + Spacing.Width;
			var y = shift*height + Spacing.Height;

			if (IsVertical)
			{
				swap(ref x, ref y);
				swap(ref imageWidth, ref imageHeight);

				if (IsFlipped)
					x = Width - x - (imageWidth - 1);
			}

			var result = new Rectangle(x, y, imageWidth, imageHeight);
			return result;
		}

		private StateClick? getCommand(int x, int y, MouseButtons button)
		{
			if (IsVertical)
			{
				if (IsFlipped)
					x = Width - 1 - x;

				swap(ref x, ref y);
			}

			var width = ImageSize.Width + Spacing.Width;
			var height = ImageSize.Height + Spacing.Height;

			int index = (2*x - Spacing.Width)/(width*2);
			if (index < 0 || index >= PropertiesCount)
				return null;

			int stateIndex = (2*y - Spacing.Width)/(height*2);

			switch (stateIndex)
			{
				case 0:
					return new StateClick(index, FilterValueState.Required, button);
				case 1:
					return new StateClick(index, FilterValueState.Ignored, button);
				case 2:
					if (HideProhibit)
						return null;
					return new StateClick(index, FilterValueState.Prohibited, button);
				default:
					return null;
			}
		}

		private static void swap<T>(ref T x, ref T y)
		{
			var tmp = x;
			x = y;
			y = tmp;
		}



		private void controlClick(object sender, MouseEventArgs e)
		{
			var command = getCommand(e.X, e.Y, e.Button);

			if (!command.HasValue)
				return;

			_lastClick = command;
			_showPreview = false;

			if (e.Button == MouseButtons.Middle)
			{
				States = null;
				return;
			}

			setButtonState(command.Value, _states);

			/**/
			Invalidate();
			Application.DoEvents();

			StateChanged?.Invoke(this, null);
		}



		private void controlPaint(object sender, PaintEventArgs e)
		{
			if (_images == null)
				createDerivedImages();

			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

			paintProhibitZone(e);
			paintSelection(e);

			for (int i = 0; i < PropertiesCount; i++)
			{
				var state = _states[i];
				var statePreview = _statesPreview[i];

				if (_images == null || i >= _images.Length)
					return;

				paintRequireButton(e, state, statePreview, i);
				paintAllowButton(e, state, statePreview, i);
				paintProhibitButton(e, state, statePreview, i);
			}

			if (_mouseInside && ShowValueHint)
				paintTooltip(e);
		}

		private void paintProhibitZone(PaintEventArgs e)
		{
			if (HideProhibit)
				return;

			for (int i = 0; i < PropertiesCount; i++)
			{
				var rectangle = (RectangleF) getPaintingRectangle(i, FilterValueState.Prohibited);
				rectangle.Inflate(new SizeF(Spacing.Width/2f, Spacing.Height/2f));

				var color = Color.FromArgb(56, ProhibitedColor);
				var color2 = Color.FromArgb(0, ProhibitedColor);

				Brush fillBrush;

				if (IsVertical)
				{
					if (IsFlipped)
					{
						rectangle = new RectangleF(
							new PointF(rectangle.Left + rectangle.Width/2f, rectangle.Top),
							new SizeF(rectangle.Width/2f, rectangle.Height));

						fillBrush = new LinearGradientBrush(
							new PointF(rectangle.Left - 1, rectangle.Top),
							new PointF(rectangle.Right, rectangle.Top),
							color2,
							color);
					}
					else
					{
						rectangle = new RectangleF(
							rectangle.Location,
							new SizeF(rectangle.Width/2f, rectangle.Height));

						fillBrush = new LinearGradientBrush(
							new PointF(rectangle.Left, rectangle.Top),
							new PointF(rectangle.Right + 1, rectangle.Top),
							color,
							color2);
					}
				}
				else
				{
					rectangle = new RectangleF(
						rectangle.Location,
						new SizeF(rectangle.Width, rectangle.Height/2f));

					fillBrush = new LinearGradientBrush(
						new PointF(rectangle.Left, rectangle.Top),
						new PointF(rectangle.Left, rectangle.Bottom + 1),
						color,
						color2);
				}

				e.Graphics.FillRectangle(fillBrush, rectangle);
			}
		}

		private void paintTooltip(PaintEventArgs e)
		{
			if (Properties == null)
				return;

			var command = getCommand(_mouseLocation.X, _mouseLocation.Y, MouseButtons.None);

			if (!command.HasValue)
				return;

			var property = Properties[command.Value.ButtonIndex] ?? "N/A";
			
			var font = Font;
			const int border = 1;

			var rectangle = getPaintingRectangle(command.Value.ButtonIndex, command.Value.ClickedState);

			int x;
			int y;

			if (IsVertical)
			{
				y = rectangle.Y - rectangle.Height;
				x = rectangle.X + (rectangle.Width - font.Height/2)/2;
			}
			else
			{
				x = rectangle.X + rectangle.Width;
				y = rectangle.Y + (rectangle.Height - font.Height)/2;
			}


			int iconWidth = HintIcon?.Width ?? 0;

			int wordWidth = Math.Max(
				iconWidth,
				(int) (property.Length*font.Size) + HintTextShift.Width);

			if (x + wordWidth > Width)
				x = Width - wordWidth;

			int wordHeight =
				(int) (font.Size*1.2f - HintTextShift.Height);

			if (IsVertical)
			{
				if (y - wordHeight < 0)
					y = wordHeight;
			}
			else
			{
				if (y + wordHeight > Height)
					y = Height - wordHeight;
			}


			if (HintIcon != null)
				e.Graphics.DrawImage(HintIcon, new Point(x, y));

			int tx = x + HintTextShift.Width;
			int ty = y + HintTextShift.Height;

			for (int i = -border; i < border + 1; i++)
				for (int j = -border; j < border + 1; j++)
				{
					e.Graphics.DrawString(
						property,
						font,
						new SolidBrush(Color.White),
						tx + i,
						ty + j);
				}

			e.Graphics.DrawString(
				property,
				font,
				new SolidBrush(Color.Black),
				tx,
				ty);
		}

		private void paintSelection(PaintEventArgs e)
		{
			for (int i = 0; i < _states.Length; i++)
			{
				var state = _states[i];

				if (EnableRequiringSome && state == FilterValueState.Ignored)
					continue;

				if (state == FilterValueState.Prohibited && HideProhibit)
					continue;

				var rectBorder = (RectangleF) getPaintingRectangle(i, state);

				rectBorder.Offset(-0.5f, -0.5f);
				var rectContent = rectBorder;
				rectBorder.Inflate(SelectionBorder, SelectionBorder);

				if (!rectBorder.IntersectsWith(e.ClipRectangle))
					continue;

				Action<Color, RectangleF> fill;
				Action<Color, RectangleF, float> fillBorder;

				if (BorderShape == BorderShape.Ellipse)
					fill = (c, r) => e.Graphics.FillEllipse(new SolidBrush(c), r);
				else if (BorderShape == BorderShape.Rectangle)
					fill = (c, r) => e.Graphics.FillRectangle(new SolidBrush(c), r);
				else
					throw new NotSupportedException();

				if (BorderShape == BorderShape.Ellipse)
					fillBorder = (c, r, w) =>
					{
						r.Inflate(-w/2, -w/2);
						e.Graphics.DrawEllipse(new Pen(c, w), r);
					};
				else if (BorderShape == BorderShape.Rectangle)
					fillBorder = (c, r, w) => { e.Graphics.DrawRectangles(new Pen(c, w), new[] { r }); };
				else
					throw new NotSupportedException();

				if (SelectionBorder > 0)
				{
					fillBorder(SelectionBorderColor, rectBorder, SelectionBorder);
					fill(SelectionColor, rectContent);
				}
				else if (SelectionBorder < 0)
				{
					fillBorder(SelectionBorderColor, rectContent, -SelectionBorder);
					fill(SelectionColor, rectBorder);
				}
				else
				{
					fill(SelectionColor, rectBorder);
				}
			}
		}



		private void mouseEnter(object sender, EventArgs e)
		{
			_lastClick = null;
			_lastClickPreview = null;
			_mouseInside = true;
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			_showPreview = false;
			_mouseInside = false;
			Invalidate();
		}

		private void mouseMove(object sender, MouseEventArgs e)
		{
			if (!_mouseInside)
				return;

			if (!ClientRectangle.Contains(e.Location))
				return;

			if (_mouseMoveCounter > 15)
				return;

			_mouseLocation = e.Location;
			//if (!this.ClientRectangle.Contains(new Point(e.X, e.Y)))
			//	return;

			var commandPreview = getCommand(e.X, e.Y, MouseButtons.Left);
			if (!commandPreview.HasValue)
				return;

			_mouseMoveCounter++;

			if (commandPreview != _lastClick)
				_lastClick = null;

			// _lastClick != null => состояние после клика уже прорисовано
			if (_lastClick == null)
			{
				if (commandPreview != _lastClickPreview)
				{
					_lastClickPreview = commandPreview;

					_showPreview = true;
					_statesPreview = _states.ToArray();
					setButtonState(commandPreview.Value, _statesPreview);

					/**/
					Invalidate();
					Application.DoEvents();
				}
			}

			_mouseMoveCounter--;
		}



		protected override void Dispose(bool disposing)
		{
			if (disposing)
				components?.Dispose();

			base.Dispose(disposing);
		}

		private void init()
		{
			SuspendLayout();
			
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			Name = "QuickFilterControl";
			Size = new Size(100, 60);
			DoubleBuffered = true;

			ResumeLayout(false);
		}



		[Category("Settings")]
		public event EventHandler StateChanged;
		
		[Category("Settings")]
		[DefaultValue(typeof(Size), "20, 20")]
		public Size ImageSize
		{
			get { return _imageSize; }
			set
			{
				_imageSize = value;
				updateSize();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Size), "0, 0")]
		public Size Spacing
		{
			get { return _spacing; }
			set
			{
				_spacing = value;
				updateSize();
			}
		}

		[Category("Settings")]
		[DefaultValue(5)]
		public int PropertiesCount
		{
			get { return _propertiesCount; }
			set
			{
				_propertiesCount = value;
				_states = new FilterValueState[value];
				StatesDefault = new FilterValueState[value];
				updateSize();
			}
		}

		[Category("Settings")]
		[DefaultValue(null)]
		public Image ImageRequired
		{
			get { return _imageRequired; }
			set
			{
				_imageRequired = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(null)]
		public Image HintIcon { get; set; }

		[Category("Settings")]
		[DefaultValue(typeof(Size), "0, 0")]
		public Size HintTextShift { get; set; }

		[Category("Settings")]
		public Image[] PropertyImages
		{
			get { return _propertyImages; }
			set
			{
				_propertyImages = value;
				createDerivedImages();
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Color), "Transparent")]
		public Color SelectionColor
		{
			get { return _selectionColor; }
			set
			{
				_selectionColor = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Color), "Transparent")]
		public Color SelectionBorderColor
		{
			get { return _selectionBorderColor; }
			set
			{
				_selectionBorderColor = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(Color), "Transparent")]
		public Color ProhibitedColor
		{
			get { return _prohibitedColor; }
			set
			{
				_prohibitedColor = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(BorderShape), "Rectangle")]
		public BorderShape BorderShape
		{
			get { return _borderShape; }
			set
			{
				_borderShape = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(0f)]
		public float SelectionBorder
		{
			get { return _selectionBorder; }
			set
			{
				_selectionBorder = value;
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(false)]
		public bool EnableCostBehavior { get; set; }

		[Category("Settings")]
		[DefaultValue(false)]
		public bool EnableMutuallyExclusive
		{
			get { return _enableMutuallyExclusive; }
			set
			{
				_enableMutuallyExclusive = value;
				updateSize();
			}
		}

		[Category("Settings")]
		[DefaultValue(false)]
		public bool HideProhibit
		{
			get { return _hideProhibit; }
			set
			{
				_hideProhibit = value;

				updateSize();

				if (value && EnableRequiringSome)
					// В режиме EnableRequiringSome невозможно визуально отличить запрещённые значения от игнорируемых
					makeProhibitedValuesIgnored();

				Invalidate();
			}
		}

		/// <summary>
		/// <para>Позволяет указывать желаемые занчения через или, то есть включает возможность выбора состояния RequiredSome.</para>
		/// <para>Состояние RequiredSome отображается на месте Ignored</para>
		/// <para>Состояние Ignored не отображается</para>
		/// <para>При отключении кнопки в состоянии RequiredSome она переходит в Ignored</para>
		/// <para>Включить состояние Prohibited при скрытых кнопках данного состояния становится невозможно</para>
		/// </summary>
		[Category("Settings"), DefaultValue(false)]
		public bool EnableRequiringSome
		{
			get { return _enableRequiringSome; }
			set
			{
				if (value != _enableRequiringSome)
				{
					_enableRequiringSome = value;

					if (_states != null && !value)
						for (int i = 0; i < _states.Length; i++)
							if (_states[i] == FilterValueState.RequiredSome)
								_states[i] = FilterValueState.Ignored;

					Invalidate();
					StateChanged?.Invoke(this, null);
				}
			}
		}

		[Category("Settings")]
		[DefaultValue(0.80f)]
		public float Opacity1Enabled
		{
			get { return _opacityEnabled; }
			set
			{
				_opacityEnabled = value;
				createDerivedImages();
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(0.65f)]
		public float Opacity4Disabled
		{
			get { return _opacityDisabled; }
			set
			{
				_opacityDisabled = value;
				createDerivedImages();
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(0.35f)]
		public float Opacity3ToDisable
		{
			get { return _opacityToDisable; }
			set
			{
				_opacityToDisable = value;
				createDerivedImages();
				Invalidate();
			}
		}

		[DefaultValue(0.20f)]
		[Category("Settings")]
		public float Opacity2ToEnable
		{
			get { return _opacityToEnable; }
			set
			{
				_opacityToEnable = value;
				createDerivedImages();
				Invalidate();
			}
		}

		[Category("Settings")]
		[DefaultValue(false)]
		public bool ShowValueHint { get; set; }

		[Category("Settings")]
		[DefaultValue(false)]
		public bool IsVertical
		{
			get { return _isVertical; }
			set
			{
				_isVertical = value;
				updateSize();
			}
		}

		[Category("Settings")]
		[DefaultValue(false)]
		public bool IsFlipped
		{
			get { return _isFlipped; }
			set
			{
				_isFlipped = value;
				Invalidate();
			}
		}



		[Browsable(false)]
		public FilterValueState[] StatesDefault { get; set; }

		[Browsable(false)]
		public FilterValueState[] States
		{
			get { return _states.ToArray(); }
			set
			{
				if (value == null || value.Length != PropertiesCount)
				{
					if (StatesDefault != null && StatesDefault.Length == PropertiesCount)
						_states = StatesDefault.ToArray();
					else
						_states = new FilterValueState[PropertiesCount];
				}
				else
					_states = value.ToArray();

				_statesPreview = _states.ToArray();

				Invalidate();
				StateChanged?.Invoke(this, null);
			}
		}

		[Browsable(false)]
		public HashSet<string> CostNeutralValues { get; set; }

		[Browsable(false)]
		public IList<string> Properties { get; set; }


		private Image[] _images;
		private Image[] _imagesToDisable;
		private Image[] _imagesToEnable;
		private Image[] _imagesDisabled;

		private FilterValueState[] _statesPreview;
		private StateClick? _lastClick;
		private StateClick? _lastClickPreview;
		private bool _showPreview;
		private bool _mouseInside;
		private Point _mouseLocation;
		private int _mouseMoveCounter;

		private FilterValueState[] _states;
		private int _propertiesCount;
		private Size _imageSize;
		private Size _spacing;
		private Image[] _propertyImages;
		private Color _selectionColor;
		private Color _selectionBorderColor;
		private Color _prohibitedColor;
		private Image _imageRequired;
		private bool _enableRequiringSome;
		private float _selectionBorder;
		private BorderShape _borderShape;
		private bool _enableMutuallyExclusive;
		private bool _hideProhibit;
		private float _opacityEnabled;
		private float _opacityDisabled;
		private float _opacityToDisable;
		private float _opacityToEnable;
		private bool _isVertical;
		private bool _isFlipped;

		private readonly IContainer components = null;
	}
}